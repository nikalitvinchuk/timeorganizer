using CommunityToolkit.Mvvm.ComponentModel;

using System.Collections.ObjectModel;
using System.Windows.Input;
using timeorganizer.DatabaseModels;

namespace timeorganizer.Services.NoteServiceExtension
{
	public partial class FilterNotesViewModel : ObservableObject // Mozna skorzystac aby uzyskac szczegoly danej notatki - nalezy podac id zadania 
	{
		private readonly DatabaseLogin _context;
		private ObservableCollection<Notes> _notes, _note;
		public ICommand GetNote { private set; get; }
		public ICommand GetFullNote { private set; get; }
		public ObservableCollection<Notes> NotesColletion { get => _notes; set => _notes = value; }
		public FilterNotesViewModel()
		{
			_context = new DatabaseLogin();
			GetNote = new Command(async () => await GetNotes());
			GetFullNote = new Command<int>(async (id) => await GetNoteById(id));
		}

		private string _content, _title;
		private int _userId;
		private int _id = -1;
		[ObservableProperty]
		public bool _isBusy;
		public string Title { get => _title; set => _title = value; }
		public string Content { get => _content; set => _content = value; } //max 255 znaków 
		public int Id { get => _id; set => _id = value; }
		public async Task GetNotes()
		{
			var activityservice = new ActivityService(); //inicjalizacja do późniejszego wywołania ChangeExpirationDate
            try
			{
				if (_userId == 0) _userId = await Getid();
				await ExecuteAsync(async () =>
				{
					var filters = new Dictionary<string, object>
				{
					{ "Userid", _userId }
				};
					if (!string.IsNullOrWhiteSpace(_title)) filters.Add("Title", _title);
					if (_id != 0) filters.Add("UserId", _userId);
                    var filters1 = new Dictionary<object, string>
                {
                };
                    _notes = new ObservableCollection<Notes>(await _context.GetFileteredAsync(_context.CreatePredicateToFiltred<Notes>(filters,filters1)));
					filters.Clear();
					OnPropertyChanged(nameof(NotesColletion));

				});
                await activityservice.ChangeExpirationDateCommand(); //przedłużanie sesji - funkcja z ActivityService

            }
			catch (Exception ex)
			{
			}

		}
		private async Task<Notes> GetNoteById(int id)
		{
            var activityservice = new ActivityService(); //inicjalizacja do późniejszego wywołania ChangeExpirationDate
            Notes note = new Notes();
			if (id != -1)
			{
				try
				{
					await ExecuteAsync(async () =>
					{
						note = await _context.GetItemByKeyAsync<Notes>(id);

					});
					//await MopupService.Instance.PushAsync(new NoteInfoPopupPage(note));
				}
				catch (Exception ex)
				{
				}
				await activityservice.ChangeExpirationDateCommand(); //przedłużanie sesji - funkcja z ActivityService
            }
			return note;
		}



		private async Task ExecuteAsync(Func<Task> operation)
		{
            var activityservice = new ActivityService(); //inicjalizacja do późniejszego wywołania ChangeExpirationDate
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

            await activityservice.ChangeExpirationDateCommand(); //przedłużanie sesji - funkcja z ActivityService
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
