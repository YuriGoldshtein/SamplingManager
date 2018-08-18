using GalaSoft.MvvmLight.CommandWpf;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using Microsoft.Win32;
using SamplingManager.App.Logic;
using SamplingManager.App.Logic.Entities;
using SamplingManager.App.Logic.Export;
using SamplingManager.App.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;

namespace SamplingManager.App.ViewModels
{
    public class MainViewModel : IMainViewModel
    {
        public string ExperimentName { get; set; }
        public bool IsDeltaOnly { get; set; }

        public ObservableCollection<MeasurementModel> Measurements { get; set; }
        public ExperimentSettingsModel ExperimentSettings { get; set; }

        public SeriesCollection WeightGraphData { get; set; }
        public IList<string> WeightGraphLabels { get; set; } = new List<string>();

        public SeriesCollection SpeedGraphData { get; set; }
        public IList<string> SpeedGraphLabels { get; set; } = new List<string>();

        public SeriesCollection AccelerationGraphData { get; set; }
        public IList<string> AccelerationGraphLabels { get; set; } = new List<string>();

        public Func<double, string> YFormatter { get; } = value => value.ToString();

        public IMeasurementManager MeasurementManager { get; }
        public IExporter Exporter { get; }

        public MainViewModel(IMeasurementManager measurementManager, IExporter exporter)
        {
            MeasurementManager = measurementManager;
            Exporter = exporter;

            StartCommand = new RelayCommand(OnStart);
            StopCommand = new RelayCommand(OnStop);
            ExportCommand = new RelayCommand(OnExport, CanExport);

            ExperimentSettings = new ExperimentSettingsModel { SamplingIntervalMilliseconds = 1000 };

            Measurements = new ObservableCollection<MeasurementModel>();

            measurementManager.MeasurementReceived.ObserveOnDispatcher().Subscribe(OnMeasurementReceived);

            InitWeightGraph();
            InitSpeedGraph();
            InitAccelerationGraph();
        }

        private void InitAccelerationGraph()
        {
            AccelerationGraphData = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Acceleration/Time",
                    Values = new ChartValues<ObservablePoint>(),
                    PointGeometry = DefaultGeometries.Diamond,
                    PointGeometrySize = 5
                }

            };

            //AccelerationGraphLabels = WeightGraphLabels;
        }

        private void InitSpeedGraph()
        {
            SpeedGraphData = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Speed/Time",
                    Values = new ChartValues<ObservablePoint>(),
                    PointGeometry = DefaultGeometries.Square,
                    PointGeometrySize = 5
                }

            };

            //SpeedGraphLabels = WeightGraphLabels;
        }

        private void InitWeightGraph()
        {
            //var mapper = Mappers.Xy<MeasurementModel>()
            //       .X(model => model.Second)
            //       .Y(model => model.Weight);
            WeightGraphData = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Weight/Time",
                    Values = new ChartValues<ObservablePoint>(),
                    PointGeometry = DefaultGeometries.Circle,
                    PointGeometrySize = 5
                }
            };
        }

        public ICommand StartCommand { get; }

        private void OnStart()
        {
            if (MeasurementManager.CurrentExperiment == null)
            {
                var settings = 
                    new ExperimentSettings(
                        TimeSpan.FromMilliseconds(ExperimentSettings.SamplingIntervalMilliseconds),
                        IsDeltaOnly);
                MeasurementManager.CreateExperiment(settings);
            }
            else
                MeasurementManager.CurrentExperiment.Settings.SamplingInterval = TimeSpan.FromMilliseconds(ExperimentSettings.SamplingIntervalMilliseconds);

            MeasurementManager.Start();
        }

        public ICommand StopCommand { get; }

        private void OnStop()
        {
            MeasurementManager.Stop();
        }

        public ICommand ExportCommand { get; }

        private bool CanExport()
        {
            return MeasurementManager?.CurrentExperiment?.Measurements?.Any() == true;
        }

        private void OnExport()
        {
            var name = ExperimentName != null ? $"{ExperimentName}_" : string.Empty;

            SaveFileDialog dlg = new SaveFileDialog
            {
                FileName = $"{name}{DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss")}",
                DefaultExt = ".csv",
                Filter = "Experiments (.csv)|*.csv"
            };

            // Show save file dialog box.
            var isSaving = dlg.ShowDialog();

            // Process save file dialog box results
            if (isSaving == true)
            {
                var fileInfo = new FileInfo(dlg.FileName);
                Exporter.ExportAsync(MeasurementManager.CurrentExperiment, fileInfo);
            }
        }

        private void OnMeasurementReceived(MeasurementData data)
        {
            const int maxValues = 15;

            var measurementModel = new MeasurementModel(data);
            Measurements.Add(measurementModel);

            //if ((Measurements.Count % 10) == 0)
            //{
            //    var timeSpan = data.Timestamp - (MeasurementManager.CurrentExperiment?.StartTime ?? data.Timestamp);
            //}

            //WeightGraphLabels.Add(timeSpan.TotalSeconds.ToString());

            if (WeightGraphData[0].Values.Count > maxValues)
                WeightGraphData[0].Values.RemoveAt(0);

            WeightGraphData[0].Values.Add(new ObservablePoint(data.Second, data.Weight));

            if (SpeedGraphData[0].Values.Count > maxValues)
                SpeedGraphData[0].Values.RemoveAt(0);

            SpeedGraphData[0].Values.Add(new ObservablePoint(data.Second, data.Speed));

            if (AccelerationGraphData[0].Values.Count > maxValues)
                AccelerationGraphData[0].Values.RemoveAt(0);

            AccelerationGraphData[0].Values.Add(new ObservablePoint(data.Second, data.Acceleration));
        }
    }
}
