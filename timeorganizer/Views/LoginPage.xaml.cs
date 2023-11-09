
using timeorganizer.PageViewModels;

namespace timeorganizer.Views;

public partial class LoginPage : ContentPage
{
    private readonly LoginPageViewModel _viewModel;

    public LoginPage(LoginPageViewModel viewModel)
    {
        BindingContext = viewModel;
        _viewModel = viewModel;
        InitializeComponent();
    }

    public void GoToRegisterClicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("RegisterPage");
    }

    private async void TryLoggOut(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MainPage());
    }
}
