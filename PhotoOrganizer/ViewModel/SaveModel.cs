using PhotoOrganizer.Common;
using Prism.Commands;
using System.Windows.Input;

namespace PhotoOrganizer.UI.ViewModel
{
    public class SaveModel
    {
        public ICommand SaveCommand;
        public ICommand SaveAllCommand;
        public ICommand DiscardCommand;
        public ICommand DiscardAllCommand;
        public ICommand CancelCommand;

        public MessageDialogResult Answer { get; private set; }

        public SaveModel()
        {
            SaveCommand = new DelegateCommand(OnSaveExecute);
            SaveAllCommand = new DelegateCommand(OnSaveAllExecute);
            DiscardCommand = new DelegateCommand(OnDiscardExecute);
            DiscardAllCommand = new DelegateCommand(OnDiscardAllExecute);
            CancelCommand = new DelegateCommand(OnCancelAllExecute);
        }

        private void OnCancelAllExecute()
        {
            Answer = MessageDialogResult.Cancel;
        }

        private void OnDiscardAllExecute()
        {
            Answer = MessageDialogResult.DiscardAll;
        }

        private void OnDiscardExecute()
        {
            Answer = MessageDialogResult.Discard;
        }

        private void OnSaveAllExecute()
        {
            Answer = MessageDialogResult.SaveAll;
        }

        private void OnSaveExecute()
        {
            Answer = MessageDialogResult.Save;
        }
    }
}
