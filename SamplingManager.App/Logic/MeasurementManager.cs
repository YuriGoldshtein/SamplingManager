using SamplingManager.App.Logic.Entities;
using SamplingManager.App.Logic.Sources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace SamplingManager.App.Logic
{
    public class MeasurementManager : IMeasurementManager
    {
        private readonly ISubject<MeasurementData> m_measurementReceived = new Subject<MeasurementData>();

        public ExperimentData CurrentExperiment { get; set; }
        public IMeasurementSource Source { get; }

        public bool IsStarted { get; set; }

        public IObservable<MeasurementData> MeasurementReceived => m_measurementReceived;

        public MeasurementManager(IMeasurementSource source)
        {
            Source = source;

            Source.MeasurementReceived.Subscribe(OnMeasurementReceived);
        }

        public void CreateExperiment(ExperimentSettings settings)
        {
            CurrentExperiment = new ExperimentData(settings);
        }

        public void Start()
        {
            if (IsStarted)
                return;

            Source.Interval = CurrentExperiment.Settings.SamplingInterval;

            if (CurrentExperiment.StartTime == null)
                CurrentExperiment.StartTime = DateTime.Now;

            Source.Start();
        }

        public void Stop()
        {
            Task.Run(() =>
            {
                Source.Stop();
                IsStarted = false;
            });
        }

        private void OnMeasurementReceived(MeasurementData data)
        {
            var isChanged = Math.Abs(data.Speed) > 0.0000001;
            var shouldUpdate = !CurrentExperiment.Settings.IsDeltaOnly || isChanged;

            if (CurrentExperiment.Settings.IsDeltaOnly && isChanged)
            {
                var lastMeasurement = CurrentExperiment.Measurements.LastOrDefault();

                if (lastMeasurement != null)
                {
                    double timeDiff = (data.Second - lastMeasurement.Second);
                    data.Speed = (data.Weight - lastMeasurement.Weight) / timeDiff;
                    data.Acceleration = (data.Speed - lastMeasurement.Speed) / timeDiff;
                 }
             }

            if (shouldUpdate)
            {
                CurrentExperiment.Measurements.Add(data);
                m_measurementReceived.OnNext(data);
            }
        }
    }
}
