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
        Task<string> SelectFileOrFolderDialogAsync(string baseFolderPath, string description, bool isFileDialog = false);
        Task ShowProgressDuringTaskAsync(string title, string message, Func<string, Task> awaitableTask, string taskParameter);
    }
}