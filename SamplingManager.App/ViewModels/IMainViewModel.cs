using System.Collections.ObjectModel;
using SamplingManager.App.Models;

namespace SamplingManager.App.ViewModels
{
    public interface IMainViewModel
    {
        ObservableCollection<MeasurementModel> Measurements { get; }
    }
}