using System;
using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

namespace VsCSharpWinForm_sample2.Helpers
{
    public class FileTransferHelper
    {
        /// Updated date: 2020-09-10
        public static TLog Logger { get; set; }

        public class FTP
        {
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

        /// Renci.SshNet.SftpClient can upload file to SFTP.
        /// https://weblog.west-wind.com/posts/2019/Aug/20/UriAbsoluteUri-and-UrlEncoding-of-Local-File-Urls
        public class SFTP
        {
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
    }
}
