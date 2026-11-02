using System.Collections.Generic;
using LeClubAppp.Models;

namespace LeClubAppp.Services
{
    public static class CanchaService
    {
        private static List<Cancha> _canchas = new List<Cancha>
        {
            new Cancha { Id = 1, Nombre = "Cancha Fútbol 5 - Sintético", PrecioHora = 10000m, TipoSuperficie = "Césped Sintético" },
            new Cancha { Id = 2, Nombre = "Cancha Fútbol 7 - Artificial", PrecioHora = 14000m, TipoSuperficie = "Césped Artificial" },
            new Cancha { Id = 3, Nombre = "Cancha Fútbol 11 - Natural", PrecioHora = 20000m, TipoSuperficie = "Césped Natural" }
        };

        public static List<Cancha> ObtenerCanchas() => _canchas;
    }
}