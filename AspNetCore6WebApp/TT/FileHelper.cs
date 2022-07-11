using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TT
{
    public class FileHelper
    {
        /// Updated date: 2022-06-17

        private static void Sleeping(int sleepingIntervalInMS)
        {
            //if (sleepingIntervalInMS >= 0) System.Threading.Thread.Sleep(sleepingIntervalInMS);
            if (sleepingIntervalInMS >= 0) new System.Threading.ManualResetEvent(false).WaitOne(sleepingIntervalInMS);
        }

        /// Delete file
        /// Return value = True if delete file successfully. Otherwise, false
        public static bool DeleteLocalFile(string filepath)
        {
            System.IO.File.Delete(filepath);
            System.Threading.Thread.Sleep(1000);
            if (!System.IO.File.Exists(filepath)) return true;
            int i = 0;
            while (i < 5 && System.IO.File.Exists(filepath))
            {
                Sleeping(500);
                i++;
            }
            return !System.IO.File.Exists(filepath);
        }

        /// Check if folder exists. If not create it
        /// Return value = True if folder exists. Otherwise, try to create folder. Return true if create folder successfully. Otherwise, false
        public static bool FolderExistsOrCreateIt(string folder)
        {
            if (System.IO.Directory.Exists(folder)) return true;
            System.IO.Directory.CreateDirectory(folder);
            return System.IO.Directory.Exists(folder);
        }

        /// Get absolte path if input path is a relative path
        /// Return value = output
        /// path = input path
        public static string GetAbsolutePathIfRelative(string path)
        {
            return System.IO.Path.IsPathRooted(path) ? path : System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly()?.Location) ?? string.Empty, path);
        }

        public static bool CopySingleFile(string source, string destination)
        {
            if (System.IO.File.Exists(destination)) System.IO.File.Delete(destination);
            else
            {
                string folder = System.IO.Path.GetDirectoryName(destination) ?? string.Empty;
                if (!System.IO.Directory.Exists(folder)) System.IO.Directory.CreateDirectory(folder);
            }
            System.IO.File.Copy(source, destination);
            return true;
        }

        public class CSV
        {
            /// Updated date: 2020-09-21
            /// https://en.wikipedia.org/wiki/Comma-separated_values
            /// https://dotnetcoretutorials.com/2018/08/04/csv-parsing-in-net-core/
            /// The field must be quoted if any below conditions meet.
            /// 1. The field contains commas (delimiter).
            /// 2. The field contains double quotes (text qualifier).
            /// 3. The field contains new line.

            public static char Delimiter { get; set; } = ',';
            public static char TextQualifier { get; set; } = '"';
            private static string DoubleTextQualifier { get { return string.Format("{0}{0}", TextQualifier); } }

            #region ReadRegion
            /// Return value = is the result field finished. True if finished, otherwise false.
            /// input = input field
            /// continuedField = the field content of the last field. If it is empty or null, it means that it is not continued field. If it is a continued field, this parameter must not be empty.
            /// output = output
            private static bool GetFieldForRead(string input, string continuedField, out string output)
            {
                output = continuedField ?? string.Empty;
                if (string.IsNullOrEmpty(input))
                {
                    /// If it is not a continued field, return empty string and return true.
                    /// If it is a continued field, return false.
                    return string.IsNullOrEmpty(continuedField);
                }
                else
                {
                    if (string.IsNullOrEmpty(continuedField))
                    {
                        /// Check the leading quote.
                        string s1 = input.TrimStart(' ', (char)9);
                        if (s1.StartsWith(TextQualifier.ToString()))
                        {
                            /// Remove the leading quote.
                            string s2 = s1[1..];
                            string s3 = s2.TrimEnd(' ', (char)9);
                            /// Replace double quotes before checking the trailing quote.
                            if (s3.Replace(DoubleTextQualifier, "").EndsWith(TextQualifier.ToString()))
                            {
                                output = s3[0..^1].Replace(DoubleTextQualifier, TextQualifier.ToString());
                                return true;
                            }
                            else
                            {
                                /// The field is not finished. Not trim the trailing spaces.
                                output = s2.Replace(DoubleTextQualifier, TextQualifier.ToString());
                                return false;
                            }
                        }
                        else
                        {
                            if (input.Contains(TextQualifier))
                            {
                                throw new Exception(string.Format("Cannot contain text qualifier (quote) in the field without leading quote and trailing quote. Field = {0}", input));
                            }
                            /// Field without quotes.
                            output = input;
                            return true;
                        }
                    }
                    else
                    {
                        /// Replace double quotes before checking the trailing quote.
                        string s4 = input.TrimEnd(' ', (char)9);
                        if (s4.Replace(DoubleTextQualifier, "").EndsWith(TextQualifier.ToString()))
                        {
                            output += s4[0..^1].Replace(DoubleTextQualifier, TextQualifier.ToString());
                            return true;
                        }
                        else
                        {
                            /// The field is not finished. Not trim the trailing spaces.
                            output += input.Replace(DoubleTextQualifier, TextQualifier.ToString());
                            return false;
                        }
                    }
                }
            }

            /// Read file with imported action.
            /// Return value = Number of records
            /// https://www.tutorialsteacher.com/csharp/constraints-in-generic-csharp
            public static int ReadFileWithImportedAction(string path, Action<string[]> handlingMethod, out string errorMessage)
            {
                errorMessage = string.Empty;
                List<string>? list = null;
                try
                {
                    int recordCount = 0;
                    using (System.IO.FileStream fs = System.IO.File.Open(path, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite))
                    {
                        using System.IO.BufferedStream bs = new(fs);
                        using System.IO.StreamReader sr = new(bs);
                        int lineIndex = 0;
                        string continuedField = string.Empty;
                        /// Loop each line.
                        string? line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            try
                            {
                                string[] array = line.Split(Delimiter);
                                /// Loop each field.
                                int lastArrayIndex = array.Length - 1;
                                int i = 0;
                                while (i < array.Length)
                                {
                                    if (GetFieldForRead(array[i], continuedField, out string output))
                                    {
                                        if (list == null) list = new List<string>();
                                        list.Add(output);
                                        continuedField = string.Empty;
                                        /// Handle the last field.
                                        if (i == lastArrayIndex)
                                        {
                                            recordCount++;
                                            handlingMethod(list?.ToArray() ?? Array.Empty<string>());
                                            /// Clear the list for next record.
                                            list?.Clear();
                                            list = null;
                                        }
                                    }
                                    else
                                    {
                                        /// Handle the last field.
                                        if (i == lastArrayIndex)
                                            continuedField = output + Environment.NewLine;
                                        else
                                            continuedField = string.Format("{0}{1}", output, Delimiter);
                                    }
                                    i++;
                                }
                            }
                            catch (Exception ex2)
                            {
                                errorMessage = string.Format("Error occurs in line index {0}: {1}{2}{3}", lineIndex, line, Environment.NewLine, ex2);
                            }
                            lineIndex++;
                        }
                    }
                    return recordCount;
                }
                catch (Exception ex)
                {
                    errorMessage = string.Format("Path = {0}{1}{2}", path, Environment.NewLine, ex);
                    return -1;
                }
                finally
                {
                    list?.Clear();
                }
            }

            /// Read file and get an array of string arraies.
            /// Return value = Number of records
            public static string[][]? ReadFileAndGetArrayOfStringArray(string path, out string errorMessage)
            {
                errorMessage = string.Empty;
                List<string[]> list = new();
                try
                {
                    int i = ReadFileWithImportedAction(path, array =>
                    {
                        list.Add(array);
                    }, out errorMessage);
                    return list.ToArray();
                }
                catch (Exception ex)
                {
                    if (!string.IsNullOrEmpty(errorMessage)) errorMessage += Environment.NewLine;
                    errorMessage += string.Format("Path = {0}{1}{2}", path, Environment.NewLine, ex);
                    return null;// Array.Empty<string[]>();
                }
            }
            #endregion

            #region WriteRegion
            private static string GetQuotedFieldForWrite(string input)
            {
                return string.Format("{0}{1}{0}", TextQualifier, input?.Replace(TextQualifier.ToString(), DoubleTextQualifier));
            }

            /// Check field before writing to CSV.
            private static string CheckAndGetFieldForWrite(string input)
            {
                if (string.IsNullOrWhiteSpace(input)) return input;
                if (input.Contains(Delimiter)
                    || input.Contains(TextQualifier)
                    || input.Contains(Environment.NewLine))
                    return GetQuotedFieldForWrite(input);
                else return input;
            }

            /// Write a string array to a string.
            /// Return Value = output string. It is null if errors occur.
            public static string WriteToString(string[] array, bool mustQuote, out string errorMessage)
            {
                errorMessage = string.Empty;
                //System.Text.StringBuilder sb = new System.Text.StringBuilder();
                try
                {
                    //int arrayLength = array?.Length ?? 0;
                    //if (arrayLength > 0)
                    //{
                    //    if (mustQuote)
                    //    {
                    //        /// The case if arrayLength >= 1.
                    //        sb.Append(GetQuotedFieldForWrite(array[0]));
                    //        if (arrayLength > 1)
                    //        {
                    //            /// Length must be larger than 1.
                    //            int i = 1;
                    //            while (i < arrayLength)
                    //            {
                    //                sb.Append(Delimiter).Append(GetQuotedFieldForWrite(array[i]));
                    //                i++;
                    //            }
                    //        }
                    //    }
                    //    else
                    //    {
                    //        /// The case if arrayLength >= 1.
                    //        sb.Append(CheckAndGetFieldForWrite(array[0]));
                    //        if (arrayLength > 1)
                    //        {
                    //            /// Length must be larger than 1.
                    //            int i = 1;
                    //            while (i < arrayLength)
                    //            {
                    //                sb.Append(Delimiter).Append(CheckAndGetFieldForWrite(array[i]));
                    //                i++;
                    //            }
                    //        }
                    //    }
                    //}
                    //sb.AppendLine();
                    //return sb.ToString();
                    if (array == null) return string.Empty;
                    string[] array2 = mustQuote ? array.Select(x => GetQuotedFieldForWrite(x)).ToArray() : array.Select(x => CheckAndGetFieldForWrite(x)).ToArray();
                    return string.Join(Delimiter.ToString(), array2) + Environment.NewLine;
                }
                catch (Exception ex)
                {
                    errorMessage = string.Format("Array = {0}{1}{2}", array, Environment.NewLine, ex);
                    return string.Empty;
                }
                //finally { sb = null; }
            }

            /// Write a string array to stream writer.
            /// Return Value = True if success, otherwise false.
            public static bool WriteToStreamWriter(System.IO.StreamWriter sw, string[] array, bool mustQuote, out string errorMessage)
            {
                try
                {
                    sw.Write(WriteToString(array, mustQuote, out errorMessage));
                    return true;
                }
                catch (Exception ex)
                {
                    errorMessage = string.Format("Array = {0}{1}{2}", array, Environment.NewLine, ex);
                    return false;
                }
            }

            /// Write a string array to CSV file.
            /// Return Value = True if success, otherwise false.
            /// isApend = True if append, otherwise false.
            public static bool WriteToFile(string path, string[] array, bool mustQuote, out string errorMessage)
            {
                try
                {
                    using System.IO.StreamWriter sw = new(path, true);
                    return WriteToStreamWriter(sw, array, mustQuote, out errorMessage);
                }
                catch (Exception ex)
                {
                    errorMessage = string.Format("Path = {1}{0}Array = {2}{0}{3}", Environment.NewLine, path, array, ex);
                    return false;
                }
            }
            #endregion
        }

        public class SFTP
        {
            public class Proxy
            {
                public string? Method { get; set; }
                public string? Host { get; set; }
                public int Port { get; set; }
                public string? Username { get; set; }
                public string? Password { get; set; }
            }

            public class WinscpMethod
            {
                /// Need install "WinSCP" in NuGet
                /// https://winscp.net/eng/docs/library

                public class ProxyMethod
                {
                    /// https://winscp.net/eng/docs/rawsettings
                    public const string None = "0";
                    public const string SOCKS4 = "1";
                    public const string SOCKS5 = "2";
                    public const string HTTP = "3";
                    public const string Telnet = "4";
                    public const string Local = "5";
                }

                ///// Get session options with proxy setting. If no proxy setting, set proxyMethod = None.
                //public static WinSCP.SessionOptions GetSessionOptions(string host, int port, string username, string password, string sshHostKeyFingerprint, Proxy proxy)
                //{
                //    WinSCP.SessionOptions sessionOptions = new()
                //    {
                //        Protocol = WinSCP.Protocol.Sftp,
                //        HostName = host,
                //        PortNumber = port,
                //        UserName = username,
                //        Password = password,
                //        SshHostKeyFingerprint = sshHostKeyFingerprint
                //    };
                //    if (proxy != null && string.IsNullOrEmpty(proxy.Method) == false && ProxyMethod.None.Equals(proxy.Method) == false)
                //    {
                //        sessionOptions.AddRawSettings("ProxyMethod", proxy.Method);
                //        sessionOptions.AddRawSettings("ProxyHost", proxy.Host ?? string.Empty);
                //        sessionOptions.AddRawSettings("ProxyPort", proxy.Port.ToString());
                //        if (!string.IsNullOrEmpty(proxy.Username)) sessionOptions.AddRawSettings("ProxyUsername", proxy.Username);
                //        if (!string.IsNullOrEmpty(proxy.Password)) sessionOptions.AddRawSettings("ProxyPassword", proxy.Password);
                //    }
                //    return sessionOptions;
                //}

                //public static WinSCP.SessionOptions GetSessionOptions(string host, int port, string username, string password, string sshHostKeyFingerprint, string proxyMethod, string proxyHost, int proxyPort, string proxyUsername, string proxyPassword)
                //{
                //    WinSCP.SessionOptions sessionOptions = new()
                //    {
                //        Protocol = WinSCP.Protocol.Sftp,
                //        HostName = host,
                //        PortNumber = port,
                //        UserName = username,
                //        Password = password,
                //        SshHostKeyFingerprint = sshHostKeyFingerprint
                //    };
                //    if (string.IsNullOrEmpty(proxyMethod) == false && ProxyMethod.None.Equals(proxyMethod) == false)
                //    {
                //        sessionOptions.AddRawSettings("ProxyMethod", proxyMethod);
                //        sessionOptions.AddRawSettings("ProxyHost", proxyHost);
                //        sessionOptions.AddRawSettings("ProxyPort", proxyPort.ToString());
                //        if (!string.IsNullOrEmpty(proxyUsername)) sessionOptions.AddRawSettings("ProxyUsername", proxyUsername);
                //        if (!string.IsNullOrEmpty(proxyPassword)) sessionOptions.AddRawSettings("ProxyPassword", proxyPassword);
                //    }
                //    return sessionOptions;
                //}

                ///// Return value = True if connect successfully. Otherwise, false.
                ///// https://stackoverflow.com/questions/46317508/checking-connection-state-with-winscp-net-assembly-in-c-sharp
                //public static bool CheckConnection(WinSCP.SessionOptions sessionOptions)
                //{
                //    if (sessionOptions == null) return false;
                //    using WinSCP.Session session = new();
                //    int attempts = 3;
                //    do
                //    {
                //        try
                //        {
                //            session.Open(sessionOptions);
                //            return true;
                //        }
                //        catch
                //        {
                //            //if (attempts == 0) Logger?.Error(ex);
                //        }
                //        attempts--;
                //    } while (attempts > 0 && !session.Opened);
                //    return session.Opened;
                //}

                //public static bool CheckConnection(WinSCP.SessionOptions[] sessionOptionsArray)
                //{
                //    if (sessionOptionsArray == null || sessionOptionsArray.Length < 1) return false;
                //    bool b = false;
                //    int i = 0;
                //    while (b == false && i < sessionOptionsArray.Length)
                //    {
                //        WinSCP.SessionOptions o = sessionOptionsArray[i];
                //        if (o != null)
                //        {
                //            b = CheckConnection(o);
                //        }
                //        i++;
                //    }
                //    return b;
                //}

                //private static void DownloadFileCore(WinSCP.SessionOptions sessionOptions, string remotePath, string localPath)
                //{
                //    using WinSCP.Session session = new();
                //    session.Open(sessionOptions);
                //    WinSCP.TransferOptions transferOptions = new()
                //    {
                //        TransferMode = WinSCP.TransferMode.Binary
                //    };
                //    WinSCP.TransferOperationResult result = session.GetFiles(remotePath, localPath, false, transferOptions);
                //    result.Check();
                //}

                ///// Need install "WinSCP" in NuGet.
                ///// https://winscp.net/eng/docs/library
                ///// https://winscp.net/eng/docs/library_session_getfiles
                //public static bool DownloadFile(WinSCP.SessionOptions sessionOptions, string remotePath, string localPath)
                //{
                //    if (sessionOptions == null) return false;
                //    /// https://weblog.west-wind.com/posts/2019/Aug/20/UriAbsoluteUri-and-UrlEncoding-of-Local-File-Urls
                //    if (System.IO.File.Exists(localPath))
                //    {
                //        if (!DeleteLocalFile(localPath)) return false;
                //    }
                //    else
                //    {
                //        string folder = System.IO.Path.GetDirectoryName(localPath) ?? string.Empty;
                //        if (!FolderExistsOrCreateIt(folder)) return false;
                //    }
                //    DownloadFileCore(sessionOptions, remotePath, localPath);
                //    return true;
                //}

                //public static bool DownloadFile(WinSCP.SessionOptions[] sessionOptionsArray, string remotePath, string localPath)
                //{
                //    if (sessionOptionsArray == null || sessionOptionsArray.Length < 1) return false;
                //    /// https://weblog.west-wind.com/posts/2019/Aug/20/UriAbsoluteUri-and-UrlEncoding-of-Local-File-Urls
                //    if (System.IO.File.Exists(localPath))
                //    {
                //        if (!DeleteLocalFile(localPath)) return false;
                //    }
                //    else
                //    {
                //        string folder = System.IO.Path.GetDirectoryName(localPath) ?? string.Empty;
                //        if (!FolderExistsOrCreateIt(folder)) return false;
                //    }
                //    bool b = true;
                //    int i = 0;
                //    while (b && i < sessionOptionsArray.Length)
                //    {
                //        try
                //        {
                //            DownloadFileCore(sessionOptionsArray[i], remotePath, localPath);
                //            b = false;// stop looping
                //        }
                //        catch
                //        {
                //        }
                //        i++;
                //    }
                //    return !b;
                //}

                //public static string[]? ListDirectory(WinSCP.SessionOptions sessionOptions, string remotePath)
                //{
                //    if (sessionOptions == null) return null;
                //    using WinSCP.Session session = new();
                //    session.Open(sessionOptions);
                //    WinSCP.RemoteDirectoryInfo result = session.ListDirectory(remotePath);
                //    return result?.Files?.Where(f => ".".Equals(f.Name) == false && "..".Equals(f.Name) == false)?.Select(f => f.FullName)?.OrderBy(s => s).ToArray();
                //}

                //private static void UploadFilesCore(WinSCP.SessionOptions sessionOptions, string remoteFolder, params string[] localPaths)
                //{
                //    using WinSCP.Session session = new();
                //    session.Open(sessionOptions);
                //    WinSCP.TransferOptions transferOptions = new()
                //    {
                //        TransferMode = WinSCP.TransferMode.Binary
                //    };
                //    foreach (var p in localPaths)
                //    {
                //        WinSCP.TransferOperationResult transferResult = session.PutFiles(p, remoteFolder, false, transferOptions);
                //        transferResult.Check();
                //    }
                //}

                //public static bool UploadFiles(WinSCP.SessionOptions sessionOptions, string remoteFolder, params string[] localPaths)
                //{
                //    if (sessionOptions == null) return false;
                //    if (localPaths == null || localPaths.Length < 1) return true;
                //    UploadFilesCore(sessionOptions, remoteFolder, localPaths);
                //    return true;
                //}

                //public static bool UploadFiles(WinSCP.SessionOptions[] sessionOptionsArray, string remoteFolder, params string[] localPaths)
                //{
                //    if (sessionOptionsArray == null || sessionOptionsArray.Length < 1) return false;
                //    if (localPaths == null || localPaths.Length < 1) return true;
                //    bool b = true;
                //    int i = 0;
                //    while (b && i < sessionOptionsArray.Length)
                //    {
                //        try
                //        {
                //            UploadFilesCore(sessionOptionsArray[i], remoteFolder, localPaths);
                //            b = false;// stop looping
                //        }
                //        catch
                //        {
                //        }
                //        i++;
                //    }
                //    return !b;
                //}
            }
        }

        public class Zip
        {
            public class SevenZip
            {
                /// http://pramaire.pixnet.net/blog/post/13525145-vb.net-%E4%BD%BF%E7%94%A87-zip%E5%A3%93%E7%B8%AE%E6%8C%87%E5%AE%9A%E7%9B%AE%E9%8C%84%E6%89%80%E6%9C%89%E6%AA%94%E6%A1%88
                /// https://msdn.microsoft.com/en-us/library/system.diagnostics.processstartinfo.redirectstandardoutput(v=vs.110).aspx
                /// https://dotblogs.com.tw/yc421206/archive/2012/04/30/71911.aspx
                /// https://msdn.microsoft.com/en-us/library/ty0d8k56(v=vs.110).aspx
                
                public static string? ExePath { get; set; }

                public static bool Action(string sevenZipPath, string args, out string output, out string error) { return Action(sevenZipPath, args, out output, out error, 7000); }
                public static bool Action(string sevenZipPath, string args, out string output, out string error, int waitingIntervalInMS)
                {
                    output = string.Empty; error = string.Empty;
                    if (!System.IO.File.Exists(sevenZipPath)) return false;
                    bool bReturn = false;
                    using (System.Diagnostics.Process p = new()
                    {
                        StartInfo = new System.Diagnostics.ProcessStartInfo()
                        {
                            FileName = sevenZipPath,
                            Arguments = args,
                            UseShellExecute = false,
                            RedirectStandardError = true,
                            RedirectStandardOutput = true,
                            WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                            CreateNoWindow = true
                        }
                    })
                    {
                        p.Start();
                        output = p.StandardOutput.ReadToEnd();
                        error = p.StandardError.ReadToEnd();
                        bReturn = p.WaitForExit(waitingIntervalInMS);
                        p.Close();
                    }
                    return bReturn;
                }

                private static string GetPossibleExePath(string? exePath)
                {
                    if (!string.IsNullOrWhiteSpace(exePath)) return exePath;
                    exePath = @"C:\Program Files\7-Zip\7z.exe";
                    if (System.IO.File.Exists(exePath)) return exePath;
                    exePath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "7z.exe");
                    if (System.IO.File.Exists(exePath)) return exePath;
                    exePath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "7za.exe");
                    if (System.IO.File.Exists(exePath)) return exePath;
                    return string.Empty;
                }

                /// Return value = operation result. True if successful. Otherwise, fail
                /// zipFilepath = Path of zip file
                /// password = Password
                /// output = Standard output of the process
                /// error = Standard error of the process
                public static bool Zip(string zipFilepath, string password, out string output, out string error, params string[] sourcePaths)
                {
                    output = string.Empty; error = string.Empty;
                    string exePath = GetPossibleExePath(ExePath);
                    /// Delete file if exists
                    if (System.IO.File.Exists(zipFilepath))
                    {
                        if (!DeleteLocalFile(zipFilepath)) return false;
                    }
                    else
                    {
                        /// Check folder exists
                        string folderpath = System.IO.Path.GetDirectoryName(zipFilepath) ?? string.Empty;
                        if (!FolderExistsOrCreateIt(folderpath)) return false;
                    }
                    string args = string.Format("a \"{0}\" {1}{2}", zipFilepath.Trim(' ', '"'), string.Join(" ", sourcePaths.Select(x => '"' + x.Trim(' ', '"') + '"')), string.IsNullOrEmpty(password) ? "" : string.Format(" -p{0}", password));
                    return Action(exePath, args, out output, out error);
                }

                /// Return value = operation result. True if successful. Otherwise, fail
                /// zipFilepath = Path of zip file
                /// password = Password
                /// targetFolderPath = Path of target folder
                /// output = Standard output of the process
                /// error = Standard error of the process
                public static bool Unzip(string zipFilepath, string password, string targetFolderPath, out string output, out string error)
                {
                    output = string.Empty; error = string.Empty;
                    string exePath = GetPossibleExePath(ExePath);
                    if (!System.IO.File.Exists(zipFilepath)) return false;
                    if (!FolderExistsOrCreateIt(targetFolderPath)) return false;
                    string args = string.Format("x \"{0}\" -o\"{1}\"{2}", zipFilepath.Trim(' ', '"'), targetFolderPath.Trim(' ', '"'), string.IsNullOrEmpty(password) ? "" : string.Format(" -p{0}", password));
                    return Action(exePath, args, out output, out error);
                }
            }
        }
    }
}
