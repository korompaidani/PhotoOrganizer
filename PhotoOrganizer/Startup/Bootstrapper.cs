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
            builder.RegisterType<BackupManager>().AsSelf();
            builder.RegisterType<ExifToFileWriter>().AsSelf();
            builder.RegisterType<BackupService>().As<IBackupService>();
            builder.RegisterType<PhotoCacheService>().As<ICacheService>();
            builder.RegisterType<PageSizeService>().As<IPageSizeService>().SingleInstance();

            builder.RegisterType<LookupDataService>().AsImplementedInterfaces();

            builder.RegisterType<NavigationViewModel>().As<INavigationViewModel>().SingleInstance();
            builder.RegisterType<PhotoDetailViewModel>().Keyed<IDetailViewModel>(nameof(PhotoDetailViewModel));
            builder.RegisterType<AlbumDetailViewModel>().Keyed<IDetailViewModel>(nameof(AlbumDetailViewModel));
            builder.RegisterType<LocationDetailViewModel>().Keyed<IDetailViewModel>(nameof(LocationDetailViewModel));

            builder.RegisterType<PeopleRepository>().As<IPeopleRepository>();
            builder.RegisterType<PhotoRepository>().As<IPhotoRepository>().SingleInstance();
            builder.RegisterType<AlbumRepository>().As<IAlbumRepository>();
            builder.RegisterType<LocationRepository>().As<ILocationRepository>();

            builder.RegisterType<BulkAttributeSetterService>().As<IBulkAttributeSetterService>().SingleInstance();
            builder.RegisterType<PhotoMetaWrapperService>().As<IPhotoMetaWrapperService>().SingleInstance();
            builder.RegisterType<SettingsHandler>().As<ISettingsHandler>().SingleInstance();
            return builder.Build();
        }
    }
}