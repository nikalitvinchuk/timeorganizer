using System.Collections.ObjectModel;
using timeorganizer.Models;

namespace timeorganizer.Views;

public partial class FlyoutMenuPage : ContentPage
{
	public FlyoutMenuPage()
	{
		InitializeComponent();
        flyoutPageItems.Add(new FlyoutPageItem { Title = "Home", MenuIcon = "home.png" });//menuIcon jest to ikona jaka ma si� wy�wietla� przy poszczeg�lnej opcji  
        flyoutPageItems.Add(new FlyoutPageItem { Title = "Contacts", MenuIcon = "contacts.png" });
        flyoutPageItems.Add(new FlyoutPageItem { Title = "Logout", MenuIcon = "settings.png" });

        collectionViewFlyout.ItemsSource = flyoutPageItems;
    }
    ObservableCollection<FlyoutPageItem> flyoutPageItems = new ObservableCollection<FlyoutPageItem>(); //kojelka wykorzystywana w menu - istneje mozliwosc generowania obiektow bezposredniu
                                                                                                       ///z bazy obecnie zrobione statycznie w ciele funkcji (linie 11-13)
	public ObservableCollection<FlyoutPageItem> FlyoutPageItems { get { return flyoutPageItems; } }


}