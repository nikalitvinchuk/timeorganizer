using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using timeorganizer.DatabaseModels;

namespace timeorganizer.Services;
public partial class SummaryServic : ObservableObject {
    private readonly DatabaseLogin _context;
    private int _userId;
    private List<object> _taskColMon, _taskColWeek, _taskColYear;
    private DateTime? _date = DateTime.Now;
    private Collection<Tasks> _tasks;
    public Collection<Tasks> Task { get => _tasks; set => _tasks = value; }
    public DateTime? Date { get => _date; set => _date = value; }
    public List<object> TasksColMon { get => _taskColMon; set => _taskColMon = value; }
    public List<object> TasksColWeek { get => _taskColWeek; set => _taskColWeek = value; }
    public List<object> TasksColYear { get => _taskColYear; set => _taskColYear = value; }
    public SummaryServic() {
        _context = new();
        _taskColMon = new List<object>();
        _taskColWeek = new List<object>();
        _taskColYear = new List<object>();
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
    public async Task TaskSummary(int FilteredYear) {
        await ExecuteAsync(async () =>
        {
            var activityservice = new ActivityService(); //inicjalizacja do późniejszego wywołania 

            _userId = await Getid();
            int targetYear = FilteredYear;
			var tasks = await _context.GetFileteredAsync<Tasks>(e => e.UserId == _userId && e.Status == "Ukończono" && e.UkonczonoDateTime.Date.Year == targetYear);
			var tasksByMonth = tasks.GroupBy(e => new { e.UkonczonoDateTime.Year, e.UkonczonoDateTime.Month }).Select(g => new {
                g.Key.Year,
				Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Key.Month),
				Count = g.Count(),
				Count100Realized = g.Count(e => e.RealizedPercent == 100),
				Count100precent = (double)g.Count(e => e.RealizedPercent == 100) / g.Count() * 100.0,
				CountNot100 = g.Count(e => e.RealizedPercent != 100),
			});

			var tasksByWeek = tasks.GroupBy(e => new { e.UkonczonoDateTime.Year, Week = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(e.UkonczonoDateTime, CalendarWeekRule.FirstDay, DayOfWeek.Sunday) }).Select(g => new {
                g.Key.Year,
                g.Key.Week,
				StartDate = SummaryServic.GetDaysOfWeek(g.Key.Week, g.Key.Year).ToString("dd.MM.yyyy"),
				EndDate = SummaryServic.GetDaysOfWeek(g.Key.Week, g.Key.Year).AddDays(6).ToString("dd.MM.yyyy"),
				Count = g.Count(),
				Count100Realized = g.Count(e => e.RealizedPercent == 100),
				Count100precent = (double)g.Count(e => e.RealizedPercent == 100) / g.Count() * 100.0,
				CountNot100 = g.Count(e => e.RealizedPercent != 100),
			});

			var tasksByYear = tasks.GroupBy(e => e.UkonczonoDateTime.Year).Select(g => new {
				Year = g.Key,
				Count = g.Count(),
				Count100Realized = g.Count(e => e.RealizedPercent == 100),
				Count100precent = (double)g.Count(e => e.RealizedPercent == 100) / g.Count() * 100.0,
				CountNot100 = g.Count(e => e.RealizedPercent != 100),
			});

			_taskColMon = tasksByMonth.Cast<object>().ToList();
			_taskColWeek = tasksByWeek.Cast<object>().ToList();
			_taskColYear = tasksByYear.Cast<object>().ToList();

            await activityservice.ChangeExpirationDateCommand(); //przedłużanie sesji - funkcja z ActivityService
        });
    }
	private static DateTime GetDaysOfWeek(int weekNumber, int year) {
        var activityservice = new ActivityService(); //inicjalizacja do późniejszego wywołania ChangeExpirationDate

        DateTime jan1 = new(year, 1, 1);
		DateTime firstMonday = jan1.AddDays((DayOfWeek.Monday - jan1.DayOfWeek + 7) % 7);
		int days = (weekNumber - 1) * 7;

		DateTime result = firstMonday.AddDays(days);

		if (DateTime.IsLeapYear(year) && result < new DateTime(year, 2, 29)) {
			result = result.AddDays(1);
		}

		return result;
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
        await activityservice.ChangeExpirationDateCommand(); //przedłużanie sesji - funkcja z ActivityService 
    }
}
