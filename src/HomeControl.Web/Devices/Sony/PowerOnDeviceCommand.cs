namespace HomeControl.Web.Devices.Sony
{
    public class PowerOnDeviceCommand : DeviceCommand
    {
        public PowerOnDeviceCommand()
            : base(0x0130)
        {
        }

        protected override void OnHandleMessage(SonyProjectorDevice device, short value)
        {
        }
    }
}
