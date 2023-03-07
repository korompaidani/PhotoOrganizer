using PhotoOrganizer.UI.Event;
using PhotoOrganizer.UI.Services;
using Prism.Commands;
using Prism.Events;
using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Input;

namespace PhotoOrganizer.UI.ViewModel
{
	public class WritingToFileViewModel : ViewModelBase
	{
		private IEventAggregator _eventAggregator;
		private IPhotoMetaWrapperService _photoMetaWrapperService;
		private ProgressBar _progressBar;

		ICommand CloseWindowCommand { get; }

		public ProgressBar ProgressBar {get{ return _progressBar; } set{_progressBar = value;}}

        public WritingToFileViewModel(IEventAggregator eventAggregator, IPhotoMetaWrapperService photoMetaWrapperService)
        {
            _eventAggregator = eventAggregator;
            _photoMetaWrapperService = photoMetaWrapperService;

            CloseWindowCommand = new DelegateCommand(OnCloseWindowCommand);
        }

        private void OnCloseWindowCommand()
        {
            _eventAggregator.GetEvent<CloseProgressWindowEvent>().Publish(
                new CloseProgressWindowEventArgs());
        }

		public void WindowContentRendered(object sender, EventArgs e)
		{
			BackgroundWorker worker = new BackgroundWorker();
			worker.WorkerReportsProgress = true;
			worker.DoWork += worker_DoWork;
			worker.ProgressChanged += worker_ProgressChanged;

			worker.RunWorkerAsync();
		}

		void worker_DoWork(object sender, DoWorkEventArgs e)
		{
			for (int i = 0; i < 100; i++)
			{
				(sender as BackgroundWorker).ReportProgress(i);
				Thread.Sleep(100);
			}
		}

		void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			ProgressBar.Value = e.ProgressPercentage;
		}
	}
}
