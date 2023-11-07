using SQLite;

namespace timeorganizer.DatabaseModels
{
    [SQLite.Table("Notes")]
    public class Notes
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; } // id notatki
        public int UserId { get; set; }

        public string Title { get; set; }
        public string Content1 { get; set; }
        public string Content2 { get; set; }
        public string Content3 { get; set; }

        public string Created { get; set; }
        public string LastUpdated { get; set; }
    }
}
