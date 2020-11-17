using System;
using System.Collections.Generic;
using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

namespace VsCSharpWinForm_sample2.Helpers
{
    public class ZipHelper
    {
        /// Updated date: 2020-09-06
        public static TLog Logger { get; set; }

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
}
