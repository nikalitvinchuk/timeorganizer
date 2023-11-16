
using timeorganizer.PageViewModels;
using Windows.Security.Cryptography.Certificates;

namespace timeorganizer.Views;

public partial class RegisterPage : ContentPage
{
    private readonly RegisterPageViewModel _viewModel;

    public RegisterPage(RegisterPageViewModel viewModel)
    {
        BindingContext = viewModel;
        _viewModel = viewModel;
        InitializeComponent();
    }

    public void GotoLoginClicked(object sender, EventArgs e)
    {
        Email.Text = string.Empty;
        Shell.Current.GoToAsync("LoginPage");
    }

}