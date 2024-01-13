using CommunityToolkit.Mvvm.ComponentModel;
using timeorganizer.Services.NoteServiceExtension;

namespace timeorganizer.Services;

public partial class NoteService : ObservableObject
{
    public AddNotesExtension AddNotes { get; } = new AddNotesExtension();

    public EditNotesService EditNotes { get; } = new EditNotesService();

    public FilterNotesViewModel ShowNotesTitle { get; } = new FilterNotesViewModel();
}