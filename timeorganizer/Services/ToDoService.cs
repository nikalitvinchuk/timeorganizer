using CommunityToolkit.Mvvm.ComponentModel;
using timeorganizer.PageViewModel;
using timeorganizer.PageViewModels;

namespace timeorganizer.Services;

public class ToDoService : ObservableObject
{
    public AddTaskExtension addTask { get; } = new AddTaskExtension();
    public AddSubTaskExtension AddSTask { get; } = new AddSubTaskExtension();
    public FilterExtension showTask { get; } = new FilterExtension();

}