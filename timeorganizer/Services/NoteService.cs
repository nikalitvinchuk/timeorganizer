using CommunityToolkit.Mvvm.ComponentModel;
using timeorganizer.Services.NoteServiceExtension;

namespace timeorganizer.Services;

public partial class NoteService : ObservableObject
{
	public AddNotesExtension AddNotes { get; } = new AddNotesExtension();
	public FilterNotesViewModel ShowNotesTitle { get; } = new FilterNotesViewModel();

}