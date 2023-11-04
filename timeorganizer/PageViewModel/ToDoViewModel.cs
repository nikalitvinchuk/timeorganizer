using CommunityToolkit.Mvvm.ComponentModel;
using timeorganizer.PageViewModels;

namespace timeorganizer.PageViewModel;

public class ToDoViewModel : ObservableObject
{
    public AddTaskViewModel addTask { get; } = new AddTaskViewModel();
    public AddSubTaskViewModel AddSTask { get; } = new AddSubTaskViewModel();
    public FilterViewModel ShowTask { get; } = new FilterViewModel();

}