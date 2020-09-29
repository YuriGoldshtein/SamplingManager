using SamplingManager.App.Logic.Entities;
using SamplingManager.App.Logic.Sources.Settings;
using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace SamplingManager.App.Logic.Sources
{
    public abstract class SerialPortScaleSource : IMeasurementSource
    {
        private Task m_task;
        private CancellationTokenSource m_cancellationTokenSource;
        private double m_lastSecond = 0;
        private readonly ISubject<MeasurementData> m_measurementReceived = new Subject<MeasurementData>();

        public IObservable<MeasurementData> MeasurementReceived { get { return m_measurementReceived; } }

        public TimeSpan Interval { get; set; }

        public SerialPort Port { get; set; }
        public IScaleSettings Settings { get; }

        protected abstract double MeasureWeight();

        protected SerialPortScaleSource(IScaleSettings settings)
        {
            Settings = settings;
        }

        public void Start()
        {
            if (m_cancellationTokenSource != null)
                return;

            m_cancellationTokenSource = new CancellationTokenSource();

            if (Port == null)
                Port = new SerialPort(Settings.DeviceName, Settings.BaudRate, Parity.None, 8, StopBits.One);

            if (!Port.IsOpen)
                Port.Open();

            m_task = Task.Run(() =>
            {
                var second = m_lastSecond + 1;
                var weight = 0.0;
                var speed = 0.0;
                var acceleration = 0.0;

                var measurement = new MeasurementData(weight, m_lastSecond, speed, acceleration, DateTime.Now);

                while (!m_cancellationTokenSource.IsCancellationRequested)
                {
                    try
                    {
                        UpdateMeasurement(ref second, out weight, out speed, out acceleration, ref measurement);
                    }
                    catch (Exception ex)
                    {
                        Trace.Write(ex);
                    }
                }

                try
                {
                    if (Port.IsOpen)
                        Port.Close();
                }
                catch (Exception ex)
                {
                    Trace.Write(ex);
                }
            }, m_cancellationTokenSource.Token);
        }

        private void UpdateMeasurement(ref double second, out double weight, out double speed, out double acceleration, ref MeasurementData measurement)
        {
            weight = MeasureWeight();

            double timeDiff = (second - measurement.Second);

            speed = (weight - measurement.Weight) / timeDiff;
            acceleration = (speed - measurement.Speed) / timeDiff;

            measurement = new MeasurementData(weight, second, speed, acceleration, DateTime.Now);

            m_lastSecond = second;
            second += Interval.TotalSeconds;

            m_measurementReceived.OnNext(measurement);
            Thread.Sleep(Interval);
        }
              
        public void Stop()
        {
            m_cancellationTokenSource.Cancel();

            try
            {
                m_task.Wait(25000);
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