using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Input;
using timeorganizer.DatabaseModels;
using timeorganizer.PageViewModel;

namespace timeorganizer.PageViewModels
{//dodawanie nowych zadań
    public partial class AddTaskViewModel : ObservableObject
    {

        private string _name, _desc, _type, _status;
        private int _userId, _relizedpr;
        private DateTime _termin;
        public string Name { get => _name; set => _name = value; }
        public string Description { get => _desc; set => _desc = value; }
        public string Typ { get => _type; set => _type = value; }
        public string Status { get => _status; set => _status = value; }
        //public int Id { get => _id; set => _id = value; }
        public int UserId { get => _userId; set => _userId = value; }
        public int Progress { get => _relizedpr; set => _relizedpr = value; }

        public string Modified;
        public DateTime Termin { get => _termin; set => _termin = value; }
        public ICommand AddTaskCommand { private set; get; }

        private readonly DatabaseLogin _context;

        

        public AddTaskViewModel()
        {
            _context = new DatabaseLogin();
            AddTaskCommand = new Command(AddTask);
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

        private async void AddTask(object obj)
        {
            if (_userId == 0) _userId = await Getid();
            Status = "Act";
            Modified = DateTime.Now.ToString("dd.MM.yyyy, HH:mm");
            Tasks Task = new()
            {
                Name = Name
                ,
                Description = Description
                ,
                Type = Typ
                ,
                UserId = _userId
                ,
                status = Status
                ,
                RealizedPercent = Progress
                ,
                Updated = null,
                Created = DateTime.Now.ToLongDateString(),
                Termin = Termin.ToString("dd.MM.yyyy")
            };
            TaskComponents SubTask = new()
            {
                Name = Name
                ,
                Description = "Test"
                ,
                TaskId = 12
                ,
                UserId = 9
            };

            await ExecuteAsync(async () =>
            {
                var activityViewModel = new ActivityViewModel(); //inicjalizacja do późniejszego wywołania ChangeExpirationDate

                List<string> list = new() { Name, Description, Typ, Status, UserId.ToString(), Progress.ToString() };
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
                        await activityViewModel.ChangeExpirationDateCommand(); //przedłużanie sesji - funkcja z ActivityViewModel 
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
                            await activityViewModel.ChangeExpirationDateCommand(); //przedłużanie sesji - funkcja z ActivityViewModel 
                            await App.Current.MainPage.DisplayAlert("Za długie", $"Pole {nazwa} jest za długie. Pole może mieć maksymalnie wartość 200 znaków", "Ok");
                            break;
                        }
                        j++;
                    }
                }

                if (i == 0)
                {
                    await _context.AddItemAsync<Tasks>(Task);
                    await _context.AddItemAsync<TaskComponents>(SubTask);
                    await activityViewModel.ChangeExpirationDateCommand(); //przedłużanie sesji - funkcja z ActivityViewModel 
                    await App.Current.MainPage.DisplayAlert("Succes", "Dodano zadanie do bazy", "Ok");
                    
                }

            });
        }
        //private async void AddTaskComponent(object obj)
        //{
        //    TaskComponents TC = new()
        //    {
        //        Name = NameU
        //        ,
        //        Description = DescriptionU
        //        ,
        //        TaskId = TaskId //?????
        //        ,
        //        UserId = _userIdU //?????
        //        ,
        //        Status = StatusU
        //        ,
        //        TaskComplited = false
        //        ,
        //        IsActive = true
        //        ,
        //        LastUpdated = DateTime.Now
        //    };

        //    await ExecuteAsync(async () =>
        //    {
        //        List<string> list = new List<string> { NameU, DescriptionU, StatusU };
        //        int i = 1;
        //        foreach (var wartosc in list)
        //        {
        //            if (string.IsNullOrEmpty(wartosc))
        //            {
        //                i = 1; break;
        //            }
        //            else
        //                i = 0;
        //        }
        //        if (i == 1)
        //        {
        //            await App.Current.MainPage.DisplayAlert("Failed", "Jedno z lub wiele pól podzadania jest puste", "Ok");
        //        }
        //        else
        //            await _context.AddItemAsync<TaskComponents>(TC);
        //    });
        //}
        private async Task ExecuteAsync(Func<Task> operation)
        {
            var activityViewModel = new ActivityViewModel(); //inicjalizacja do późniejszego wywołania ChangeExpirationDate
            IsBusy = true;
            try
            {
                await operation?.Invoke();
            }
            catch (Exception ex)
            {
                await activityViewModel.ChangeExpirationDateCommand(); //przedłużanie sesji - funkcja z ActivityViewModel 
                await App.Current.MainPage.DisplayAlert("ERROR SQL", ex.Message, "Ok");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}