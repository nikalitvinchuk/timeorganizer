using SQLite;

namespace timeorganizer.DatabaseModels //model zadan glownych 
{
    [Table("Tasks")]
    public class Tasks
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; } //nazwa zadania
        public string Description { get; set; } //opis zadania
        public string Type { get; set; } //typ zadania
        public int UserId { get; set; } //użytkownik tworzacy
        public string Status { get; set; } // Aktywne, Ukończono, Rem (Usunięto)
        public int RealizedPercent { get; set; } //procent realizacji obliczany na podstawie liczby podzadań/liczba zralizowanych
        public string Created { get; set; }  //data utworzenia
        public DateTime TerminDateTime { get; set; }  //data utworzenia
        public DateTime UkonczonoDateTime { get; set; }  //data utworzenia
        public string Termin { get; set; } //data do kiedy trzeba wykonać zadanie - Greg
        public string Updated { get; set; } //data aktualizacji     // ZMIENIŁEM TYP POLA NA STRINGA - Greg

    }
}
