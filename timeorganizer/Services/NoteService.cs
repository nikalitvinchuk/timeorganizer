using CommunityToolkit.Mvvm.ComponentModel;
using timeorganizer.Services.NoteServiceExtension;

namespace timeorganizer.Services;

public partial class NoteService : ObservableObject
{
    public AddNotesExtension AddNotes { get; } = new AddNotesExtension();

    public EditNotesService EditNotes { get; } = new EditNotesService();

    public FilterNotesViewModel ShowNotesTitle { get; } = new FilterNotesViewModel();

    //public async NoteServiceExtension GetNoteById(int noteId)

    //public async EditNotesService Updated()
    //{
    //    //przykład
    //    var existingNote = ShowNotesTitle.NotesColletion.FirstOrDefault(n => n.Id == EditNotes.noteId);
    //    if (existingNote != null)
    //    {
    //        existingNote.Title = ShowNotesTitle.NotesColletion.Title;
    //        existingNote.Content = EditNotes.Content;
    //        existingNote.Created = EditNotes.Created;
    //    }

        //    //czyszczenie danych po edycji notatki
        //    editNotes = new Notes();

        //    object value = await NoteServiceExtension.CompletedTask;
        //}

        //public async Notes<bool> DeleteNoteById(int noteId)
        //{
        //    //przykład
        //    var noteToRemove = NotesList.FirstOrDefault(n => n.Id == noteId);
        //    if (noteToRemove != null)
        //    {
        //        NotesList.Remove(noteToRemove);
        //        return true;
        //    }

        //    return false;
        //}
}