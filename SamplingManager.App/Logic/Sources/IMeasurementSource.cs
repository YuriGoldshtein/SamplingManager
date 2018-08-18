using SamplingManager.App.Logic.Entities;
using System;

namespace SamplingManager.App.Logic.Sources
{
    public interface IMeasurementSource
    {
        IObservable<MeasurementData> MeasurementReceived { get; }
        TimeSpan Interval { get; set; }

        void Start();
        void Stop();
    }
}
