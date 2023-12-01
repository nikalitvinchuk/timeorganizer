
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
//using Mopups.PreBaked.PopupPages.Login;
//using Mopups.Services;
using System.Windows.Input;
using timeorganizer.DatabaseModels;
namespace timeorganizer.Services
{
	public partial class SettingsService : ObservableObject
	{
		private readonly DatabaseLogin _context;


		public SettingsService()
		{
			_context = new DatabaseLogin();
			ChangePassword = new RelayCommand(ChangePasswordCommand_execute);
			ChangeEmail = new RelayCommand(ChangeEmailCommand_execute);
			ChangeEmailAndPassword = new RelayCommand(ChangeAllCommand_execute);
			DeleteAccount = new RelayCommand(DeleteAccountCommand);
		}
		private string _email, _password, _passwordconfirm, _currentpassword;
		private int _id; // zmienna ustalona z user session z pomoca SecureStorge

		public string Email { get => _email; set => _email = value; }
		//public string Login { get => _login; set => _login = value; } - LOGIN NIEMOZLIWY DO ZMIANY 
		public string Password { get => _password; set => _password = value; }
		public string ConfirmPassword { get => _passwordconfirm; set => _passwordconfirm = value; }
		public string CurrentPassword { get => _currentpassword; set => _currentpassword = value; }

		[ObservableProperty]
		private bool _IsBusy = false;
		// KOMENDY DO WYWOLANIA W DANEJ KOMENDZIE - NA DANA METODE POWINNO OTWIERAC SIE NOWE OKNO Z ODPOWIEDNIMI
		// POLAMI DO UZUPELNIENIA 
		public ICommand ChangePassword { private set; get; }
		public ICommand ChangeEmail { private set; get; }
		public ICommand ChangeEmailAndPassword { private set; get; }

		public ICommand DeleteAccount { private set; get; }

		// WALIDACJA HASEL


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
		private async Task<bool> validatepassword()
		{
			if (string.IsNullOrWhiteSpace(_currentpassword)) return false;
			if (_currentpassword.Length == 0) return false;
			if (string.IsNullOrWhiteSpace(_password)) return false;
			if (_password.Length == 0) return false;
			if (string.IsNullOrWhiteSpace(_passwordconfirm)) return false;
			if (_passwordconfirm.Length == 0) return false;
			Users user = new Users();
			await ExecuteAsync(async () =>
			{
				user = await _context.GetItemByKeyAsync<Users>(_id);
			});
			if (user == null) return false;
			if (!Helpers.Passwordhash.Veryfypass(_currentpassword, user.Password)) { await App.Current.MainPage.DisplayAlert("Błąd", "Błędne stare hasło, spróbuj ponownie", "Ok"); return false; }
			if (Helpers.Passwordhash.Veryfypass(_password, user.Password)) { await App.Current.MainPage.DisplayAlert("Błąd", "Nowe i stare hasło są identyczne, spróbuj ponownie", "Ok"); return false; }
			if (_password != _passwordconfirm) { await App.Current.MainPage.DisplayAlert("Błąd", "Hasła niezgodne", "Ok"); return false; }

			return true;
		}

		// WALIDACJA EMAIL
		private async Task<bool> validateEmail()
		{
			if (string.IsNullOrWhiteSpace(_email)) return false;
			if (_email.Length == 0) return false;
			if (!_email.Contains('@') || !_email.Contains('.'))
			{
				await App.Current.MainPage.DisplayAlert("Błąd", "Niepoprawy format email", "Ok");
				return false;
			}
			return true;
		}

