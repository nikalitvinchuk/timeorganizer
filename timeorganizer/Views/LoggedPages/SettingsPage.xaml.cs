using timeorganizer.PageViewModel;

namespace timeorganizer.Views.LoggedPages;

public partial class SettingsPage : ContentPage
{
    private readonly SettingsViewModel _viewModel;

    public SettingsPage()
	{
        if (BindingContext == null)
        {
            BindingContext = new SettingsViewModel();
        }
        InitializeComponent();
    }

}