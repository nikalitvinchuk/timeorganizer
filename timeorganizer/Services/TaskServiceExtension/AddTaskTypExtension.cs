using CommunityToolkit.Mvvm.ComponentModel;
using timeorganizer.DatabaseModels;

namespace timeorganizer.Services.TaskServiceExtension {
    public partial class AddTaskTypExtension : ObservableObject {
        private string _name, _status;
        private int _userId;
        public string Name { get => _name; set => _name = value; }
        public string Status { get => _status; set => _status = value; }
        private readonly DatabaseLogin _context;
        public AddTaskTypExtension() {
            _context = new DatabaseLogin();
        }
        private async Task<int> Getid() {
            string _tokenvalue = await SecureStorage.Default.GetAsync("token");
            var getids = await _context.GetFileteredAsync<UserSessions>(t => t.Token == _tokenvalue);
            if (getids.Any(t => t.Token == _tokenvalue)) {
                var getid = getids.First(t => t.Token == _tokenvalue);
                return getid.UserId;
            }
            else {
                return 0;
            }
        }
        public async Task AddTaskTyp() {
            _userId = await Getid();
            _status = "Aktywne";
            await ExecuteAsync(async () => {
                TaskTyp task = new() { 
                    Name= _name,
                    Status= _status,
                    UserId= _userId
                };
                await _context.AddItemAsync(task);
            });
        }

        [ObservableProperty]
        public bool _isBusy;
        private async Task ExecuteAsync(Func<Task> operation) {
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