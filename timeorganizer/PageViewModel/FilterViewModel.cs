using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;


namespace timeorganizer.PageViewModels
{

    public partial class FilterViewModel : ObservableObject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; } //w Tasks.cs jest: public string Type { get; set; } //typ zadania
        public int UserId { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime Updated { get; set; }
        public string Status { get; set; } //tego nie mam w Tasks.cs
        public bool IsDone { get; set; } //tego nie mam w Tasks.cs
        public int Priority { get; set; } //tego nie mam w Tasks.cs
        public int RealizedPercent { get; set; }
    }

    public class TaskManager
    {
        private string connectionString = "Data Source=Timeorgranizer.db3";

        //Pobierania wszystkich zadań z bazy danych, wykonuje zapytanie SQL do tabeli "Tasks" i mapuje wynik na obiekty klasy Task
        public List<Task> GetAllTasks()
        {
            List<Task> tasks = new List<Task>();

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))

            {
                //connection.Open();

                string query = "SELECT * FROM Tasks";
                //SQLiteCommand command = new SQLiteCommand(query, connection);
                //using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    //while (reader.Read())
                    //{
                    //    Task task = new Task
                    //    {
                    //        Id = Convert.ToInt32(reader["Id"]),
                    //        Name = reader["Name"].ToString(),
                    //        Category = reader["Category"].ToString(),
                    //        IsDone = Convert.ToBoolean(reader["IsDone"]),
                    //        DueDate = Convert.ToDateTime(reader["DueDate"]),
                    //        Priority = Convert.ToInt32(reader["Priority"])
                    //    };
                    //    tasks.Add(task);
                    //}
                }
                //command.Dispose();
            }
            return tasks;
        }

        //Sortowanie zadania wg kategorii
        //public List<Task> SortTasksByCategory(List<Task> tasks)
        //{
        //    return tasks.OrderBy(task => task.Category).ToList();
        //}
        //Sortowanie zadania wg statusu (wykonane/niewykonane)
        //public List<Task> SortTasksByStatus(List<Task> tasks)
        //{
        //    return tasks.OrderBy(task => task.IsDone).ToList();
        //}
        //Sortowanie zadania wg czasu (terminu wykonania)
        //public List<Task> SortTasksByTime(List<Task> tasks)
        //{
        //    return tasks.OrderBy(task => task.DueDate).ToList();
        //}
        //Sortowanie zadania wg priorytetu
        //public List<Task> SortTasksByPriority(List<Task> tasks)
        //{
        //    return tasks.OrderBy(task => task.Priority).ToList();
        //}
    }

    //Pobieranie wszystkich zadań z bazy danych do obiektu TaskManager; później zadania są sortowane wg kategorii, statusu, czasu i priorytetu
    //class Program
    //{
    //    //static void Main(string[] args)
    //    {
    //        TaskManager taskManager = new TaskManager();
    //        //List<Task> tasks = taskManager.GetAllTasks();

    //        // Sortowanie zadań według kategorii
    //       //List<Task> tasksByCategory = taskManager.SortTasksByCategory(tasks);

    //        // Sortowanie zadań według statusu
    //        //List<Task> tasksByStatus = taskManager.SortTasksByStatus(tasks);

    //        // Sortowanie zadań według czasu
    //        //List<Task> tasksByTime = taskManager.SortTasksByTime(tasks);

    //        // Sortowanie zadań według priorytetu
    //        //List<Task> tasksByPriority = taskManager.SortTasksByPriority(tasks);
    //    }
    //}
}