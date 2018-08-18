using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using SamplingManager.App.Logic;
using SamplingManager.App.Logic.Export;
using SamplingManager.App.Logic.Sources;
using SamplingManager.App.Logic.Sources.Settings;
using SamplingManager.App.ViewModels;
using SamplingManager.App.Views;

namespace SamplingManager.App.Installers
{
    public class MainInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<IMainWindow>().ImplementedBy<MainWindow>().LifestyleSingleton());
            container.Register(Component.For<IMainViewModel>().ImplementedBy<MainViewModel>().LifestyleSingleton());

            container.Register(Component.For<IMeasurementManager>().ImplementedBy<MeasurementManager>().LifestyleSingleton());
            container.Register(Component.For<IMeasurementSource>().ImplementedBy<KWScaleSource>().LifestyleSingleton());
            container.Register(Component.For<IScaleSettings>()
                .ImplementedBy<ScaleSettings>()
                .LifestyleSingleton()
                .DependsOn(
                Dependency.OnAppSettingsValue("DeviceName"),
                Dependency.OnAppSettingsValue("BaudRate")));

            container.Register(Component.For<IExporter>().ImplementedBy<CsvExporter>().LifestyleSingleton());
        }
    }
}
