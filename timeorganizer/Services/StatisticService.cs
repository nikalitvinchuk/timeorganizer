
using CommunityToolkit.Mvvm.ComponentModel;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using timeorganizer.DatabaseModels;

namespace timeorganizer.Services
{
    public partial class StatisticService: ObservableObject
    {
        
        private readonly DatabaseLogin _context;
        public ObservableCollection<Tasks> TaskList = new ObservableCollection<Tasks>(); 
        public StatisticService()
        {
            _context = new DatabaseLogin();
        }
        public string ComplitedPrecent { get; set; }
        public double Total { get; set; }
        public double Realized { get; set; }
        public double Inprogress { get; set; }
        public double Active { get; set; }
        [ObservableProperty]
        private bool _IsBusy = false;
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
        public async Task GetUserRealizedTasks()
        {
            var activityservice = new ActivityService(); //inicjalizacja do późnie

            int _userid = await Getid();
            if (_userid == 0) { return; }
            await ExecuteAsync(async () =>
            {
                
                TaskList = new ObservableCollection<Tasks>(await _context.GetFileteredAsync<Tasks>(t => t.UserId == _userid));
                int _all = TaskList.Where(t => t.Status != "Rem").Count();
                int _realized = TaskList.Where(t => t.Status == "Ukończono").Count();
                int _inprogress = TaskList.Where(t => t.RealizedPercent > 0 && t.RealizedPercent < 100 && t.Status != "Rem").Count();

                Total = _all;
                double tmp = 0;
                if (_all != 0)
                tmp = (double)_realized / _all * 100;
                if (tmp != 0)
                    ComplitedPrecent = Convert.ToInt32(tmp).ToString() + '%';
                else
                    ComplitedPrecent = "0%";
                Inprogress = _inprogress;
                Realized = _realized;
                Active = _all - _realized - _inprogress;

            });
            await activityservice.ChangeExpirationDateCommand(); //przedłużanie sesji - funkcja z ActivityService
            return;
        }
        private async Task ExecuteAsync(Func<Task> operation)
        {
            var activityViewModel = new ActivityService(); //inicjalizacja do późniejszego wywołania ChangeExpirationDate
            IsBusy = true;
            try
            {
                await operation?.Invoke();
            }
            catch (Exception ex)
            {
                await activityViewModel.ChangeExpirationDateCommand(); //przedłużanie sesji - funkcja z ActivityViewModel 
                await App.Current.MainPage.DisplayAlert("ERROR SQL", ex.Message, "Ok");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
