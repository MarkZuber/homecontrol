using System;

namespace HomeControl.Web.Devices.Sony
{
    public class EnumeratedDeviceCommand<T> : DeviceCommand
    {
        private readonly Func<SonyProjectorDevice, Action<T>> _getSetterAction;
        private readonly Func<SonyProjectorDevice, Func<short, T>> _getValueFunction;

        public EnumeratedDeviceCommand(short commandKey, Func<SonyProjectorDevice, Action<T>> getSetterAction, Func<SonyProjectorDevice, Func<short, T>> getValueFunction)
            : base(commandKey)
        {
            _getSetterAction = getSetterAction;
            _getValueFunction = getValueFunction;
        }

        protected override void OnHandleMessage(SonyProjectorDevice device, short value)
        {
            _getSetterAction(device)(_getValueFunction(device)(value));
        }
    }
}
