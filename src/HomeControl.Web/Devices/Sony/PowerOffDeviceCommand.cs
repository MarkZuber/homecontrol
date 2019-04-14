namespace HomeControl.Web.Devices.Sony
{
    public class PowerOffDeviceCommand : DeviceCommand
    {
        public PowerOffDeviceCommand()
            : base(0x0130)
        {
        }

        protected override void OnHandleMessage(SonyProjectorDevice device, short value)
        {
        }
    }
}
