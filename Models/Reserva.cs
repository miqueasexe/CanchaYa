using System;

namespace LeClubAppp.Models
{
    public class Reserva
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string Cancha { get; set; } = string.Empty;
        public string CanchaNombre => Cancha; // Alias para compatibilidad con MisReservasPage
        public DateTime FechaHora { get; set; }
        public decimal PrecioOriginal { get; set; }
        public decimal PrecioFinal { get; set; }
        public bool AplicoDescuentoFrecuencia { get; set; }
        public int PuntosGanados { get; set; }
    }
}