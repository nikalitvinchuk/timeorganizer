using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;
using timeorganizer.DatabaseModels;

namespace timeorganizer.Services.NoteServiceExtension
{
    public partial class AddNotesExtension : ObservableObject
    {
        private string _title, _content;
        private int _userId;
        public string Title { get => _title; set => _title = value; }
        public string Content { get => _content; set => _content = value; } // MAX 255 ZNAKÓW trzeba dodać ograniczenie po stronie xamla do entry
                                                                            //public int Id { get => _id; set => _id = value; }
        public int UserId { get => _userId; set => _userId = value; }

        public string Modified;


        private readonly DatabaseLogin _context;
        public AddNotesExtension()
        {
            _context = new DatabaseLogin();

        }
        private async Task<int> Getid()
        {
            try
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
            catch (Exception ex)
            {
                return 0;
            }

        }
        [ObservableProperty]
        private bool _isBusy;

        public async Task AddNote()
        {
            var activityservice = new ActivityService(); //inicjalizacja do późniejszego wywołania ChangeExpirationDate

            try
            {
                if (_userId == 0) _userId = await Getid();
                if (_userId != 0)
                {
                    Notes note = new()
                    {
                        Title = _title,
                        Content = _content,
                        Created = DateTime.Now.ToLongDateString(),
                        UserId = _userId,
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
                            _title = "";
                            _content = "";
                        }
                        
                    });
                }

            }
            catch (Exception ex)
            {
                //await activityViewModel.ChangeExpirationDateCommand();
            }
            await activityservice.ChangeExpirationDateCommand(); //przedłużanie sesji - funkcja z ActivityService 
        }

        private ObservableCollection<Notes> _notesList;
        public ObservableCollection<Notes> NotesList
        {
            get => _notesList;
            set => SetProperty(ref _notesList, value);
        }

        public ICommand DeleteCommand => new Command<Notes>(async (note) => await DeleteNoteAsync(note));

        private async Task DeleteNoteAsync(Notes note)
        {
            try
            {
                await ExecuteAsync(async () =>
                {
                    await _context.DeleteItemAsync<Notes>(note);
                    NotesList.Remove(note);
                    await App.Current.MainPage.DisplayAlert("Succes", "Usunięto notatkę z bazy", "Ok");
                });
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("ERROR SQL", ex.Message, "Ok");
            }
        }

        private async Task ExecuteAsync(Func<Task> operation)
        {
            //var activityViewModel = new ActivityViewModel(); //inicjalizacja do późniejszego wywołania ChangeExpirationDate
            //IsBusy = true;
            try
            {
                await operation?.Invoke();
            }
            catch (Exception ex)
            {
                //await activityViewModel.ChangeExpirationDateCommand(); //przedłużanie sesji - funkcja z ActivityViewModel 
                await App.Current.MainPage.DisplayAlert("ERROR SQL", ex.Message, "Ok");
            }
            finally
            {
                //IsBusy = false;
            }
        }
    }
}