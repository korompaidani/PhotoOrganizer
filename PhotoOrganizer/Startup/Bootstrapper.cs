using Autofac;
using PhotoOrganizer.Data;
using PhotoOrganizer.DataAccess;
using PhotoOrganizer.ViewModel;
using Prism.Events;

namespace PhotoOrganizer.Startup
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
            
            builder.RegisterType<LookupDataService>().AsImplementedInterfaces();
            builder.RegisterType<NavigationViewModel>().As<INavigationViewModel>();
            builder.RegisterType<PhotoDetailViewModel>().As<IPhotoDetailViewModel>();
            builder.RegisterType<PhotoDataService>().As<IPhotoDataService>();

            return builder.Build();
        }
    }
}
