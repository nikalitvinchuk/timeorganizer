using Intents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using timeorganizer.DatabaseModels;
using VideoToolbox;

namespace timeorganizer.PageViewModel
{
     public partial class SettingsViewModel
    {
        private readonly DatabaseLogin _context;

        public SettingsViewModel(DatabaseLogin database)
        {
            _context = database;
            ChangePassword = new Command(ChangePasswordCommand);
            ChangeEmail = new Command(ChangeEmailCommand);
            ChangeEmailAndPassword = new Command(ChangeAllCommand);
            DeleteAccount = new Command(DeleteAccountCommand);
        }
        private string _email, _password, _login, _passwordconfirm, _currentpassword;
        private int _id; // zmienna powinna byc ustawiona przez z tabeli UsersSession
        public string Email { get => _email; set => _email = value; }
        //public string Login { get => _login; set => _login = value; } - LOGIN NIEMOZLIWY DO ZMIANY 
        public string Password { get => _password; set => _password = value; }
        public string ConfirmPassword { get => _passwordconfirm; set => _passwordconfirm = value; }
        public string CurrentPassword { get => _currentpassword; set => _currentpassword = value; }
        public int Id { get => _id; set => _id = value; }

        // KOMENDY DO WYWOLANIA W DANEJ KOMENDZIE - NA DANA METODE POWINNO OTWIERAC SIE NOWE OKNO Z ODPOWIEDNIMI
        // POLAMI DO UZUPELNIENIA 
        public ICommand ChangePassword { private set; get; } 
        public ICommand ChangeEmail { private set; get; }
        public ICommand ChangeEmailAndPassword { private set; get; }
        public ICommand DeleteAccount { private set; get; }

        // WALIDACJA HASEL
        private bool validatepassword()
        {
            if (string.IsNullOrWhiteSpace(_currentpassword)) return false;
            if (_currentpassword.Length == 0) return false;
            if (string.IsNullOrWhiteSpace(_password)) return false;
            if (_password.Length == 0) return false;
            if (string.IsNullOrWhiteSpace(_passwordconfirm)) return false;
            if (_passwordconfirm.Length == 0) return false;
            Users user = new Users();
            user = _context.GetItemByKeyAsync<Users>(_id).Result ;
            if (user == null) return false;
            if (user.Password != CurrentPassword) return false;
            if (user.Password ==  Password) return false;
            if (_password != _passwordconfirm) return false;
       
            return true;
        }

        // WALIDACJA EMAIL
        private bool validateEmail()
        {
            if( string.IsNullOrWhiteSpace(_email)) return false;
            if (_email.Length == 0) return false;
            if (!_email.Contains('@') || !_email.Contains('.')) return false;

            return true;
        }

        // ZMIANA EMAIL
        private async void ChangeEmailCommand()
        {
            Users user = new Users();
            user = _context.GetItemByKeyAsync<Users>( _id).Result ;
            if(validateEmail())
            {
                user.Email = _email;
                user.DataModified = (DateTime.Now).ToLongDateString();
                await _context.UpdateItemAsync<Users>(user);
            }
        }
        // ZMIANA HASLA
        private async void ChangePasswordCommand()
        {
            Users user = new Users();
            user = _context.GetItemByKeyAsync<Users>(_id).Result;
            if (validatepassword())
            {
                user.Password = _password;
                user.DataModified = (DateTime.Now).ToLongDateString();
                await _context.UpdateItemAsync(user);
            }
        }
        // USUWANIE KONTA
        private async void DeleteAccountCommand()
        {
            Users user = new Users();
            user = _context.GetItemByKeyAsync<Users>(_id).Result;
            if (_passwordconfirm == user.Password)
            {
                await _context.DeleteItemAsync<Users>(user);
            }

        }
        // ZMIANA HASLA I EMAIL
        private void ChangeAllCommand()
        {
             ChangeEmailCommand();
             ChangePasswordCommand();
        }
    }
}
