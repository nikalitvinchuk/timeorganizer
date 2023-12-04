using CommunityToolkit.Mvvm.ComponentModel;
using SQLitePCL;
using timeorganizer.DatabaseModels;

namespace timeorganizer.Services;
public partial class EditTaskService : ObservableObject {
    private string Updated, _created;
    private int _id;
    private DateTime _updated;

    public DateTime DUptaded { get => _updated; set => _updated = value; }
    public int TaskId { get => _id; set => _id = value; }
    public int typ ;

    public Tasks EditZadanie;
    public TaskComponents EditEtap;

    private readonly DatabaseLogin _context;
    public EditTaskService() {
        _context = new();
    }
    public async Task GetTask(int tid) {
        _id = tid;
        if (typ == 0) {
            await ExecuteAsync(async () => {
                EditZadanie = await _context.GetItemByKeyAsync<Tasks>(_id);
            });
        }else if(typ == 1) {
            await ExecuteAsync(async () => {
                EditEtap = await _context.GetItemByKeyAsync<TaskComponents>(_id);
            });
        }
    }

    public async Task Update() {
        if (typ == 0) {
            await ExecuteAsync(async () => {
                await _context.UpdateItemAsync<Tasks>(EditZadanie);
                await App.Current.MainPage.DisplayAlert("Sukcess", "Zmieniono dane", "Ok");
            });
        }else if (typ == 1) {
            await ExecuteAsync(async () => {
                await _context.UpdateItemAsync<TaskComponents>(EditEtap);
                await App.Current.MainPage.DisplayAlert("Sukcess", "Zmieniono dane", "Ok");
            });
        }
    }

    [ObservableProperty]
    public bool _isBusy;
    private async Task ExecuteAsync(Func<Task> operation) {
        var activityservice = new ActivityService(); //inicjalizacja do późniejszego wywołania ChangeExpirationDate
        IsBusy = true;
        try {
            await operation?.Invoke();
        }
        catch (Exception ex) {
            await App.Current.MainPage.DisplayAlert("ERROR SQL", ex.Message, "Ok");
        }
        finally {
            IsBusy = false;
        }
    }
}
