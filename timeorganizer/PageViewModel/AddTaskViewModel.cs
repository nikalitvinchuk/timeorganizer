using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Input;
using timeorganizer.DatabaseModels;

namespace timeorganizer.PageViewModels
{//dodawanie nowych zadań
    public partial class AddTaskViewModel : ObservableObject
    {
        private string _name, _desc, _type, _status;
        private int _userId, _relizedpr;
        private DateTime Modified;
        public string Name { get => _name; set => _name= value; }
        public string Description { get => _desc; set => _desc= value; }
        public string Typ { get => _type; set => _type= value; }
        public string Status{ get => _status; set => _status = value; }
        public int UserId { get => _userId; set => _userId = value; }
        public int Progress { get => _relizedpr; set => _relizedpr= value; }
        public ICommand RegisterCommand { private set; get; }



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
            Tasks Task = new() {
                Name=Name
                , Description=Description
                , Type=Typ
                , UserId=UserId
                , status=Status
                , RealizedPercent=Progress
            };

            await ExecuteAsync(async () =>
            {
                    await _context.AddItemAsync<Tasks>(Task);

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