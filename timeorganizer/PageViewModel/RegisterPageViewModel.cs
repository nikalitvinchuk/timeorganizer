using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Input;
using timeorganizer.DatabaseModels;

namespace timeorganizer.PageViewModels
{// przykladowa rejestracja z wykorzystaniem dostepu do bazy danych, zrobilem bo mogloby byc wam ciezko ogarnac, nie ma zrobionej weryfikacji poprawnosci itd
    //trzeba bedzie dorobic.
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
            Users User = new()
            {
                Email = Email,
                Password = Password,
                Id = Id,
                Login = Login,
                DataCreated = DateTime.Now,
                RememberMe = false
            };

            await ExecuteAsync(async () =>
            {
                if (User.Id == 0)
                {
                    // Create product
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

            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}