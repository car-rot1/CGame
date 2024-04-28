using System;
using System.IO;
using System.Security.Cryptography;

namespace CGame
{
    public static class DESEncrypt
    {
        public static string Encrypt(string data, string key, string iv)
        {
            var byKey = System.Text.Encoding.ASCII.GetBytes(key);
            var byIV = System.Text.Encoding.ASCII.GetBytes(iv);

            var cryptoProvider = new DESCryptoServiceProvider();
            var ms = new MemoryStream();
            var cst = new CryptoStream(ms, cryptoProvider.CreateEncryptor(byKey, byIV), CryptoStreamMode.Write);

            var sw = new StreamWriter(cst);
            sw.Write(data);
            sw.Flush();
            cst.FlushFinalBlock();
            sw.Flush();
            return Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length);
        }
        
        public static string Decrypt(string data, string key, string iv)
        {
            var byKey = System.Text.Encoding.ASCII.GetBytes(key);
            var byIV = System.Text.Encoding.ASCII.GetBytes(iv);

            byte[] byEnc;
            try
            {
                byEnc = Convert.FromBase64String(data);
            }
            catch
            {
                return null;
            }

            var cryptoProvider = new DESCryptoServiceProvider();
            var ms = new MemoryStream(byEnc);
            var cst = new CryptoStream(ms, cryptoProvider.CreateDecryptor(byKey, byIV), CryptoStreamMode.Read);
            var sr = new StreamReader(cst);
            return sr.ReadToEnd();
        }
    }
}