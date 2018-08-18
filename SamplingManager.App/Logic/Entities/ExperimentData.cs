using System;
using System.Collections.Generic;

namespace SamplingManager.App.Logic.Entities
{
    public class ExperimentData
    {
        public ExperimentSettings Settings { get; set; }
        public DateTime? StartTime { get; set; }
        public IList<MeasurementData> Measurements { get; set; }

        public ExperimentData(ExperimentSettings settings)
        {
            Settings = settings;
            Measurements = new List<MeasurementData>();
        }
    }
}