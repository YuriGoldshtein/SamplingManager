using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using SamplingManager.App.Logic;
using SamplingManager.App.Logic.Export;
using SamplingManager.App.Logic.Sources;
using SamplingManager.App.Logic.Sources.Settings;
using SamplingManager.App.ViewModels;
using SamplingManager.App.Views;
using System;
using System.Configuration;

namespace SamplingManager.App.Installers
{
    public class MainInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<IMainWindow>().ImplementedBy<MainWindow>().LifestyleSingleton());
            container.Register(Component.For<IMainViewModel>().ImplementedBy<MainViewModel>().LifestyleSingleton());

            container.Register(Component.For<IMeasurementManager>().ImplementedBy<MeasurementManager>().LifestyleSingleton());

            RegisterMeasurementSource(container);

            container.Register(Component.For<IScaleSettings>()
                .ImplementedBy<ScaleSettings>()
                .LifestyleSingleton()
                .DependsOn(
                Dependency.OnAppSettingsValue("DeviceName"),
                Dependency.OnAppSettingsValue("BaudRate")));

            container.Register(Component.For<IExporter>().ImplementedBy<CsvExporter>().LifestyleSingleton());
        }

        private static void RegisterMeasurementSource(IWindsorContainer container)
        {
            var model = ConfigurationManager.AppSettings["Model"];

            switch (model)
            {
                case "KWS":
                    container.Register(
                        Component.For<IMeasurementSource>()
                        .ImplementedBy<KWScaleSource>()
                        .LifestyleSingleton());
                    break;

                case "RADWAG":
                    container.Register(
                        Component
                        .For<IMeasurementSource>()
                        .ImplementedBy<RadwagScaleSource>()
                        .LifestyleSingleton());
                    break;
                default:
                    throw new NotSupportedException($"The model: {model} is not supported!");
            }
        }
    }
}
