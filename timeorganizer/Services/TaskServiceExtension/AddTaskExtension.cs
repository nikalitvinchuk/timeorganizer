using CommunityToolkit.Mvvm.ComponentModel;
using timeorganizer.DatabaseModels;

namespace timeorganizer.Services.TaskServiceExtension
{//dodawanie nowych zadań
	public partial class AddTaskExtension : ObservableObject
	{

		private string _name, _desc, _type, _status, _termin2;
		private int _userId, _relizedpr;
		private DateTime _termin = DateTime.Now;
		public string Name { get => _name; set => _name = value; }
		public string Description { get => _desc; set => _desc = value; }
		public string Typ { get => _type; set => _type = value; }
		public string Status { get => _status; set => _status = value; }
		//public int Id { get => _id; set => _id = value; }
		public int UserId { get => _userId; set => _userId = value; }
		public int Progress { get => _relizedpr; set => _relizedpr = value; }

		public string Modified;
		public DateTime Termin { get => _termin; set => _termin = value; }
		public string Termin2 { get => _termin2; set => _termin2 = value; }
		//public ICommand AddTaskCommand { private set; get; }

		private readonly DatabaseLogin _context;



		public AddTaskExtension()
		{
			Termin2 = DateTime.Now.ToString("dd.MM.yyyy");
            _context = new DatabaseLogin();
			//AddTaskCommand = new Command(AddTask);
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
			else
			{
				return 0;
			}

		}
		
		public async Task AddTask()
		{
			if (_userId == 0) _userId = await Getid();

			Status = "Aktywne";
			Modified = DateTime.Now.ToString("dd.MM.yyyy, HH:mm");
			Tasks Task = new()
			{
				Name = Name,
				Description = Description,
				Type = Typ,
				UserId = _userId,
				Status = Status,
				RealizedPercent = Progress,
				Updated = null,
				Created = DateTime.Now.ToLongDateString(),
				Termin = Termin.ToString("dd.MM.yyyy")
			};

			await ExecuteAsync(async () =>
			{
				if (await SecureStorage.Default.GetAsync("token") is null)
					throw new Exception("ERROR");
				var activityservice = new ActivityService(); //inicjalizacja do późniejszego wywołania ChangeExpirationDate

				List<string> list = new() { Name, Description, Typ};
				int i = 0;
				string nazwa = "";
				int j = 1;
				foreach (var wartosc in list)
				{
					if (string.IsNullOrEmpty(wartosc))
					{
						i = 1;
						nazwa = j switch
						{
							1 => "Tytuł",
							2 => "Opis",
							3 => "Typ",
							_ => "",
						};
						await activityservice.ChangeExpirationDateCommand(); //przedłużanie sesji - funkcja z ActivityService 
						await App.Current.MainPage.DisplayAlert("Błąd_Puste", $"Pole {nazwa} jest puste", "Ok");
						break;
					}
					else
					{
						if (wartosc.Length > 100)
						{
							i = 1;
							nazwa = j switch
							{
								1 => "Tytuł",
								2 => "Opis",
								3 => "Typ",
								_ => "",
							};
							await activityservice.ChangeExpirationDateCommand(); //przedłużanie sesji - funkcja z ActivityViewModel 
							await App.Current.MainPage.DisplayAlert("Za długie", $"Pole {nazwa} jest za długie. Pole może mieć maksymalnie wartość 100 znaków", "Ok");
							break;
						}
						j++;
					}
				}
				if (i == 0)
				{
					await _context.AddItemAsync<Tasks>(Task);
					await activityservice.ChangeExpirationDateCommand(); //przedłużanie sesji - funkcja z ActivityViewModel 
                    Name = string.Empty;
                    Description = string.Empty;
                    Typ = string.Empty;
                    await App.Current.MainPage.DisplayAlert("Succes", "Dodano zadanie do bazy", "Ok");
                    
                }
			});
		}

        [ObservableProperty]
        public bool _isBusy;
        private async Task ExecuteAsync(Func<Task> operation)
		{
            var activityservice = new ActivityService(); //inicjalizacja do późniejszego wywołania ChangeExpirationDate
            IsBusy = true;
            try {
                await operation?.Invoke();
            }
            catch (Exception ex) {
                await App.Current.MainPage.DisplayAlert("ERROR SQL", ex.Message, "Ok");
            }
            finally {
                IsBusy = false;
            }
        }
	}
}