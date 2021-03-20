using PhotoOrganizer.Common;
using System.Threading.Tasks;

namespace PhotoOrganizer.UI.View.Services
{
    public interface IMessageDialogService
    {
        Task<MessageDialogResult> ShowOkCancelDialogAsync(string text, string title);
        Task<MessageDialogResult> ShowYesOrNoDialogAsync(string text, string title);
        Task ShowInfoDialogAsync(string text);
        Task<MessageDialogResult> ShowSaveDialog();
    }
}