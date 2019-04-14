using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using HomeControl.Web.Devices.Transports;
using Newtonsoft.Json;

namespace HomeControl.Web.Devices.Sony
{
    public class SonySetting
    {
        [JsonConstructor]
        public SonySetting(short deviceKey, string displayName)
        {
            DeviceKey = deviceKey;
            DisplayName = displayName;
        }

        public short DeviceKey { get; }
        public string DisplayName { get; }

        public override string ToString()
        {
            return DisplayName;
        }
    }

    public class SonyCalibrationSettings
    {
        public const short ItemKey = 0x0002;

        public IEnumerable<SonySetting> All =>
            new List<SonySetting>
            {
                CalibrationFilm1,
                CalibrationFilm2,
                CalibrationRef,
                CalibrationTv,
                CalibrationPhoto,
                CalibrationGame,
                CalibrationBrightCinema,
                CalibrationBrightTv,
                CalibrationUser
            };

        public SonySetting CalibrationFilm1 { get; } = new SonySetting(0x0000, "Film 1");
        public SonySetting CalibrationFilm2 { get; } = new SonySetting(0x0001, "Film 2");
        public SonySetting CalibrationRef { get; } = new SonySetting(0x0002, "Reference");
        public SonySetting CalibrationTv { get; } = new SonySetting(0x0003, "TV");
        public SonySetting CalibrationPhoto { get; } = new SonySetting(0x0004, "Photo");
        public SonySetting CalibrationGame { get; } = new SonySetting(0x0005, "Game");
        public SonySetting CalibrationBrightCinema { get; } = new SonySetting(0x0006, "Bright Cinema");
        public SonySetting CalibrationBrightTv { get; } = new SonySetting(0x0007, "Bright TV");
        public SonySetting CalibrationUser { get; } = new SonySetting(0x0008, "User");

        public SonySetting FromDeviceKey(short deviceKey)
        {
            foreach (var input in All)
            {
                if (input.DeviceKey == deviceKey)
                {
                    return input;
                }
            }

            return new SonySetting(-1, "UNKNOWN");
        }
    }

    public class SonyProjectorDevice : ISonyProjectorDevice, IDisposable
    {
        public const short ItemContrast = 0x0010;
        public const short ItemBrightness = 0x0011;
        public const short ItemColor = 0x0012;
        public const short ItemHue = 0x0013;
        public const short ItemSharpness = 0x0014;
        public const short ItemColorTemp = 0x0017;

        public const short ItemLampControl = 0x001A;
        public const short LampControlLow = 0x0000;
        public const short LampControlHigh = 0x0001;

        public const short ItemBlackLevelAdjust = 0x001C;
        public const short BlackLevelOff = 0x0000;
        public const short BlackLevelLow = 0x0001;
        public const short BlackLevelHigh = 0x0002;
        public const short BlackLevelMiddle = 0x0003;

        public const short ItemAdvancedIris = 0x001D;
        public const short AdvancedIrisOff = 0x0000;
        public const short AdvancedIrisManual = 0x0001;
        public const short AdvancedIrisAutoFull = 0x0002;
        public const short AdvancedIrisAutoLimited = 0x0003;

        public const short ItemFilmMode = 0x001F;
        public const short ItemGammaCorrection = 0x0022;
        public const short ItemNoiseReduction = 0x0025;
        public const short ItemColorSpace = 0x003B;
        public const short ItemUserGainRed = 0x0050;
        public const short ItemUserGainGreen = 0x0051;
        public const short ItemUserGainBlue = 0x0052;
        public const short ItemUserBiasRed = 0x0053;
        public const short ItemUserBiasGreen = 0x0054;
        public const short ItemUserBiasBlue = 0x0055;
        public const short IrisManual = 0x0057;

        public const short ItemFilmProjection = 0x0058;
        public const short FilmProjectionOff = 0x0000;
        public const short FilmProjectionOn = 0x0001;

        public const short ItemMotionEnhancer = 0x0059;
        public const short MotionEnhancerOff = 0x0000;
        public const short MotionEnhancerLow = 0x0001;
        public const short MotionEnhancerHigh = 0x0002;

        public const short ItemXvColor = 0x005A;

        public const short ItemRealityCreation = 0x0067;
        public const short RealityCreationOff = 0x0000;
        public const short RealityCreationOn = 0x0001;

        public const short ItemResolution = 0x0068;
        public const short ItemNoiseFiltering = 0x0069;

        public const short ItemColorCorrection = 0x006A;
        public const short ItemClearWhite = 0x006B;
        public const short ItemMpegNoiseReduction = 0x006C;
        public const short ItemSmoothGradation = 0x006D;

