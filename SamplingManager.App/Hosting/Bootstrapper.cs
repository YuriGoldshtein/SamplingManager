using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using SamplingManager.App.Installers;
using SamplingManager.App.Views;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamplingManager.App.Hosting
{
    public static class Bootstrapper
    {
        public static IWindsorContainer Container { get; private set; }

        public static void Start()
        {
            BuildContainer();
            InstallContainer();

            var mainWindow = Container.Resolve<IMainWindow>();
            mainWindow.Show();
        }

        private static void InstallContainer()
        {
            Container.Install(new MainInstaller());
        }

        private static void BuildContainer()
        {
            Container = new WindsorContainer();
            Container.Kernel.Resolver.AddSubResolver(new CollectionResolver(Container.Kernel));
        }
    }
}
