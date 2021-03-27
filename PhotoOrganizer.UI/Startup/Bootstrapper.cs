using Autofac;
using PhotoOrganizer.DataAccess;
using PhotoOrganizer.UI.ViewModel;
using Prism.Events;
using PhotoOrganizer.UI.Data.Lookups;
using PhotoOrganizer.UI.Data.Repositories;
using PhotoOrganizer.UI.View.Services;
using PhotoOrganizer.UI.Services;
using PhotoOrganizer.FileHandler;
using PhotoOrganizer.UI.StateMachine;
using PhotoOrganizer.UI.StateMachine.MetaSerializationStates;

namespace PhotoOrganizer.UI.Startup
{
    public class Bootstrapper
    {
        public static IContainer Container { get; private set; }

        public void Bootstrap()
        {
            var builder = new ContainerBuilder();
            
            builder.RegisterType<ApplicationContext>().AsSelf().SingleInstance();
            builder.RegisterType<PhotoOrganizerDbContext>().AsSelf();

            builder.RegisterType<EventAggregator>().As<IEventAggregator>().SingleInstance();

            builder.RegisterType<MainWindow>().AsSelf();
            builder.RegisterType<MainViewModel>().AsSelf();

            builder.RegisterType<MessageDialogService>().As<IMessageDialogService>();

            builder.RegisterType<DirectoryReader>().AsSelf();
            builder.RegisterType<XmlWriterComponent>().AsSelf();

            builder.RegisterType<DirectoryReaderWrapperService>().As<IDirectoryReaderWrapperService>();
            builder.RegisterType<BackupManager>().AsSelf();
            builder.RegisterType<MyExifReaderWriter>().Keyed<IExifReaderWriter>(nameof(MyExifReaderWriter));
            builder.RegisterType<ExifLibraryReaderWriter>().Keyed<IExifReaderWriter>(nameof(ExifLibraryReaderWriter));
            builder.RegisterType<BackupService>().As<IBackupService>();
            builder.RegisterType<PhotoCacheService>().As<ICacheService>();
            builder.RegisterType<PageSizeService>().As<IPageSizeService>().SingleInstance();

            builder.RegisterType<LookupDataService>().AsImplementedInterfaces();

            builder.RegisterType<NavigationViewModel>().As<INavigationViewModel>().SingleInstance();
            builder.RegisterType<PhotoDetailViewModel>().Keyed<IDetailViewModel>(nameof(PhotoDetailViewModel));
            builder.RegisterType<AlbumDetailViewModel>().Keyed<IDetailViewModel>(nameof(AlbumDetailViewModel));
            builder.RegisterType<LocationDetailViewModel>().Keyed<IDetailViewModel>(nameof(LocationDetailViewModel));

            builder.RegisterType<PeopleRepository>().As<IPeopleRepository>();
            builder.RegisterType<PhotoRepository>().As<IPhotoRepository>();
            builder.RegisterType<AlbumRepository>().As<IAlbumRepository>();
            builder.RegisterType<LocationRepository>().As<ILocationRepository>().SingleInstance();
            builder.RegisterType<MaintenanceRepository>().As<IMaintenanceRepository>().SingleInstance();

            builder.RegisterType<BulkAttributeSetterService>().As<IBulkAttributeSetterService>().SingleInstance();
            builder.RegisterType<PhotoMetaWrapperService>().As<IPhotoMetaWrapperService>().SingleInstance();
            builder.RegisterType<SettingsHandler>().As<ISettingsHandler>().SingleInstance();

            builder.RegisterType<PhotoDetailContext>().As<IPhotoDetailContext>();
            builder.RegisterType<OpeningPhotoDetailState>().AsSelf();
            builder.RegisterType<OpenPhotoDetailState>().AsSelf();
            builder.RegisterType<ClosingPhotoDetailState>().AsSelf();
            builder.RegisterType<ClosedPhotoDetailState>().AsSelf();

            builder.RegisterType<FileSystem>().AsSelf();

            Container = builder.Build();
        }
    }
}