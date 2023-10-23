using timeorganizer.Models;

namespace timeorganizer.Views;

public partial class LoggedMainPage : FlyoutPage
{
	public LoggedMainPage()
	{
		InitializeComponent();
        flyoutPage.collectionViewFlyout.SelectionChanged += OnSelectionChanged;
    }

    void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var item = e.CurrentSelection.FirstOrDefault() as FlyoutPageItem;
        {

            if (!((IFlyoutPageController)this).ShouldShowSplitMode)
                IsPresented = false;

            switch (item.Title)
            {
                case "Home":
                    //Detail = new NavigationPage(new ContactListPage());
                    break;

                case "Contacts":
                    //Detail = new NavigationPage(new ContactListPage());

                    break;

                case "Logout":
                    App.Current.MainPage = new AppShell();

                    break;
            }
        }
    }

}