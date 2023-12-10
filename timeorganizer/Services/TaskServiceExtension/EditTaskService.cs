using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;
using System.Data;
using System.Linq.Expressions;
using timeorganizer.DatabaseModels;

namespace timeorganizer.Services;
public partial class EditTaskService : ObservableObject {
    private string Updated, _created;
    private int _id, _typ;
    private DateTime _updated, _termin;

    public DateTime DUptaded { get => _updated; set => _updated = value; }
    public int TaskId { get => _id; set => _id = value; }
    public int typ { get => _typ; set => _typ = value; }

    public Tasks EditZadanie = new();
    public TaskComponents EditEtap = new();
    public DateTime Termin { get => _termin; set => _termin = value; }
    private readonly DatabaseLogin _context;

    public EditTaskService() {
        _context = new();
    }
    public async Task GetTask(int tid) {
        _id = tid;
        if (_typ == 0) {
            await ExecuteAsync(async () => {
                EditZadanie = await _context.GetItemByKeyAsync<Tasks>(_id);
                if (DateTime.TryParse(EditZadanie.Termin, out DateTime parsedTermin)) {
                    _termin = parsedTermin;
                }
            });
        }
        else if (_typ == 1) {
            await ExecuteAsync(async () => {
                EditEtap = await _context.GetItemByKeyAsync<TaskComponents>(_id);
            });
        }
    }
    public async Task CheckFinComp(Tasks task) {
        await ExecuteAsync(async () => {
                var row = await _context.GetFileteredAsync<TaskComponents>(e => e.Status == "Ukończono" && e.TaskId == task.Id);
                int liczba = row.Count();
                row= await _context.GetFileteredAsync<TaskComponents>(e => e.Status != "Rem" && e.TaskId == task.Id);
            if (liczba == row.Count()) {
                bool confirmation = await App.Current.MainPage.DisplayAlert("Potwierdzenie", "Ukończenie tego zadania będzię się wiązało z brakiem możliwości pozostałych podzadań", "Ok", "Anuluj");

                if (confirmation) {
                    task.Status = "Ukończono";
                    await _context.UpdateItemAsync(task);
                    await App.Current.MainPage.DisplayAlert("Sukcess", "Zadanie oznaczone jako ukończone", "Ok");
                }

            }
        });
    }
        public async Task Update<TTable>(TTable T) where TTable : class, new() {
        await ExecuteAsync(async () => {
            await _context.UpdateItemAsync(T);
            await App.Current.MainPage.DisplayAlert("Sukcess", "Zmieniono dane", "Ok");
        });
    }
    public async Task Usun<TTable>(TTable T) where TTable : class, new() {
        await ExecuteAsync(async () => {
            bool confirmation = await App.Current.MainPage.DisplayAlert("Potwierdzenie", "Czy na pewno chcesz to usunąć?\n Ta operacja jest nieodwracalna", "Ok", "Anuluj");

            if (confirmation) {
                if (T is Tasks task) {
                    List<TaskComponents> etapy = (List<TaskComponents>)await _context.GetFileteredAsync<TaskComponents>(e => e.TaskId == task.Id);
                    foreach (var etap in etapy) {
                        etap.Status = "Rem";
                        await _context.UpdateItemAsync(etap);
                    }
                }

                (T as dynamic).Status = "Rem";
                await _context.UpdateItemAsync(T);
                await App.Current.MainPage.DisplayAlert("Sukcess", "Usunięto", "Ok");
            }
            else {
                await App.Current.MainPage.DisplayAlert("", "Anulowano", "Ok");
            }
        });
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
