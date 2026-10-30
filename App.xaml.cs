using Microsoft.Maui.Controls;
using LeClubAppp.Views;

namespace LeClubAppp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new NavigationPage(new LoginPage()));
        }
    }
}