using CommunityToolkit.Mvvm.ComponentModel;
using System.Text;
using System.Windows.Input;
using timeorganizer.DatabaseModels;



namespace timeorganizer.PageViewModel
{
    public partial class SettingsViewModel : ObservableObject
    {
        private readonly DatabaseLogin _context;

        public SettingsViewModel()
        {
            _context = new DatabaseLogin();
            ChangePassword = new Command(ChangePasswordCommand);
            ChangeEmail = new Command(ChangeEmailCommand);
            ChangeEmailAndPassword = new Command(ChangeAllCommand);
            DeleteAccount = new Command(DeleteAccountCommand);


        }
        private string _email, _password, _login, _passwordconfirm, _currentpassword;
        private int _id; // zmienna ustalona z user session z pomoca SecureStorge

        public string Email { get => _email; set => _email = value; }
        //public string Login { get => _login; set => _login = value; } - LOGIN NIEMOZLIWY DO ZMIANY 
        public string Password { get => _password; set => _password = value; }
        public string ConfirmPassword { get => _passwordconfirm; set => _passwordconfirm = value; }
        public string CurrentPassword { get => _currentpassword; set => _currentpassword = value; }

        [ObservableProperty]
        private bool _IsBusy = false;
        // KOMENDY DO WYWOLANIA W DANEJ KOMENDZIE - NA DANA METODE POWINNO OTWIERAC SIE NOWE OKNO Z ODPOWIEDNIMI
        // POLAMI DO UZUPELNIENIA 
        public ICommand ChangePassword { private set; get; }
        public ICommand ChangeEmail { private set; get; }
        public ICommand ChangeEmailAndPassword { private set; get; }
        public ICommand DeleteAccount { private set; get; }

        // WALIDACJA HASEL

        //POBRANIE ID Z SESJI 
        private async Task<int> Getid()
        {
            string _tokenvalue = await SecureStorage.Default.GetAsync("token");
            var getids = await _context.GetFileteredAsync<UserSessions>(t => t.Token == _tokenvalue);
            if (getids.Any(t => t.Token == _tokenvalue))
            {
                var getid = getids.First(t => t.Token == _tokenvalue);
                return getid.UserId;
            }
            else { return 0; }
        }
        private async Task<bool> validatepassword()
        {
            if (string.IsNullOrWhiteSpace(_currentpassword)) return false;
            if (_currentpassword.Length == 0) return false;
            if (string.IsNullOrWhiteSpace(_password)) return false;
            if (_password.Length == 0) return false;
            if (string.IsNullOrWhiteSpace(_passwordconfirm)) return false;
            if (_passwordconfirm.Length == 0) return false;
            Users user = new Users();
            await ExecuteAsync(async () =>
            {
                user = await _context.GetItemByKeyAsync<Users>(_id);
            });
            if (user == null) return false;
            if (user.Password != CurrentPassword) return false;
            if (user.Password == Password) return false;
            if (_password != _passwordconfirm) return false;

            return true;
        }

        // WALIDACJA EMAIL
        private bool validateEmail()
        {
            if (string.IsNullOrWhiteSpace(_email)) return false;
            if (_email.Length == 0) return false;
            if (!_email.Contains('@') || !_email.Contains('.')) return false;

            return true;
        }

        // ZMIANA EMAIL
        private async void ChangeEmailCommand()
        {
            await ExecuteAsync(async () =>
            {
                if (_id == 0) _id = await Getid();

                Users user = new Users();
                user = await _context.GetItemByKeyAsync<Users>(_id);
                if (validateEmail())
                {
                    user.Email = _email;
                    user.DataModified = (DateTime.Now).ToLongDateString();
                    await _context.UpdateItemAsync<Users>(user);
                }
            }
            );
            
        }
        // ZMIANA HASLA
        private async void ChangePasswordCommand()

        {
            await ExecuteAsync(async () =>
            {
                if (_id == 0) _id = await Getid();
                Users user = new Users();
                user = await _context.GetItemByKeyAsync<Users>(_id);
                if (await validatepassword())
                {
                    user.Password = _password;
                    user.DataModified = (DateTime.Now).ToLongDateString();
                    await _context.UpdateItemAsync(user);
                }
            }
            );
            
        }
        // USUWANIE KONTA
        private async void DeleteAccountCommand()
        {
            await ExecuteAsync(async () =>
            {
                if (_id == 0) _id = await Getid();
                Users user = new Users();
                user = await _context.GetItemByKeyAsync<Users>(_id);
                if (_passwordconfirm == user.Password)
                {
                    await _context.DeleteItemAsync<Users>(user);
                }
                App.Current.MainPage = new AppShell();
            }
            );
            
        }
        // ZMIANA HASLA I EMAIL
        private void ChangeAllCommand()
        {

            ChangeEmailCommand();
            ChangePasswordCommand();
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
    }
}
