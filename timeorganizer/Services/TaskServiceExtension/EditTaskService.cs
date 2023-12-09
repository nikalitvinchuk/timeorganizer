using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.AspNetCore.Components;
using SQLitePCL;
using System.Collections.ObjectModel;
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
    private NavigationManager _navigationManager;
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
        }else if(_typ == 1) {
            await ExecuteAsync(async () => {
                EditEtap = await _context.GetItemByKeyAsync<TaskComponents>(_id);
            });
        }
    }

    public async Task Update() {
        if (_typ == 0) {
            await ExecuteAsync(async () => {
                EditZadanie.Termin = _termin.ToString("dd.MM.yyyy");
                await _context.UpdateItemAsync<Tasks>(EditZadanie);
                await App.Current.MainPage.DisplayAlert("Sukcess", "Zmieniono dane", "Ok");
            });
        }else if (_typ == 1) {
            await ExecuteAsync(async () => {
                await _context.UpdateItemAsync<TaskComponents>(EditEtap);
                await App.Current.MainPage.DisplayAlert("Sukcess", "Zmieniono dane", "Ok");
            });
        }
    }
    public async Task Usun() {
        if (_typ == 0) {
            await ExecuteAsync(async () => {

                bool potwierdzenine = await App.Current.MainPage.DisplayAlert("Potwierdzenie", "Czy na pewno chcesz to usunąć?\n Ta operacja jest nie odwracalna", "Ok", "Anuluj");

                if (potwierdzenine) {
                    List<TaskComponents> etapy;
                    etapy = (List<TaskComponents>)await _context.GetFileteredAsync<TaskComponents>(e => e.TaskId == EditZadanie.Id);
                    foreach (var element in etapy) {
                        element.Status = "Rem";
                        await _context.UpdateItemAsync<TaskComponents>(element);
                    }
                    EditZadanie.Status = "Rem";
                    await _context.UpdateItemAsync<Tasks>(EditZadanie);
                    await App.Current.MainPage.DisplayAlert("Sukcess", "Usunięto", "Ok");
                }
                else {
                    await App.Current.MainPage.DisplayAlert("", "Anulowano", "Ok");
                }

            });
        }
        else if (_typ == 1) {
            await ExecuteAsync(async () => {
                bool potwierdzenine = await App.Current.MainPage.DisplayAlert("Potwierdzenie", "Czy na pewno chcesz to usunąć?\n Ta operacja jest nie odwracalna", "Ok", "Anuluj");

                if (potwierdzenine) {
                    EditEtap.Status = "Rem";
                    await _context.UpdateItemAsync<TaskComponents>(EditEtap);
                    await App.Current.MainPage.DisplayAlert("Sukcess", "Usunięto", "Ok");
                }
                else {
                    await App.Current.MainPage.DisplayAlert("", "Anulowano", "Ok");
                }

            });
        }
    }
    //public async Task Ukoncz(int _tid) {

    //}
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
