using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeControl.Web.Services
{
    public class StreamDeckKeyInfo
    {
        public StreamDeckKeyInfo(string key, string normalImageFileName, string keyPressedFileName)
        {
            Key = key;
            NormalImageFileName = normalImageFileName;
            KeyPressedImageFileName = keyPressedFileName;
        }

        public string Key { get; }
        public string NormalImageFileName { get; }
        public string KeyPressedImageFileName { get; }
    }
}
