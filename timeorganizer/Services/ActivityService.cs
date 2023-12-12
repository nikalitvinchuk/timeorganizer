using CommunityToolkit.Mvvm.ComponentModel;
using System.Diagnostics;
using System.Security.Claims;
using timeorganizer.DatabaseModels;
using timeorganizer.Services;
//using timeorganizer.Views;


namespace timeorganizer.Services
{
	public class ActivityService : ObservableObject
	{



        public static IServiceProvider ServiceProviders
      =>
#if WINDOWS10_0_17763_0_OR_GREATER
			MauiWinUIApplication.Current.Services;
#elif ANDROID
    MauiApplication.Current.Services;
#elif IOS || MACCATALYST
          MauiUIApplicationDelegate.Current.Services;
#else
            null;
#endif


        private readonly DatabaseLogin _context;
		private DateTime LastActivity { get; set; }
		private System.Timers.Timer timer;

		public ActivityService()
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

                        session.ExpirationDate = DateTime.Now.AddMinutes(3);
                        //session.ExpirationDate = DateTime.Now.AddSeconds(10);
                        LastActivity = DateTime.Now;
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
        [Obsolete]
        public async Task CheckLastActivityVsExpirationDate()
		{
			string userToken = await GetToken();

			var sessions = await _context.GetFileteredAsync<UserSessions>(t => t.Token == userToken);
			var session = sessions.FirstOrDefault();
			if (session != null)
			{
				if (session.Token == userToken)
				{
					DateTime timeNow = DateTime.Now;

					// Debug.WriteLine("ExpirationDate " + session.ExpirationDate);
					// Debug.WriteLine("Czas teraz " + timeNow);

					if (timeNow >= session.ExpirationDate)
					{
						Debug.WriteLine("TimeNow=ExpirationDate");
						SecureStorage.Default.Remove("token");
						timer.Enabled = false;
						timer.Stop();
                        Device.BeginInvokeOnMainThread(() =>
						{
							App.Current.MainPage.DisplayAlert("Nastąpiło wylogowanie", "Twoja sesja wygasła. Zaloguj się ponownie.", "Ok");
							logout();
						});

						return;
					}
					return;
				}
				return;
			}

		}


        
        private void logout()
        {
            var auth = ServiceProviders.GetRequiredService<AuthServiceSetUser>();

            var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity());

            auth.CurrentUser = authenticatedUser;
            App.Current.MainPage = new MainPage();
        }

    }









}
