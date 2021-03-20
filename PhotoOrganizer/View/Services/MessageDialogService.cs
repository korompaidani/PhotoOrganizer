using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using PhotoOrganizer.UI.ViewModel;
using System;
using System.Threading.Tasks;
using System.Windows;
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
            var result = await MetroWindow.ShowMessageAsync(title, text, MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary);
            return result == MahApps.Metro.Controls.Dialogs.MessageDialogResult.Affirmative
                ? MessageDialogResult.Yes
                : MessageDialogResult.No;
        }

        public async Task<MessageDialogResult> ShowSaveDialog()
        {
            var window = new SaveDialog();
            var model = new SaveModel();

            window.DataContext = model;
            window.Owner = Application.Current.MainWindow;

            var dialogResult = await ShowDialogAsync(window);
            window.Close();
            window = null;

            return model.Answer;
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