        public const short ItemPicturePosition = 0x0066;
        public const short PicturePosition185To1 = 0x0000;
        public const short PicturePosition235To1 = 0x0001;
        public const short PicturePositionCustom1 = 0x0002;
        public const short PicturePositionCustom2 = 0x0003;
        public const short PicturePositionCustom3 = 0x0004;

        public const short ItemAspect = 0x0020;
        public const short AspectNormal = 0x0001;
        public const short AspectVerticalStretch = 0x000B;
        public const short Aspect185To1Zoom = 0x000C;
        public const short Aspect235To1Zoom = 0x000D;
        public const short AspectStretch = 0x000E;
        public const short AspectSqueeze = 0x000F;

        public const short ItemOverScan = 0x0023;
        public const short OverScanOff = 0x0000;
        public const short OverScanOn = 0x0001;

        public const short ItemInput = 0x0001;
        public const short InputA = 0x0002;
        public const short InputComponent = 0x0003;
        public const short InputHdmi1 = 0x0004;
        public const short InputHdmi2 = 0x0005;

        public const short ItemPictureMuting = 0x0030;

        public const short ItemHdmi1DynamicRange = 0x006E;
        public const short ItemHdmi2DynamicRange = 0x006F;
        public const short HdmiDynamicRangeAuto = 0x0000;
        public const short HdmiDynamicRangeLimit = 0x0001;
        public const short HdmiDynamicRangeFull = 0x0002;

        public const short ItemDisplaySelect2D3D = 0x0060;
        public const short DisplaySelectAuto = 0x0000;
        public const short DisplaySelect3D = 0x0001;
        public const short DisplaySelect2D = 0x0002;

        public const short ItemFormat3D = 0x0061;
        public const short Format3DSimulated = 0x0000;
        public const short Format3DSideBySide = 0x0001;
        public const short Format3DOverUnder = 0x0002;

        public const short ItemDepthAdjust3D = 0x0062;

        public const short ItemSimulated3DEffect = 0x0063;
        public const short Simulated3DEffectHigh = 0x0000;
        public const short Simulated3DEffectMiddle = 0x0001;
        public const short Simulated3DEffectLow = 0x0002;

        public const short ItemBrightness3D = 0x0072;
        public const short Brightness3DHigh = 0x0000;
        public const short Brightness3DStandard = 0x0001;

        // GET ONLY
        public const short ItemStatusError = 0x0101;
        public const short ErrorNone = 0x0000;
        public const short ErrorLamp = 0x0001;
        public const short ErrorFan = 0x0002;
        public const short ErrorCover = 0x0004;
        public const short ErrorTemp = 0x0008;
        public const short ErrorD5V = 0x0010;
        public const short ErrorPower = 0x0020;
        public const short ErrorTempWarning = 0x0040;
        public const short ErrorNvmData = 0x0080;

        public const short ItemStatusPower = 0x0102;
        public const short StatusPowerStandby = 0x0000;
        public const short StatusPowerStartup = 0x0001;
        public const short StatusPowerStartupLamp = 0x0002;
        public const short StatusPowerOn = 0x0003;
        public const short StatusPowerCooling1 = 0x0004;
        public const short StatusPowerCooling2 = 0x0005;
        public const short StatusPowerSavingCooling1 = 0x0006;
        public const short StatusPowerSavingCooling2 = 0x0007;
        public const short StatusPowerSavingStandby = 0x0008;

        public const short ItemLampTimer = 0x0113;
        public const short ItemStatusError2 = 0x0125;

        private readonly IEnumerable<DeviceCommand> _deviceCommands = new List<DeviceCommand>
        {
            CalibrationSettingCommand,
        };

        private readonly object _lockObj = new object();
        private readonly BlockingCollection<byte[]> _outgoingMessages = new BlockingCollection<byte[]>(new ConcurrentQueue<byte[]>());
        private readonly SonyNetworkProjectorTcpTransport _tcpTransport;

        private SonySetting _calibrationSetting;
        private readonly ManualResetEventSlim _connectionAvailableEvent = new ManualResetEventSlim();

        private bool _devicePower;

        private Timer _pingTimer = null;

        public SonyProjectorDevice()
        {
            DeviceKey = string.Empty;

            _tcpTransport = new SonyNetworkProjectorTcpTransport(_outgoingMessages);
            _tcpTransport.ConnectionStateChanged += TcpTransport_ConnectionStateChanged;
        }

