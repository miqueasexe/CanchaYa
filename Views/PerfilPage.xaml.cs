using System;
using Microsoft.Maui.Controls;
using LeClubAppp.Services;

namespace LeClubAppp.Views
{
    public partial class PerfilPage : ContentPage
    {
        public PerfilPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            CargarDatosPerfil();
        }

        private void CargarDatosPerfil()
        {
            var usuario = UsuarioService.UsuarioActual;

            if (usuario == null)
            {
                lblNombre.Text = "Usuario no autenticado";
                lblEmail.Text = "-";
                return;
            }

            lblNombre.Text = usuario.Nombre;
            lblEmail.Text = usuario.Email;

            // Obtener datos reales de reservas y puntos del servicio
            var reservas = ReservaService.ObtenerReservasPorUsuario(usuario.IdUsuario);
            int totalReservas = reservas.Count;
            int puntos = ReservaService.ObtenerPuntosUsuario(usuario.IdUsuario);

            lblPuntos.Text = $"{puntos} pts";
            lblTotalReservas.Text = totalReservas.ToString();

            // Calcular Nivel del Usuario según cantidad de reservas
            if (totalReservas >= 8)
            {
                lblNivel.Text = "👑 CLIENTE VIP (ORO)";
                frameNivel.BackgroundColor = Color.FromArgb("#EAB308"); // Dorado
                lblBeneficioInfo.Text = "¡Atención preferencial y 20% OFF permanente disponible!";
            }
            else if (totalReservas >= 4)
            {
                lblNivel.Text = "⭐ CLIENTE HABITUAL (PLATA)";
                frameNivel.BackgroundColor = Color.FromArgb("#64748B"); // Plateado
                lblBeneficioInfo.Text = "¡Ya tenés habilitado el 20% OFF por frecuencia en tus reservas!";
            }
            else
            {
                lblNivel.Text = "🥉 CLIENTE BRONCE";
                frameNivel.BackgroundColor = Color.FromArgb("#B45309"); // Bronce
                int faltantes = 4 - totalReservas;
                lblBeneficioInfo.Text = $"Te faltan {faltantes} reserva(s) para activar el 20% OFF por frecuencia.";
            }
        }

        private async void OnVolverClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}