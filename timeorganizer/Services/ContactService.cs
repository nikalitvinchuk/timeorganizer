using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using timeorganizer.DatabaseModels;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Diagnostics;

namespace timeorganizer.Service
{
    public class ContactService : ObservableObject
    {
        private string _emailValue, _reportValue;
        private int _userId;

        public string EmailValue { get => _emailValue; set => _emailValue = value; }
        public string ReportValue { get => _reportValue; set => _reportValue = value; }
        public int UserId { get => _userId; set => _userId = value; }


        private bool _emailError;
        private bool _reportError;


        public bool EmailError
        {
            get => _emailError;
            set => SetProperty(ref _emailError, value);
        }
        public bool ReportError
        {
            get => _reportError;
            set => SetProperty(ref _reportError, value);
        }



        public ICommand MessageCommand { get; private set; }

        private readonly DatabaseLogin _context;

        public ContactService()
        {
            _context = new DatabaseLogin();
            MessageCommand = new Command(SendMessage);
        }

        private async Task<int> Getid()
        {
            string _tokenvalue = await SecureStorage.Default.GetAsync("token");
            var getids = await _context.GetFileteredAsync<UserSessions>(t => t.Token == _tokenvalue);
            if (getids.Any(t => t.Token == _tokenvalue))
            {
                var getid = getids.First(t => t.Token == _tokenvalue);
                return getid.UserId;
            }
            else
            {
                return 0;
            }

        }

        private async void SendMessage(object obj)
        {
            if (!isValidInput())
            {
                //wyswietli komunikat w zaleznoœci od wystepuj¹cego b³edu w podaniu danych
                return;
            }
            if (_userId == 0) _userId = await Getid();

            try
            {
                Message newMessage = new()
                {
                    Email = _emailValue,
                    UserId = _userId,
                    MessageText = _reportValue,
                    DateCreate = DateTime.Now.ToString(),
                    Status = "new"
                };

                await _context.AddItemAsync<Message>(newMessage);
                Application.Current.MainPage.DisplayAlert("Thank You", "Report was sent", "OK");

                EmailValue = "";
                ReportValue = "";
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Wyst¹pi³ b³¹d podczas dodawania wiadomoœci do bazy danych: {ex.Message}");
            }
        }

        private bool isValidInput()
        {
            EmailError = string.IsNullOrEmpty(EmailValue) || !isValidEmail(EmailValue);
            ReportError = string.IsNullOrEmpty(ReportValue) || ReportValue.Length < 15;

            if (EmailError)
            {
                Application.Current.MainPage.DisplayAlert("B³¹d", "Wpisz poprawny email", "OK");
                return false;
            }
            else if (ReportError)
            {
                Application.Current.MainPage.DisplayAlert("B³¹d", "Wpisz minimum 15 znaków", "OK");
                return false;
            }
            return true;
        }

        private bool isValidEmail(string email) 
        {
            try
            {
                var newmail = new System.Net.Mail.MailAddress(EmailValue);
                return newmail.Address == email; //jeslli powodzenia zwraca email
            }
            catch
            {
                return false;
            }
        }

    }
}
