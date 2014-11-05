using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace WpfApplication1
{
    class CryptoManager
    {
        private SHA256 SHA256;
        private Object toEncode;

        public CryptoManager(Object obj)
        {
            SHA256 = SHA256Managed.Create();
            toEncode = obj;
        }

        public byte[] GetHashFromString()
        {
            byte[] hashValue;
            hashValue = SHA256.ComputeHash(Encoding.Unicode.GetBytes((string) toEncode));
            return hashValue;
        }

    }
}
