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
    protected override async void OnAppearing() {
        base.OnAppearing();
        FilterViewModel filterViewModel = new();
        listaZadan = await filterViewModel.FilterTasks();
        taskView.ItemsSource = listaZadan;
        OnPropertyChanged(nameof(listaZadan));
    }
    private void Refresh(object sender, EventArgs e) {
        OnAppearing();
    }
    private void OnAddButtonClicked(object sender, EventArgs e)
    {
        AddTaskLayout.IsVisible = true;
        ButtonAddTask.IsVisible = false;
    }
}