using System;
using System.Collections.Generic;
using Microsoft.Maui.Controls;
using LeClubAppp.Services;
using LeClubAppp.Models;

namespace LeClubAppp.Views
{
    public partial class ReservaPage : ContentPage
    {
        private decimal _precioBase = 10000m;

        public ReservaPage()
        {
            InitializeComponent();
            Inicializar("Cancha Fútbol 5", 10000m);
        }

        public ReservaPage(Cancha cancha)
        {
            InitializeComponent();
            if (cancha != null)
                Inicializar(cancha.Nombre, cancha.PrecioHora);
            else
                Inicializar("Cancha Fútbol 5", 10000m);
        }

        public ReservaPage(string cancha, decimal precio)
        {
            InitializeComponent();
            Inicializar(cancha, precio);
        }

        private void Inicializar(string cancha, decimal precio)
        {
            _precioBase = precio;
            NombreCanchaLabel.Text = string.IsNullOrEmpty(cancha) ? "Cancha Fútbol 5" : cancha;
            PrecioCanchaLabel.Text = $"${_precioBase:N0} / hora";
            HorarioPicker.ItemsSource = new List<string> { "18:00 hs", "19:00 hs", "20:00 hs", "21:00 hs", "22:00 hs" };

            if (UsuarioService.UsuarioActual != null)
                NombreClienteEntry.Text = UsuarioService.UsuarioActual.Nombre;

            FechaPicker.Date = DateTime.Today;
            CalcularPrecio();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Notificación automática si el usuario ya tiene suficientes puntos para un descuento
            if (UsuarioService.UsuarioActual != null && UsuarioService.UsuarioActual.PuntosAcumulados >= 500)
            {
                DisplayAlert("¡Descuento Disponible!", "¡Felicitaciones! Tenés los puntos suficientes para aplicar un descuento en tu reserva.", "OK");
            }
        }

        private void OnPuntosToggled(object sender, ToggledEventArgs e) => CalcularPrecio();

        private void CalcularPrecio()
        {
            var u = UsuarioService.UsuarioActual;
            if (u == null) return;

            decimal final = _precioBase;

            bool aplica20 = ReservaService.AplicaDescuentoFrecuencia(u.IdUsuario);
            FrameDescuentoFrecuencia.IsVisible = aplica20;
            if (aplica20) final -= (_precioBase * 0.20m);

            if (SwitchPuntos.IsToggled)
            {
                if (u.PuntosAcumulados >= 500)
                {
                    final -= 2000m;
                    if (final < 0) final = 0;
                }
                else
                {
                    DisplayAlert("Atención", $"Tenés {u.PuntosAcumulados} pts. Necesitás 500 pts.", "OK");
                    SwitchPuntos.IsToggled = false;
                    return;
                }
            }

            PrecioFinalLabel.Text = $"${final:N0}";
        }

        private async void OnConfirmarReservaClicked(object sender, EventArgs e)
        {
            var u = UsuarioService.UsuarioActual;
            if (u == null || HorarioPicker.SelectedIndex == -1)
            {
                await DisplayAlert("Atención", "Seleccioná un horario.", "OK");
                return;
            }

            DateTime fechaSeleccionada = FechaPicker.Date ?? DateTime.Today;

            var r = ReservaService.CrearReserva(u, NombreCanchaLabel.Text, fechaSeleccionada, _precioBase, SwitchPuntos.IsToggled);
            await DisplayAlert("¡Éxito!", $"Reserva lista. Total: ${r.PrecioFinal:N0}. +100 puntos ganados.", "Genial");
            CalcularPrecio();
        }

        private async void OnVolverClicked(object sender, EventArgs e)
        {
            if (Navigation.NavigationStack.Count > 1)
            {
                await Navigation.PopAsync();
            }
            else
            {
                if (Application.Current != null)
                    Application.Current.MainPage = new NavigationPage(new CanchasPage());
            }
        }
    }
}