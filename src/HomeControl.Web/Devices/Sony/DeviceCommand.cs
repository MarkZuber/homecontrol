using System.IO;
using System.Text;

namespace HomeControl.Web.Devices.Sony
{
public abstract class DeviceCommand
    {
        protected DeviceCommand(short commandKey)
        {
            CommandKey = commandKey;
        }

        public short CommandKey { get; }

        public byte[] FormatSdcpMessage(bool isGetOperation, SonySetting value)
        {
            // Packet Structure
            // Header(2) | Community(4) | Command(4) | Data(n)

            var ms = new MemoryStream();

            // Header
            ms.WriteByte(0x02); // VERSION sub field.  This is a fixed value of 02h which means "version2"
            ms.WriteByte(0x0A); // category number of the projector which is 0x0A

            // Community
            // always filled with "SONY"
            ms.Write(Encoding.ASCII.GetBytes("SONY"), 0, 4);

            // Command (request)
            // Request(1) | Item No(2) | Data Length(1)

            // Request
            // SET (0x00)
            // GET (0x01)
            ms.WriteByte(isGetOperation ? (byte)0x01 : (byte)0x00);

            // Item Number
            // ms.Write(BitConverter.GetBytes(CommandKey), 0, sizeof(short));
            WriteShort(ms, CommandKey);

            if (isGetOperation)
            {
                // zero data length for a request operation
                ms.WriteByte(0);
            }
            else
            {
                // Data Length
                ms.WriteByte(sizeof(short));

                // Data
                // ms.Write(BitConverter.GetBytes(value.DeviceKey), 0, sizeof(short));
                WriteShort(ms, value?.DeviceKey ?? (short)0);
            }

            return ms.ToArray();
        }

        private static void WriteShort(Stream ms, short value)
        {
            ms.WriteByte((byte)((value >> 8) & 0x00FF));
            ms.WriteByte((byte)(value & 0x00FF));
        }

        private static short ReadShort(Stream ms)
        {
            short value = 0;

            value = (short)((ms.ReadByte() << 8) & 0xFF00);
            value |= (short)(ms.ReadByte() & 0x00FF);

            return value;
        }

        public bool HandleMessage(SonyProjectorDevice device, byte[] message)
        {
            var ms = new MemoryStream(message);

            var version = (byte)ms.ReadByte();
            var category = (byte)ms.ReadByte();

            var community = new byte[4];
            ms.Read(community, 0, 4);

            var itemNumber = ReadShort(ms);

            if (itemNumber != CommandKey)
            {
                return false;
            }

            var dataLength = (byte)ms.ReadByte();
            var dataValue = ReadShort(ms);

            OnHandleMessage(device, dataValue);
            return true;
        }

        protected abstract void OnHandleMessage(SonyProjectorDevice device, short value);
    }
}
