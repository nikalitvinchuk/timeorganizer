using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Input;
using timeorganizer.DatabaseModels;

namespace timeorganizer.PageViewModels
{// przykladowa rejestracja z wykorzystaniem dostepu do bazy danych
    public partial class RegisterPageViewModel : ObservableObject
    {
        private string _email, _password, _password2, _login;
        int _id;
        public string Email { get => _email; set => _email = value; }
        public string Login { get => _login; set => _login = value; }
        public string Password { get => _password2; set => _password2 = value; }
        public string PasswordVerification { get => _password; set => _password = value; }
        public int Id { get => _id; set => _id = value; }
        public ICommand RegisterCommand { private set; get; }


        //zmienne które przekazujemy do registerpage
        private bool _emailError;
        private bool _loginError;
        private bool _passwordError;
        private bool _passwordVerificationError;

        //odświeżenie danych w przypadku zmian
        public bool EmailError
        {
            get => _emailError;
            set => SetProperty(ref _emailError, value);
        }
        public bool LoginError
        {
            get => _loginError;
            set => SetProperty(ref _loginError, value);
        }
        public bool PasswordError
        {
            get => _passwordError;
            set => SetProperty(ref _passwordError, value);
        }
        public bool PasswordVerificationError
        {
            get => _passwordVerificationError;
            set => SetProperty(ref _passwordVerificationError, value);
        }




        private readonly DatabaseLogin _context;
        public RegisterPageViewModel(DatabaseLogin context)
        {
            _context = context;
            RegisterCommand = new Command(CreateUser);
        }

        [ObservableProperty]
        private bool _isBusy;


        private async void CreateUser(object obj)
        {
            if (!isValidInput()) //metoda do weryfikacji poprawności danych
            {
                //wyswietli komunikat w zalezności od wystepującego błedu w podaniu danych
                return;
            }

            var allUser = await _context.GetFileteredAsync<Users>(user => user.Login == Login || user.Email == Email); //pobieramy wszystkie rekordy z users
            if (allUser.Count() > 0) //jeśli którys z elementów jest taki sam wyswietli sie alert
            {
                await Application.Current.MainPage.DisplayAlert("Błąd", "Login już istnieje w bazie, wpisz inny", "OK");
                return;
            }
            Email = "";
            Password = "";
            Login = "";

            Users User = new()
            {
                Email = _email,
                Password = _password,
                Id = Id,
                Login = _login,
                DataCreated = (DateTime.Now).ToLongDateString(),
                RememberMe = false,
                DataModified = null
            };

            await ExecuteAsync(async () =>
            {
                if (User.Id == 0)
                {
                    //{Przykłąd użycie filtra:  
                    //var lista = await _context.GetFileteredAsync<Users>(x => x.Login == Login && x.Id == Id);
                    // } -Greg92
                    await _context.AddItemAsync<Users>(User);

                    await Shell.Current.GoToAsync("LoginPage");
                }
                else
                { }
            });

        }


        private async Task ExecuteAsync(Func<Task> operation)
        {
            IsBusy = true;
            try
            {
                await operation?.Invoke();
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("ERROR SQL", ex.Message, "Ok");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private bool isValidInput() //metoda sprawdza poprawnosc danych, zwraca true lub false wraz z komunikatem oraz przypisuje konkretne błedy do przekazania
        {
            //przypisanie błędów
            EmailError = string.IsNullOrEmpty(Email) || !isValidEmail(Email);
            LoginError = string.IsNullOrEmpty(Login) || Login.Length < 4;
            PasswordError = string.IsNullOrEmpty(Password) || Password.Length < 6;
            PasswordVerificationError = Password != PasswordVerification;

            //komunikaty+zwrócenie wartości true lub false
            if (EmailError)
            {
                Application.Current.MainPage.DisplayAlert("Błąd", "Wpisz poprawny email", "OK");
                return false;
            }
            else if (LoginError)
            {
                Application.Current.MainPage.DisplayAlert("Błąd", "Długość znaków w podanym loginie nie może być mniejsza niż 4", "OK");
                return false;
            }
            else if (PasswordError)
            {
                Application.Current.MainPage.DisplayAlert("Błąd", "Długość hasła nie może być mniejsza niż 6", "OK");
                return false;
            }
            else if (PasswordVerificationError)
            {
                Application.Current.MainPage.DisplayAlert("Błąd", "Hasła w obu polach się nie pokrywają", "OK");
                return false;
            }

            return true;
        }

        private bool isValidEmail(string email) //metoda do sprawdzenia poprawności podanego maila;
                                                //tworzy obiekt w systemie System.Net.Mail.MailAddress, jesli powodzenie, poprawny mail
        {
            try
            {
                var newmail = new System.Net.Mail.MailAddress(email);
                return newmail.Address == email; //jeslli powodzenia zwraca email
            }
            catch
            {
                return false;
            }
        }

    }
}