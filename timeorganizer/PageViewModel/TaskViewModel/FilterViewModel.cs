using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;
using timeorganizer.DatabaseModels;

namespace timeorganizer.PageViewModel {

    public partial class FilterViewModel : ObservableObject {

        private string _name, _description, _typ, _status, _created;
        private int _priority, _prcomplited, _userId;

        public int Id { get; set; }
        //public string Name { get => _name; set => _name = value; }
        public string Description { get => _description; set => _description = value; }
        public string Typ { get => _typ; set => _typ = value; }

        public int UserId;
        public string Created { get => _created; set => _created = value; }

        public string Updated;
        public string Status { get => _status; set => _status = value; } //tego nie mam w Tasks.cs
        public bool IsDone { get; set; }
        public int Priority { get => _priority; set => _priority = value; }
        public int RealizedPercent { get; set; }
        public ObservableCollection<Tasks> TasksCollection { get; set; }
        public ICommand ShowTasks { private set; get; }

        private readonly DatabaseLogin _context;
        public FilterViewModel() {
            _context = new DatabaseLogin();
            //ShowTasks = new Command(async () => await FilterTasksAsync());
            //ShowTasks = new Command(FilterTasks);
        }
        private async Task<int> Getid() {
            string _tokenvalue = await SecureStorage.Default.GetAsync("token");
            var getids = await _context.GetFileteredAsync<UserSessions>(t => t.Token == _tokenvalue);
            if (getids.Any(t => t.Token == _tokenvalue)) {
                var getid = getids.First(t => t.Token == _tokenvalue);
                return getid.UserId;
            }
            else { return 0; }
        }

        [ObservableProperty]
        private bool _isBusy;

        public async Task<ObservableCollection<Tasks>> FilterTasks() {
            if (_userId == 0) { _userId = await Getid(); }
            await ExecuteAsync(async () => {
                var tasks = await _context.GetFileteredAsync<Tasks>(x => x.UserId == _userId);
                TasksCollection = new ObservableCollection<Tasks>(tasks);
            });
            return TasksCollection;
        }
        private async Task ExecuteAsync(Func<Task> operation) {
            IsBusy = true;
            try {
                await operation?.Invoke();
            }
            catch (Exception ex) {
            }
            finally {
                IsBusy = false;
            }
        }
    }

}