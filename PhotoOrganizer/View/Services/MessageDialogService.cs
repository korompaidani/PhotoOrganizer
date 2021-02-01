using System.Windows;

namespace PhotoOrganizer.UI.View.Services
{
    public class MessageDialogService : IMessageDialogService
    {
        public MessageDialogResult ShowOkCancelDialog(string text, string title)
        {
            var result = MessageBox.Show(text, title, MessageBoxButton.OKCancel);
            return result == MessageBoxResult.OK
                ? MessageDialogResult.Ok
                : MessageDialogResult.Cancel;
        }

        public MessageDialogResult ShowYesOrNoDialog(string text, string title)
        {
            var result = MessageBox.Show(text, title, MessageBoxButton.YesNo);
            return result == MessageBoxResult.Yes
                ? MessageDialogResult.Yes
                : MessageDialogResult.No;
        }
    }

    public enum MessageDialogResult
    {
        Ok,
        Cancel,
        Yes,
        No
    }
}
