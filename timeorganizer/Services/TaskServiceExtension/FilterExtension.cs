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

	public partial class FilterExtension : ObservableObject
    {

		private string _name, _description, _typ, _status, _created, _termin;
		public int _priority, _prcomplited, _userId;
		private DateTime? _terminD;
		private List<string> _statStr = new() { null, "Ukonczono", "Aktywne" };

		public string Name { get => _name; set => _name = value; }
		public string Description { get => _description; set => _description = value; }
		public string Typ { get => _typ; set => _typ = value; }
		public List<string> StatusStr { get => _statStr; set => _statStr = value; }

		public int UserId;
		public string Termin { get => _termin; set => _termin= value; }
		public DateTime? TerminD { get => _terminD; set => _terminD= value; }
		public string Updated;
        public string Status { get => _status; set => _status = value; }

        private ObservableCollection<Tasks> _collection = new();
        public ObservableCollection<Tasks> TasksCollection
        {
            get => _collection;
            set => SetProperty(ref _collection, value);
        }
        private readonly DatabaseLogin _context;
		public FilterExtension()
		{
			_context = new DatabaseLogin();
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
			_termin = _terminD?.ToString("dd.MM.yyyy");
                await ExecuteAsync(async () =>
			{
                var activityservice = new ActivityService(); //inicjalizacja do późniejszego wywołania ChangeExpirationDate

                _userId = await Getid();
                var filters = new Dictionary<string, object>
				{
					{ "Userid", _userId }
				};
				if (!string.IsNullOrWhiteSpace(_name)) filters.Add("Name", _name);
				if (!string.IsNullOrWhiteSpace(_description)) filters.Add("Description", _description);
				if (!string.IsNullOrWhiteSpace(_typ)) filters.Add("Type", _typ);
				if (!string.IsNullOrWhiteSpace(_status)) {
					filters.Add("Status", _status);
				}
				else {
                    _status = "Aktywne";
                    filters.Add("Status", _status);
                }
				
				if (!string.IsNullOrWhiteSpace(_termin)) filters.Add("Termin", _termin);
                _collection = new ObservableCollection<Tasks>(await _context.GetFileteredAsync<Tasks>(_context.CreatePredicateToFiltred<Tasks>(filters)));
                filters.Clear();
				await activityservice.ChangeExpirationDateCommand(); //przedłużanie sesji - funkcja z ActivityService
			});
		}

		private async Task ExecuteAsync(Func<Task> operation){
			var activityservice = new ActivityService(); //inicjalizacja do późniejszego wywołania ChangeExpirationDate
			IsBusy = true;
			try{
				await operation?.Invoke();
			}
			catch (Exception ex){
                await App.Current.MainPage.DisplayAlert("ERROR SQL", ex.Message, "Ok");
            }
			finally{
				IsBusy = false;
			}
		}
	}
}