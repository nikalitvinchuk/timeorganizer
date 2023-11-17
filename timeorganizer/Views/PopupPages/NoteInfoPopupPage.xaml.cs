using Mopups.Pages;
using Mopups.Services;
using timeorganizer.DatabaseModels;
using timeorganizer.PageViewModel;

namespace timeorganizer.Views.PopupPages;


public partial class NoteInfoPopupPage : PopupPage
{
    private Notes _note;
    public NoteInfoPopupPage(Notes note)
    {
        this._note = note;
        InitializeComponent();
        Title.Text= _note.Title;
        Content.Text = _note.Content;
    }


    private void Deny_Clicked(object sender, EventArgs e)
    {
        MopupService.Instance.PopAsync(true);

    }




}