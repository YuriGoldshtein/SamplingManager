using SamplingManager.App.Logic.Sources.Settings;
using System;

namespace SamplingManager.App.Logic.Sources
{
    public class RadwagScaleSource : SerialPortScaleSource, IMeasurementSource
    {
        public RadwagScaleSource(IScaleSettings settings) : base(settings)
        {
        }

        protected override double MeasureWeight()
        {
            Port.DiscardInBuffer();
            var dataString = Port.ReadLine();

            var subStrings = dataString.Split(new[] { " ", "g" }, StringSplitOptions.RemoveEmptyEntries);
            
            var weightString = subStrings[1];
            var weightKg = double.Parse(weightString);
            
            var weight = weightKg;
            
            return weight;
        }
    }
}
