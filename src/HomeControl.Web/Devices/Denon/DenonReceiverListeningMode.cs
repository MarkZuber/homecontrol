// -----------------------------------------------------------------------
// <copyright file="DenonReceiverListeningMode.cs" company="ZubeNET">
//   Copyright...
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace HomeControl.Web.Devices.Denon
{
  public class DenonReceiverListeningMode
  {
    [JsonConstructor]
    public DenonReceiverListeningMode(string deviceKey, string displayName)
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

  public class DenonReceiverListeningModes
  {
    public DenonReceiverListeningMode Stereo { get; } = new DenonReceiverListeningMode("00", "Stereo");

    public DenonReceiverListeningMode Direct { get; } = new DenonReceiverListeningMode("01", "DIRECT");
    public DenonReceiverListeningMode Surround { get; } = new DenonReceiverListeningMode("02", "SURROUND");
    public DenonReceiverListeningMode Film { get; } = new DenonReceiverListeningMode("03", "FILM");
    public DenonReceiverListeningMode Thx { get; } = new DenonReceiverListeningMode("04", "THX");
    public DenonReceiverListeningMode Action { get; } = new DenonReceiverListeningMode("05", "ACTION");

    public DenonReceiverListeningMode Musical { get; } = new DenonReceiverListeningMode("06", "MUSICAL");

    // { "07", "MONO MOVIE" );
    public DenonReceiverListeningMode Orchestra { get; } = new DenonReceiverListeningMode("08", "ORCHESTRA");
    public DenonReceiverListeningMode Unplugged { get; } = new DenonReceiverListeningMode("09", "UNPLUGGED");
    public DenonReceiverListeningMode StudioMix { get; } = new DenonReceiverListeningMode("0A", "STUDIO-MIX");
    public DenonReceiverListeningMode TvLogic { get; } = new DenonReceiverListeningMode("0B", "TV LOGIC");
    public DenonReceiverListeningMode AllChStereo { get; } = new DenonReceiverListeningMode("0C", "ALL CH STEREO");
    public DenonReceiverListeningMode TheaterDimensional { get; } = new DenonReceiverListeningMode("0D", "THEATER DIMENSIONAL");
    public DenonReceiverListeningMode Enhanced { get; } = new DenonReceiverListeningMode("0E", "ENHANCED");
    public DenonReceiverListeningMode Mono { get; } = new DenonReceiverListeningMode("0F", "MONO");
    public DenonReceiverListeningMode PureAudio { get; } = new DenonReceiverListeningMode("11", "PURE AUDIO");
    public DenonReceiverListeningMode PureAudio2 { get; } = new DenonReceiverListeningMode("12", "PURE AUDIO");
    public DenonReceiverListeningMode PureAudio3 { get; } = new DenonReceiverListeningMode("13", "PURE AUDIO");
    public DenonReceiverListeningMode PureAudio4 { get; } = new DenonReceiverListeningMode("16", "PURE AUDIO");
    public DenonReceiverListeningMode Surround51 { get; } = new DenonReceiverListeningMode("40", "5.1ch Surround");
    public DenonReceiverListeningMode DolbyEx { get; } = new DenonReceiverListeningMode("41", "DOLBY EX");
    public DenonReceiverListeningMode ThxCinema { get; } = new DenonReceiverListeningMode("42", "THX Cinema");
    public DenonReceiverListeningMode ThxSurroundEx { get; } = new DenonReceiverListeningMode("43", "THX Surround EX");
    public DenonReceiverListeningMode ThxMusic { get; } = new DenonReceiverListeningMode("44", "THX Music");
    public DenonReceiverListeningMode ThxGames { get; } = new DenonReceiverListeningMode("45", "THX Games");

    public DenonReceiverListeningMode DtsSurround { get; } = new DenonReceiverListeningMode("DTS SURROUND", "DTS Surround");

    public IEnumerable<DenonReceiverListeningMode> All =>
      new List<DenonReceiverListeningMode>
      {
        Stereo,
        Direct,
        Surround,
        Film,
        Thx,
        Action,
        Musical,
        Orchestra,
        Unplugged,
        StudioMix,
        TvLogic,
        AllChStereo,
        TheaterDimensional,
        Enhanced,
        Mono,
        PureAudio,
        PureAudio2,
        PureAudio3,
        PureAudio4,
        Surround51,
        DolbyEx,
        ThxCinema,
        ThxSurroundEx,
        ThxMusic,
        ThxSurroundEx,
        ThxGames,
        DtsSurround
      };

    public DenonReceiverListeningMode FromDeviceKey(string deviceKey)
    {
      foreach (var mode in All)
      {
        if (mode.DeviceKey.Equals(deviceKey, StringComparison.OrdinalIgnoreCase))
        {
          return mode;
        }
      }

      Console.WriteLine($"Unknown Device Key: {deviceKey}");
      return new DenonReceiverListeningMode(string.Empty, "UNKNOWN");
    }
  }
}
