using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using timeorganizer.DatabaseModels;
using timeorganizer.PageViewModel;
using timeorganizer.Views;

namespace timeorganizer.PageViewModel
{
    public class ActivityViewModel : ObservableObject
    {
        private readonly DatabaseLogin _context;
        public ActivityViewModel()
        {
            _context = new DatabaseLogin();
        }


        private int _id; // zmienna ustalona z user session z pomoca SecureStorge

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

        //pobranie i zwrócenie tokenu
        private async Task<string> GetToken()
        {
            return await SecureStorage.Default.GetAsync("token");
        }


        //funkcja po wywołaniu której sesja jest przedłużana - NL
        public async Task ChangeExpirationDateCommand()
        {
            Debug.WriteLine("Metoda ChangeExpirationDateCommand rozpoczęta.");

            int userId = await Getid();
            string userToken = await GetToken();

            Debug.WriteLine("Pobrano token użytkownika: " + userToken);
            if (userId != 0)
            {
                Debug.WriteLine("Uzyskano ID użytkownika.");

                var sessions = await _context.GetFileteredAsync<UserSessions>(t => t.UserId == userId);
                foreach (var session in sessions)
                {
                    if (session.Token == userToken)
                    {
                        Debug.WriteLine("Pobrano sesję użytkownika: " + userId);

                        session.ExpirationDate = DateTime.Now.AddMinutes(7).ToString("dd-MM-yyyy HH:mm:ss");
                        await _context.UpdateItemAsync<UserSessions>(session);

                        Debug.WriteLine("Zaktualizowano datę wygaśnięcia sesji.");
                        return;
                    }
                }

                Debug.WriteLine("Sesja użytkownika nie została pobrana lub token jest niezgodny.");
            }
            else
            {
                Debug.WriteLine("Brak ID użytkownika.");
            }
        }



    }
}
