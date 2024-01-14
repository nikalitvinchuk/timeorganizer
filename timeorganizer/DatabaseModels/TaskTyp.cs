using SQLite;

namespace timeorganizer.DatabaseModels //model zadan glownych 
{
    [Table("TaskTyp")]
    public class TaskTyp {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; } //nazwa zadania    
        public string Status { get; set; } // Aktywne, Rem (Usunięto)
        public int UserId { get; set; } // Aktywne, Rem (Usunięto)
    }
}
