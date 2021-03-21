using PhotoOrganizer.Common;
using System;
using System.Threading.Tasks;

namespace PhotoOrganizer.UI.View.Services
{
    public interface IMessageDialogService
    {
        Task<MessageDialogResult> ShowOkCancelDialogAsync(string text, string title);
        Task<MessageDialogResult> ShowYesOrNoDialogAsync(string text, string title);
        Task<MessageDialogResult> ShowExtendOrOverwriteCancelDialogAsync(string text, string title);
        Task ShowInfoDialogAsync(string text);
        Task<MessageDialogResult> ShowSaveSaveAllDiscardDiscardAllDialogAsync();
        Task<string> SelectFolderPathAsync(string baseFolderPath);
        Task ShowProgressDuringTaskAsync(string title, string message, Func<string, Task> awaitableTask, string taskParameter);
    }
}