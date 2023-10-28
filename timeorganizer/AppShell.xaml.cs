using timeorganizer.DatabaseModels;
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
            //Routing.RegisterRoute("Calendar", typeof(CalendarPage)); //dodanie działania na stronie instagram ??
            //Routing.RegisterRoute("ToDo", typeof(ToDotPage)); //???
            //Routing.RegisterRoute("Notes", typeof(NotesPage));
            //Routing.RegisterRoute("Statistics", typeof(StatisticsPage));
            //Routing.RegisterRoute("Contacts", typeof(ContactsPage));
            //Routing.RegisterRoute("Instargram", typeof(InstargramPage));
            //Routing.RegisterRoute("Settings", typeof(SettingsPage));
            //Routing.RegisterRoute("Logout", typeof(LogoutPage));
            InitializeComponent();
        }
    }
}