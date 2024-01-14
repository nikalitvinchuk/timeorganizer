using CommunityToolkit.Mvvm.Input;
using Microsoft.AspNetCore.Components.Authorization;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using timeorganizer.DatabaseModels;
using timeorganizer;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Security.Claims;

namespace timeorganizer.Services
{
	public class LoginService 
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
        private string _login, _password;

		private int _id;
		public string Login { get => _login; set => _login = value; }
		public string Email { get; private set; }
		public string DateCreate { get; private set; }
		public string Password { get => _password; set => _password = value; }
        private readonly DatabaseLogin _context;
        public string LoginValue { get; set; } // Właściwość do przechowywania danych wprowadzonych w polu Login
		public string PassValue { get; set; } // Właściwość do przechowywania danych wprowadzonych w polu Password
		public ICommand LoginCommand;
		public LoginService()
		{
			_context = new DatabaseLogin();
			LoginValue = string.Empty;
			PassValue = string.Empty;
			LoginCommand = new RelayCommand(LoginUser);
		}


		public async void LoginUser()
		{
			
			var activityservice = new ActivityService(); //inicjalizacja do późniejszego wywołania ChangeExpirationDate

			if (!isValidEntry())
			{
				//w przypadku braku poprawnych danych - alert
				return ;
			}

			var users = await _context.GetFileteredAsync<Users>(u => u.Login == LoginValue); //pobrana lista użytkowników zgodnych z LoginValue
			var user = users.FirstOrDefault(); //pobieramy pierwszy element który pasuje
			if (user != null && Helpers.Passwordhash.Veryfypass(PassValue, user.Password))
			{

               
                //-------------------------OBSŁUGA SESJI-----------------------------
                string sessionToken = Guid.NewGuid().ToString(); //unikalny token sesji (GUID)
				DateTime dateCreated = DateTime.Now;
                DateTime expirationDate = DateTime.Now.AddMinutes(3);
                //DateTime expirationDate = DateTime.Now.AddSeconds(10);

                //dodanie tokena do PAMIECI ABY BYL DO NIEGO DOSTEP -JB 
                await SecureStorage.Default.SetAsync("token", sessionToken);
                //dodawanie danych do tabeli
                var session = new UserSessions
				{

					UserId = user.Id,
					Token = sessionToken,
					DateCreated = dateCreated,
					ExpirationDate = expirationDate,
					DateCreatedText = dateCreated.ToString("yyyy-MM-dd HH:mm:ss"),
					ExpirationDateText = expirationDate.ToString("yyyy-MM-dd HH:mm:ss")
				};
				await _context.AddItemAsync<UserSessions>(session); // NALEZY DOROBIC PRZEDLUZANIE SESJI I AUTOMATYCZNE WYLOGOWANIE 
																	// w przeciwnym wypadku nie korzystamy z expirationDate
																	//  po uplywie tego czasu powinno wylogowac- JB

				Debug.WriteLine("Obsługa sesji - OK");
				App.Current.MainPage = new MainPage();
				//-----------------------KONIEC OBSŁUGI SESJI-------------------------
				var auth = ServiceProviders.GetRequiredService<AuthServiceSetUser>();

                var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
        		new Claim(ClaimTypes.Role, "Administrator"),
        		new Claim(ClaimTypes.NameIdentifier, LoginValue)}, "Basic"));

                auth.CurrentUser = authenticatedUser;
                await activityservice.ChangeExpirationDateCommand(); //przedłużanie sesji - funkcja z ActivityViewModel 
				await activityservice.CheckLastActivityVsExpirationDate(); //przedłużanie sesji - funkcja z ActivityViewModel 

				Email = user.Email;
				Login = user.Login;
				DateCreate = user.DataCreated;
			}
			else
			{
				//komunikat o błędnych danych logowania
				await Application.Current.MainPage.DisplayAlert("Błąd", "Niepoprawny login lub hasło", "OK");
				
			}
			LoginValue = "";
			PassValue = "";

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
			else { return 0; }
		}

		private bool isValidEntry() //obsługa w przypadku braku podanych danych
		{
			if (string.IsNullOrEmpty(PassValue) && string.IsNullOrEmpty(LoginValue))
			{
				Application.Current.MainPage.DisplayAlert("Błąd", "Nie podałeś loginu i hasła", "OK");
				return false;
			}

			else if (string.IsNullOrEmpty(LoginValue))
			{
				Application.Current.MainPage.DisplayAlert("Błąd", "Nie podałeś loginu", "OK");
				return false;
			}
			else if (string.IsNullOrEmpty(PassValue))
			{
				Application.Current.MainPage.DisplayAlert("Błąd", "Nie podałeś hasła", "OK");
				return false;
			}
			return true;
		}
		public async Task InfoUser()
		{
			_id = await Getid();
			Users user = new Users();
			user = await _context.GetItemByKeyAsync<Users>(_id);
			if (user != null)
			{
				Email = user.Email;
				Login = user.Login;
			}
			else
			{
				Email = string.Empty;
				Login = string.Empty;
			}
		}
	}
}
