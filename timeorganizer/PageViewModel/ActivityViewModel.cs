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
        private string LastActivity { get; set; }
        private System.Timers.Timer timer;

        public ActivityViewModel()
        {
            _context = new DatabaseLogin();
            timer = new System.Timers.Timer(1000); //wywołanie co sekunde 

        }
 // zmienna ustalona z user session z pomoca SecureStorge

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
        //musi być sprawdzana od początku po zalogowaniu! - do rozbudowy
        public async Task ChangeExpirationDateCommand()
        {

            timer.Elapsed += async (sender, e) => await CheckLastActivityVsExpirationDate();
            timer.Enabled = true;
            Debug.WriteLine("Metoda ChangeExpirationDateCommand rozpoczęta.");

            int userId = await Getid();
            string userToken = await GetToken();
            
            Debug.WriteLine("Pobrano token użytkownika: " + userToken);
            if (userId != 0)
            {
                Debug.WriteLine("Uzyskano ID użytkownika");

                var sessions = await _context.GetFileteredAsync<UserSessions>(t => t.UserId == userId);
                foreach (var session in sessions)
                {
                    if (session.Token == userToken)
                    {
                        Debug.WriteLine("Pobrano sesję użytkownika: " + userId);

                        session.ExpirationDate = DateTime.Now.AddMinutes(10).ToString("dd-MM-yyyy HH:mm:ss");
                        LastActivity = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                        Debug.WriteLine("w change mamy lastactivity: " + LastActivity);
                        await _context.UpdateItemAsync<UserSessions>(session);

                        Debug.WriteLine("Zaktualizowano datę wygaśnięcia sesji.");
                        return;
                    }
                }

                Debug.WriteLine("Sesja użytkownika nie została pobrana lub token jest niezgodny.");
                return;
            }
            else
            {
                Debug.WriteLine("Brak ID użytkownika.");
                return;
            }
        }


        //funkcja do automatycznego wylogowania - dokonczyc
        public async Task CheckLastActivityVsExpirationDate()
        {
            string userToken = await GetToken();

                var sessions = await _context.GetFileteredAsync<UserSessions>(t => t.Token == userToken);
                var session = sessions.FirstOrDefault();
            if (session != null)
            {
                if (session.Token == userToken)
                {
                    var timeNow = DateTime.Now;

                    Debug.WriteLine("ExpirationDate " + session.ExpirationDate);
                    Debug.WriteLine("Czas teraz " + timeNow);

                    if (timeNow >= DateTime.Parse(session.ExpirationDate))
                    {
                        Debug.WriteLine("TimeNow=ExpirationDate");
                        SecureStorage.Default.Remove("token");
                        timer.Enabled = false;
                        timer.Stop();
                        return;
                    }
                    return;
                }
                return;
            }

        }

    }
}
