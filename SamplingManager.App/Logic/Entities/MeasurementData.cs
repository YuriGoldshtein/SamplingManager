using System;

namespace SamplingManager.App.Logic.Entities
{
    public class MeasurementData
    {
        public double Weight { get; set; }
        public double Second { get; set; }
        public double Speed { get; set; }
        public double Acceleration { get; set; }
        public DateTime Timestamp { get; set; }

        public MeasurementData(double weight, double second, double speed, double acceleration, DateTime timestamp)
        {
            Weight = weight;
            Second = second;
            Speed = speed;
            Acceleration = acceleration;
            Timestamp = timestamp;
        }
    }
}