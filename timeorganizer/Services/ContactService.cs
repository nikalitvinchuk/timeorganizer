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

namespace timeorganizer.Service
{
    public class ContactService : ObservableObject
    {
        private string _emailValue, _reportValue;

        public string EmailValue { get => _emailValue; set => _emailValue = value; }
        public string ReportValue { get => _reportValue; set => _reportValue = value; }

        public ICommand MessageCommand { get; private set; }

        private readonly DatabaseLogin _context;

        public ContactService()
        {
            _context = new DatabaseLogin();
            MessageCommand = new RelayCommand(SendMessage);
        }
        private async void SendMessage()
        {
            Message newMessage = new()
            {
                Email = _emailValue,
                MessageText = _reportValue,
                DateCreate = DateTime.Now.ToString(),
                Status = "new"
            };

            await _context.AddItemAsync<Message>(newMessage);

            EmailValue = "";
            ReportValue = "";

        }

    }
}