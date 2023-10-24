using timeorganizer.Views;

namespace timeorganizer
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            //rejestracja sciezek dostepowych w menu glownym pozostale sciezki sa obzlugiwane przez flyoutpagemenu
            Routing.RegisterRoute("RegisterPage", typeof(RegisterPage));
            Routing.RegisterRoute("LoginPage", typeof(LoginPage));
            Routing.RegisterRoute("MainPage", typeof(MainPage));
            InitializeComponent();
        }
    }
}