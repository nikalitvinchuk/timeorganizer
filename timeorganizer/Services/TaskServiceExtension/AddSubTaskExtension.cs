using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Input;
using timeorganizer.DatabaseModels;
using timeorganizer.PageViewModel;

namespace timeorganizer.PageViewModels
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
        public ObservableCollection<Tasks> Zadanie { get; set; }
        public ICommand AddSubTaskCommand { private set; get; }
        public ICommand GetTaskComm { private set; get; }
        private readonly DatabaseLogin _context;

        public AddSubTaskExtension(){
            _context = new DatabaseLogin();     
            AddSubTaskCommand = new Command(AddSubTask);
            GetTaskComm = new RelayCommand(GetTask);
        }
        private async void GetTask() {
            await ExecuteAsync(async () => {
                var temp = await _context.GetItemByKeyAsync<Tasks>(_tid);
                Zadanie = new ObservableCollection<Tasks> { temp };
            });
        }
        private async void Getid(){
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
        private async void AddSubTask(object obj)
        {
            Getid();
            TaskComponents TC = new()
            {
                Name = Name
                ,Description = Description
                ,TaskId = TaskId
                ,UserId = _userId
                ,Status = Status
                ,TaskComplited = false
                ,IsActive = true
                ,Created = DateTime.Now.ToLongDateString(), // Przy dodadawniu powinno byc Created a nie Updated
                LastUpdated = null
            };

            await ExecuteAsync(async () =>
            {
                List<string> list = new List<string> { Name, Description, Status };
                int i = 1;
                string nazwa;
                foreach (var wartosc in list)
                {
                    if (string.IsNullOrEmpty(wartosc))
                    {
                        i = 1;
#pragma warning disable CS8509
                        nazwa = wartosc switch
                        {
                            nameof(Name) => "Nazwa zadania"
                            ,
                            nameof(Description) => "Opis zadania"
                        };
                    }
                    else
                        i = 0;
                }
                if (i == 1)
                {
                    //await activityViewModel.ChangeExpirationDateCommand(); //przed³u¿anie sesji - funkcja z ActivityViewModel 
                    await App.Current.MainPage.DisplayAlert("Failed", "Jedno z lub wiele pól podzadania jest puste", "Ok");
                }
                else
                    //await activityViewModel.ChangeExpirationDateCommand(); //przed³u¿anie sesji - funkcja z ActivityViewModel 
                    await _context.AddItemAsync<TaskComponents>(TC);
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
            catch (Exception ex) {
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