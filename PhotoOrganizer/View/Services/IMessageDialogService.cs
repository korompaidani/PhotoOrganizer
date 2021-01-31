namespace PhotoOrganizer.UI.View.Services
{
    public interface IMessageDialogService
    {
        MessageDialogResult ShowOkCancelDialog(string text, string title);
        MessageDialogResult ShowYesOrNoDialog(string text, string title);
    }
}