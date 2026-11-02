using System;
using System.Linq;
using Microsoft.Maui.Controls;
using LeClubAppp.Services;
using LeClubAppp.Models;

namespace LeClubAppp.Views
{
    public partial class MisReservasPage : ContentPage
    {
        public MisReservasPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            CargarReservas();
        }

        private void CargarReservas()
        {
            var u = UsuarioService.UsuarioActual;
            if (u == null) return;

            var lista = ReservaService.ObtenerReservas(u.IdUsuario);
            ReservasCollectionView.ItemsSource = lista;
            SinReservasLabel.IsVisible = !lista.Any();
        }

        private async void OnCancelarReservaClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is Reserva reserva)
            {
                bool confirma = await DisplayAlert("Confirmar", $"¿Deseás cancelar la reserva de {reserva.Cancha}?", "Sí, cancelar", "No");
                if (confirma)
                {
                    ReservaService.EliminarReserva(reserva.Id);
                    CargarReservas();
                }
            }
        }
    }
}