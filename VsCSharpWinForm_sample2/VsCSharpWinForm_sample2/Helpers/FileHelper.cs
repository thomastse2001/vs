using System;
using System.Collections.Generic;
using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

namespace VsCSharpWinForm_sample2.Helpers
{
    public partial class FileHelper
    {
        /// Updated date: 2021-05-24
        public static TLog Logger { get; set; }

        #region CsvRegion
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
                            string s2 = s1.Substring(1);
                            string s3 = s2.TrimEnd(' ', (char)9);
                            /// Replace double quotes before checking the trailing quote.
                            if (s3.Replace(DoubleTextQualifier, "").EndsWith(TextQualifier.ToString()))
                            {
                                output = s3.Substring(0, s3.Length - 1).Replace(DoubleTextQualifier, TextQualifier.ToString());
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
                            output += s4.Substring(0, s4.Length - 1).Replace(DoubleTextQualifier, TextQualifier.ToString());
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
            public static int ReadFileWithImportedAction(string path, Action<string[]> handlingMethod)
            {
                List<string> list = null;
                try
                {
                    int recordCount = 0;
                    using (System.IO.FileStream fs = System.IO.File.Open(path, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite))
                    {
                        using (System.IO.BufferedStream bs = new System.IO.BufferedStream(fs))
                        {
                            using (System.IO.StreamReader sr = new System.IO.StreamReader(bs))
                            {
                                int lineIndex = 0;
                                string continuedField = string.Empty;
                                /// Loop each line.
                                string line;
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
                                                    handlingMethod(list?.ToArray());
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
                                        Logger?.Error("Error occurs in line index {0}: {1}", lineIndex, line);
                                        Logger?.Error(ex2);
                                    }
                                    lineIndex++;
                                }
                            }
                        }
                    }
                    return recordCount;
                }
                catch (Exception ex)
                {
                    Logger?.Error("Path = {0}", path);
                    Logger?.Error(ex);
                    return -1;
                }
                finally
                {
                    list?.Clear();
                    list = null;
                }
            }

            /// Read file and get an array of string arraies.
            /// Return value = Number of records
            public static string[][] ReadFileAndGetArrayOfStringArray(string path)
            {
                List<string[]> list = new List<string[]>();
                try
                {
                    int i = ReadFileWithImportedAction(path, array =>
                    {
                        list.Add(array);
                    });
                    return list.ToArray();
                }
                catch (Exception ex)
                {
                    Logger?.Error("Path = {0}", path);
                    Logger?.Error(ex);
                    return null;
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
            public static string WriteToString(string[] array, bool mustQuote)
            {
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
                    if (array == null) return null;
                    string[] array2 = mustQuote ? array.Select(x => GetQuotedFieldForWrite(x)).ToArray() : array.Select(x => CheckAndGetFieldForWrite(x)).ToArray();
                    return string.Join(Delimiter.ToString(), array2) + Environment.NewLine;
                }
                catch (Exception ex)
                {
                    Logger?.Error("Array = {0}", array);
                    Logger?.Error(ex);
                    return null;
                }
                //finally { sb = null; }
            }

            /// Write a string array to stream writer.
            /// Return Value = True if success, otherwise false.
            public static bool WriteToStreamWriter(System.IO.StreamWriter sw, string[] array, bool mustQuote)
            {
                try
                {
                    sw.Write(WriteToString(array, mustQuote));
                    return true;
                }
                catch (Exception ex)
                {
                    Logger?.Error("Array = {0}", array);
                    Logger?.Error(ex);
                    return false;
                }
            }

            /// Write a string array to CSV file.
            /// Return Value = True if success, otherwise false.
            /// isApend = True if append, otherwise false.
            public static bool WriteToFile(string path, string[] array, bool mustQuote)
            {
                try
                {
                    using (System.IO.StreamWriter sw = new System.IO.StreamWriter(path, true))
                    {
                        return WriteToStreamWriter(sw, array, mustQuote);
                    }
                }
                catch (Exception ex)
                {
                    Logger?.Error("Path = {0}", path);
                    Logger?.Error("Array = {0}", array);
                    Logger?.Error(ex);
                    return false;
                }
            }
            #endregion
        }
        #endregion

        #region FtpRegion
        public class FTP
        {
            /// Updated date: 2020-09-10
            
