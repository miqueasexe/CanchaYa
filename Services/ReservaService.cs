using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using LeClubAppp.Models;

namespace LeClubAppp.Services
{
    public static class ReservaService
    {
        // 1. Obtener la lista de canchas desde SQL Server (Mapea IdCancha a la propiedad Id)
        public static List<Cancha> ObtenerCanchas()
        {
            List<Cancha> canchas = new List<Cancha>();

            using (SqlConnection conexion = new SqlConnection(ConexionBD.CadenaConexion))
            {
                string query = "SELECT IdCancha, Nombre, TipoSuperficie, PrecioHora FROM Canchas";
                SqlCommand cmd = new SqlCommand(query, conexion);

                try
                {
                    conexion.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            canchas.Add(new Cancha
                            {
                                Id = Convert.ToInt32(reader["IdCancha"]),
                                Nombre = reader["Nombre"].ToString() ?? "",
                                TipoSuperficie = reader["TipoSuperficie"].ToString() ?? "",
                                PrecioHora = Convert.ToDecimal(reader["PrecioHora"])
                            });
                        }
                    }
                }
                catch { }
            }

            return canchas;
        }

        // 2. Método CrearReserva que espera 5 argumentos (ReservaPage.xaml.cs)
        public static Reserva CrearReserva(Usuario usuario, string canchaNombre, DateTime fecha, decimal precioBase, bool usarPuntos)
        {
            decimal final = precioBase;

            if (usuario.NivelActual == "Oro") final *= 0.80m;
            else if (usuario.NivelActual == "Plata") final *= 0.90m;

            if (usarPuntos && usuario.PuntosAcumulados >= 500)
            {
                final -= 2000m;
                if (final < 0) final = 0;
            }

            using (SqlConnection conexion = new SqlConnection(ConexionBD.CadenaConexion))
            {
                string queryCancha = "SELECT TOP 1 IdCancha FROM Canchas WHERE Nombre LIKE @Nombre";
                SqlCommand cmdCancha = new SqlCommand(queryCancha, conexion);
                cmdCancha.Parameters.AddWithValue("@Nombre", "%" + canchaNombre + "%");

                conexion.Open();
                object? idCanchaObj = cmdCancha.ExecuteScalar();
                int idCancha = idCanchaObj != null ? Convert.ToInt32(idCanchaObj) : 1;

                string query = @"INSERT INTO Reservas (IdUsuario, IdCancha, FechaHora, PrecioOriginal, PrecioFinal) 
                                OUTPUT INSERTED.IdReserva
                                VALUES (@IdUsuario, @IdCancha, @FechaHora, @PrecioOriginal, @PrecioFinal)";

                SqlCommand cmd = new SqlCommand(query, conexion);
                cmd.Parameters.AddWithValue("@IdUsuario", usuario.IdUsuario);
                cmd.Parameters.AddWithValue("@IdCancha", idCancha);
                cmd.Parameters.AddWithValue("@FechaHora", fecha);
                cmd.Parameters.AddWithValue("@PrecioOriginal", precioBase);
                cmd.Parameters.AddWithValue("@PrecioFinal", final);

                int idReservaInsertada = (int)cmd.ExecuteScalar();

                // Sumar 250 puntos fijos
                UsuarioService.SumarPuntos(250);

                return new Reserva
                {
                    Id = idReservaInsertada,
                    UsuarioId = usuario.IdUsuario,
                    Cancha = canchaNombre,
                    FechaHora = fecha,
                    PrecioOriginal = precioBase,
                    PrecioFinal = final,
                    PuntosGanados = 250
                };
            }
        }

        // 3. Sobrecarga de CrearReserva que recibe objeto Cancha
        public static (bool Exito, string Mensaje) CrearReserva(Cancha canchaObj, DateTime fechaHora)
        {
            if (UsuarioService.UsuarioActual == null)
                return (false, "No hay una sesión activa.");

            var res = CrearReserva(UsuarioService.UsuarioActual, canchaObj.Nombre, fechaHora, canchaObj.PrecioHora, false);
            return (res != null, "¡Reserva realizada con éxito!");
        }

        // 4. Obtener reservas desde SQL Server
        public static List<Reserva> ObtenerReservas(int usuarioId = 0)
        {
            List<Reserva> reservas = new List<Reserva>();

            if (usuarioId == 0 && UsuarioService.UsuarioActual != null)
                usuarioId = UsuarioService.UsuarioActual.IdUsuario;

            if (usuarioId == 0) return reservas;

            using (SqlConnection conexion = new SqlConnection(ConexionBD.CadenaConexion))
            {
                string query = @"SELECT r.IdReserva, r.IdUsuario, r.FechaHora, r.PrecioOriginal, r.PrecioFinal, c.Nombre AS CanchaNombre
                                 FROM Reservas r
                                 INNER JOIN Canchas c ON r.IdCancha = c.IdCancha
                                 WHERE r.IdUsuario = @UsuarioId
                                 ORDER BY r.FechaHora DESC";

                SqlCommand cmd = new SqlCommand(query, conexion);
                cmd.Parameters.AddWithValue("@UsuarioId", usuarioId);

                try
                {
                    conexion.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            reservas.Add(new Reserva
                            {
                                Id = Convert.ToInt32(reader["IdReserva"]),
                                UsuarioId = Convert.ToInt32(reader["IdUsuario"]),
                                Cancha = reader["CanchaNombre"].ToString() ?? "",
                                FechaHora = Convert.ToDateTime(reader["FechaHora"]),
                                PrecioOriginal = Convert.ToDecimal(reader["PrecioOriginal"]),
                                PrecioFinal = Convert.ToDecimal(reader["PrecioFinal"]),
                                PuntosGanados = 250
                            });
                        }
                    }
                }
                catch { }
            }

            return reservas;
        }

        // 5. Eliminar Reserva y reiniciar puntos si ya no quedan reservas
        public static bool EliminarReserva(int reservaId)
        {
            int usuarioId = UsuarioService.UsuarioActual?.IdUsuario ?? 0;

            using (SqlConnection conexion = new SqlConnection(ConexionBD.CadenaConexion))
            {
                string query = "DELETE FROM Reservas WHERE IdReserva = @ReservaId";
                SqlCommand cmd = new SqlCommand(query, conexion);
                cmd.Parameters.AddWithValue("@ReservaId", reservaId);

                try
                {
                    conexion.Open();
                    int filasAfectadas = cmd.ExecuteNonQuery();

                    if (filasAfectadas > 0 && usuarioId > 0)
                    {
                        // Verificar cuántas reservas le quedan al usuario
                        string queryCheck = "SELECT COUNT(*) FROM Reservas WHERE IdUsuario = @UsuarioId";
                        SqlCommand cmdCheck = new SqlCommand(queryCheck, conexion);
                        cmdCheck.Parameters.AddWithValue("@UsuarioId", usuarioId);
                        int totalReservas = (int)cmdCheck.ExecuteScalar();

                        // Si ya no le quedan reservas, ponemos los puntos en 0
                        if (totalReservas == 0)
                        {
                            string queryReset = "UPDATE Usuarios SET PuntosAcumulados = 0 WHERE IdUsuario = @UsuarioId";
                            SqlCommand cmdReset = new SqlCommand(queryReset, conexion);
                            cmdReset.Parameters.AddWithValue("@UsuarioId", usuarioId);
                            cmdReset.ExecuteNonQuery();

                            if (UsuarioService.UsuarioActual != null)
                            {
                                UsuarioService.UsuarioActual.PuntosAcumulados = 0;
                            }
                        }
                    }

                    return filasAfectadas > 0;
                }
                catch { return false; }
            }
        }

        public static bool EliminarReserva(Reserva reserva) => reserva != null && EliminarReserva(reserva.Id);

        // 6. Métodos auxiliares necesarios para ReservaPage y PerfilPage
        public static bool AplicaDescuentoFrecuencia(int usuarioId) => false;

        public static int ObtenerPuntosUsuario(int usuarioId) => UsuarioService.UsuarioActual?.PuntosAcumulados ?? 0;

        public static List<Reserva> ObtenerReservasPorUsuario(int usuarioId) => ObtenerReservas(usuarioId);

        public static List<Reserva> ObtenerReservasDelUsuario() => ObtenerReservas(0);
    }
}