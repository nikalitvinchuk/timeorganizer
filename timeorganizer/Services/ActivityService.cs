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
		private int userid = 0;
		public ActivityService()
		{
			_context = new DatabaseLogin();
			timer = new System.Timers.Timer(5000); //wywołanie co sekunde 

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

		private UserSessions session = new UserSessions();
		//funkcja po wywołaniu której sesja jest przedłużana - NL
		//musi być sprawdzana od początku po zalogowaniu! - do rozbudowy
		public async Task ChangeExpirationDateCommand()
		{

			timer.Elapsed += async (sender, e) => await CheckLastActivityVsExpirationDate();
			timer.Enabled = true;
			Debug.WriteLine("Metoda ChangeExpirationDateCommand rozpoczęta.");
			if(userid== 0)
			{
                userid = await Getid();
            }
			string userToken = await GetToken();

			Debug.WriteLine("Pobrano token użytkownika: " + userToken);
			if (userid != 0)
			{
				Debug.WriteLine("Uzyskano ID użytkownika");

				var sessions = await _context.GetFileteredAsync<UserSessions>(t => t.UserId == userid);
				foreach (var session in sessions)
				{
					if (session.Token == userToken)
					{
						Debug.WriteLine("Pobrano sesję użytkownika: " + userid);

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

		private async Task GetExpirationDate(string token)
		{
			var tmp = await _context.GetFileteredAsync<UserSessions>(t => t.Token == token);
			session = tmp.FirstOrDefault();
        }
        //funkcja do automatycznego wylogowania - dok{onczyc
        [Obsolete]
        public async Task CheckLastActivityVsExpirationDate()
		{
			string userToken = await GetToken();
			if(session == null || session.ExpirationDate <= DateTime.Now) 
			{
				await GetExpirationDate(userToken);
			}
			
			if (session != null)
			{
				if (session.Token == userToken)
				{
					DateTime timeNow = DateTime.Now;


					if (timeNow >= session.ExpirationDate)
					{
						
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
