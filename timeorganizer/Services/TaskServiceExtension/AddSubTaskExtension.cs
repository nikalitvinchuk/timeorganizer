using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;
using timeorganizer.DatabaseModels;


namespace timeorganizer.Services.TaskServiceExtension
{
	public partial class AddSubTaskExtension : ObservableObject
	{
		private string _name, _desc, _status;
		private int _userId, _tid;
		public string Name { get => _name; set => _name = value; }
		public string Description { get => _desc; set => _desc = value; }
		public string Status { get => _status; set => _status = value; }
		public int TaskId { get => _tid; set => _tid = value; }
		public int UserId { get => _userId; set => _userId = value; }
		public bool TaskComplited;
		public bool IsActive;
		public Tasks Zadanie { get; set; }
		public Collection<TaskComponents> PodZadanie { get; set; }
		private readonly DatabaseLogin _context;
		public AddSubTaskExtension(){
			_context = new DatabaseLogin();
		}
		public async Task<int> GetTask(){
			await ExecuteAsync(async () => { 
				Zadanie = await _context.GetItemByKeyAsync<Tasks>(_tid);
                var temp = await _context.GetFileteredAsync<TaskComponents>(e => e.TaskId == Zadanie.Id);
                PodZadanie = new ObservableCollection<TaskComponents>(temp);
			});
			return 0;
		}
		private async void Getid()
		{
			string _tokenvalue = await SecureStorage.Default.GetAsync("token");
			var getids = await _context.GetAllAsync<UserSessions>();
			if (getids.Any(t => t.Token == _tokenvalue)) // mo¿na dodac EXPIRYDATE POZNIEJ i weryfikowac czy mo¿na wykonaæ operacje !
			{
				var getid = getids.First(t => t.Token == _tokenvalue);
				_userId = getid.UserId;
			}
		}
		[ObservableProperty]
		private bool _isBusy;
		//                  FUNKCJA DODAWANIA PODZADANIA - powinno byæ w osobnym modelu -JB
		public async Task AddSubTask()
		{
			Getid();
			TaskComponents TC = new()
			{
				Name = Name,
				Description = Description,
				TaskId = TaskId,
				UserId = _userId,
				Status = Status,
				TaskComplited = false,
				IsActive = true,
				Created = DateTime.Now.ToLongDateString(), // Przy dodadawniu powinno byc Created a nie Updated
				LastUpdated = null
			};
			await ExecuteAsync(async () =>
			{
				await _context.AddItemAsync<TaskComponents>(TC);
                await App.Current.MainPage.DisplayAlert("Uda³o siê", "Uda³o siê", "Ok");
            });
		}
		private async Task ExecuteAsync(Func<Task> operation)
		{
			//var activityViewModel = new ActivityViewModel(); //inicjalizacja do póŸniejszego wywo³ania ChangeExpirationDate
			IsBusy = true;
			try
			{
				await operation?.Invoke();
			}
			catch (Exception ex)
			{
				//await activityViewModel.ChangeExpirationDateCommand(); //przed³u¿anie sesji - funkcja z ActivityViewModel 
				await App.Current.MainPage.DisplayAlert("ERROR SQL", ex.Message, "Ok");
			}

			finally
			{
				IsBusy = false;
			}
		}
	}
}