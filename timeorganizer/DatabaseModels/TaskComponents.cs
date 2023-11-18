using SQLite;

namespace timeorganizer.DatabaseModels //model podzadan obecnie nie wykorzystywany
{
    [SQLite.Table("TaskComponents")]
    public class TaskComponents
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; } // id zadania

        public string Name { get; set; }
        public string Description { get; set; }

        public int TaskId { get; set; }
        public int UserId { get; set; }
        public string Status { get; set; }
        public bool TaskComplited { get; set; }
        public bool IsActive { get; set; }

        public string Created { get; set; }
        public string LastUpdated { get; set; }


    }
}
