using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;
using timeorganizer.DatabaseModels;

namespace timeorganizer.PageViewModel.NotesViewModel
{
    public partial class FilterNotesViewModel : ObservableObject // Mozna skorzystac aby uzyskac szczegoly danej notatki - nalezy podac id zadania 
    {
        private readonly DatabaseLogin _context;
        private ObservableCollection<Notes> _notes;
        public ICommand GetNote { private set; get; }
        public ObservableCollection<Notes> NotesColletion { get => _notes; set => _notes = value; }
        public FilterNotesViewModel()
        {
            _context = new DatabaseLogin();
            GetNote = new Command(async () => await GetNotes());
        }

        private string _content1, _content2, _content3, _title;
        private int _userId;
        private int _id;
        [ObservableProperty]
        public bool isBusy;
        public string Title { get => _title; set => _title = value; }
        public string Content1 { get => _content1; set => _content1 = value; } //max 255 znaków 
        public string Content2 { get => _content2; set => _content2 = value; } //max 255 znaków 
        public string Content3 { get => _content3; set => _content3 = value; } //max 255 znaków 
        public int Id { get => _id; set => _id = value; }
        private async Task<ObservableCollection<Notes>> GetNotes()
        {
            if (_userId == 0) _userId = await Getid();
            await ExecuteAsync(async () =>
            {
                var filters = new Dictionary<string, object>
                {
                    { "Userid", _userId }
                };
                if (!string.IsNullOrWhiteSpace(_title)) filters.Add("Title", _title);
                if (_id != 0) filters.Add("Id", _id);

                NotesColletion = new ObservableCollection<Notes>(await _context.GetFileteredAsync(_context.CreatePredicateToFiltred<Notes>(filters)));
                filters.Clear();
                OnPropertyChanged(nameof(NotesColletion));

            });
            return NotesColletion;
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
    }
}
