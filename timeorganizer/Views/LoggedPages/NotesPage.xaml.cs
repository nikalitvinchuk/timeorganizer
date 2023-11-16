using System;
using System.Collections.ObjectModel;
using timeorganizer.Helpers;
using timeorganizer.PageViewModel;
//using Xamarin.Forms;
//using timeorganizer.Models;


namespace timeorganizer.Views.LoggedPages
{
    public partial class NotesPage : ContentPage
    {


        public NotesPage()
        {
            if (BindingContext == null)
                BindingContext = new NoteViewModel();

            InitializeComponent();
        }


        // Obs³uga zdarzenia wybrania notatki z listy
        private void OnNoteSelected(object sender, SelectedItemChangedEventArgs e)
        {
            // Mo¿esz dodaæ dodatkow¹ logikê obs³ugi, jeœli chcesz
        }
    }
}
