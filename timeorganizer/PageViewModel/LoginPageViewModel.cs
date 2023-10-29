using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Windows.Input;
using timeorganizer.DatabaseModels;
using timeorganizer.Views;

namespace timeorganizer.PageViewModels
{
    public partial class LoginPageViewModel
    {
        private string _login, _password;
        public string Login { get => _login; set => _login = value; }
        public string Password { get => _password; set => _password = value; }

        public ICommand LoginCommand { get; private set; }

        private readonly DatabaseLogin _context;

        public string LoginValue { get; set; } // Właściwość do przechowywania danych wprowadzonych w polu Login
        public string PassValue { get; set; } // Właściwość do przechowywania danych wprowadzonych w polu Password

        public LoginPageViewModel(DatabaseLogin context)
        {
            _context = context;
            LoginCommand = new Command(LoginUser);

            LoginValue = string.Empty;
            PassValue = string.Empty;
        }

        private async void LoginUser(object obj)
        {
            if (!isValidEntry())
            {
                //w przypadku braku poprawnych danych - alert
                return;
            }

            var users = await _context.GetFileteredAsync<Users>(u => u.Login == LoginValue); //pobrana lista użytkowników zgodnych z LoginValue
            var user = users.FirstOrDefault(); //pobieramy pierwszy element który pasuje
            if (user != null && user.Password == PassValue)
            {
                await Application.Current.MainPage.DisplayAlert("Sukces", "Zalogowano pomyślnie", "OK");
                App.Current.MainPage = new LoggedMainPage(); // zmiana domyslnego widoku na widok flyout
            }
            else
            {
                //komunikat o błędnych danych logowania
                await Application.Current.MainPage.DisplayAlert("Błąd", "Niepoprawny login lub hasło", "OK");
            }
        }

        private bool isValidEntry() //obsługa w przypadku braku podanych danych
        {
            if (string.IsNullOrEmpty(PassValue) && string.IsNullOrEmpty(LoginValue))
            {
                Application.Current.MainPage.DisplayAlert("Błąd", "Nie podałeś loginu i hasła", "OK");
                return false;
            }

            else if (string.IsNullOrEmpty(LoginValue))
            {
                Application.Current.MainPage.DisplayAlert("Błąd", "Nie podałeś loginu", "OK");
                return false;
            }
            else if (string.IsNullOrEmpty(PassValue))
            {
                Application.Current.MainPage.DisplayAlert("Błąd", "Nie podałeś hasła", "OK");
                return false;
            }
            return true;
        }

    }
}
