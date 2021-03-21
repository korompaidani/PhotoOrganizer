using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using PhotoOrganizer.UI.ViewModel;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using MessageDialogResult = PhotoOrganizer.Common.MessageDialogResult;

namespace PhotoOrganizer.UI.View.Services
{
    public class MessageDialogService : IMessageDialogService
    {
        private MetroWindow MetroWindow => (MetroWindow)App.Current.MainWindow;

        public async Task<MessageDialogResult> ShowOkCancelDialogAsync(string text, string title)
        {            
            var result = await MetroWindow.ShowMessageAsync(title, text, MessageDialogStyle.AffirmativeAndNegative);
            return result == MahApps.Metro.Controls.Dialogs.MessageDialogResult.Affirmative
                ? MessageDialogResult.Ok
                : MessageDialogResult.Cancel;
        }

        public async Task ShowInfoDialogAsync(string text)
        {
            await MetroWindow.ShowMessageAsync("Info", text);
        }

        public async Task<MessageDialogResult> ShowYesOrNoDialogAsync(string text, string title)
        {
            var settings = new MetroDialogSettings
            {
                DefaultButtonFocus = MahApps.Metro.Controls.Dialogs.MessageDialogResult.Affirmative,
                AffirmativeButtonText = "Yes",
                NegativeButtonText = "No",
                FirstAuxiliaryButtonText = "Cancel"
            };

            var result = await MetroWindow.ShowMessageAsync(title, text, MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, settings);
            
            if (result == MahApps.Metro.Controls.Dialogs.MessageDialogResult.Affirmative)
            {
                return MessageDialogResult.Yes;
            }
            if (result == MahApps.Metro.Controls.Dialogs.MessageDialogResult.Negative)
            {
                return MessageDialogResult.No;
            }

            return MessageDialogResult.Cancel;
        }

        public async Task<MessageDialogResult> ShowExtendOrOverwriteCancelDialogAsync(string text, string title)
        {
            var settings = new MetroDialogSettings
            {
                DefaultButtonFocus = MahApps.Metro.Controls.Dialogs.MessageDialogResult.Affirmative,
                AffirmativeButtonText = "Extend",
                NegativeButtonText = "Overwrite",
                FirstAuxiliaryButtonText = "Cancel"
            };

            var result = await MetroWindow.ShowMessageAsync(title, text, MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, settings);
            
            if(result == MahApps.Metro.Controls.Dialogs.MessageDialogResult.Affirmative)
            {
                return MessageDialogResult.Extend;
            }
            if (result == MahApps.Metro.Controls.Dialogs.MessageDialogResult.Negative)
            {
                return MessageDialogResult.Overwrite;
            }

            return MessageDialogResult.Cancel;
        }

        public async Task<string> SelectFolderPathAsync(string baseFolderPath, string description)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            folderDialog.SelectedPath = baseFolderPath;
            folderDialog.Description = description;
            folderDialog.ShowDialog();
            var selectedPath = folderDialog.SelectedPath;

            if (string.IsNullOrEmpty(selectedPath))
            {
                return string.Empty;
            }

            var isValid = Directory.Exists(selectedPath);

            if (!isValid)
            {
                var settings = new MetroDialogSettings
                {
                    DefaultButtonFocus = MahApps.Metro.Controls.Dialogs.MessageDialogResult.Affirmative,
                    AffirmativeButtonText = "Yes",
                    NegativeButtonText = "No",
                    FirstAuxiliaryButtonText = "Cancel"
                };

                var result = await MetroWindow.ShowMessageAsync("Warning", "The selected path is invalid, would you like to try again?", MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, settings);

                if (result == MahApps.Metro.Controls.Dialogs.MessageDialogResult.Affirmative)
                {
                    return await SelectFolderPathAsync(baseFolderPath, description);
                }

                return string.Empty;
            }

            return selectedPath;
        }

        public async Task<MessageDialogResult> ShowSaveSaveAllDiscardDiscardAllDialogAsync()
        {
            var window = new SaveDialog();
            var model = new SaveModel();

            window.DataContext = model;
            window.Owner = System.Windows.Application.Current.MainWindow;

            var dialogResult = await ShowDialogAsync(window);
            window.Close();
            window = null;

            return model.Answer;
        }

        public async Task<MessageDialogResult> ShowSaveDialogAsync(string text, string title)
        {
            var settings = new MetroDialogSettings
            {
                DefaultButtonFocus = MahApps.Metro.Controls.Dialogs.MessageDialogResult.Negative,
                AffirmativeButtonText = "Save",
                NegativeButtonText = "Save All",
                FirstAuxiliaryButtonText = "Discard",
                SecondAuxiliaryButtonText = "Discard All",
            };

            var result = await MetroWindow.ShowMessageAsync(title, text, MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, settings);

            if (result == MahApps.Metro.Controls.Dialogs.MessageDialogResult.Affirmative)
            {
                return MessageDialogResult.Save;
            }
            if (result == MahApps.Metro.Controls.Dialogs.MessageDialogResult.Negative)
            {
                return MessageDialogResult.SaveAll;
            }
            if (result == MahApps.Metro.Controls.Dialogs.MessageDialogResult.FirstAuxiliary)
            {
                return MessageDialogResult.Discard;
            }
            if (result == MahApps.Metro.Controls.Dialogs.MessageDialogResult.SecondAuxiliary)
            {
                return MessageDialogResult.DiscardAll;
            }

            return MessageDialogResult.Cancel;
        }

        public async Task ShowProgressDuringTaskAsync(string title, string message, Func<string, Task> awaitableTask, string taskParameter)
        {
            var progress = await MetroWindow.ShowProgressAsync(title, message, false);
            progress.SetIndeterminate();

            await awaitableTask(taskParameter);
            await progress.CloseAsync();

            progress = null;
        }

        private Task<bool?> ShowDialogAsync(Window self)
        {
            if (self == null) throw new ArgumentNullException("self");

            TaskCompletionSource<bool?> completion = new TaskCompletionSource<bool?>();
            self.Dispatcher.BeginInvoke(new Action(() => completion.SetResult(self.ShowDialog())));

            return completion.Task;
        }        
    }    
}
