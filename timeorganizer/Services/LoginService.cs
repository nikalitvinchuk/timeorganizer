using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using timeorganizer.DatabaseModels;

namespace timeorganizer.Service
{
    public class LoginService
    {
        private static readonly string[] Summaries = new[]
      {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private string _login, _password;
        public string Login { get => _login; set => _login = value; }
        public string Password { get => _password; set => _password = value; }

        public ICommand LoginCommand { get; private set; }

        private readonly DatabaseLogin _context;

        public string LoginValue { get; set; } // Właściwość do przechowywania danych wprowadzonych w polu Login
        public string PassValue { get; set; } // Właściwość do przechowywania danych wprowadzonych w polu Password

        public LoginService()
        {
            _context = new DatabaseLogin();
            LoginCommand = new RelayCommand(LoginUser);

            LoginValue = string.Empty;
            PassValue = string.Empty;
        }
        public async void LoginUser()
        {
            if (!isValidEntry())
            {
                //w przypadku braku poprawnych danych - alert
                return;
            }

            var users = await _context.GetFileteredAsync<Users>(u => u.Login == LoginValue); //pobrana lista użytkowników zgodnych z LoginValue
            var user = users.FirstOrDefault(); //pobieramy pierwszy element który pasuje
            if (user != null && Helpers.Passwordhash.Veryfypass(PassValue, user.Password))
            {

                //-------------------------OBSŁUGA SESJI-----------------------------
                string sessionToken = Guid.NewGuid().ToString(); //unikalny token sesji (GUID)
                string dateCreated = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                string expirationDate = DateTime.Now.AddMinutes(10).ToString("dd-MM-yyyy HH:mm:ss");

                //dodanie tokena do PAMIECI ABY BYL DO NIEGO DOSTEP -JB 

                 await SecureStorage.Default.SetAsync("token", sessionToken);
                
                //dodawanie danych do tabeli
                var session = new UserSessions
                {

                    UserId = user.Id,
                    Token = sessionToken,
                    DateCreated = dateCreated,
                    ExpirationDate = expirationDate
                };
                await _context.AddItemAsync<UserSessions>(session); // NALEZY DOROBIC PRZEDLUZANIE SESJI I AUTOMATYCZNE WYLOGOWANIE 
                                                                    // w przeciwnym wypadku nie korzystamy z expirationDate
                                                                    //  po uplywie tego czasu powinno wylogowac- JB

                //-----------------------KONIEC OBSŁUGI SESJI-------------------------


                await Application.Current.MainPage.DisplayAlert("Sukces", "Zalogowano pomyślnie", "OK"); // zmiana domyslnego widoku na widok flyout
                App.Current.MainPage = new MainPageLogged();            
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
