using Autofac;
using PhotoOrganizer.ViewModel;

namespace PhotoOrganizer.Startup
{
    public class Bootstrapper
    {
        public IContainer Bootstrap()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<MainWindow>().AsSelf();
            builder.RegisterType<MainViewModel>().AsSelf();
            builder.RegisterType<PhotoDataService>().As<IPhotoDataService>();

            return builder.Build();
        }
    }
}
