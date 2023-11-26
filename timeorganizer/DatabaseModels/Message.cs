using SQLite;

namespace timeorganizer.DatabaseModels;

[Table("Message")] // wys³anie wiadomoœci do tabeli w bazie danych o nazwie Send???? Model do kontaktu

public class Message
{

    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Email { get; set; } //nazwa podawanego maila zwrotnego przez klienta
    public string MessageText { get; set; } //komentarz/informacja  jak¹ chce daæ klinet
    public string DateCreate { get; set; }
    public string Status { get; set; }


}