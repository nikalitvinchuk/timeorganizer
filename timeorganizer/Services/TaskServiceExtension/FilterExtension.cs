using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Windows.Input;
using timeorganizer.DatabaseModels;

namespace timeorganizer.Services.TaskServiceExtension
{

	public partial class FilterExtension : ObservableObject, INotifyPropertyChanged
    {

		private string _name, _description, _typ, _status, _created;
		private int _priority, _prcomplited, _userId;
		private DateTime _createD = DateTime.Now;

		public int Id { get; set; }
		public string Name { get => _name; set => _name = value; }
		public string Description { get => _description; set => _description = value; }
		public string Typ { get => _typ; set => _typ = value; }

		public int UserId;
		public string Created { get => _created; set => _created = value; }
		public DateTime CreatedD { get => _createD; set => _createD = value; }

		public string Updated;
		public string Status { get => _status; set => _status = value; }
		public bool IsDone { get; set; }
		public int Priority { get => _priority; set => _priority = value; }
		public int RealizedPercent { get; set; }




        public event PropertyChangedEventHandler PropertyChanged22;
        protected void RaisePropertyChanged(string propertyName)
            => PropertyChanged22?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        /// <summary>
        /// Set a property and raise a property changed event if it has changed
        /// </summary>
        protected bool SetProperty<T>(ref T property, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(property, value))
            {
                return false;
            }

            property = value;
            RaisePropertyChanged(propertyName);
            return true;
        }
        private ObservableCollection<Tasks> _collection = new();
        public ObservableCollection<Tasks> TasksCollection
        {
            get => _collection;
            set => SetProperty(ref _collection, value);
        }
        
        public ObservableCollection<TaskComponents> SubTasksCollection { get; set; }
		public ICommand MoveTask { private set; get; }

        private readonly DatabaseLogin _context;
		public FilterExtension()
		{
			_context = new DatabaseLogin();
			//FilterTasks();
        }

		private async Task<int> Getid()
		{
            var _tokenvalue = await SecureStorage.Default.GetAsync("token");
            var getids = await _context.GetFileteredAsync<UserSessions>(t => t.Token == _tokenvalue);
            if (getids.Any(t => t.Token == _tokenvalue))
			{
				var getid = getids.First(t => t.Token == _tokenvalue);
				return getid.UserId;
			}
			else { return 0; }
		}

		[ObservableProperty]
		public bool _isBusy;

		public async Task FilterTasks()
		{
			
			await ExecuteAsync(async () =>
			{
                var activityservice = new ActivityService(); //inicjalizacja do późniejszego wywołania ChangeExpirationDate

                if (_userId == 0)
                {

                    _userId = await Getid();
                }
                var filters = new Dictionary<string, object>
				{
					{ "Userid", _userId }
				};
				if (Id != 0) filters.Add("Id", Id);
				if (!string.IsNullOrWhiteSpace(_name)) filters.Add("Name", _name);
				if (!string.IsNullOrWhiteSpace(_description)) filters.Add("Description", _description);
				if (!string.IsNullOrWhiteSpace(_typ)) filters.Add("Type", _typ);
				if (!string.IsNullOrWhiteSpace(_status)) filters.Add("Status", _status);
				if (!string.IsNullOrWhiteSpace(_created)) filters.Add("Created", _created);
                //Tasks SubTask = new() {
                //    Name = "TestN",
                //    Description = "Test",
                //    UserId = _userId
                //};
                //await _context.AddItemAsync<Tasks>(SubTask);
                _collection = new ObservableCollection<Tasks>(await _context.GetFileteredAsync<Tasks>(_context.CreatePredicateToFiltred<Tasks>(filters)));
				

                filters.Clear();
				await activityservice.ChangeExpirationDateCommand(); //przedłużanie sesji - funkcja z ActivityService
			});
		}

		private async Task ExecuteAsync(Func<Task> operation)
		{
			var activityservice = new ActivityService(); //inicjalizacja do późniejszego wywołania ChangeExpirationDate

			IsBusy = true;
			try
			{
				await operation?.Invoke();
			}
			catch (Exception ex)
			{

			}
			finally
			{
				IsBusy = false;
			}

		}



	}

}