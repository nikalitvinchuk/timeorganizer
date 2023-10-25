
using timeorganizer.DatabaseModels;

namespace timeorganizer
{
    public partial class App : Application
    {
        
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();

        }
    }
}