        private static EnumeratedDeviceCommand<SonySetting> CalibrationSettingCommand { get; } = new EnumeratedDeviceCommand<SonySetting>(
            SonyCalibrationSettings.ItemKey,
            d => d.SetCalibrationSetting,
            d => d.AvailableCalibrationSettings.FromDeviceKey);

        private static PowerOnDeviceCommand PowerOnSettingCommand { get; } = new PowerOnDeviceCommand();
        private static PowerOffDeviceCommand PowerOffSettingCommand { get; } = new PowerOffDeviceCommand();
        public event EventHandler<ConnectionStateChangedEventArgs> ConnectionStateChanged;

        /// <inheritdoc />
        public async Task ConnectAsync(string deviceIpAddress, int devicePort)
        {
            IpAddress = deviceIpAddress;
            Port = devicePort;

            await _tcpTransport.ConnectAsync(
                IpAddress,
                Port,
                new byte[]
                {
                    12,
                    34,
                    56
                });
        }

        public bool IsConnected => _tcpTransport.ConnectionState == ConnectionState.Connected;

        public async Task DisconnectAsync()
        {
            await _tcpTransport.DisconnectAsync();
        }


        // TODO: set these up properly!
        /// <inheritdoc />
        public string IpAddress { get; private set; }

        /// <inheritdoc />
        public int Port { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <inheritdoc />
        public string DevicePhysicalAddress { get; }

        /// <inheritdoc />
        public string DisplayName { get; }

        /// <inheritdoc />
        public string DeviceKey { get; }

        public bool DevicePower
        {
            get => _devicePower;
            set
            {
                SetDevicePower(value);
                if (value)
                {
                    SendDeviceCommand(PowerOnSettingCommand, new SonySetting(0x0001, "PowerStartup"));
                }
                else
                {
                    SendDeviceCommand(PowerOffSettingCommand, new SonySetting(0x0000, "PowerStandby"));
                }
            }
        }

        public SonyCalibrationSettings AvailableCalibrationSettings { get; } = new SonyCalibrationSettings();

        public SonySetting CalibrationSetting
        {
            get => _calibrationSetting;
            set
            {
                SetCalibrationSetting(value);
                SendDeviceCommand(CalibrationSettingCommand, value);
            }
        }

        //public ISonyNetworkProjector ToPropertyBag()
        //{
        //    return new SonyNetworkProjectorPropertyBag(this);
        //}

        internal void SetDevicePower(bool value)
        {
            _devicePower = value;
            RaisePropertyChanged(nameof(DevicePower));
        }

        internal void SetCalibrationSetting(SonySetting value)
        {
            _calibrationSetting = value;
            RaisePropertyChanged(nameof(CalibrationSetting));
        }

        private void SendDeviceCommand(DeviceCommand deviceCommand, SonySetting value)
        {
            SendSdcpMessage(false, deviceCommand, value);
        }

        private void SendQueryCommand(DeviceCommand deviceCommand)
        {
            SendSdcpMessage(true, deviceCommand, null);
        }

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void TcpTransport_ConnectionStateChanged(object sender, ConnectionStateChangedEventArgs e)
        {
            ConnectionStateChanged?.Invoke(this, e);

            _pingTimer?.Dispose();
            _pingTimer = null;

            if (e.ConnectionState == ConnectionState.Connected)
            {
                // this sets up the keep alive with the connection so the projector will keep responding
                _pingTimer = new Timer(
                    (ctx) => { SendQueryCommand(CalibrationSettingCommand); },
                    null,
                    (int)TimeSpan.FromSeconds(5).TotalMilliseconds,
                    (int)TimeSpan.FromSeconds(5).TotalMilliseconds);

                RequeryAllStates();
            }
        }

        public void RequeryAllStates()
        {
            //foreach (var command in _deviceCommands)
            //{
            //    SendQueryCommand(command.CommandPrefix);
            //}
        }

        private void SendSdcpMessage(bool isGetOperation, DeviceCommand deviceCommand, SonySetting value)
        {
            var message = deviceCommand.FormatSdcpMessage(isGetOperation, value);
            _outgoingMessages.Add(message);

            // todo: do this in a background thread with a signal...
            // write the data to the network stream
            // await _tcpTransport.SendMessageByteAsync(ms.ToArray());

            // read the response from the network stream (we MUST read a response before we can send another command anyway...  Need to ensure this is serialized...
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (_tcpTransport.ConnectionState == ConnectionState.Connected)
            {
                DisconnectAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            }
            // _outgoingMessages?.Dispose();
            // _connectionAvailableEvent?.Dispose();
            // _pingTimer?.Dispose();
        }
    }
}
