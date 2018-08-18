using SamplingManager.App.Logic.Entities;
using System;
using System.Diagnostics;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace SamplingManager.App.Logic.Sources
{
    public class DummyMeasurementSource : IMeasurementSource
    {
        private Task m_task;
        private CancellationTokenSource m_cancellationTokenSource;
        private double m_lastSecond;
        private readonly ISubject<MeasurementData> m_measurementReceived = new Subject<MeasurementData>();

        public IObservable<MeasurementData> MeasurementReceived { get { return m_measurementReceived; } }

        public TimeSpan Interval { get; set; }

        public void Start()
        {
            if (m_cancellationTokenSource != null)
                return;

            m_cancellationTokenSource = new CancellationTokenSource();

            m_task = Task.Run(() =>
            {
                var weight = 1.00;
                var second = m_lastSecond;
                var speed = 0.0;
                var acceleration = 0.0;

                var random = new Random();
                const int weightRange = 5;
                
                while (!m_cancellationTokenSource.IsCancellationRequested)
                {
                    var measurement = new MeasurementData(weight, second, speed, acceleration, DateTime.Now);
                    m_lastSecond = second;

                    second += Interval.TotalSeconds;
                    double timeDiff = (second - measurement.Second);

                    weight = random.NextDouble() * weightRange;
                    speed = (weight - measurement.Weight) / timeDiff;
                    acceleration = (speed - measurement.Speed) / timeDiff;

                    Thread.Sleep(Interval);
                    m_measurementReceived.OnNext(measurement);
                }
            }, m_cancellationTokenSource.Token);
        }

        public void Stop()
        {
            m_cancellationTokenSource.Cancel();

            try
            {
                m_task.Wait(5000);
                m_cancellationTokenSource.Dispose();
                m_cancellationTokenSource = null;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);                
            }
        }
    }
}
