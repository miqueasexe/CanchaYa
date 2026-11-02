using System;
using Microsoft.Maui.Controls;
using LeClubAppp.Services;

namespace LeClubAppp.Views
{
    public partial class RegistroPage : ContentPage
    {
        public RegistroPage()
        {
            InitializeComponent();
        }

        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            string nombre = txtNombre.Text?.Trim() ?? string.Empty;
            string email = txtEmail.Text?.Trim() ?? string.Empty;
            string pass = txtPassword.Text?.Trim() ?? string.Empty;

            if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(pass))
            {
                await DisplayAlert("Atención", "Completá todos los campos.", "OK");
                return;
            }

            var nuevoUsuario = UsuarioService.Registrar(nombre, email, pass);
            await DisplayAlert("¡Éxito!", "Cuenta creada correctamente.", "OK");

            // Redirige directamente al catálogo de Canchas Disponibles
            if (Application.Current != null)
            {
                Application.Current.MainPage = new NavigationPage(new CanchasPage());
            }
        }

        private async void OnBackToLoginClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}