using timeorganizer.Models;
using System.Threading.Tasks; // Dodany brakuj¹cy using
using timeorganizer.Views.LoggedPages;

namespace timeorganizer.Views
{
    public partial class LoggedMainPage : FlyoutPage
    {
        public LoggedMainPage()
        {
            InitializeComponent();
            flyoutPage.collectionViewFlyout.SelectionChanged += OnSelectionChangedAsync;
        }

        async void OnSelectionChangedAsync(object sender, SelectionChangedEventArgs e)
        {
            var item = e.CurrentSelection.FirstOrDefault() as FlyoutPageItem;

            if (!((IFlyoutPageController)this).ShouldShowSplitMode)
                IsPresented = false;

            switch (item.Title)
            {
                case "Home":
                    Detail = new NavigationPage(new MainPage());
                    break;

                case "Calendar":
                    Detail = new NavigationPage(new CalendarPage());
                    break;

                case "ToDo":
                    Detail = new NavigationPage(new ToDoPage());
                    break;

                case "Statistics":
                    Detail = new NavigationPage(new StatisticsPage());
                    break;

                case "Settings":
                    Detail = new NavigationPage(new SettingsPage());
                    break;

                case "Notes":
                    Detail = new NavigationPage(new NotesPage());
                    break;


                case "Contacts":
                    Detail = new NavigationPage(new ContactsPage());
                    break;

                case "Instagram":
                    Detail = new NavigationPage(new InstagramPage());
                    break;




                case "Logout":
                    App.Current.MainPage = new AppShell(); // Wylogowanie - zmiana domyœlnego widoku

                    break;
            }
        }
    }
}
