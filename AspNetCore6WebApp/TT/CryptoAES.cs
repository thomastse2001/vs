using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TT
{
    public class CryptoAES
    {
        /// https://blog.johnwu.cc/article/net-core-aes-cryptography.html

        private static System.Security.Cryptography.Aes GetCryptography(byte[] key, byte[] iv)
        {
            var cr = System.Security.Cryptography.Aes.Create();
            cr.Mode = System.Security.Cryptography.CipherMode.CBC;
            cr.Padding = System.Security.Cryptography.PaddingMode.PKCS7;
            cr.Key = key;
            cr.IV = iv;
            return cr;
        }

        private static System.Security.Cryptography.Rfc2898DeriveBytes GetDeriveBytes(string password)
        {
            return new System.Security.Cryptography.Rfc2898DeriveBytes(password, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
        }

        public static byte[] Encrypt(byte[] data, byte[] key, byte[] iv)
        {
            var cr = GetCryptography(key, iv);
            var transform = cr.CreateEncryptor();
            return transform.TransformFinalBlock(data, 0, data.Length);
        }

        public static byte[] Encrypt(byte[] data, string password)
        {
            var pdb = GetDeriveBytes(password);
            return Encrypt(data, pdb.GetBytes(32), pdb.GetBytes(16));
        }

        public static string Encrypt(string text, string password)
        {
            return Convert.ToBase64String(Encrypt(System.Text.Encoding.Unicode.GetBytes(text), password));
        }

        public static byte[] Decrypt(byte[] cipherData, byte[] key, byte[] iv)
        {
            var cr = GetCryptography(key, iv);
            var transform = cr.CreateDecryptor();
            return transform.TransformFinalBlock(cipherData, 0, cipherData.Length);
        }

        public static byte[] Decrypt(byte[] cipherData, string password)
        {
            var pdb = GetDeriveBytes(password);
            return Decrypt(cipherData, pdb.GetBytes(32), pdb.GetBytes(16));
        }

        public static string Decrypt(string text, string password)
        {
            return System.Text.Encoding.Unicode.GetString(Decrypt(Convert.FromBase64String(text), password));
        }
    }
}
