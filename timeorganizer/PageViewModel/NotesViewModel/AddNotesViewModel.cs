using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Input;
using timeorganizer.DatabaseModels;

namespace timeorganizer.PageViewModel.NotesViewModel
{
    public partial class AddNotesViewModel : ObservableObject
    {
        private string _title, _content1, _content2, _content3;
        private int _userId;
        public string Title { get => _title; set => _title = value; }
        public string Content1 { get => _content1; set => _content1 = value; } // MAX 255 ZNAKÓW trzeba dodać ograniczenie po stronie xamla do entry
        public string Content2 { get => _content2; set => _content2 = value; } // MAX 255 ZNAKÓW trzeba dodać ograniczenie po stronie xamla do entry
        public string Content3 { get => _content3; set => _content3 = value; } // MAX 255 ZNAKÓW trzeba dodać ograniczenie po stronie xamla do entry
        //public int Id { get => _id; set => _id = value; }
        public int UserId { get => _userId; set => _userId = value; }

        public string Modified;
        public ICommand AddNoteCommand { private set; get; }

        private readonly DatabaseLogin _context;
        public AddNotesViewModel()
        {
            _context = new DatabaseLogin();
            AddNoteCommand = new Command(AddNote);
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

        private async void AddNote(object obj)
        {
            if (_userId == 0) _userId = await Getid();
            Notes note = new()
            {
                Title = _title,
                Content1 = _content1,
                Content2 = _content2,
                Content3 = _content3,
                Created = DateTime.Now.ToLongDateString(),
                LastUpdated = null
            };

            await ExecuteAsync(async () =>
            {
                List<string> list = new() { Title };
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
                            _ => "",
                        };
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
                                _ => "",
                            };
                            await App.Current.MainPage.DisplayAlert("Za długie", $"Pole {nazwa} jest za długie. Pole może mieć maksymalnie wartość 200 znaków", "Ok");
                            break;
                        }
                        j++;
                    }
                }

                if (i == 0)
                {
                    await _context.AddItemAsync<Notes>(note);
                    await App.Current.MainPage.DisplayAlert("Succes", "Dodano notatkę do bazy", "Ok");
                }

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
                await App.Current.MainPage.DisplayAlert("ERROR SQL", ex.Message, "Ok");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
