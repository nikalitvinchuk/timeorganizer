
using timeorganizer.Views;

namespace timeorganizer
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            
            InitializeComponent();
        }

        private async void OnRegisterClicked(object sender, EventArgs e)
        {
          await Shell.Current.GoToAsync("RegisterPage");
           

            //SemanticScreenReader.Announce(CounterBtn.Text);
        }
        private async void OnLoginClicked(object sender, EventArgs e)
        {
           await Shell.Current.GoToAsync("LoginPage");
        }
    }
}