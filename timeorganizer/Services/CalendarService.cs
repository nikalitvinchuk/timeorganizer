
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using timeorganizer.DatabaseModels;

namespace timeorganizer.Services
{
    public class CalendarService : ObservableObject
    {
        public int _userId;
        private DateTime _dateValue;
        public DateTime DateValue
        {
            get => _dateValue;
            set
            {
                _dateValue = value;
                Debug.WriteLine($"Wartość DateValue została zaktualizowana: {_dateValue}");
                ReadTasks();
            }
        }


        private ObservableCollection<Tasks> _collection = new();
        public ObservableCollection<Tasks> TasksCollection
        {
            get => _collection;
            set => SetProperty(ref _collection, value);
        }

        private readonly DatabaseLogin _context;

        public CalendarService()
        {
            _context = new DatabaseLogin();
        }

        private async Task<int> Getid()
        {
            var _tokenvalue = await SecureStorage.Default.GetAsync("token");
            var getids = await _context.GetFileteredAsync<UserSessions>(t => t.Token == _tokenvalue);
            if (getids.Any(t => t.Token == _tokenvalue))
            {
                var getid = getids.First(t => t.Token == _tokenvalue);
                return getid.UserId;
            }
            else { return 0; }
        }
        public async Task ReadTasks()
        {
			var activityservice = new ActivityService(); //inicjalizacja do późniejszego wywołania ChangeExpirationDate

			_userId = await Getid();

                var filters1 = new Dictionary<object, string>
                {
                    { _userId,"Equal" }

                };
                var filters = new Dictionary<string, object>
                {
                    {"Userid",_userId }
                };
                if (DateValue != default)
                {
                    filters.Add("Termin", DateValue.ToString("dd.MM.yyyy"));
                    filters1.Add(DateValue.ToString("dd.MM.yyyy"), "Equal");
                }

                _collection = new ObservableCollection<Tasks>(await _context.GetFileteredAsync<Tasks>(_context.CreatePredicateToFiltred<Tasks>(filters, filters1)));
            filters.Clear();
			await activityservice.ChangeExpirationDateCommand(); //przedłużanie sesji - funkcja z ActivityService 


		}
    }
}
