using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.AspNetCore.Components;
using System.Collections.ObjectModel;
using System.Windows.Input;
using timeorganizer.DatabaseModels;
using timeorganizer.Service;

namespace timeorganizer.Services
{

    public partial class FilterExtension : ObservableObject
    {

        private string _name, _description, _typ, _status, _created;
        private int _priority, _prcomplited, _userId;
        private DateTime _createD=DateTime.Now;
        private ObservableCollection<Tasks> _filtered = new ();

        public int Id { get; set; }
        public string Name { get => _name; set => _name = value; }
        public string Description { get => _description; set => _description = value; }
        public string Typ { get => _typ; set => _typ = value; }

        public int UserId;
        public string Created { get => _created; set => _created = value; }
        public DateTime CreatedD { get => _createD; set =>_createD=value; }

        public string Updated;
        public string Status { get => _status; set => _status = value; } 
        public bool IsDone { get; set; }
        public int Priority { get => _priority; set => _priority = value; }
        public int RealizedPercent { get; set; }
        public ObservableCollection<Tasks> TasksCollection { get => _filtered; set => _filtered = value; }
        public ObservableCollection<TaskComponents> SubTasksCollection { get; set; }
        public ICommand ShowTasks { private set; get; }
        public ICommand MoveTask { private set; get; }

        private readonly DatabaseLogin _context;
        public FilterExtension()
        {
            _filtered = new();
            _context = new DatabaseLogin();
            ShowTasks = new RelayCommand(FilterTasks);
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

        [ObservableProperty]
        private bool _isBusy;

        public async void FilterTasks()
        {
            var activityservice = new ActivityService(); //inicjalizacja do późniejszego wywołania ChangeExpirationDate

            if (_userId == 0) { _userId = await Getid(); }
            await ExecuteAsync(async () =>
            {
                OnPropertyChanged(nameof(TasksCollection));
                var filters = new Dictionary<string, object>
                {
                    { "Userid", _userId }
                };
                if (Id !=0) filters.Add("Id", Id);
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
                TasksCollection = new ObservableCollection<Tasks>(await _context.GetFileteredAsync<Tasks>( _context.CreatePredicateToFiltred<Tasks>(filters)));
                await Task.Delay(1000);
                filters.Clear();
                OnPropertyChanged(nameof(TasksCollection));
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
                await App.Current.MainPage.DisplayAlert("Nastąpiło wylogowanie", ex.Message, "Ok");
                App.Current.MainPage = new MainPage();

            }
            finally
            {
                IsBusy = false;
            }
          
        }



    }

}