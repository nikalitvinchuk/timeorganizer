using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Globalization;
using timeorganizer.DatabaseModels;

namespace timeorganizer.Services;
public partial class SummaryServic : ObservableObject {
    private readonly DatabaseLogin _context;
    private int _userId;
    private DateTime? _date1, _date2;
    private Collection<Tasks> _tasks;
    public Collection<Tasks> Task { get => _tasks; set => _tasks = value; }
    public DateTime? Date1 { get => _date1; set => _date1 = value; }
    public DateTime? Date2 { get => _date2; set => _date2 = value; }
    public SummaryServic() {
        _context = new();
    }
    private async Task<int> Getid() {
        string _tokenvalue = await SecureStorage.Default.GetAsync("token");
        var getids = await _context.GetFileteredAsync<UserSessions>(t => t.Token == _tokenvalue);
        if (getids.Any(t => t.Token == _tokenvalue)) {
            var getid = getids.First(t => t.Token == _tokenvalue);
            return getid.UserId;
        }
        else {
            return 0;
        }
    }
    string format = "dd.MM.yyyy";
    public async void TaskSummary() {
        await ExecuteAsync(async () =>
        {
            _userId = await Getid();

            Task = (Collection<Tasks>)await _context.GetFileteredAsync<Tasks>(e =>
                e.UserId == _userId &&
                DateTime.ParseExact(e.Termin, "dd.MM.yyyy", CultureInfo.InvariantCulture) >= _date1 &&
                DateTime.ParseExact(e.Termin, "dd.MM.yyyy", CultureInfo.InvariantCulture) <= _date2
            );
        });
    }


    [ObservableProperty]
    public bool _isBusy;
    private async Task ExecuteAsync(Func<Task> operation) {
        var activityservice = new ActivityService(); //inicjalizacja do późniejszego wywołania ChangeExpirationDate
        _userId = await Getid();
        IsBusy = true;
        try {
            await operation?.Invoke();
        }
        catch (Exception ex) {
            await App.Current.MainPage.DisplayAlert("ERROR", ex.Message, "Ok");
        }
        finally {
            IsBusy = false;
        }
    }
}
