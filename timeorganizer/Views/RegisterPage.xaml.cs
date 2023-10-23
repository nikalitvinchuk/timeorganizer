namespace timeorganizer.Views;

public partial class RegisterPage : ContentPage
{
	public RegisterPage()
	{
		InitializeComponent();
	}
	public string EmailValue;
	public string PassValue;
	public string LoginValue;
	public string PassVerificationValue;
    private void OnTextChangedEmail(object sender, EventArgs e)
    {
        var text = ((Entry)sender).Text;
        EmailValue = text;
    }
    private void OnTextChangedPass(object sender, EventArgs e)
    {
        var text = ((Entry)sender).Text;
        PassValue = text;
    }
    private void OnTextChangedPassVerification(object sender, EventArgs e)
    {
        var text = ((Entry)sender).Text;
        PassVerificationValue = text;
    }
    private void OnTextChangedLogin(object sender, EventArgs e)
    {
        var text = ((Entry)sender).Text;
        LoginValue = text;
    }
    public void TryRegister(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(EmailValue) && !string.IsNullOrEmpty(LoginValue))
        {
            if(!string.IsNullOrEmpty(PassValue)&& !string.IsNullOrEmpty(PassVerificationValue))
            {
                if(PassValue==PassVerificationValue)
                {
                    Application.Current.MainPage = new AppShell();
                }
            }
        }
    }

    public void GotoLoginClicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("LoginPage");
    }
}