            /// Return value = true if success. Otherwise, false.
            public static bool DeleteFile(string uriPath, string username, string password)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(uriPath))
                    {
                        Logger?.Error("Empty URI path.");
                        return false;
                    }
                    System.Net.FtpWebRequest request = (System.Net.FtpWebRequest)System.Net.WebRequest.Create(uriPath);
                    request.Method = System.Net.WebRequestMethods.Ftp.DeleteFile;
                    request.Credentials = new System.Net.NetworkCredential(username, password);
                    Logger?.Debug("Delete FTP file {0}", uriPath);
                    using (System.Net.FtpWebResponse response = (System.Net.FtpWebResponse)request.GetResponse())
                    {
                        string s = response.StatusDescription;
                        Logger?.Debug(s);
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Logger?.Error("URI = {0}", uriPath);
                    Logger?.Error(ex);
                    return false;
                }
            }

            /// Return value = true if success. Otherwise, false.
            public static bool DownloadFile(string uriPath, string username, string password, string filepath)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(uriPath))
                    {
                        Logger?.Error("Empty URI path.");
                        return false;
                    }
                    if (string.IsNullOrWhiteSpace(filepath))
                    {
                        Logger?.Error("Empty file path to download.");
                        return false;
                    }
                    if (System.IO.File.Exists(filepath))
                    {
                        Logger?.Debug("Delete file {0}", filepath);
                        System.IO.File.Delete(filepath);
                        System.Threading.Thread.Sleep(1000);
                        if (System.IO.File.Exists(filepath))
                        {
                            Logger?.Error("Cannot delete file {0}", filepath);
                            return false;
                        }
                    }
                    else
                    {
                        string folder = System.IO.Path.GetDirectoryName(filepath);
                        if (!System.IO.Directory.Exists(folder))
                        {
                            Logger?.Debug("Create folder {0}", folder);
                            System.IO.Directory.CreateDirectory(folder);
                            System.Threading.Thread.Sleep(1000);
                            if (!System.IO.Directory.Exists(folder))
                            {
                                Logger?.Error("Cannot create folder {0}", folder);
                                return false;
                            }
                        }
                    }
                    Logger?.Debug("Download file {0} from FTP {1}", filepath, uriPath);
                    using (System.Net.WebClient webClient = new System.Net.WebClient())
                    {
                        webClient.Credentials = new System.Net.NetworkCredential(username, password);
                        webClient.DownloadFile(uriPath, filepath);
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Logger?.Error("URI = {0}", uriPath);
                    Logger?.Error("Filepath = {0}", filepath);
                    Logger?.Error(ex);
                    return false;
                }
            }

            /// Check file exists in FTP.
            public static bool FileExists(string uriPath, string username, string password)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(uriPath))
                    {
                        Logger?.Error("Empty URI path.");
                        return false;
                    }
                    System.Net.FtpWebRequest request = (System.Net.FtpWebRequest)System.Net.WebRequest.Create(uriPath);
                    request.Method = System.Net.WebRequestMethods.Ftp.GetFileSize;
                    request.Credentials = new System.Net.NetworkCredential(username, password);
                    try
                    {
                        using (System.Net.FtpWebResponse response = (System.Net.FtpWebResponse)request.GetResponse())
                        {
                            return true;
                        }
                    }
                    catch (System.Net.WebException ex2)
                    {
                        Logger?.Error("URI = {0}", uriPath);
                        Logger?.Error(ex2.ToString());
                        using (System.Net.FtpWebResponse response = (System.Net.FtpWebResponse)ex2.Response)
                        {
                            if (response.StatusCode == System.Net.FtpStatusCode.ActionNotTakenFileUnavailable) return false;
                        }
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    Logger?.Error("URI = {0}", uriPath);
                    Logger?.Error(ex);
                    return false;
                }
            }

            /// Return Value = string array that contain the file names. E.g. abc.txt, def.log, etc.
            public static string[] ListDirectory(string uriFolder, string username, string password)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(uriFolder))
                    {
                        Logger?.Error("Empty URI path.");
                        return null;
                    }
                    System.Net.FtpWebRequest request = (System.Net.FtpWebRequest)System.Net.WebRequest.Create(uriFolder);
                    request.Method = System.Net.WebRequestMethods.Ftp.ListDirectory;
                    request.Credentials = new System.Net.NetworkCredential(username, password);
                    string ss = null;
                    using (System.Net.FtpWebResponse response = (System.Net.FtpWebResponse)request.GetResponse())
                    {
                        using (System.IO.Stream responseStream = response.GetResponseStream())
                        {
                            using (System.IO.StreamReader reader = new System.IO.StreamReader(responseStream))
                            {
                                ss = reader.ReadToEnd();
                            }
                        }
                        //Logger?.Debug("Status = {0}", response.StatusDescription);
                    }
                    //Logger?.Debug(ss);
                    return ss?.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                }
                catch (Exception ex)
                {
                    Logger?.Error("URI = {0}", uriFolder);
                    Logger?.Error(ex);
                    return null;
                }
            }

            /// Return value = true if success. Otherwise, false.
            /// uriPath = Destination path of the file uploaded to the FTP.
            /// filepath = source file path in the local machine.
            public static bool UploadFile(string uriPath, string username, string password, string filepath)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(uriPath))
                    {
                        Logger?.Error("Empty URI path.");
                        return false;
                    }
                    if (string.IsNullOrWhiteSpace(filepath))
                    {
                        Logger?.Error("Empty file path to upload.");
                        return false;
                    }
                    if (!System.IO.File.Exists(filepath))
                    {
                        Logger?.Error("Cannot find file to upload. {0}", filepath);
                        return false;
                    }
                    Logger?.Debug("Upload file {0} to FTP {1}", filepath, uriPath);
                    using (System.Net.WebClient webClient = new System.Net.WebClient())
                    {
                        webClient.Credentials = new System.Net.NetworkCredential(username, password);
                        webClient.UploadFile(uriPath, filepath);
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Logger?.Error("URI = {0}", uriPath);
                    Logger?.Error("Filepath = {0}", filepath);
                    Logger?.Error(ex);
                    return false;
                }
            }
        }
        #endregion

        #region SftpRegion
        public class SFTP
        {
            /// Renci.SshNet.SftpClient can upload file to SFTP.
            /// https://weblog.west-wind.com/posts/2019/Aug/20/UriAbsoluteUri-and-UrlEncoding-of-Local-File-Urls

            /// Return value = true if success. Otherwise, false.
            public static bool DeleteFile(string uriFile, string username, string password)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(uriFile))
                    {
                        Logger?.Error("Empty SFTP path.");
                        return false;
                    }
                    Uri sftpUri = new Uri(uriFile);
                    string host = sftpUri?.Host;/// 10.10.255.255
                    string remotePath = sftpUri?.AbsolutePath;/// /abc_folder/sub_folder/abc.zip
                    if (string.IsNullOrWhiteSpace(host))
                    {
                        Logger?.Error("Cannot find the host. UriFile = {0}", uriFile);
                        return false;
                    }
                    //using (Renci.SshNet.SftpClient client = new Renci.SshNet.SftpClient(host, username, password))
                    //{
                    //    client.Connect();
                    //    if (!client.IsConnected)
                    //    {
                    //        Logger?.Error("Fail to connect SFTP {0}", uriFile);
                    //        return false;
                    //    }
                    //    Logger?.Debug("Delete SFTP file {0}", uriFile);
                    //    client.DeleteFile(remotePath);
                    //    client.Disconnect();
                    //}
                    return true;
                }
                catch (Exception ex)
                {
                    Logger?.Error("URI = {0}", uriFile);
                    Logger?.Error(ex);
                    return false;
                }
            }

            /// Return value = ture if success. Otherwise, false.
            public static bool DownloadFile(string uriFile, string username, string password, string filepath)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(uriFile))
                    {
                        Logger?.Error("Empty SFTP path.");
                        return false;
                    }
                    if (string.IsNullOrWhiteSpace(filepath))
                    {
                        Logger?.Error("Empty file path to upload.");
                        return false;
                    }
                    if (System.IO.File.Exists(filepath))
                    {
                        Logger?.Debug("Delete file {0}", filepath);
                        System.IO.File.Delete(filepath);
                        System.Threading.Thread.Sleep(1000);
                        if (System.IO.File.Exists(filepath))
                        {
                            Logger?.Error("SFTP.DownloadFile: Cannot delete file {0}", filepath);
                            return false;
                        }
                    }
                    else
                    {
                        string folder = System.IO.Path.GetDirectoryName(filepath);
                        if (!System.IO.Directory.Exists(folder))
                        {
                            Logger?.Debug("Create folder {0}", folder);
                            System.IO.Directory.CreateDirectory(folder);
                            System.Threading.Thread.Sleep(1000);
                            if (!System.IO.Directory.Exists(folder))
                            {
                                Logger?.Error("SFTP.DownloadFile: Cannot create folder {0}", folder);
                                return false;
                            }
                        }
                    }
                    /// Renci.SshNet.SftpClient can upload file to SFTP.
                    /// https://weblog.west-wind.com/posts/2019/Aug/20/UriAbsoluteUri-and-UrlEncoding-of-Local-File-Urls
                    Uri sftpUri = new Uri(uriFile);
                    string host = sftpUri?.Host;/// 10.10.255.255
                    string remotePath = sftpUri?.AbsolutePath;/// /abc_folder/sub_folder/abc.zip
                    if (string.IsNullOrWhiteSpace(host))
                    {
                        Logger?.Error("Cannot find the host. UriFile = {0}", uriFile);
                        return false;
                    }
                    Logger?.Debug("Download file {0} from SFTP {1}", filepath, uriFile);
                    //using (Renci.SshNet.SftpClient client = new Renci.SshNet.SftpClient(host, username, password))
                    //{
                    //    client.Connect();
                    //    if (!client.IsConnected)
                    //    {
                    //        Logger?.Error("Fail to connect SFTP {0}", uriFile);
                    //        return false;
                    //    }
                    //    using (System.IO.Stream fs = System.IO.File.OpenWrite(filepath))
                    //    {
                    //        client.DownloadFile(remotePath, fs);
                    //    }
                    //    client.Disconnect();
                    //}
                    return true;
                }
                catch (Exception ex)
                {
                    Logger?.Error("URI = {0}", uriFile);
                    Logger?.Error("Filepath = {0}", filepath);
                    Logger?.Error(ex);
                    return false;
                }
            }

            /// Return Value = string array that contain the file names. E.g. abc.txt, def.log, etc.
            public static string[] ListDirectory(string uriFolder, string username, string password)
            {
                try
                {
                    Uri sftpUri = new Uri(uriFolder);
                    string host = sftpUri?.Host;/// 10.10.255.255
                    string directory = sftpUri?.AbsolutePath;/// /abc_folder/sub_folder
                    if (string.IsNullOrWhiteSpace(host))
                    {
                        Logger?.Error("Cannot find the host. UriFolder = {0}", uriFolder);
                        return null;
                    }
                    List<string> stringList = new List<string>();
                    //using (Renci.SshNet.SftpClient client = new Renci.SshNet.SftpClient(host, username, password))
                    //{
                    //    client.Connect();
                    //    if (!client.IsConnected)
                    //    {
                    //        Logger?.Error("Fail to connect SFTP {0}", uriFolder);
                    //        return null;
                    //    }
                    //    IEnumerable<Renci.SshNet.Sftp.SftpFile> list = client.ListDirectory(directory);
                    //    if (list != null)
                    //    {
                    //        foreach (var o in list)
                    //        {
                    //            if (o != null && o.IsDirectory == false && string.IsNullOrWhiteSpace(o.Name) == false)
                    //            {
                    //                stringList.Add(o.Name);
                    //            }
                    //        }
                    //    }
                    //    client.Disconnect();
                    //}
                    if (stringList.Count > 0) return stringList.ToArray();
                    return null;
                }
                catch (Exception ex)
                {
                    Logger?.Error("URI = {0}", uriFolder);
                    Logger?.Error(ex);
                    return null;
                }
            }

            /// Return value = true if success. Otherwise, false.
            public static bool UploadFile(string uriFolder, string username, string password, string filepath)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(uriFolder))
                    {
                        Logger?.Error("Empty SFTP path.");
                        return false;
                    }
                    if (string.IsNullOrWhiteSpace(filepath))
                    {
                        Logger?.Error("Empty file path to upload.");
                        return false;
                    }
                    if (!System.IO.File.Exists(filepath))
                    {
                        Logger?.Error("Cannot find file to upload. {0}", filepath);
                        return false;
                    }
                    /// WebClient does not work.
                    ///// https://weblogs.asp.net/dixin/upload-any-file-to-ftp-server-via-c
                    //using (System.Net.WebClient webClient = new System.Net.WebClient())
                    //{
                    //    webClient.Credentials = new System.Net.NetworkCredential(username, password);
                    //    byte[] response = webClient.UploadFile(uri, System.Net.WebRequestMethods.Ftp.UploadFile, filepath);
                    //    Logger?.Debug("Response = {0}", System.Text.Encoding.ASCII.GetString(response));
                    //}
                    /// FtpWebRequest does not work.
                    //System.Net.FtpWebRequest request = (System.Net.FtpWebRequest)System.Net.WebRequest.Create(uri);
                    //request.Method = System.Net.WebRequestMethods.Ftp.UploadFile;
                    //request.Credentials = new System.Net.NetworkCredential(username, password);
                    //using (System.IO.FileStream fileStream = new System.IO.FileStream(filepath, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                    //{
                    //    using (System.IO.Stream requestStream = request.GetRequestStream())
                    //    {
                    //        fileStream.CopyTo(requestStream);
                    //    }
                    //}
                    //using (System.Net.FtpWebResponse response = (System.Net.FtpWebResponse)request.GetResponse())
                    //{
                    //    Logger?.Debug("Status code = {0}", response.StatusCode);
                    //}
                    /// Renci.SshNet.SftpClient can upload file to SFTP.
                    /// https://weblog.west-wind.com/posts/2019/Aug/20/UriAbsoluteUri-and-UrlEncoding-of-Local-File-Urls
                    Uri sftpUri = new Uri(uriFolder);
                    string host = sftpUri?.Host;/// 10.10.255.255
                    string directory = sftpUri?.AbsolutePath;/// /abc_folder/sub_folder
                    Logger?.Debug("Uri folder = {0}", uriFolder);
                    Logger?.Debug("Host = {0}", host);
                    Logger?.Debug("Directory = {0}", directory);
                    if (string.IsNullOrWhiteSpace(host))
                    {
                        Logger?.Error("Cannot find the host. UriFolder = {0}", uriFolder);
                        return false;
                    }
                    //using (Renci.SshNet.SftpClient client = new Renci.SshNet.SftpClient(host, username, password))
                    //{
                    //    client.Connect();
                    //    if (!client.IsConnected)
                    //    {
                    //        Logger?.Error("Fail to connect SFTP {0}", uriFolder);
                    //        return false;
                    //    }
                    //    if (!string.IsNullOrWhiteSpace(directory)) { client.ChangeDirectory(directory); }
                    //    Logger?.Debug("Upload file {0} to SFTP {1}", filepath, uriFolder);
                    //    using (System.IO.FileStream fs = new System.IO.FileStream(filepath, System.IO.FileMode.Open))
                    //    {
                    //        client.UploadFile(fs, System.IO.Path.GetFileName(filepath), true);
                    //    }
                    //    client.Disconnect();
                    //}
                    return true;
                }
                catch (Exception ex)
                {
                    Logger?.Error("URI = {0}", uriFolder);
                    Logger?.Error("Filepath = {0}", filepath);
                    Logger?.Error(ex);
                    return false;
                }
            }

            public class WinscpMethod
            {
                /// Need install "WinSCP" in NuGet.
                /// https://winscp.net/eng/docs/library
                public static bool UploadFiles(string uriFolder, int port, string username, string password, string sshHostKeyFingerprint, string proxyHost, int proxyPort, string proxyUsername, string proxyPassword, params string[] filepaths)
                {
                    try
                    {
                        if ((filepaths?.Length ?? 0) < 1) return true;
                        /// https://weblog.west-wind.com/posts/2019/Aug/20/UriAbsoluteUri-and-UrlEncoding-of-Local-File-Urls
                        Uri sftpUri = new Uri(uriFolder);
                        string host = sftpUri?.Host;// 10.15.255.5
                        string directory = sftpUri?.AbsolutePath;// /Dunhill_uk/FCDB_TestData

                        //WinSCP.SessionOptions sessionOptions = new WinSCP.SessionOptions
                        //{
                        //    Protocol = WinSCP.Protocol.Sftp,
                        //    HostName = host,
                        //    PortNumber = port,
                        //    UserName = username,
                        //    Password = password,
                        //    SshHostKeyFingerprint = sshHostKeyFingerprint
                        //};
                        //sessionOptions.AddRawSettings("ProxyMethod", "2");
                        //sessionOptions.AddRawSettings("ProxyHost", proxyHost);
                        //sessionOptions.AddRawSettings("ProxyPort", proxyPort.ToString());
                        //sessionOptions.AddRawSettings("ProxyUsername", proxyUsername);
                        //sessionOptions.AddRawSettings("ProxyPassword", proxyPassword);

                        //using (WinSCP.Session session = new WinSCP.Session())
                        //{
                        //    session.Open(sessionOptions);
                        //    WinSCP.TransferOptions transferOptions = new WinSCP.TransferOptions()
                        //    {
                        //        TransferMode = WinSCP.TransferMode.Binary
                        //    };
                        //    foreach (var p in filepaths)
                        //    {
                        //        WinSCP.TransferOperationResult transferResult = session.PutFiles(p, directory, false, transferOptions);
                        //        transferResult.Check();
                        //    }
                        //    return true;
                        //}
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Logger?.Error("URI = {0}", uriFolder);
                        Logger?.Error("Proxy Host = {0}", proxyHost);
                        Logger?.Error("Filepath = {0}", string.Join("|", filepaths));
                        Logger?.Error(ex);
                        return false;
                    }
                }
            }
        }
        #endregion

        #region ZipRegion
        
        public class Zip
        {
            /// Updated date: 2020-09-06
            ///// Must reference the System.IO.Compression.FileSystem assembly in project.
            ///// https://www.codeguru.com/csharp/.net/zip-and-unzip-files-programmatically-in-c.htm
            ///// https://docs.microsoft.com/en-us/dotnet/standard/io/how-to-compress-and-extract-files
            //public class Compression
            //{
            //    public static bool Zip(string sourcePath, string zipPath)
            //    {
            //        try
            //        {
            //            if (string.IsNullOrEmpty(zipPath) || string.IsNullOrEmpty(sourcePath)) return false;
            //            if (System.IO.File.Exists(zipPath)) System.IO.File.Delete(zipPath);
            //            if (System.IO.Directory.Exists(sourcePath)) ZipFile.CreateFromDirectory(sourcePath, zipPath);
            //            else if (System.IO.File.Exists(sourcePath))
            //            {
            //                using (ZipArchive a = ZipFile.Open(zipPath, ZipArchiveMode.Create))
            //                {
            //                    a.CreateEntryFromFile(sourcePath, System.IO.Path.GetFileName(sourcePath));
            //                }
            //            }
            //            else
            //            {
            //                return false;
            //            }
            //            return true;
            //        }
            //        catch (Exception ex)
            //        {
            //            Logger?.Error(ex);
            //            return false;
            //        }
            //    }

            //    public static bool Unzip(string zipPath, string destDir)
            //    {
            //        try
            //        {
            //            if (string.IsNullOrEmpty(zipPath) || string.IsNullOrEmpty(destDir)
            //                || System.IO.File.Exists(zipPath) == false
            //                )
            //            { return false; }
            //            ZipFile.ExtractToDirectory(zipPath, destDir);// The destination directory will be created if it does not exists.
            //            return true;
            //        }
            //        catch (Exception ex)
            //        {
            //            Logger?.Error(ex);
            //            return false;
            //        }
            //    }
            //}

            /// Need to install Ionic.Zip in NuGet Package Manager.
            /// https://dotblogs.com.tw/im_sqz777/2017/11/14/002053
            public class DotNet
            {
                /// Return value = true if success. Otherwise, false.
                /// Need to install Ionic.Zip in NuGet Package Manager.
                public static bool Zip(string zipFilepath, string password, params string[] sourcePaths)
                {
                    List<string> sourceFilepaths = new List<string>();
                    List<string> sourceFolderpaths = new List<string>();
                    try
                    {
                        if ((sourcePaths?.Length ?? 0) < 1)
                        {
                            Logger?.Error("Cannot find source path.");
                            return false;
                        }
                        bool b = false;
                        foreach (string s in sourcePaths)
                        {
                            if (System.IO.File.Exists(s)) sourceFilepaths.Add(s);
                            else if (System.IO.Directory.Exists(s)) sourceFolderpaths.Add(s);
                            else
                            {
                                Logger?.Error("Cannot find source {0}", s);
                                b = true;
                            }
                        }
                        if (b) return false;
                        /// Delete the file if it exists.
                        if (System.IO.File.Exists(zipFilepath))
                        {
                            Logger?.Debug("Delete file {0}", zipFilepath);
                            System.IO.File.Delete(zipFilepath);
                            System.Threading.Thread.Sleep(1000);
                            if (System.IO.File.Exists(zipFilepath))
                            {
                                Logger?.Error("Zip file exists and cannot delete it. {0}", zipFilepath);
                                return false;
                            }
                        }
                        else
                        {
                            /// Check folder exists.
                            string folderpath = System.IO.Path.GetDirectoryName(zipFilepath);
                            if (!System.IO.Directory.Exists(folderpath))
                            {
                                Logger?.Debug("Create directory {0}", folderpath);
                                System.IO.Directory.CreateDirectory(folderpath);
                                System.Threading.Thread.Sleep(1000);
                                if (!System.IO.Directory.Exists(folderpath))
                                {
                                    Logger?.Error("Cannot create directory {0}", folderpath);
                                    return false;
                                }
                            }
                        }
                        Logger?.Debug("Zip files or folders to {0}", zipFilepath);
                        //using (Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile(zipFilepath, System.Text.Encoding.Default))/// Need to install Ionic.Zip in NuGet Package Manager.
                        //{
                        //    if (!string.IsNullOrEmpty(password)) zip.Password = password;
                        //    foreach (string s in sourceFilepaths)
                        //    {
                        //        zip.AddFile(s, "");
                        //    }
                        //    foreach (string s in sourceFolderpaths)
                        //    {
                        //        zip.AddDirectory(s, System.IO.Path.GetFileName(s));
                        //    }
                        //    zip.Save();
                        //}
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Logger?.Error("Zip filepath = {0}", zipFilepath);
                        Logger?.Error(ex);
                        return false;
                    }
                    finally
                    {
                        sourceFilepaths.Clear(); sourceFilepaths = null;
                        sourceFolderpaths.Clear(); sourceFolderpaths = null;
                    }
                }

                /// Return value = true if success. Otherwise, false.
                /// Need to install Ionic.Zip in NuGet Package Manager.
                public static bool Unzip(string zipFilepath, string password, string targetFolderPath)
                {
                    try
                    {
                        if (!System.IO.File.Exists(zipFilepath))
                        {
                            Logger?.Error("Cannot find zip file {0}", zipFilepath);
                            return false;
                        }
                        /// Check folder exists.
                        if (!System.IO.Directory.Exists(targetFolderPath))
                        {
                            Logger?.Debug("Create directory {0}", targetFolderPath);
                            System.IO.Directory.CreateDirectory(targetFolderPath);
                            System.Threading.Thread.Sleep(1000);
                            if (!System.IO.Directory.Exists(targetFolderPath))
                            {
                                Logger?.Error("Cannot create directory {0}", targetFolderPath);
                                return false;
                            }
                        }
                        Logger?.Debug("Unzip file {0} to folder {1}", zipFilepath, targetFolderPath);
                        //using (Ionic.Zip.ZipFile zip = Ionic.Zip.ZipFile.Read(zipFilepath))/// Need to install Ionic.Zip in NuGet Package Manager.
                        //{
                        //    if (!string.IsNullOrEmpty(password)) { zip.Password = password; }
                        //    zip.ExtractAll(targetFolderPath, Ionic.Zip.ExtractExistingFileAction.OverwriteSilently);/// Need to install Ionic.Zip in NuGet Package Manager.
                        //}
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Logger?.Error("Zip filepath = {0}", zipFilepath);
                        Logger?.Error(ex);
                        return false;
                    }
                }
            }

            /// http://pramaire.pixnet.net/blog/post/13525145-vb.net-%E4%BD%BF%E7%94%A87-zip%E5%A3%93%E7%B8%AE%E6%8C%87%E5%AE%9A%E7%9B%AE%E9%8C%84%E6%89%80%E6%9C%89%E6%AA%94%E6%A1%88
            /// https://msdn.microsoft.com/en-us/library/system.diagnostics.processstartinfo.redirectstandardoutput(v=vs.110).aspx
            /// https://dotblogs.com.tw/yc421206/archive/2012/04/30/71911.aspx
            /// https://msdn.microsoft.com/en-us/library/ty0d8k56(v=vs.110).aspx
            public class SevenZip
            {
                public static string ExePath { get; set; }

                public static bool Action(string sevenZipPath, string args, out string output, out string error) { return Action(sevenZipPath, args, out output, out error, 7000); }
                public static bool Action(string sevenZipPath, string args, out string output, out string error, int waitingIntervalInMS)
                {
                    bool bReturn = false;
                    output = ""; error = "";
                    try
                    {
                        if (!System.IO.File.Exists(sevenZipPath))
                        {
                            Logger?.Error(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + ". Cannot find file " + sevenZipPath);
                            return false;
                        }
                        using (System.Diagnostics.Process p = new System.Diagnostics.Process())
                        {
                            p.StartInfo.FileName = sevenZipPath;
                            p.StartInfo.Arguments = args;
                            p.StartInfo.UseShellExecute = false;
                            p.StartInfo.RedirectStandardError = true;
                            p.StartInfo.RedirectStandardOutput = true;
                            p.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                            p.StartInfo.CreateNoWindow = true;
                            p.Start();
                            output = p.StandardOutput.ReadToEnd();
                            error = p.StandardError.ReadToEnd();
                            //p.WaitForExit();
                            bReturn = p.WaitForExit(waitingIntervalInMS);
                            p.Close();
                        }
                        return bReturn;
                    }
                    catch (Exception ex)
                    {
                        Logger?.Error(ex);
                        return false;
                    }
                }

                private static string GetPossibleExePath(string exePath)
                {
                    if (!string.IsNullOrWhiteSpace(exePath)) return exePath;
                    exePath = @"C:\Program Files\7-Zip\7z.exe";
                    if (System.IO.File.Exists(exePath)) return exePath;
                    exePath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "7z.exe");
                    if (System.IO.File.Exists(exePath)) return exePath;
                    exePath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "7za.exe");
                    if (System.IO.File.Exists(exePath)) return exePath;
                    return null;
                }

                /// Return value = operation result. True if successful. Otherwise, fail.
                /// zipFilepath = Path of zip file.
                /// password = Password.
                /// output = Standard output of the process.
                /// error = Standard error of the process.
                public static bool Zip(string zipFilepath, string password, out string output, out string error, params string[] sourcePaths)
                {
                    output = null; error = null;
                    try
                    {
                        string exePath = GetPossibleExePath(ExePath);
                        /// Delete the file if it exists.
                        if (System.IO.File.Exists(zipFilepath))
                        {
                            Logger?.Debug("Delete file {0}", zipFilepath);
                            System.IO.File.Delete(zipFilepath);
                            System.Threading.Thread.Sleep(1000);
                            if (System.IO.File.Exists(zipFilepath))
                            {
                                Logger?.Error("Zip file exists and cannot delete it. {0}", zipFilepath);
                                return false;
                            }
                        }
                        else
                        {
                            /// Check folder exists.
                            string folderpath = System.IO.Path.GetDirectoryName(zipFilepath);
                            if (!System.IO.Directory.Exists(folderpath))
                            {
                                Logger?.Debug("Create directory {0}", folderpath);
                                System.IO.Directory.CreateDirectory(folderpath);
                                System.Threading.Thread.Sleep(1000);
                                if (!System.IO.Directory.Exists(folderpath))
                                {
                                    Logger?.Error("Cannot create directory {0}", folderpath);
                                    return false;
                                }
                            }
                        }
                        Logger?.Debug("Zip files or folders to {0}", zipFilepath);
                        string args = string.Format("a \"{0}\" {1}{2}", zipFilepath.Trim(' ', '"'), string.Join(" ", sourcePaths.Select(x => '"' + x.Trim(' ', '"') + '"')), string.IsNullOrEmpty(password) ? "" : string.Format(" -p{0}", password));
                        //Logger?.Debug(args);
                        return Action(exePath, args, out output, out error);
                    }
                    catch (Exception ex)
                    {
                        Logger?.Error(ex);
                        return false;
                    }
                }

                /// Return value = operation result. True if successful. Otherwise, fail.
                /// zipFilepath = Path of zip file.
                /// password = Password.
                /// targetFolderPath = Path of target folder.
                /// output = Standard output of the process.
                /// error = Standard error of the process.
                public static bool Unzip(string zipFilepath, string password, string targetFolderPath, out string output, out string error)
                {
                    output = null; error = null;
                    try
                    {
                        string exePath = GetPossibleExePath(ExePath);
                        if (!System.IO.File.Exists(zipFilepath))
                        {
                            Logger?.Error("Cannot find zip file {0}", zipFilepath);
                            return false;
                        }
                        /// Check folder exists.
                        if (!System.IO.Directory.Exists(targetFolderPath))
                        {
                            Logger?.Debug("Create directory {0}", targetFolderPath);
                            System.IO.Directory.CreateDirectory(targetFolderPath);
                            System.Threading.Thread.Sleep(1000);
                            if (!System.IO.Directory.Exists(targetFolderPath))
                            {
                                Logger?.Error("Cannot create directory {0}", targetFolderPath);
                                return false;
                            }
                        }
                        Logger?.Debug("Unzip file {0} to folder {1}", zipFilepath, targetFolderPath);
                        string args = string.Format("x \"{0}\" -o\"{1}\"{2}", zipFilepath.Trim(' ', '"'), targetFolderPath.Trim(' ', '"'), string.IsNullOrEmpty(password) ? "" : string.Format(" -p{0}", password));
                        //Logger?.Debug(args);
                        return Action(exePath, args, out output, out error);
                    }
                    catch (Exception ex)
                    {
                        Logger?.Error(ex);
                        return false;
                    }
                }
            }
        }
        #endregion

        public static byte[] ConvertFileToByteArray(string filepath, int mode)
        {
            /// Mode = 2. [File size (4 byptes)][File content]
            /// Mode = 3. [File name length (4 bytes)][File name][File content]
            /// Otherwise. [File content]
            try
            {
                byte[] data = System.IO.File.ReadAllBytes(filepath);
                if (mode == 3)
                {
                    int iFileLength = data?.Length ?? 0;
                    if (iFileLength > int.MaxValue - 8) throw new Exception(string.Format("Exceed the maximum data size {0}. Data size = [1}", int.MaxValue - 8, iFileLength));
                    string filename = System.IO.Path.GetFileName(filepath);
                    byte[] nameByteArray = System.Text.Encoding.UTF8.GetBytes(System.IO.Path.GetFileName(filepath));
                    int iNameLength = nameByteArray?.Length ?? 0;
                    if (iNameLength > int.MaxValue - 10) throw new Exception(string.Format("Exceed the maximum data size {0}. File name length = [1}", int.MaxValue - 10, iNameLength));
                    byte[] rByte = new byte[4 + iNameLength + iFileLength];
                    BitConverter.GetBytes(iNameLength).CopyTo(rByte, 0);
                    if (iNameLength > 0) nameByteArray.CopyTo(rByte, 4);
                    if (iFileLength > 0) data.CopyTo(rByte, 4 + iNameLength);
                    return rByte;
                }
                return data;
                //switch (mode)
                //{
                //    case 2:
                //        int iLength = data?.Length ?? 0;
                //        if (iLength > int.MaxValue-8) throw new Exception(string.Format("Exceed the maximum data size {0}. Data size = [1}", int.MaxValue-8, iLength));
                //        byte[] rByte = new byte[4 + iLength];
                //        BitConverter.GetBytes(iLength).CopyTo(rByte, 0);
                //        if (iLength > 0) data.CopyTo(rByte, 4);
                //        return rByte;
                //    case 3:

                //        break;
                //    default:
                //        return data;
                //}
            }
            catch (Exception ex)
            {
                Logger?.Error(ex);
                return null;
            }
        }


    }
}