		// ZMIANA EMAIL
		private async void ChangeEmailCommand_execute()
		{

			var activityViewModel = new ActivityService(); //inicjalizacja do późniejszego wywołania ChangeExpirationDate

			await ExecuteAsync(async () =>
			{
				if (_id == 0) _id = await Getid();

				Users user = new Users();
				user = await _context.GetItemByKeyAsync<Users>(_id);
				if (await validateEmail())
				{
					user.Email = _email;
					user.DataModified = (DateTime.Now).ToLongDateString();
					await _context.UpdateItemAsync<Users>(user);
					//await MopupService.Instance.PopAsync(true);
					await activityViewModel.ChangeExpirationDateCommand(); //przedłużanie sesji - funkcja z ActivityViewModel 
					await App.Current.MainPage.DisplayAlert("Success", "Dane zostały poprawnie zmienione", "Ok");
				}
				else
				{

				}
			}
			);

		}
		// ZMIANA HASLA
		private async void ChangePasswordCommand_execute()

		{
			var activityViewModel = new ActivityService(); //inicjalizacja do późniejszego wywołania ChangeExpirationDate
			await ExecuteAsync(async () =>
			{
				if (_id == 0) _id = await Getid();
				Users user = new Users();
				user = await _context.GetItemByKeyAsync<Users>(_id);
				if (await validatepassword())
				{
					user.Password = Helpers.Passwordhash.HashPassword(_password);
					user.DataModified = (DateTime.Now).ToLongDateString();
					await _context.UpdateItemAsync(user);
					//await MopupService.Instance.PopAsync(true);
					await activityViewModel.ChangeExpirationDateCommand(); //przedłużanie sesji - funkcja z ActivityViewModel 
					await App.Current.MainPage.DisplayAlert("Success", "Dane zostały poprawnie zmienione", "Ok");
				}
			}
			);

		}
		// USUWANIE KONTA
		private async void DeleteAccountCommand()
		{
			var activityViewModel = new ActivityService(); //inicjalizacja do późniejszego wywołania ChangeExpirationDate
			bool answer = await App.Current.MainPage.DisplayAlert("Usuwanie Konta", "Czy chcesz usunąć swoje konto?", "Tak", "Nie");
			if (answer)
			{
				await ExecuteAsync(async () =>
				{
					if (_id == 0) _id = await Getid();
					Users user = new Users();
					user = await _context.GetItemByKeyAsync<Users>(_id);
					if (_passwordconfirm == user.Password)
					{
						await _context.DeleteItemAsync<Users>(user);
					}
					await activityViewModel.ChangeExpirationDateCommand(); //przedłużanie sesji - funkcja z ActivityViewModel 
					App.Current.MainPage = new MainPage();
				}
				);
			}
		}
		// ZMIANA HASLA I EMAIL
		private async void ChangeAllCommand_execute()
		{
			var activityViewModel = new ActivityService(); //inicjalizacja do późniejszego wywołania ChangeExpirationDate
			await ExecuteAsync(async () =>
			{

				if (_id == 0) _id = await Getid();
				Users user = new Users();
				user = await _context.GetItemByKeyAsync<Users>(_id);
				if (await validatepassword() && await validateEmail())
				{
					user.Password = _password;
					user.Email = _email;
					user.DataModified = (DateTime.Now).ToLongDateString();
					await _context.UpdateItemAsync(user);
					// await MopupService.Instance.PopAsync(true);
					await activityViewModel.ChangeExpirationDateCommand(); //przedłużanie sesji - funkcja z ActivityViewModel 
					await App.Current.MainPage.DisplayAlert("Success", "Dane zostały poprawnie zmienione", "Ok");
				}
				else
				{
					await App.Current.MainPage.DisplayAlert("Błąd", "Niepoprawy format email", "Ok");
				}
			}
			);

		}

		private async Task ExecuteAsync(Func<Task> operation)
		{
			var activityViewModel = new ActivityService(); //inicjalizacja do późniejszego wywołania ChangeExpirationDate
			IsBusy = true;
			try
			{
				await operation?.Invoke();
			}
			catch (Exception ex)
			{
				await activityViewModel.ChangeExpirationDateCommand(); //przedłużanie sesji - funkcja z ActivityViewModel 
				await App.Current.MainPage.DisplayAlert("ERROR SQL", ex.Message, "Ok");
			}
			finally
			{
				IsBusy = false;
			}
		}
	}
}
