using timeorganizer.PageViewModel;
using timeorganizer.PageViewModels;

namespace timeorganizer.Views.LoggedPages;

public partial class ToDoPage : ContentPage
{
    public ToDoPage()
    {
        if (BindingContext == null)
            BindingContext = new ToDoViewModel();
        InitializeComponent();
    }
    private void OnAddButtonClicked(object sender, EventArgs e)
    {
        AddTaskLayout.IsVisible = true;
        ButtonAddTask.IsVisible = false;
    }
}