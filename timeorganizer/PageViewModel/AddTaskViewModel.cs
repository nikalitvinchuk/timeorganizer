using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Input;
using timeorganizer.DatabaseModels;

namespace timeorganizer.PageViewModels
{// przykladowa rejestracja z wykorzystaniem dostepu do bazy danych, zrobilem bo mogloby byc wam ciezko ogarnac, nie ma zrobionej weryfikacji poprawnosci itd
    //trzeba bedzie dorobic.
    public partial class AddTaskViewModel : ObservableObject
    {
        private string _name, _desc, _type, _status;
        private int _userId, _relizedpr, _id;
        public string Name { get => _name; set => _name= value; }
        public string Description { get => _desc; set => _desc= value; }
        public string Typ { get => _type; set => _type= value; }
        public string Status{ get => _status; set => _status = value; }
        public int Id { get => _id; set => _id= value; }
        public int UserId { get => _userId; set => _userId = value; }
        public int Progress { get => _relizedpr; set => _relizedpr= value; }

        private DateTime Modified;
        public ICommand RegisterCommand { private set; get; }

        //                  ZMIENNE DLA DODANIA PODZADANIA

        private string _nameU, _descU, _statusU;
        private int _userIdU,  _tid;
        public string NameU { get => _nameU; set => _nameU = value; }
        public string DescriptionU { get => _descU; set => _descU = value; }
        public string StatusU { get => _statusU; set => _statusU = value; }
        public int TaskId { get => _tid; set => _tid = value; } //Pobierane skąd???? Wcześniej klikniętego zadania???Musi zostać tu przekazane przy wejściu? Elemnt który pokazuje zadanie po kliknięciu musi pobrać z bazy informacje o tym zadaniu, a w sumie jego ID? 
        public int UserIdU { get => _userIdU; set => _userIdU = value; } //Pobierane z sesji zalogowanego użytkownika
        private bool TaskComplited; // Ustawiane przed dodaniem do modelu
        private bool IsActive; //Ustawiane potem prrzed dodaniem do bazy

        //                  FUNKCJE PLIKU
        private readonly DatabaseLogin _context;
        public AddTaskViewModel(DatabaseLogin context)
        {
            _context = context;
            RegisterCommand = new Command(AddTask);
        }

        [ObservableProperty]
        private bool _isBusy;

        private async void AddTask(object obj)
        {
            Modified = DateTime.Now;
            Tasks Task = new() {
                Name=Name
                , Description=Description
                , Type=Typ
                , UserId=UserId//   POBRANY SKĄD??? OBECNEJ SESJI UŻYTKOWNIKA??
                , status=Status
                , RealizedPercent=Progress
                , Updated=Modified
            };

            await ExecuteAsync(async () =>
            {
                List<string> list = new List<string> { Name, Description, Typ, Status, UserId.ToString(),  Progress.ToString() };
                int i = 1; 
                foreach (var wartosc in list)
                {
                    if (string.IsNullOrEmpty(wartosc)) {
                        i = 1; break;
                    }
                    else
                        i = 0;
                }
                if(i == 1){
                    await App.Current.MainPage.DisplayAlert("Failed", "Jedno z lub wiele pól jest puste", "Ok");
                }else
                    await _context.AddItemAsync<Tasks>(Task);
            });
        }

        //                  FUNKCJA DODAWANIA PODZADANIA 

        private async void AddTaskComponent(object obj){
            TaskComponents TC = new() { 
                Name = NameU
                , Description = DescriptionU
                , TaskId=TaskId //?????
                , UserId = _userIdU //?????
                , Status = StatusU
                , TaskComplited = false
                , IsActive = true
                , LastUpdated = DateTime.Now
            };

            await ExecuteAsync(async () =>
            {
                List<string> list = new List<string> { NameU, DescriptionU, StatusU };
                int i = 1;
                foreach (var wartosc in list)
                {
                    if (string.IsNullOrEmpty(wartosc))
                    {
                        i = 1; break;
                    }
                    else
                        i = 0;
                }
                if (i == 1)
                {
                    await App.Current.MainPage.DisplayAlert("Failed", "Jedno z lub wiele pól podzadania jest puste", "Ok");
                }
                else
                    await _context.AddItemAsync<TaskComponents>(TC);
            });
        }
        private async Task ExecuteAsync(Func<Task> operation)
        {
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