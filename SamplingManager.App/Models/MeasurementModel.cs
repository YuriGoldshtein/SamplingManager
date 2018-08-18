using SamplingManager.App.Logic.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamplingManager.App.Models
{
    public class MeasurementModel
    {
        protected MeasurementData Entity { get; set; }

        public double Weight { get { return Entity.Weight; } }
        public double Second { get { return Entity.Second; } }
        public double Speed { get { return Entity.Speed; } }
        public double Acceleration { get { return Entity.Acceleration; } }
        public string Time { get { return Entity.Timestamp.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture); } }
        public string Date { get { return Entity.Timestamp.ToShortDateString(); } }

        public MeasurementModel(MeasurementData entity)
        {
            Entity = entity;
        }
    }
}
