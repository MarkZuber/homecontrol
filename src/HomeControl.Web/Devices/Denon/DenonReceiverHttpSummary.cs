// -----------------------------------------------------------------------
// <copyright file="DenonReceiverHttpSummary.cs" company="ZubeNET">
//   Copyright...
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Xml;

namespace HomeControl.Web.Devices.Denon
{
    public class DenonReceiverHttpSummary
    {
        public DenonReceiverHttpSummary(string xml)
        {
            var doc = new XmlDocument();
            doc.LoadXml(xml);

            IsMuted = string.Compare(doc.SelectSingleNode("/item/Mute/value").InnerText, "on", StringComparison.OrdinalIgnoreCase) == 0;
            IsVolumeAbsolute = string.Compare(doc.SelectSingleNode("/item/VolumeDisplay/value").InnerText, "Absolute", StringComparison.OrdinalIgnoreCase) == 0;

            string volumeValue = doc.SelectSingleNode("/item/MasterVolume/value").InnerText;
            if (string.Compare(volumeValue, "--", StringComparison.OrdinalIgnoreCase) == 0)
            {
              Volume = 0;
            }
            else
            {
              Volume = Convert.ToInt32(double.Parse(volumeValue));
              if (IsVolumeAbsolute)
              {
                  Volume += 80;
              }
            }
        }

        public bool IsMuted { get; }
        public bool IsVolumeAbsolute { get; }
        public int Volume { get; }
    }
}
