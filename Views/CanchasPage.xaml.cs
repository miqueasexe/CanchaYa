using LeClubAppp.Models;
using LeClubAppp.Services;
using LeClubAppp.Views;
using Microsoft.Maui.Controls;
using System;

namespace LeClubAppp.Views
{
    public partial class CanchasPage : ContentPage
    {
        public CanchasPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            CanchasCollectionView.ItemsSource = CanchaService.ObtenerCanchas();
        }

        private async void OnReservarCanchaClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is Cancha cancha)
            {
                await Navigation.PushAsync(new ReservaPage(cancha));
            }
        }

        private async void OnVerMisReservasClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MisReservasPage());
        }

        private async void OnCerrarSesionClicked(object sender, EventArgs e)
        {
            bool salir = await DisplayAlert("Cerrar Sesión", "¿Querés salir a la pantalla de login?", "Sí", "No");
            if (salir)
            {
                UsuarioService.UsuarioActual = null;
                if (Application.Current != null)
                {
                    Application.Current.MainPage = new NavigationPage(new LoginPage());
                }
            }
        }
        private async void OnVerPerfilClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PerfilPage());
        }
    }
}