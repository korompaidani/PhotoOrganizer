using PhotoOrganizer.UI.View.Services;
using Prism.Events;
using System;
using System.Threading.Tasks;

namespace PhotoOrganizer.UI.ViewModel
{
    public class ShelveViewModel : DetailViewModelBase, IShelveViewModel
    {
        public ShelveViewModel(IEventAggregator eventaggregator, IMessageDialogService messageDialogService) : base(eventaggregator, messageDialogService)
        {

        }

        public override Task LoadAsync(int id)
        {
            throw new NotImplementedException();
        }

        protected override void OnDeleteExecute()
        {
            throw new NotImplementedException();
        }

        protected override bool OnSaveCanExecute()
        {
            throw new NotImplementedException();
        }

        protected override void OnSaveExecute()
        {
            throw new NotImplementedException();
        }
    }
}
