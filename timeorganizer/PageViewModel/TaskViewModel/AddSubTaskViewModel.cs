using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Windows.Input;
using timeorganizer.DatabaseModels;

namespace timeorganizer.PageViewModels{
    public partial class AddSubTaskViewModel : ObservableObject{
        private string _name, _desc, _status;
        private int _userId, _tid;
        public string Name { get => _name; set => _name = value; }
        public string Description { get => _desc; set => _desc = value; }
        public string Status { get => _status; set => _status = value; }
        public int TaskId { get => _tid; set => _tid = value; } 
        public int UserId { get => _userId; set => _userId = value; }
        private bool TaskComplited; 
        private bool IsActive;
        public ICommand AddSubTaskCommand { private set; get; }

        private readonly DatabaseLogin _context;
        public AddSubTaskViewModel(){
            _context = new DatabaseLogin();
            AddSubTaskCommand = new Command(AddSubTask);
        }
        private async void Getid(){
            string _tokenvalue = await SecureStorage.Default.GetAsync("token");
            var getids = await _context.GetAllAsync<UserSessions>();
            if (getids.Any(t => t.Token == _tokenvalue)) // mo�na dodac EXPIRYDATE POZNIEJ i weryfikowac czy mo�na wykona� operacje !
            {
                var getid = getids.First(t => t.Token == _tokenvalue);
                _userId = getid.UserId;
            }
        }
        [ObservableProperty]
        private bool _isBusy;
        //                  FUNKCJA DODAWANIA PODZADANIA - powinno by� w osobnym modelu -JB
        private async void AddSubTask(object obj){
            TaskComponents TC = new(){
                Name = Name
                ,Description = Description
                ,TaskId = TaskId
                ,UserId = _userId
                ,Status = Status
                ,TaskComplited = false
                ,IsActive = true
                ,LastUpdated = DateTime.Now
            };

            await ExecuteAsync(async () =>{
                List<string> list = new List<string> { Name, Description, Status };
                int i = 1;
                string nazwa;
                foreach (var wartosc in list){
                    if (string.IsNullOrEmpty(wartosc)){
                        i = 1;
#pragma warning disable CS8509
                        nazwa = wartosc switch {
                            nameof(Name) => "Nazwa zadania"
                            ,nameof(Description) => "Opis zadania"
                        };
                    }
                    else
                        i = 0;
                }
                if (i == 1){
                    await App.Current.MainPage.DisplayAlert("Failed", "Jedno z lub wiele p�l podzadania jest puste", "Ok");
                }
                else
                    await _context.AddItemAsync<TaskComponents>(TC);
            });
        }
        private async Task ExecuteAsync(Func<Task> operation){
            IsBusy = true;
#pragma warning disable CS0168 // Zmienna jest zadeklarowana, ale nie jest nigdy u�ywana
            try {
                await operation?.Invoke();
            }
            catch (Exception ex) {}
#pragma warning restore CS0168 // Zmienna jest zadeklarowana, ale nie jest nigdy u�ywana
            finally {
                IsBusy = false;
            }
        }
    }
}