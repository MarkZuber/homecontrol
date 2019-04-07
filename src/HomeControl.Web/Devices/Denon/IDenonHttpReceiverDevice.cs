// -----------------------------------------------------------------------
// <copyright file="IDenonNetworkReceiver.cs" company="ZubeNET">
//   Copyright...
// </copyright>
// -----------------------------------------------------------------------

namespace HomeControl.Web.Devices.Denon
{
  public interface IDenonHttpReceiverDevice
    {
    DenonReceiverInputs AvailableInputs { get; }
    DenonReceiverListeningModes AvailableListeningModes { get; }

    bool DevicePower { get; set; }
    bool MainZonePower { get; set; }
    int MainZoneVolume { get; set; }
    bool MainZoneMute { get; set; }
    DenonReceiverListeningMode MainZoneListeningMode { get; set; }
    DenonReceiverInput MainZoneInput { get; set; }

    bool Zone2Power { get; set; }
    bool Zone2Mute { get; set; }
    bool Zone3Power { get; set; }
    bool Zone3Mute { get; set; }
  }
}
