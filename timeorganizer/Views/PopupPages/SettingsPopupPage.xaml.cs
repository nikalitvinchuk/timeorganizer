using Mopups.Pages;
using Mopups.Services;
using timeorganizer.PageViewModel;

namespace timeorganizer.Views.PopupPages;


public partial class SettingsPopupPage : PopupPage
{
    private int _value;
    public SettingsPopupPage(int value)
    {

        this._value = value;
        this.BindingContext = new SettingsViewModel();

        InitializeComponent();
        if (_value == 1)
        {
            Email.IsVisible = true;
            Email_label.IsVisible= true;
            AcceptButton.Command = (BindingContext as SettingsViewModel).ChangeEmail;
        }
        else if (_value == 2)
        {
            Password.IsVisible = true;
            Password_label.IsVisible= true;
            PasswordVerif.IsVisible = true;
            PasswordVerif_label.IsVisible = true;            
            AcceptButton.Command = (BindingContext as SettingsViewModel).ChangePassword;

        }
        else if (_value == 3)
        {
            Password.IsVisible = true;
            Password_label.IsVisible= true;
            PasswordVerif.IsVisible = true;
            PasswordVerif_label.IsVisible = true;
            Email.IsVisible = true;
            Email_label.IsVisible = true;
            AcceptButton.Command = (BindingContext as SettingsViewModel).ChangeEmailAndPassword;
        }
    }


    private void Deny_Clicked(object sender, EventArgs e)
    {
        MopupService.Instance.PopAsync(true);

    }




}