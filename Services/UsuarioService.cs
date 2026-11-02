using System;
using Microsoft.Data.SqlClient;
using LeClubAppp.Models;

namespace LeClubAppp.Services
{
    public static class UsuarioService
    {
        public static Usuario? UsuarioActual { get; set; }

        // Iniciar Sesión consultando en SQL Server
        public static (bool Exito, string Mensaje) IniciarSesion(string email, string password)
        {
            using (SqlConnection conexion = new SqlConnection(ConexionBD.CadenaConexion))
            {
                string query = "SELECT IdUsuario, Nombre, Email, Password, PuntosAcumulados, TotalReservasHistoricas FROM Usuarios WHERE Email = @Email AND Password = @Password";
                SqlCommand cmd = new SqlCommand(query, conexion);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Password", password);

                try
                {
                    conexion.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            UsuarioActual = new Usuario
                            {
                                IdUsuario = Convert.ToInt32(reader["IdUsuario"]),
                                Nombre = reader["Nombre"].ToString() ?? "",
                                Email = reader["Email"].ToString() ?? "",
                                Password = reader["Password"].ToString() ?? "",
                                PuntosAcumulados = Convert.ToInt32(reader["PuntosAcumulados"]),
                                TotalReservasHistoricas = Convert.ToInt32(reader["TotalReservasHistoricas"])
                            };
                            return (true, "OK");
                        }
                    }
                    return (false, "Correo o contraseña incorrectos.");
                }
                catch (Exception ex)
                {
                    return (false, $"Error de conexión: {ex.Message}");
                }
            }
        }

        // Registrar un nuevo usuario en la Base de Datos SQL Server
        public static (bool Exito, string Mensaje) RegistrarUsuario(string nombre, string email, string password)
        {
            using (SqlConnection conexion = new SqlConnection(ConexionBD.CadenaConexion))
            {
                string query = @"INSERT INTO Usuarios (Nombre, Email, Password, PuntosAcumulados, TotalReservasHistoricas) 
                                OUTPUT INSERTED.IdUsuario
                                VALUES (@Nombre, @Email, @Password, 0, 0)";

                SqlCommand cmd = new SqlCommand(query, conexion);
                cmd.Parameters.AddWithValue("@Nombre", nombre);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Password", password);

                try
                {
                    conexion.Open();
                    int nuevoId = (int)cmd.ExecuteScalar();

                    UsuarioActual = new Usuario
                    {
                        IdUsuario = nuevoId,
                        Nombre = nombre,
                        Email = email,
                        Password = password,
                        PuntosAcumulados = 0,
                        TotalReservasHistoricas = 0
                    };

                    return (true, "OK");
                }
                catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
                {
                    return (false, "El correo ya está registrado.");
                }
                catch (Exception ex)
                {
                    return (false, $"Error al registrar: {ex.Message}");
                }
            }
        }

        // Método auxiliar para compatibilidad
        public static Usuario Registrar(string nombre, string email, string password)
        {
            RegistrarUsuario(nombre, email, password);
            return UsuarioActual!;
        }

        // Sumar puntos e incrementarlos en la base de datos
        public static void SumarPuntos(int puntos)
        {
            if (UsuarioActual == null) return;

            UsuarioActual.PuntosAcumulados += puntos;
            UsuarioActual.TotalReservasHistoricas += 1;

            using (SqlConnection conexion = new SqlConnection(ConexionBD.CadenaConexion))
            {
                string query = "UPDATE Usuarios SET PuntosAcumulados = @Puntos, TotalReservasHistoricas = @Historicas WHERE IdUsuario = @Id";
                SqlCommand cmd = new SqlCommand(query, conexion);
                cmd.Parameters.AddWithValue("@Puntos", UsuarioActual.PuntosAcumulados);
                cmd.Parameters.AddWithValue("@Historicas", UsuarioActual.TotalReservasHistoricas);
                cmd.Parameters.AddWithValue("@Id", UsuarioActual.IdUsuario);

                try
                {
                    conexion.Open();
                    cmd.ExecuteNonQuery();
                }
                catch { }
            }
        }
    }
}