using CommunityToolkit.Mvvm.ComponentModel;
using timeorganizer.Services.TaskServiceExtension;

namespace timeorganizer.Services;

public class ToDoService : ObservableObject
{
	public AddTaskExtension addTask { get; } = new AddTaskExtension();
	public AddSubTaskExtension AddSTask { get; } = new AddSubTaskExtension();
	public FilterExtension showTask { get; } = new FilterExtension();
	public EditTaskService EditTask { get; } = new();
}