using CommunityToolkit.Mvvm.ComponentModel;
using SQLitePCL;
using timeorganizer.DatabaseModels;

namespace timeorganizer.Services;
public partial class EditNotesService : ObservableObject
{
    private string Updated, _created;
    private int _id;
    private DateTime _updated;

    //public DateTime Updated { get => _updated; set => _updated = value; }
    public int noteId { get => _id; set => _id = value; }
    public int typ;

    public Notes EditNote;

    private readonly DatabaseLogin _context;
    public EditNotesService()
    {
        _context = new();
    }
    //public async Notes GetNotes(int id)
    //{
    //    await ExecuteAsync(async () =>
    //    {
    //        EditNote = await _context.GetItemByKeyAsync<Notes>(_id);
    //    });
    //}

    public async Task GetNotes(int id)
    {
        //await ExecuteAsync(async () =>
        {
            EditNote = await _context.GetItemByKeyAsync<Notes>(id);
        }
        //);
    }

    public async Task Update()
    {
        //await ExecuteAsync(async () =>
        {
            var activityservice = new ActivityService(); //inicjalizacja do późniejszego wywołania ChangeExpirationDate
            await _context.UpdateItemAsync<Notes>(EditNote);
            await activityservice.ChangeExpirationDateCommand(); //przedłużanie sesji - funkcja z ActivityService 

            await App.Current.MainPage.DisplayAlert("Sukces", "Zmieniono dane", "Ok");
        }//);
    }

    //[ObservableProperty]
    //public bool IsBusy;// { get; set; }

    //public static async Notes ExecuteAsync(Func<Notes> operation)
    //{
    //    var activityService = new ActivityService();
    //    IsBusy = true;
    //    try
    //    {
    //        await operation?.Invoke();
    //    }
    //    catch (Exception ex)
    //    {
    //        await App.Current.MainPage.DisplayAlert("ERROR SQL", ex.Message, "Ok");
    //    }
    //    finally
    //    {
    //        IsBusy = false;
    //    }
    //}

}
