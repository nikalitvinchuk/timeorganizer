using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.AspNetCore.Components;
using System.Collections.ObjectModel;
using System.Windows.Input;
using timeorganizer.DatabaseModels;

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

            //ShowTasks = new Command(FilterTasks);
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
            //var activityViewModel = new ActivityViewModel(); //inicjalizacja do późniejszego wywołania ChangeExpirationDate

            if (_userId == 0) { _userId = await Getid(); }
            //List<string> Lista_zapytan = new();
            //List<string> ListaZmienych = new() { "Name", "Description", "Type", "Status", "Created" };
            await ExecuteAsync(async () =>
            {
                //foreach (string Zmienna in ListaZmienych) {
                //    var wartosc = GetType().GetProperty(Zmienna)?.GetValue(this, null)?.ToString();
                //    if (!string.IsNullOrEmpty(wartosc)) {
                //        Lista_zapytan.Add($"{Zmienna} = '{wartosc}'");
                //    }
                //}
                //var query = "SELECT * FROM Tasks WHERE UserId = " + _userId;
                //if (Lista_zapytan.Count > 0) {
                //    query += " AND " + string.Join(" AND ", Lista_zapytan);
                //}
                //var tasks = await _context.GetByQuery<Tasks>(query);
                //
                //

                //_created = CreatedD.ToString("dd.MM.yyyy, HH:mm");

                // ZMIENIONE TAK ABY KORZYSTALO Z GOTOWEJ FUNKCJI :)  - JB
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
                filters.Clear();
                OnPropertyChanged(nameof(TasksCollection));
                

            });
            //await activityViewModel.ChangeExpirationDateCommand(); //przedłużanie sesji - funkcja z ActivityViewModel 
            
        }

        private async Task ExecuteAsync(Func<Task> operation)
        {
            //var activityViewModel = new ActivityViewModel(); //inicjalizacja do późniejszego wywołania ChangeExpirationDate

            IsBusy = true;
            try
            {
                await operation?.Invoke();
            }
            catch (Exception ex)
            {
               // await activityViewModel.ChangeExpirationDateCommand(); //przedłużanie sesji - funkcja z ActivityViewModel 
                await App.Current.MainPage.DisplayAlert("ERROR SQL", ex.Message, "Ok");

            }
            finally
            {
                IsBusy = false;
            }
           

        }



    }

}