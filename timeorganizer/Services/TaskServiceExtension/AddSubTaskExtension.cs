using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using timeorganizer.DatabaseModels;


namespace timeorganizer.Services.TaskServiceExtension
{
	public partial class AddSubTaskExtension : ObservableObject
	{
        //tworzenie zmiennych prywatnych kt�re b�d� przechowywa� warto�ci z p�l frontendu i b�d� u�ywane  w operacjach w kodzie backendowym 
        private string _name, _desc, _status; 
		private int _userId, _tid;
        // tworzenie zmiennych publicznych kt�re b�d� wi�zane z polami frontendu
        // { get => _name; set => _name = value; } tworzenie powi�zania aktywnego zmiennych publicznych z zmiennymi prywatnymi
        public string Name { get => _name; set => _name = value; }
		public string Description { get => _desc; set => _desc = value; }
		public string Status { get => _status; set => _status = value; }
		public int TaskId { get => _tid; set => _tid = value; }
		public Tasks Zadanie { get; set; } // zmienna Zadanie jest typu klasy Tasks kt�ra to klasa jest naszym modelem tabeli
        public Collection<TaskComponents> PodZadanie { get; set; } // Collection<> zmienna kt�a pozwala przechowywa� wiele pozycji danego typu, w tym wyhttps://0.0.0.0/loginpadku naszej klasy modelu tabeli pod zada� TaskComponents
		private readonly DatabaseLogin _context; //zmienna _context typu DatabaseLogin (jest to klasa umo�yliwiaj�ca wykonywanie operacji na naszej bazie, posiada stworzone przez kub� funkcj� pozwalaj�ce wykonywa� konkretne operacje na bazie z zakresu CRUD) 
		//konstruktor wewn�trz kt�ego tworzymy obiekt klast DatabaseLogin i przypisujemy go do zmienne _context, umo�liwa to wywo�anie operacji na bazie w kodzie za pomoc�	   _context.NazwaFunkcji
		public AddSubTaskExtension(){
			_context = new DatabaseLogin();
		}
		//Funkcja stworzona w celu uzyskania listy zada� z bazy i przypisanie do zmiennych
		public async Task GetTask(){
			await ExecuteAsync(async () => { 
				Zadanie = await _context.GetItemByKeyAsync<Tasks>(_tid);
                var temp = await _context.GetFileteredAsync<TaskComponents>(e => e.TaskId == Zadanie.Id && (e.Status == "Aktywne" || e.Status == "Ukonczone"));
                PodZadanie = new ObservableCollection<TaskComponents>(temp);
			});
			//zwraca zero poniewa� funkcja nie jest void wi�c musi posiada� w sobie return a jej wynik nie przypisujemy do �adnego konkretnej zmiennej poniewa� robimy to wewn�trz metody, funkcja jest typu int wi�c zwracmy liczb� w tym wypadku 0
		}
        //Funkcja stworzona w celu uzyskania id obecnie zalogowanego u�ytkownika
        private async void Getid()
		{
			string _tokenvalue = await SecureStorage.Default.GetAsync("token");
			var getids = await _context.GetAllAsync<UserSessions>();
			if (getids.Any(t => t.Token == _tokenvalue)) // mo�na dodac EXPIRYDATE POZNIEJ i weryfikowac czy mo�na wykona� operacje !
			{
				var getid = getids.First(t => t.Token == _tokenvalue);
				_userId = getid.UserId;
			}
            /// <summary>
            /// ta funkcja jest typu void i nie musi nic zwraca�, to zale�y czy u�ywamy typuu void czy nie od tego jak chcemy wywo��� funkcj�, je�li chcemy u�y� ja z operatorem 'await' czyli operatorem kt�ry oznacza czekaj na uko�czenie funkcji, to taka funkcja nie mo�e posiada� typu void i musi by� typu Task, je�li chcemy dodatkowo aby ta funkcja zwraca�a warto�� to deklaracja b�zie ywgldac tak Task<tu_wstaw_typ_zmiennej_jaki_ma_by�_zwracany>
            /// Natomiast je�li chcemy WEWN�TRZ jakiej� funkcji wywo�a� jak�� inn� funkcj� z operatorem await to dodatkowo ta funkcja musi mie� operator async
            /// </summary>
        }
        //zmienna obserwowalna, pozwala nam okre�li� kiedy po��czenie do bazy jest zaj�te(czyli wykonywanie jest na niej operacja)
        [ObservableProperty]
		private bool _isBusy;

		public async Task AddSubTask()
		{
			Getid();
			TaskComponents TC = new()
			{
				Name = Name,
				Description = Description,
				TaskId = TaskId,
				UserId = _userId,
				Status = "Aktywne",
				IsActive = true,
				Created = DateTime.Now.ToLongDateString(), 
				LastUpdated = null
			};
			await ExecuteAsync(async () =>
			{
				await _context.AddItemAsync<TaskComponents>(TC);
                await App.Current.MainPage.DisplayAlert("Uda�o si�", "Uda�o si�", "Ok");
				Name = "";
				Description = "";
            });
		}
		//funkcja dzi�ki kt�rej mo�emy wywo�a� jaki� fragment kodu a je�li ten napotka jaki� b��d w sobie to zamiast zacina� ca�� aplikacj� to wy�wietli nam b��d i poka�e tre�� b��du
		private async Task ExecuteAsync(Func<Task> operation)
		{
			//var activityViewModel = new ActivityViewModel(); //inicjalizacja do p�niejszego wywo�ania ChangeExpirationDate
			IsBusy = true;
			try{
				await operation?.Invoke();
			}
			catch (Exception ex){
				//await activityViewModel.ChangeExpirationDateCommand(); //przed�u�anie sesji - funkcja z ActivityViewModel 
				await App.Current.MainPage.DisplayAlert("ERROR SQL", ex.Message, "Ok");
			}

			finally{
				IsBusy = false;
			}
		}
	}
}