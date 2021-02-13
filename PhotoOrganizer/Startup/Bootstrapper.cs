using Autofac;
using PhotoOrganizer.DataAccess;
using PhotoOrganizer.UI.ViewModel;
using Prism.Events;
using PhotoOrganizer.UI.Data.Lookups;
using PhotoOrganizer.UI.Data.Repositories;
using PhotoOrganizer.UI.View.Services;
using PhotoOrganizer.UI.Services;
using PhotoOrganizer.FileHandler;

namespace PhotoOrganizer.UI.Startup
{
    public class Bootstrapper
    {
        public IContainer Bootstrap()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<PhotoOrganizerDbContext>().AsSelf();

            builder.RegisterType<EventAggregator>().As<IEventAggregator>().SingleInstance();

            builder.RegisterType<MainWindow>().AsSelf();
            builder.RegisterType<MainViewModel>().AsSelf();

            builder.RegisterType<MessageDialogService>().As<IMessageDialogService>();

            builder.RegisterType<DirectoryReader>().AsSelf();
            builder.RegisterType<DirectoryReaderWrapperService>().As<IDirectoryReaderWrapperService>();


            builder.RegisterType<LookupDataService>().AsImplementedInterfaces();

            builder.RegisterType<NavigationViewModel>().As<INavigationViewModel>();
            builder.RegisterType<PhotoDetailViewModel>().Keyed<IDetailViewModel>(nameof(PhotoDetailViewModel));
            builder.RegisterType<AlbumDetailViewModel>().Keyed<IDetailViewModel>(nameof(AlbumDetailViewModel));
            builder.RegisterType<YearDetailViewModel>().Keyed<IDetailViewModel>(nameof(YearDetailViewModel));

            builder.RegisterType<PhotoRepository>().As<IPhotoRepository>();
            builder.RegisterType<AlbumRepository>().As<IAlbumRepository>();
            builder.RegisterType<YearRepository>().As<IYearRepository>();
            return builder.Build();
        }
    }
}
