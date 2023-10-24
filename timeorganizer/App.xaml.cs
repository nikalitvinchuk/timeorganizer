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
        static DatabaseLogin database;
        public static DatabaseLogin Database
        {
            get
            {
                if (database == null)
                {
                    database = new DatabaseLogin(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "timeorganizer.db")); //podpiecie  bazy do projeku 
                    // obecnie localDB.
                }
                return database;
            }
        }
    }

}