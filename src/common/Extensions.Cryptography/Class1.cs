using Common.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extensions.Cryptography
{
    public static class HmacStringExtension
    {
        public static string Cryptofy(this string value, string privateKey, string salt = "")
        {
            Hmac512Utility cryptoClient = new Hmac512Utility();

            return cryptoClient.GetHash(value, privateKey);
        }
    }
}
