using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcApp1.BAL
{
    public class CommonHelper
    {
        //public class UiStatus
        //{
        //    public const int PreProgress = 1;
        //    public const int Progress = 2;
        //    public const int Confirm = 3;
        //    public const int Result = 4;
        //}

        /// Compute hash from string
        /// Return value = hash string of the input parameter 'sSrc'
        /// input = input string
        /// encoding = to define encoding method. Default method is UTF8. Can use other encoding methods including UTF8, UTF7, UTF32, ASCII, Unicode, BigEndianUnicode, Default (ANSI).
        /// algorithm = to define hash algorithm. Default method is SHA256. The hash algorithms includes HMACMD5, HMACRIPEMD160, HMACSHA1, HMACSHA256, HMACSHA384, HMACSHA512, MD5, SHA1, SHA256, SHA384, SHA512.
        /// https://stackoverflow.com/questions/3984138/hash-string-in-c-sharp
        /// https://support.microsoft.com/en-us/help/307020/how-to-compute-and-compare-hash-values-by-using-visual-c
        public static string ComputeHashFromString(string input) { return ComputeHashFromString(input, "UTF8"); }
        public static string ComputeHashFromString(string input, string encoding) { return ComputeHashFromString(input, encoding, "SHA256"); }
        public static string ComputeHashFromString(string input, string encoding, string algorithm)
        {
            byte[] myBytes1;
            byte[] myBytes2 = null;
            System.Text.StringBuilder sb = null;
            try
            {
                switch (encoding.Trim().ToUpper())
                {
                    case "UTF8":
                        myBytes1 = System.Text.Encoding.UTF8.GetBytes(input);
                        break;
                    case "ASCII":
                        myBytes1 = System.Text.Encoding.ASCII.GetBytes(input);
                        break;
                    case "Default":
                        myBytes1 = System.Text.Encoding.Default.GetBytes(input);
                        break;
                    case "UTF7":
                        myBytes1 = System.Text.Encoding.UTF7.GetBytes(input);
                        break;
                    case "UTF32":
                        myBytes1 = System.Text.Encoding.UTF32.GetBytes(input);
                        break;
                    case "UNICODE":
                        myBytes1 = System.Text.Encoding.Unicode.GetBytes(input);
                        break;
                    case "BIGENDIANUNICODE":
                        myBytes1 = System.Text.Encoding.BigEndianUnicode.GetBytes(input);
                        break;
                    default:
                        myBytes1 = System.Text.Encoding.UTF8.GetBytes(input);
                        break;
                }
                switch (algorithm.Trim().ToUpper())
                {
                    case "SHA256":
                        myBytes2 = System.Security.Cryptography.SHA256.Create().ComputeHash(myBytes1);
                        break;
                    case "HMACMD5":
                        myBytes2 = System.Security.Cryptography.HMACMD5.Create().ComputeHash(myBytes1);
                        break;
                    case "HMACRIPEMD160":
                        myBytes2 = System.Security.Cryptography.HMACRIPEMD160.Create().ComputeHash(myBytes1);
                        break;
                    case "HMACSHA1":
                        myBytes2 = System.Security.Cryptography.HMACSHA1.Create().ComputeHash(myBytes1);
                        break;
                    case "HMACSHA256":
                        myBytes2 = System.Security.Cryptography.HMACSHA256.Create().ComputeHash(myBytes1);
                        break;
                    case "HMACSHA384":
                        myBytes2 = System.Security.Cryptography.HMACSHA384.Create().ComputeHash(myBytes1);
                        break;
                    case "HMACSHA512":
                        myBytes2 = System.Security.Cryptography.HMACSHA512.Create().ComputeHash(myBytes1);
                        break;
                    case "MD5":
                        myBytes2 = System.Security.Cryptography.MD5.Create().ComputeHash(myBytes1);
                        break;
                    case "SHA1":
                        myBytes2 = System.Security.Cryptography.SHA1.Create().ComputeHash(myBytes1);
                        break;
                    case "SHA384":
                        myBytes2 = System.Security.Cryptography.SHA384.Create().ComputeHash(myBytes1);
                        break;
                    case "SHA512":
                        myBytes2 = System.Security.Cryptography.SHA512.Create().ComputeHash(myBytes1);
                        break;
                    default:
                        myBytes2 = System.Security.Cryptography.SHA256.Create().ComputeHash(myBytes1);
                        break;
                }
                sb = new System.Text.StringBuilder();
                if (myBytes2 != null && myBytes2.Length > 0)
                {
                    foreach (byte bb in myBytes2)
                    { sb.Append(bb.ToString("X2")); }
                }
                return sb.ToString();
            }
            catch { return null; }
            finally { myBytes1 = null; myBytes2 = null; }
        }

        public static string GetSessionValue(HttpRequestBase request, string key)
        {
            if (string.IsNullOrWhiteSpace(key) || request?.Params == null) return "";
            if (!request.Params.AllKeys.Contains(key)) return "";
            return request[key].ToString();
        }

        public static int[] ConvertStringToIntArray(string input, string separator)
        {
            if (string.IsNullOrWhiteSpace(input)) return null;
            string[] strArray = input.Split(new string[] { separator }, StringSplitOptions.RemoveEmptyEntries);
            List<int> intList = new List<int>();
            try
            {
                if (strArray != null)
                {
                    foreach (string s in strArray)
                    {
                        if (!string.IsNullOrWhiteSpace(s))
                        {
                            if (int.TryParse(s, out int i))
                            {
                                intList.Add(i);
                            }
                        }
                    }
                    strArray = null;
                }
                return intList.Count < 1 ? null : intList.ToArray();
            }
            finally
            {
                intList.Clear();
                intList = null;
            }
        }
    }
}