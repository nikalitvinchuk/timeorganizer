

using SQLitePCL;
using timeorganizer.DatabaseModels;

namespace timeorganizer.Views;

public partial class LoginPage : ContentPage
{
    public string LoginValue;
    
    public string PassValue ;
    public LoginPage()
    {
        InitializeComponent();
    }
    private void OnTextChangedLoggin(object sender, EventArgs e) // przepisanie wartosci z pola entry login. uruchamiany po karzdej zmianie warosci w oknie
    {
        var text = ((Entry)sender).Text;
       LoginValue = text;
    }
    private void OnTextChangedPass(object sender, EventArgs e) // przepisanie wartosci z pola entry password. uruchamiany po karzdej zmianie warosci w oknie
    {
        var text = ((Entry)sender).Text;
        PassValue = text;
    }


    private void TryLoggIn(object sender, EventArgs e) // uruchamiane po kliknieciu przycisku zaloguj
	{
        if(!isValidEntry())
        {
            //w przypadku braku poprawnych danych - alert
            return;
        }


        if (!string.IsNullOrEmpty(LoginValue))
        {
            if (!string.IsNullOrEmpty(PassValue))
            {
                if (PassValue == LoginValue) //weryfikacja do zmiany po podpiêciu bazy i stworzeniu rejestracji
                {
                    App.Current.MainPage = new LoggedMainPage(); // zmiana domyslnego widoku na widok flyout
                    // do zmiany na poŸniejszym etapie projektu
                }
            }   
        }
    }
    private bool isValidEntry()
    {
        if (string.IsNullOrEmpty(PassValue) && string.IsNullOrEmpty(LoginValue))
        {
            Application.Current.MainPage.DisplayAlert("B³¹d", "Nie poda³eœ loginu i has³a", "OK");
            return false;
        }
        
        else if (string.IsNullOrEmpty(LoginValue))
        {
            Application.Current.MainPage.DisplayAlert("B³¹d", "Nie poda³eœ loginu", "OK");
            return false;
        }
        else if (string.IsNullOrEmpty(PassValue))
        {
            Application.Current.MainPage.DisplayAlert("B³¹d", "Nie poda³eœ has³a", "OK");
            return false;
        }
        else
        {
            Application.Current.MainPage.DisplayAlert("B³¹d", "Niepoprawne dane", "OK");
            return false;
        }
        return true;
    }
    private async void TryLoggOut(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MainPage());
    }

}