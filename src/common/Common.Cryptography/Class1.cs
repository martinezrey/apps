using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Extensions.ByteExtensions;

namespace Common.Cryptography
{
    public class Hmac512Utility
    {
        public string GetHash(string stringToHash, string privateKey)
        {
            var keyByte = Encoding.UTF8.GetBytes(privateKey);

            using (var hmacsha512 = new HMACSHA512(keyByte))
            {
                hmacsha512.ComputeHash(Encoding.UTF8.GetBytes(stringToHash));

                return hmacsha512.Hash.ByteToString();
            }
        }

        public string GetRandomKey(int size)
        {
            byte[] secretkey = new Byte[size];

            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(secretkey);
            }

            return secretkey.ByteToString();
        }
    }
}
