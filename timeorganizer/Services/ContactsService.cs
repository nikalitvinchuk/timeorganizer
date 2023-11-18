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
            // Tutaj mo�esz u�y� _context do wys�ania wiadomo�ci do bazy danych lub wywo�a� odpowiednie metody.

            // Przyk�adowa operacja wysy�ania wiadomo�ci do bazy danych

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


            // Dodatkowo, je�li chcesz obs�u�y� odpowied� z bazy danych, mo�esz to zrobi� tutaj.
        }
        // else
        // {
        // Obs�uga b��d�w, na przyk�ad wy�wietlenie komunikatu u�ytkownikowi.
        // DisplayAlert("Wype�nij wszystkie pola przed wys�aniem wiadomo�ci.");
        //}
    }
}