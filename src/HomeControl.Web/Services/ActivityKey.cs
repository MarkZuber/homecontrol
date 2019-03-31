using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeControl.Web.Services
{
    public static class ActivityKey
    {
        public const string TheaterXboxOn = "theater.xbox.on";
        public const string TheaterPs4On = "theater.ps4.on";
        public const string TheaterFireTvOn = "theater.firetv.on";
        public const string TheaterAppleTvOn = "theater.appletv.on";

        public const string TheaterProjectorOn = "theater.projector.on";
        public const string TheaterProjectorOf = "theater.projector.off";

        public const string TheaterOn = "theater.on";
        public const string TheaterOff = "theater.off";

        public const string TheaterVolumeUp = "theater.volume.up";
        public const string TheaterVolumeDown = "theater.volume.down";
        public const string TheaterVolumeToggleMute = "theater.volume.togglemute";
    }
}
