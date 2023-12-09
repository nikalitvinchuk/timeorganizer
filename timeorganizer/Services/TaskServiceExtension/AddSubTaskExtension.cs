using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using timeorganizer.DatabaseModels;


namespace timeorganizer.Services.TaskServiceExtension
{
	public partial class AddSubTaskExtension : ObservableObject
	{
        //tworzenie zmiennych prywatnych które bêd¹ przechowywaæ wartoœci z pól frontendu i bêd¹ u¿ywane  w operacjach w kodzie backendowym 
        private string _name, _desc, _status; 
		private int _userId, _tid;
        // tworzenie zmiennych publicznych które bêd¹ wi¹zane z polami frontendu
        // { get => _name; set => _name = value; } tworzenie powi¹zania aktywnego zmiennych publicznych z zmiennymi prywatnymi
        public string Name { get => _name; set => _name = value; }
		public string Description { get => _desc; set => _desc = value; }
		public string Status { get => _status; set => _status = value; }
		public int TaskId { get => _tid; set => _tid = value; }
		public Tasks Zadanie { get; set; } // zmienna Zadanie jest typu klasy Tasks która to klasa jest naszym modelem tabeli
        public Collection<TaskComponents> PodZadanie { get; set; } // Collection<> zmienna któa pozwala przechowywaæ wiele pozycji danego typu, w tym wyhttps://0.0.0.0/loginpadku naszej klasy modelu tabeli pod zadañ TaskComponents
		private readonly DatabaseLogin _context; //zmienna _context typu DatabaseLogin (jest to klasa umo¿yliwiaj¹ca wykonywanie operacji na naszej bazie, posiada stworzone przez kubê funkcjê pozwalaj¹ce wykonywaæ konkretne operacje na bazie z zakresu CRUD) 
		//konstruktor wewn¹trz któego tworzymy obiekt klast DatabaseLogin i przypisujemy go do zmienne _context, umo¿liwa to wywo³anie operacji na bazie w kodzie za pomoc¹	   _context.NazwaFunkcji
		public AddSubTaskExtension(){
			_context = new DatabaseLogin();
		}
		//Funkcja stworzona w celu uzyskania listy zadañ z bazy i przypisanie do zmiennych
		public async Task GetTask(){
			await ExecuteAsync(async () => { 
				Zadanie = await _context.GetItemByKeyAsync<Tasks>(_tid);
                var temp = await _context.GetFileteredAsync<TaskComponents>(e => e.TaskId == Zadanie.Id && (e.Status == "Aktywne" || e.Status == "Ukonczone"));
                PodZadanie = new ObservableCollection<TaskComponents>(temp);
			});
			//zwraca zero poniewa¿ funkcja nie jest void wiêc musi posiadaæ w sobie return a jej wynik nie przypisujemy do ¿adnego konkretnej zmiennej poniewa¿ robimy to wewn¹trz metody, funkcja jest typu int wiêc zwracmy liczbê w tym wypadku 0
		}
        //Funkcja stworzona w celu uzyskania id obecnie zalogowanego u¿ytkownika
        private async void Getid()
		{
			string _tokenvalue = await SecureStorage.Default.GetAsync("token");
			var getids = await _context.GetAllAsync<UserSessions>();
			if (getids.Any(t => t.Token == _tokenvalue)) // mo¿na dodac EXPIRYDATE POZNIEJ i weryfikowac czy mo¿na wykonaæ operacje !
			{
				var getid = getids.First(t => t.Token == _tokenvalue);
				_userId = getid.UserId;
			}
            /// <summary>
            /// ta funkcja jest typu void i nie musi nic zwracaæ, to zale¿y czy u¿ywamy typuu void czy nie od tego jak chcemy wywo³¹æ funkcjê, jeœli chcemy u¿yæ ja z operatorem 'await' czyli operatorem który oznacza czekaj na ukoñczenie funkcji, to taka funkcja nie mo¿e posiadaæ typu void i musi byæ typu Task, jeœli chcemy dodatkowo aby ta funkcja zwraca³a wartoœæ to deklaracja bêzie ywgldac tak Task<tu_wstaw_typ_zmiennej_jaki_ma_byæ_zwracany>
            /// Natomiast jeœli chcemy WEWN¥TRZ jakiejœ funkcji wywo³aæ jak¹œ inn¹ funkcjê z operatorem await to dodatkowo ta funkcja musi mieæ operator async
            /// </summary>
        }
        //zmienna obserwowalna, pozwala nam okreœliæ kiedy po³¹czenie do bazy jest zajête(czyli wykonywanie jest na niej operacja)
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
                await App.Current.MainPage.DisplayAlert("Uda³o siê", "Uda³o siê", "Ok");
				Name = "";
				Description = "";
            });
		}
		//funkcja dziêki której mo¿emy wywo³aæ jakiœ fragment kodu a jeœli ten napotka jakiœ b³¹d w sobie to zamiast zacinaæ ca³¹ aplikacjê to wyœwietli nam b³¹d i poka¿e treœæ b³êdu
		private async Task ExecuteAsync(Func<Task> operation)
		{
			//var activityViewModel = new ActivityViewModel(); //inicjalizacja do póŸniejszego wywo³ania ChangeExpirationDate
			IsBusy = true;
			try{
				await operation?.Invoke();
			}
			catch (Exception ex){
				//await activityViewModel.ChangeExpirationDateCommand(); //przed³u¿anie sesji - funkcja z ActivityViewModel 
				await App.Current.MainPage.DisplayAlert("ERROR SQL", ex.Message, "Ok");
			}

			finally{
				IsBusy = false;
			}
		}
	}
}