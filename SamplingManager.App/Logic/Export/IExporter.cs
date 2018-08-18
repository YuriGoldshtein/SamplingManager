using SamplingManager.App.Logic.Entities;
using System;
using System.IO;

namespace SamplingManager.App.Logic.Export
{
    public interface IExporter
    {
        IObservable<ExperimentData> ExportStarted { get; }
        IObservable<ExperimentData> ExportFinished { get; }

        void Export(ExperimentData experiment, FileInfo fileInfo);
        void ExportAsync(ExperimentData experiment, FileInfo fileInfo);
    }
}
