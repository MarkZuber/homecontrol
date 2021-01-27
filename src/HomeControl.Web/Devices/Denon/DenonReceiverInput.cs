// -----------------------------------------------------------------------
// <copyright file="DenonReceiverInput.cs" company="ZubeNET">
//   Copyright...
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HomeControl.Web.Devices.Denon
{
    public class DenonReceiverInput
    {
        [JsonConstructor]
        public DenonReceiverInput(string deviceKey, string displayName)
        {
            DeviceKey = deviceKey;
            DisplayName = displayName;
        }

        public string DeviceKey { get; }
        public string DisplayName { get; }

        public override string ToString()
        {
            return DisplayName;
        }
    }

    public class DenonReceiverInputs
    {
        public IEnumerable<DenonReceiverInput> All =>
          new List<DenonReceiverInput>
          {
        Phono,
        Cd,
        Tuner,
        Dvd,
        Tv,
        SatCbl,
        Mplay,
        Game,
        HdRadio,
        Net,
        Pandora,
        SiriusXm,
        Spotify,
        LastFm,
        Flickr,
        Radio,
        Server,
        Favorites,
        Aux1,
        Aux2,
        Bluetooth,
        UsbIpod,
        Usb,
        IpodDirect,
        RadioRecent,
        UsbFavorites,
          };

        public DenonReceiverInput Phono { get; } = new DenonReceiverInput("PHONO", "Phono");
        public DenonReceiverInput Cd { get; } = new DenonReceiverInput("CD", "CD");
        public DenonReceiverInput Tuner { get; } = new DenonReceiverInput("TUNER", "Tuner");
        public DenonReceiverInput Dvd { get; } = new DenonReceiverInput("DVD", "DVD");
        public DenonReceiverInput Tv { get; } = new DenonReceiverInput("TV", "TV");
        public DenonReceiverInput SatCbl { get; } = new DenonReceiverInput("SAT/CBL", "SAT/CBL");
        public DenonReceiverInput Mplay { get; } = new DenonReceiverInput("MPLAY", "MediaPlayer");
        public DenonReceiverInput Game { get; } = new DenonReceiverInput("GAME", "Game");
        public DenonReceiverInput HdRadio { get; } = new DenonReceiverInput("HDRADIO", "HDRadio");
        public DenonReceiverInput Net { get; } = new DenonReceiverInput("NET", "Net");
        public DenonReceiverInput Pandora { get; } = new DenonReceiverInput("PANDORA", "PANDORA");
        public DenonReceiverInput SiriusXm { get; } = new DenonReceiverInput("SIRIUSXM", "SIRIUSXM");
        public DenonReceiverInput Spotify { get; } = new DenonReceiverInput("SPOTIFY", "SPOTIFY");
        public DenonReceiverInput LastFm { get; } = new DenonReceiverInput("LASTFM", "LASTFM");
        public DenonReceiverInput Flickr { get; } = new DenonReceiverInput("FLICKR", "FLICKR");
        public DenonReceiverInput Radio { get; } = new DenonReceiverInput("IRADIO", "IRADIO");
        public DenonReceiverInput Server { get; } = new DenonReceiverInput("SERVER", "SERVER");
        public DenonReceiverInput Favorites { get; } = new DenonReceiverInput("FAVORITES", "FAVORITES");
        public DenonReceiverInput Aux1 { get; } = new DenonReceiverInput("AUX1", "AUX1");
        public DenonReceiverInput Aux2 { get; } = new DenonReceiverInput("AUX2", "AUX2");
        public DenonReceiverInput Bluetooth { get; } = new DenonReceiverInput("BT", "Bluetooth");
        public DenonReceiverInput UsbIpod { get; } = new DenonReceiverInput("USB/IPOD", "USB/IPOD");
        public DenonReceiverInput Usb { get; } = new DenonReceiverInput("USB", "USB");
        public DenonReceiverInput IpodDirect { get; } = new DenonReceiverInput("IPD", "IPod Direct");
        public DenonReceiverInput RadioRecent { get; } = new DenonReceiverInput("IRP", "IRadioRecent");
        public DenonReceiverInput UsbFavorites { get; } = new DenonReceiverInput("FVP", "USB Favorites");

        public DenonReceiverInput FromDeviceKey(string deviceKey)
        {
            foreach (var input in All)
            {
                if (input.DeviceKey.Equals(deviceKey, StringComparison.OrdinalIgnoreCase))
                {
                    return input;
                }
            }

            return new DenonReceiverInput(string.Empty, "UNKNOWN");
        }
    }
}
