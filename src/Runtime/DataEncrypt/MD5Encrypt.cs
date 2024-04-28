using System;
using System.Security.Cryptography;
using System.Text;

namespace CGame
{
    public static class MD5Encrypt
    {
        public static string Encrypt(string data)
        {
            var md5 = new MD5CryptoServiceProvider();
            var bytes = Encoding.Default.GetBytes(data);
            var encryptData = md5.ComputeHash(bytes);
            return Convert.ToBase64String(encryptData);
        }
    }
}