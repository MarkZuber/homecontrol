namespace HomeControl.Web.Devices.Sony
{
    public interface ISonyProjectorDevice
    {
        bool DevicePower { get; set; }

        SonyCalibrationSettings AvailableCalibrationSettings { get; }
        SonySetting CalibrationSetting { get; set; }
    }
}
