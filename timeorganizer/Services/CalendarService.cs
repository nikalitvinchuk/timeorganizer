
using CommunityToolkit.Mvvm.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using timeorganizer.DatabaseModels;

namespace timeorganizer.Services
{
    public class CalendarService : ObservableObject
    {
        private DateTime _dateValue;

        public DateTime DateValue
        {
            get => _dateValue;
            set
            {
                _dateValue = value;
                Debug.WriteLine($"Wartość DateValue została zaktualizowana: {_dateValue}");
            }
        }

        public ICommand DateCommand { get; private set; }
        private readonly DatabaseLogin _context;

        public CalendarService()
        {
            _context = new DatabaseLogin();
            DateCommand = new Command(ReadTasks);
        }

        private async void ReadTasks(object obj)
        {

        }
    }
}
