namespace LeClubAppp.Models
{
    public class Cancha
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal PrecioHora { get; set; } = 10000m;
        public decimal Precio => PrecioHora;
        public decimal PrecioPorHora => PrecioHora;
        public string TipoSuperficie { get; set; } = "Césped Sintético";
        public string Tipo => TipoSuperficie;
    }
}