using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TT
{
    public class GeneralHelper
    {
        public static string GetDefaultFolder(string defaultFolder)
        {
            if (string.IsNullOrEmpty(defaultFolder)) return System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly()?.Location) ?? string.Empty;
            else return defaultFolder.Trim().TrimEnd(new char[] { (char)9, ' ', System.IO.Path.DirectorySeparatorChar });
        }

        public static string GetDefaultAbsolutePathIfRelative(string path) { return GetDefaultAbsolutePathIfRelative(path, string.Empty); }
        public static string GetDefaultAbsolutePathIfRelative(string path, string defaultFolder)
        {
            if (string.IsNullOrEmpty(path)) return GetDefaultFolder(defaultFolder);
            path = path.Trim(new char[] { (char)9, ' ', System.IO.Path.DirectorySeparatorChar });
            if (System.IO.Path.IsPathRooted(path)) return path;
            else return System.IO.Path.Combine(GetDefaultFolder(defaultFolder), path);
        }

        public static void Sleeping(int sleepingIntervalInMS)
        {
            //if (sleepingIntervalInMS >= 0) System.Threading.Thread.Sleep(sleepingIntervalInMS);
            if (sleepingIntervalInMS >= 0) new System.Threading.ManualResetEvent(false).WaitOne(sleepingIntervalInMS);
        }

        public static bool ExitFileExists(bool isDeleteIfExist, params string[] extArray)
        {
            if (extArray == null || extArray.Length < 1) extArray = new string[] { ".exit", ".end", ".quit", ".stop", ".close", ".finish" };
            string pathNoExt = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly()?.Location) ?? string.Empty,
                System.IO.Path.GetFileNameWithoutExtension(System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName ?? string.Empty));
            bool bReturn = false;
            bool b = true;
            int i = 0;
            while (b && i < extArray.Length)
            {
                string? s = extArray[i]?.Trim().ToLower();
                if (!string.IsNullOrWhiteSpace(s))
                {
                    string path = pathNoExt + s;
                    if (System.IO.File.Exists(path))
                    {
                        bReturn = true;
                        if (isDeleteIfExist)
                        {
                            Sleeping(500);
                            System.IO.File.Delete(path);
                        }
                        else b = false;// stop looping if choose to not delete the EXIT file
                    }
                }
                i++;
            }
            return bReturn;
        }

        public static bool GetArguments(string[] args, string switchString) { return GetArguments(args, switchString, out _); }
        public static bool GetArguments(string[] args, string switchString, out string output)
        {
            output = string.Empty;
            if (args == null || args.Length < 1) return false;
            bool bReturn = false;
            if (string.IsNullOrEmpty(switchString) == false && switchString.Length > 0)
            {
                switchString = switchString.ToLower();
                int i = 0;
                while (bReturn == false && i < args.Length)
                {
                    string s = args[i];
                    if (s.Length >= switchString.Length)
                    {
                        if (s[..switchString.Length].ToLower().Equals(switchString))
                        {
                            bReturn = true;
                            if (s.Length > switchString.Length) output = s[switchString.Length..];
                        }
                    }
                    i++;
                }
            }
            return bReturn;
        }

        /// versionText should be in the format of X.X.X.X, where X is an non-negative integer. E.g. 12.0.6.83
        /// Return null if wrong format or cannot parse to integer.
        /// https://www.techiedelight.com/convert-string-array-to-int-array-csharp/
        private static int[] GetVersionNumberArray(string versionText)
        {
            if (string.IsNullOrWhiteSpace(versionText)) return Array.Empty<int>();
            try { return Array.ConvertAll(versionText.Split('.'), s => int.Parse(s)); }
            catch { return Array.Empty<int>(); }
        }

        /// Version should be in the format of X.X.X.X, where X is an non-negative integer. E.g. 12.0.6.83
        /// Return value = True if the current version is up-to-date. False if the current version is outdated. Null if error.
        public static bool? IsVersionUpdated(string currentVersion, string mostUpdatedVersion, out string errorMessage)
        {
            errorMessage = string.Empty;
            try
            {
                int[] currentVersionNumbers = GetVersionNumberArray(currentVersion);
                if (currentVersionNumbers == null || currentVersionNumbers.Length < 1)
                {
                    errorMessage = string.Format("Current version is in a wrong format. {0}", currentVersion);
                    return null;
                }
                int[] mostUpdatedVersionNumbers = GetVersionNumberArray(mostUpdatedVersion);
                if (mostUpdatedVersionNumbers == null || mostUpdatedVersionNumbers.Length < 1)
                {
                    errorMessage = string.Format("Most updated version is in a wrong format. {0}", mostUpdatedVersion);
                    return null;
                }
                int minLength = currentVersionNumbers.Length >= mostUpdatedVersionNumbers.Length ? currentVersionNumbers.Length : mostUpdatedVersionNumbers.Length;
                int i = 0;
                while (i < minLength && currentVersionNumbers[i] == mostUpdatedVersionNumbers[i])
                { i++; }
                if (i < minLength) return currentVersionNumbers[i] > mostUpdatedVersionNumbers[i];
                if (currentVersionNumbers.Length == mostUpdatedVersionNumbers.Length) return true;
                if (currentVersionNumbers.Length > mostUpdatedVersionNumbers.Length)
                {
                    while (i < currentVersionNumbers.Length && currentVersionNumbers[i] == 0)
                    { i++; }
                    return i == currentVersionNumbers.Length || currentVersionNumbers[i] > 0;
                }
                else
                {
                    while (i < mostUpdatedVersionNumbers.Length && mostUpdatedVersionNumbers[i] == 0)
                    { i++; }
                    return i == mostUpdatedVersionNumbers.Length || mostUpdatedVersionNumbers[i] <= 0;
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return null;
            }
        }

        /// System.Net.Http.HttpClient
        /// https://docs.microsoft.com/en-us/dotnet/csharp/tutorials/console-webapiclient
        /// https://www.c-sharpcorner.com/article/calling-web-api-using-httpclient/
        /// https://marcus116.blogspot.com/2018/02/c-web-api-httpclient.html
        /// https://blog.csdn.net/u010476739/article/details/119782562
        /// 
        ///// WebRequest, WebClient, HttpWebRequest and ServicePoint classes are obsolete, use the System.Net.Http.HttpClient class instead
        ///// https://docs.microsoft.com/en-us/dotnet/core/compatibility/networking/6.0/webrequest-deprecated
        //public static async Task<string> HttpPost(string url, string data, string contentType, System.Text.Encoding encoding, string proxyUrl, string proxyUsername, string proxyPassword)
        //{
        //    /// https://stackoverflow.com/questions/4015324/how-to-make-an-http-post-web-request
        //    /// https://www.codeproject.com/Questions/5280136/How-to-send-JSON-data-from-ASP-NET-webform-vs-2010
        //    if (string.IsNullOrWhiteSpace(url)) return string.Empty;
        //    /// Request
        //    if (!string.IsNullOrWhiteSpace(proxyUrl))
        //    {
        //    }
        //    using HttpClient client = new()
        //    {
        //        BaseAddress = new Uri(proxyUrl)
        //    };
        //    var response = await client.PostAsync(url, new StringContent(data, encoding, contentType));
        //    return await response.Content.ReadAsStringAsync();
        //}

        public static async System.Threading.Tasks.Task<string> HttpPostJson(string url, string jsonText, System.Text.Encoding encoding, IEnumerable<KeyValuePair<string, string>> headers, string proxyUrl, string proxyUsername, string proxyPassword)
        {
            /// https://zetcode.com/csharp/httpclient/
            /// https://stackoverflow.com/questions/9145667/how-to-post-json-to-a-server-using-c
            if (string.IsNullOrWhiteSpace(url)) return string.Empty;
            var requestContent = new System.Net.Http.StringContent(jsonText, encoding, "application/json");
            if (headers != null)
            {
                foreach (var h in headers)
                {
                    requestContent.Headers.Add(h.Key, h.Value);
                }
            }
            using var client = string.IsNullOrWhiteSpace(proxyUrl) ?
                new System.Net.Http.HttpClient() :
                new System.Net.Http.HttpClient(new System.Net.Http.HttpClientHandler()
                {
                    Proxy = new System.Net.WebProxy(proxyUrl, false, null, new System.Net.NetworkCredential(proxyUsername, proxyPassword)),
                    UseProxy = true
                });
            System.Net.Http.HttpResponseMessage response = await client.PostAsync(url, requestContent);
            using var reader = new StreamReader(await response.Content.ReadAsStreamAsync());
            return await reader.ReadToEndAsync();
        }
    }
}
