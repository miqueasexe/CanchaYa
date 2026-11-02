using System;
using Microsoft.Maui.Controls;
using LeClubAppp.Services;

namespace LeClubAppp.Views
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            string email = txtEmail.Text?.Trim() ?? string.Empty;
            string pass = txtPassword.Text?.Trim() ?? string.Empty;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(pass))
            {
                await DisplayAlert("Atención", "Ingresá tu correo y contraseña.", "OK");
                return;
            }

            var resultado = UsuarioService.IniciarSesion(email, pass);

            if (resultado.Exito)
            {
                // Limpia la pila y establece CanchasPage como pantalla raíz única
                if (Application.Current != null)
                {
                    Application.Current.MainPage = new NavigationPage(new CanchasPage());
                }
            }
            else
            {
                await DisplayAlert("Error", resultado.Mensaje, "OK");
            }
        }

        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RegistroPage());
        }
    }
}