using SamplingManager.App.Logic.Entities;
using System;

namespace SamplingManager.App.Logic
{
    public interface IMeasurementManager
    {
        ExperimentData CurrentExperiment { get; set; }
        IObservable<MeasurementData> MeasurementReceived { get; }

        void CreateExperiment(ExperimentSettings settings);
        void Start();
        void Stop();
    }
}