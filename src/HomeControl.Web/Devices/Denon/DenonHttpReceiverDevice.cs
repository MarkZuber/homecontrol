// -----------------------------------------------------------------------
// <copyright file="DenonHttpReceiverDevice.cs" company="ZubeNET">
//   Copyright...
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Net.Http;

namespace HomeControl.Web.Devices.Denon
{
    public class DenonHttpReceiverDevice : IDenonHttpReceiverDevice
    {
        public DenonHttpReceiverDevice(string deviceHostAddress)
        {
            DeviceHostAddr = deviceHostAddress;
        }

        public string DeviceHostAddr { get; }

        public DenonReceiverInputs AvailableInputs => new DenonReceiverInputs();

        public DenonReceiverListeningModes AvailableListeningModes => new DenonReceiverListeningModes();

        public bool DevicePower
        {
            get => false;
            set => SendCommand(value ? "PWON" : "PWSTANDBY");
        }

        public bool MainZonePower
        {
            get => false;
            set => SendCommand(value ? "ZMON" : "ZMSTANDBY");
        }

        public int MainZoneVolume
        {
            get => LoadSummary().Volume;
            set
            {
                int volume = value;
                if (volume < 0)
                {
                    volume = 0;
                }
                if (volume > 100)
                {
                    volume = 100;
                }
                SendCommand($"MV{ToDenonValue(volume)}");
            }
        }

        public bool MainZoneMute
        {
            get => LoadSummary().IsMuted;
            set => SendCommand(value ? "MUON" : "MUOFF");
        }

        public DenonReceiverListeningMode MainZoneListeningMode
        {
            get => new DenonReceiverListeningModes().Direct;
            set { } // DON'T KNOW COMMAND FOR THIS YET  SendCommand()
        }

        public DenonReceiverInput MainZoneInput
        {
            get => new DenonReceiverInputs().Game;
            set => SendCommand($"SI{value.DeviceKey}");
        }

        public bool Zone2Power { get; set; }

        public bool Zone2Mute { get; set; }

        public bool Zone3Power { get; set; }

        public bool Zone3Mute { get; set; }

        private string GetCmdUrl()
        {
            return $"http://{DeviceHostAddr}/goform/formiPhoneAppDirect.xml?";
        }

        private string GetStatusUrl()
        {
            return $"http://{DeviceHostAddr}/goform/";
        }

        private void SendCommand(string command)
        {
            string url = GetCmdUrl() + System.Net.WebUtility.UrlEncode(command);
            HttpClient client = new HttpClient();
            var response = client.GetAsync(new Uri(url)).Result;
        }

        private DenonReceiverHttpSummary LoadSummary()
        {
            string url = GetStatusUrl() + "formMainZone_MainZoneXml.xml";
            HttpClient client = new HttpClient();
            var response = client.GetAsync(new Uri(url)).Result;
            string xml = response.Content.ReadAsStringAsync().Result;
            return new DenonReceiverHttpSummary(xml);
        }

        private string ToDenonValue(int percent)
        {
            // clamp
            int actualPercent = Math.Max(0, Math.Min(percent, 95));
            string dbString = string.Format($"{actualPercent:D2}");
            //// Round to nearest number divisible by 0.5
            //percent = percent.divide(POINTFIVE).setScale(0, RoundingMode.UP).multiply(POINTFIVE)
            //                 .min(connection.getMainVolumeMax()).max(BigDecimal.ZERO);

            //string dbString = string.valueOf(percent.intValue());

            //if (percent.compareTo(BigDecimal.TEN) == -1)
            //{
            //    dbString = "0" + dbString;
            //}
            //if (percent.remainder(BigDecimal.ONE).equals(POINTFIVE))
            //{
            //    dbString = dbString + "5";
            //}

            return dbString;
        }

        private int FromDenonValue(string str)
        {
            /*
             * 455 = 45.5
             * 45 = 45
             * 045 = 4.5
             * 04 = 4
             */
            double value = double.Parse(str);
            if (string.Compare(str, "99", StringComparison.Ordinal) == 0 || (str.StartsWith("0") && str.Length > 2))
            {
                value = value / 10.0;
            }

            return Convert.ToInt32(value);
        }
    }
}
