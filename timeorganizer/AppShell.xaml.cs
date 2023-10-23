using timeorganizer.Views;

namespace timeorganizer
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            Routing.RegisterRoute("RegisterPage", typeof(RegisterPage));
            Routing.RegisterRoute("LoginPage", typeof(LoginPage));
            Routing.RegisterRoute("MainPage", typeof(MainPage));
            InitializeComponent();
        }
    }
}