using SQLite;

namespace timeorganizer.DatabaseModels;

    [Table("Send")] // wys�anie wiadomo�ci do tabeli w bazie danych o nazwie Send???? Model do kontaktu

    public class Message
    {
 
            [PrimaryKey, AutoIncrement]
            public int Id { get; set; }
            public string Email { get; set; } //nazwa podawanego maila zwrotnego przez klienta
            public string MessageText { get; set; } //komentarz/informacja  jak� chce da� klinet


    }
