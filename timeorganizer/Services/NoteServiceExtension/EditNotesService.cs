using CommunityToolkit.Mvvm.ComponentModel;
using SQLitePCL;
using timeorganizer.DatabaseModels;

namespace timeorganizer.Services;
public partial class EditNotesService : ObservableObject
{
    private string Updated, _created;
    private int _id;
    private DateTime _updated;

    public int noteId { get => _id; set => _id = value; }
    public int typ;

    public Notes EditNote;

    private readonly DatabaseLogin _context;
    public EditNotesService()
    {
        _context = new();
    }

    public async Task GetNotes(int id)
    {
        {
            EditNote = await _context.GetItemByKeyAsync<Notes>(id);
        }
    }

    public async Task Update()
    {
        {
            var activityservice = new ActivityService(); //inicjalizacja do późniejszego wywołania ChangeExpirationDate
            await _context.UpdateItemAsync<Notes>(EditNote);
            await activityservice.ChangeExpirationDateCommand(); //przedłużanie sesji - funkcja z ActivityService 

            await App.Current.MainPage.DisplayAlert("Sukces", "Zmieniono dane", "Ok");
        }
    }

}
