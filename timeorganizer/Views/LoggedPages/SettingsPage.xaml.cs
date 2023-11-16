using Microsoft.Extensions.Configuration;
using Mopups.Interfaces;
using Mopups.Services;
using timeorganizer.PageViewModel;

namespace timeorganizer.Views.LoggedPages;

public partial class SettingsPage : ContentPage
{
    private readonly SettingsViewModel _viewModel;
    IPopupNavigation popupNavigation;
    public SettingsPage()
	{
        if (BindingContext == null)
        {
            BindingContext = new SettingsViewModel();
        }
        InitializeComponent();
    }
    

}