﻿using PhotoOrganizer.Common;
using PhotoOrganizer.UI.Event;
using PhotoOrganizer.UI.View.Services;
using Prism.Commands;
using Prism.Events;
using System;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PhotoOrganizer.UI.ViewModel
{
    public abstract class DetailViewModelBase : ViewModelBase, IDetailViewModel
    {
        private bool _hasChanges;
        private int _id;
        private string _title;

        protected readonly IEventAggregator EventAggregator;
        public ICommand SaveCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public ICommand CloseDetailViewCommand { get; }

        protected IMessageDialogService MessageDialogService { get; set; }

        public bool HasChanges
        {
            get { return _hasChanges; }
            set
            {
                if(_hasChanges != value)
                {
                    _hasChanges = value;
                    OnPropertyChanged();
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public int Id
        {
            get { return _id; }
            protected set
            {
                _id = value;
            }
        }

        public string Title
        {
            get { return _title; }
            protected set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        public DetailViewModelBase(IEventAggregator eventAggregator, IMessageDialogService messageDialogService)
        {
            EventAggregator = eventAggregator;
            MessageDialogService = messageDialogService;
            SaveCommand = new DelegateCommand(OnSaveExecute, OnSaveCanExecute);
            DeleteCommand = new DelegateCommand(OnDeleteExecute);
            CloseDetailViewCommand = new DelegateCommand(OnCloseDetailViewExecute);
        }        

        public abstract Task LoadAsync(int id);

        public abstract Task SaveChanges(bool isClosing);

        protected abstract void OnDeleteExecute();

        protected abstract bool OnSaveCanExecute();

        protected abstract void OnSaveExecute();

        protected async virtual void OnCloseDetailViewExecute()
        {
            if (HasChanges)
            {
                var result = await MessageDialogService.ShowOkCancelDialogAsync(
                    "You've made changes. Do you really want to close this item?", "Question");
                if(result == MessageDialogResult.Cancel)
                {
                    return;
                }
            }

            EventAggregator.GetEvent<AfterDetailClosedEvent>()
                .Publish(new AfterDetailClosedEventArgs
                {
                    Id = this.Id,
                    ViewModelName = this.GetType().Name
                });
        }

        protected virtual void RaiseDetailDeletedEvent(int modelId)
        {
            EventAggregator.GetEvent<AfterDetailDeletedEvent>().Publish(
                new AfterDetailDeletedEventArgs
                {
                    Id = modelId,
                    ViewModelName = this.GetType().Name
                });
        }

        protected virtual void RaiseDetailSavedEvent(int modelId, string title)
        {
            EventAggregator.GetEvent<AfterDetailSavedEvent>().Publish(
                new AfterDetailSavedEventArgs
                {
                    Id = modelId,
                    Title = title,
                    ViewModelName = this.GetType().Name
                });
        }

        protected virtual void RaiseCollectionSavedEvent()
        {
            EventAggregator.GetEvent<AfterCollectionSavedEvent>()
                .Publish(new AfterCollectionSavedEventArgs
                {
                    ViewModelName = this.GetType().Name
                });
        }

        protected async Task SaveWithOptimisticConcurrencyAsync(Func<Task> saveFunc,
            Action afterSaveAction)
        {
            try
            {
                await saveFunc();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var databaseValues = ex.Entries.Single().GetDatabaseValues();
                if (databaseValues == null)
                {
                    await MessageDialogService.ShowInfoDialogAsync("The entity has been deleted by another user");
                    RaiseDetailDeletedEvent(Id);
                    return;
                }

                var result = await MessageDialogService.ShowOkCancelDialogAsync("The entity has been changed in the meantime by someone else. Click OK to save your changes anyway, click Cancel to reload the entity from the database.", "Question");

                if (result == MessageDialogResult.Ok)
                {
                    // Update the original values with database-values
                    var entry = ex.Entries.Single();
                    entry.OriginalValues.SetValues(entry.GetDatabaseValues());
                    await saveFunc();
                }
                else
                {
                    // Reload entity from db
                    await ex.Entries.Single().ReloadAsync();
                    await LoadAsync(Id);
                }
            }

            afterSaveAction();
        }        
    }
}
