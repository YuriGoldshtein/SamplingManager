namespace SamplingManager.App.Logic.Sources.Settings
{
    public interface IScaleSettings
    {
        string DeviceName { get; }
        int BaudRate { get; }
    }
}
