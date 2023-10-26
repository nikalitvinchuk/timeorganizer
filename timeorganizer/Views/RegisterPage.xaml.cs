
using timeorganizer.PageViewModels;

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
        Shell.Current.GoToAsync("LoginPage");
    }
}