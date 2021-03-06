﻿using SamplingManager.App.Logic.Sources.Settings;
using System;

namespace SamplingManager.App.Logic.Sources
{
    public class KWScaleSource : SerialPortScaleSource, IMeasurementSource
    {
        public KWScaleSource(IScaleSettings settings) : base(settings)
        {
        }

        protected override double MeasureWeight()
        {
            double weight;
            Port.DiscardInBuffer();
            var dataString = Port.ReadLine();
            var subStrings = dataString.Split(new[] { " ", "kg" }, StringSplitOptions.None);
            var weightString = subStrings[2];
            var weightKg = double.Parse(weightString);
            weight = weightKg * 1000.0;
            return weight;
        }
    }
}
