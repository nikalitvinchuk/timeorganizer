using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Windows.Input;
using timeorganizer.DatabaseModels;
using CommunityToolkit.Mvvm;

namespace timeorganizer.Services;

public partial class ContactsService : ObservableObject
{
    private string _email, _message;
    public string Email { get => _email; set => _email = value; }
    public string Message { get => _message; set => _message = value; }
    public ICommand SendCommand { get; private set; }

    private readonly DatabaseLogin _context;

    public ContactsService(DatabaseLogin context)
    {
        _context = context;
        SendCommand = new Command(SendMessage);
    }

    private async void SendMessage()
    {
        if (!string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(Message))
        {
            // Tutaj mo¿esz u¿yæ _context do wys³ania wiadomoœci do bazy danych lub wywo³aæ odpowiednie metody.

            // Przyk³adowa operacja wysy³ania wiadomoœci do bazy danych

            Message newMessage = new Message()
            {
                Email = Email,
                MessageText = Message
            };

            // await _context.SendMessageAsync(newMessage);

            //{
            //    Email = string.Empty;
            //    Message = string.Empty;
            //}


            // Dodatkowo, jeœli chcesz obs³u¿yæ odpowiedŸ z bazy danych, mo¿esz to zrobiæ tutaj.
        }
        // else
        // {
        // Obs³uga b³êdów, na przyk³ad wyœwietlenie komunikatu u¿ytkownikowi.
        // DisplayAlert("Wype³nij wszystkie pola przed wys³aniem wiadomoœci.");
        //}
    }
}