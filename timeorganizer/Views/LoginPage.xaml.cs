

namespace timeorganizer.Views;

public partial class LoginPage : ContentPage
{
    public string LoginValue;
    
    public string PassValue ;
    public LoginPage()
	{
        
        InitializeComponent();
    }
    private void OnTextChangedLoggin(object sender, EventArgs e)
    {
        var text = ((Entry)sender).Text;
       LoginValue = text;
    }
    private void OnTextChangedPass(object sender, EventArgs e)
    {
        var text = ((Entry)sender).Text;
        PassValue = text;
    }
    private void TryLoggIn(object sender, EventArgs e)
	{
        if (!string.IsNullOrEmpty(LoginValue))
        {
            if (!string.IsNullOrEmpty(PassValue))
            {
                if (PassValue == LoginValue) //weryfikacja do zmiany po podpiêciu bazy i stworzeniu rejestracji
                {
                    App.Current.MainPage = new LoggedMainPage();
                    // do zmiany na poŸniejszym etapie projektu
                }
            }   
        }
    }
}