using CommunityToolkit.Mvvm.ComponentModel;
using timeorganizer.PageViewModel.NotesViewModel;


namespace timeorganizer.PageViewModel;

public partial class NoteViewModel : ObservableObject
{
    public AddNotesViewModel AddNotes { get; } = new AddNotesViewModel();
    public FilterNotesViewModel ShowNotesTitle { get; } = new FilterNotesViewModel(); 

}