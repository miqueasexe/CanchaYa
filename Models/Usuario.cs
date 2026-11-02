using System;

namespace LeClubAppp.Models
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int PuntosAcumulados { get; set; } = 0;
        public int TotalReservasHistoricas { get; set; } = 0;

        public string NivelActual
        {
            get
            {
                if (PuntosAcumulados >= 1000) return "Oro";
                if (PuntosAcumulados >= 500) return "Plata";
                return "Bronce";
            }
        }
    }
}