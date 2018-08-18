using SamplingManager.App.Logic.Entities;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace SamplingManager.App.Logic.Export
{
    public class CsvExporter : IExporter
    {
        private ISubject<ExperimentData> m_exportStarted = new Subject<ExperimentData>();
        private ISubject<ExperimentData> m_exportFinished = new Subject<ExperimentData>();

        public IObservable<ExperimentData> ExportStarted => m_exportStarted;

        public IObservable<ExperimentData> ExportFinished => m_exportFinished;

        public void Export(ExperimentData experiment, FileInfo fileInfo)
        {
            if (experiment == null)
                return;

            if (fileInfo == null)
                throw new ArgumentNullException(nameof(fileInfo));

            try
            {
                if (!fileInfo.Directory.Exists)
                    Directory.CreateDirectory(fileInfo.Directory.FullName);

                StringBuilder sb = new StringBuilder();
                ToCsv(experiment, sb);

                File.WriteAllText(fileInfo.FullName, sb.ToString());
            }
            catch (Exception ex)
            {
                Trace.Write(ex);
            }
        }

        public void ExportAsync(ExperimentData experiment, FileInfo fileInfo)
        {
            Task.Run(() => Export(experiment, fileInfo));
        }

        public void ToCsv(ExperimentData data, StringBuilder sb)
        {
            if (data == null)
                return;

            if (sb == null)
                sb = new StringBuilder();

            sb.AppendLine("Weight,Second,Speed,Acceleration,Time,Date");

            data.Measurements.ToList().ForEach(meas => ToCsv(meas, sb));
        }

        public void ToCsv(MeasurementData data, StringBuilder sb)
        {
            if (data == null)
                return;

            string time = data.Timestamp.ToString(" HH:mm:ss.fff", CultureInfo.InvariantCulture);
            string date = data.Timestamp.ToShortDateString();
            sb.AppendLine($"{data.Weight},{data.Second},{data.Speed},{data.Acceleration},{time},{date}");
        }
    }
}
