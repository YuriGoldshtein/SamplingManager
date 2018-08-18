using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamplingManager.App.Logic.Entities
{
    public class ExperimentSettings
    {
        public TimeSpan SamplingInterval { get; set; }
        public bool IsDeltaOnly { get; set; }

        public ExperimentSettings(TimeSpan samplingInterval, bool isDeltaOnly)
        {
            SamplingInterval = samplingInterval;
            IsDeltaOnly = isDeltaOnly;
        }
    }
}
