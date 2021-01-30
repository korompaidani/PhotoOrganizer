﻿using PhotoOrganizer.FileHandler;
using PhotoOrganizer.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace PhotoOrganizer.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public INavigationViewModel NavigationViewModel { get; }
        public IPhotoDetailViewModel PhotoDetailViewModel { get; }

        public MainViewModel(INavigationViewModel navigationViewModel, IPhotoDetailViewModel photoDetailViewModel)
        {
            NavigationViewModel = navigationViewModel;
            PhotoDetailViewModel = photoDetailViewModel;
        }

        public async Task LoadAsync()
        {
            await NavigationViewModel.LoadAsync();
        }
    }
}
