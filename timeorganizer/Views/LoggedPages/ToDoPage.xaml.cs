using System.Collections.ObjectModel;
using timeorganizer.DatabaseModels;
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
    ObservableCollection<Tasks> listaZadan = new();
    ObservableCollection<Tasks> listaSubZadan = new();
    
    protected override async void OnAppearing() {
        base.OnAppearing();

        //FilterViewModel filterViewModel = new();
        //listaSubZadan = await filterViewModel.FilterTasks();
        //listaZadan = await filterViewModel.FilterTasks();

        ////
        //taskView.ItemsSource = filterViewModel.TasksCollection;

        //
        //subTaskView.ItemsSource = listaSubZadan;
    }
    
    private void Refresh(object sender, EventArgs e) {
        

    }
    private void OnShowButtonClicekd(object sender, EventArgs e) {
        if (taskView.IsVisible == true) 
            taskView.IsVisible = false;
        else
            taskView.IsVisible = true;
    }
    private void OnAddButtonClicked(object sender, EventArgs e)
    {
        AddTaskLayout.IsVisible = true;
        ButtonAddTask.IsVisible = false;
    }
}