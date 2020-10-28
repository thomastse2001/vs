using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VsCSharpWinForm_sample2.Helpers
{
    public class GeneralT
    {
        /// General functions.
        /// Updated date: 2020-09-04
        /// To zip or unzip file, need to install Ionic.Zip in NuGet Package Manager.

        public static TLog Logger { get; set; }

        /// Get the default folder. Return the App Home if the input path is null.
        /// Return value = default folder
        public static string GetDefaultFolder(string defaultFolder)
        {
            try
            {
                if (string.IsNullOrEmpty(defaultFolder))
                { return System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location); }
                else { return defaultFolder.Trim().TrimEnd(new char[] { (char)9, ' ', System.IO.Path.DirectorySeparatorChar }); }
            }
            catch (Exception ex)
            {
                Logger?.Error("Default folder = {0}", defaultFolder);
                Logger?.Error(ex);
                return null;
            }
        }

        /// Get the default absolte path if the input path is a relative path.
        /// Return value = output.
        /// path = input path
        /// defaultFolder = default folder
        public static string GetDefaultAbsolutePathIfRelative(string path) { return GetDefaultAbsolutePathIfRelative(path, string.Empty); }
        public static string GetDefaultAbsolutePathIfRelative(string path, string defaultFolder)
        {
            char[] cArrayTrim = { (char)9, ' ', System.IO.Path.DirectorySeparatorChar };
            try
            {
                if (string.IsNullOrEmpty(path)) { return GetDefaultFolder(defaultFolder); }
                path = path.Trim(cArrayTrim);
                if (System.IO.Path.IsPathRooted(path)) { return path; }
                else { return GetDefaultFolder(defaultFolder) + System.IO.Path.DirectorySeparatorChar + path; }
            }
            catch (Exception ex)
            {
                Logger.Error("Path = {0}", path);
                Logger.Error(ex);
                return null;
            }
        }

        /// Verify if the folder exists or not. If the folder does not exist, create it.
        /// Return Value = true if the folder exists. False if the folder does not exist and it fails to create it.
        /// folder = folder path
        public static bool FolderExistsOrCreateIt(string folder)
        {
            try
            {
                folder = folder.TrimEnd((char)9, ' ', System.IO.Path.DirectorySeparatorChar).Trim();
                if (System.IO.Directory.Exists(folder)) { return true; }
                System.IO.Directory.CreateDirectory(folder);
                if (System.IO.Directory.Exists(folder)) { return true; }
                Logger.Warn("Cannot create the folder {0}", folder);
                return false;
            }
            catch (Exception ex)
            {
                Logger.Error("Folder = {0}", folder);
                Logger.Error(ex);
                return false;
            }
        }

        /// Read text in a file to a buffer.
        /// Return value = string stored the content of file
        /// filepath = path of the file to be read
        /// https://support.microsoft.com/en-us/help/816149/how-to-read-from-and-write-to-a-text-file-by-using-visual-c
        public static string ReadTextFromFile(string filepath)
        {
            try
            {
                if (!System.IO.File.Exists(filepath)) { return null; }
                string sReturn = "";
                using (System.IO.StreamReader sr = new System.IO.StreamReader(filepath))
                {
                    sReturn = sr.ReadToEnd();
                }
                return sReturn;
            }
            catch (Exception ex)
            {
                Logger.Error("Filepath = {0}", filepath);
                Logger.Error(ex);
                return null;
            }
        }

        /// Write text to a file.
        /// Return value = true if sccess. False otherwise.
        /// Filepath = file path
        /// Content = content to be written to a file
        /// isAppend = false if overwrite the file. True if append to the file if the file already exists.
        /// https://support.microsoft.com/en-us/help/816149/how-to-read-from-and-write-to-a-text-file-by-using-visual-c
        public static bool WriteTextToFile(string filepath, string content) { return WriteTextToFile(filepath, content, false); }
        public static bool WriteTextToFile(string filepath, string content, bool isAppend)
        {
            try
            {
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(filepath, isAppend))
                {
                    sw.Write(content);
                    sw.Flush();
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error("Filepath = {0}", filepath);
                Logger.Error(ex);
                return false;
            }
        }

        /// Write a lock file.
        /// Return value = false if the lock file already exists. True if the lock file does not exist, and able to create a lock file successfully.
        /// This function requires to call the "WriteTextToFile" function.
        public static bool WriteLockFile() { return WriteLockFile(""); }
        public static bool WriteLockFile(string path) { return WriteLockFile(path, ""); }
        public static bool WriteLockFile(string path, string content)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            try
            {
                /// Get the file path of lock file.
                if (string.IsNullOrEmpty(path))
                {
                    //sFolder = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
                    //sPath = sFolder + System.IO.Path.DirectorySeparatorChar + System.IO.Path.GetFileNameWithoutExtension(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + ".lock";
                    sb.Append(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location)).Append(System.IO.Path.DirectorySeparatorChar);
                    sb.Append(System.IO.Path.GetFileNameWithoutExtension(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName)).Append(".lock");
                    path = sb.ToString();
                    sb.Length = 0;
                    sb.Capacity = 0;
                }
                if (System.IO.File.Exists(path)) { return false; }
                else
                {
                    if (string.IsNullOrEmpty(content))
                    {
                        sb.Append("Already running the same EXE in the folder ").Append(System.IO.Path.GetDirectoryName(path)).AppendLine();
                        sb.Append("If the above application is not running, please delete this LOCK file.").AppendLine().AppendLine();
                        sb.Append("User name = ").Append(System.Environment.UserName).AppendLine();
                        sb.Append("User domain = ").Append(System.Environment.UserDomainName).AppendLine();
                        sb.Append("Computer name = ").Append(System.Environment.MachineName).AppendLine();
                        sb.Append("Session name = ").Append(System.Environment.GetEnvironmentVariable("SESSIONNAME")).AppendLine();
                        content = sb.ToString();
                    }
                    return WriteTextToFile(path, content, false);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return false;
            }
            finally { sb = null; }
        }

        /// Remove a lock file.
        /// Return value = false if the lock file already exists. True if the lock file does not exist, or is removed successfully.
        public static bool RemoveLockFile() { return RemoveLockFile(""); }
        public static bool RemoveLockFile(string path)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            try
            {
                /// Get the file path of lock file.
                if (string.IsNullOrEmpty(path))
                {
                    sb.Append(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location)).Append(System.IO.Path.DirectorySeparatorChar);
                    sb.Append(System.IO.Path.GetFileNameWithoutExtension(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName)).Append(".lock");
                    path = sb.ToString();
                }
                /// Check if the lock file exists. If it exists, delete it.
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                    return !System.IO.File.Exists(path);
                }
                else { return true; }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return false;
            }
            finally { sb = null; }
        }

        /// Check if the EXIT file exists
        /// sExtensions = file extensions of the EXIT files. If more than one, separate them with delimiter in the value of parameter "sDelimiter".
        /// sDelimiter = delimiter to separate various file extension.
        /// bDeleteIfExist = whether delete the EXIT files if they exist. True = delete. False = not delete.
        /// This function requires to call the "WriteLog" function.
        //public static bool ExitFileExists() { return ExitFileExists(true); }
        //public static bool ExitFileExists(bool bDeleteIfExist) { return ExitFileExists(bDeleteIfExist, ".end|.exit|.quit|.stop|.close|.finish"); }
        //public static bool ExitFileExists(bool bDeleteIfExist, string sExtensions) { return ExitFileExists(bDeleteIfExist, sExtensions, "|"); }
        public static bool ExitFileExists(bool isDeleteIfExist, params string[] extArray)
        {
            bool bReturn = false;
            bool bLoop = true;
            int i = 0;
            string s, sPath, sPathNoExt;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            try
            {
                /// Check extension.
                if (extArray == null || extArray.Length < 1)
                {
                    extArray = new string[] { ".end", ".exit", ".quit", ".stop", ".close", ".finish" };
                }
                /// Get the file path of the EXIT file without extension. E.g. sPathNoExt="C:\ABC\App1" if full path="C:\ABC\App1.exe"
                sb.Append(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location)).Append(System.IO.Path.DirectorySeparatorChar);
                sb.Append(System.IO.Path.GetFileNameWithoutExtension(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName));
                sPathNoExt = sb.ToString();
                while (bLoop && (i < extArray.Length))
                {
                    s = extArray[i]?.Trim().ToLower();
                    if (!string.IsNullOrEmpty(s))
                    {
                        /// Get the file path of target file.
                        sPath = sPathNoExt + s;
                        if (System.IO.File.Exists(sPath))
                        {
                            /// Set true to indicate that the EXIT file exists.
                            bReturn = true;
                            if (isDeleteIfExist)
                            {
                                try { System.IO.File.Delete(sPath); }
                                catch
                                {
                                    /// Try to delete it with loop.
                                    int i2 = 0;
                                    while (System.IO.File.Exists(sPath) && i2 < 7)
                                    {
                                        try { System.IO.File.Delete(sPath); }
                                        catch { System.Threading.Thread.Sleep(500); }
                                        i2 += 1;
                                    }
                                }
                                if (System.IO.File.Exists(sPath))
                                { Logger?.Error("Cannot delete an EXIT file {0}", sPath); }
                            }
                            else { bLoop = false; }/// stop looping if choose to not delete the EXIT file.
                        }
                    }
                    i += 1;
                }
            }
            catch (Exception ex) { Logger?.Error(ex); }
            finally { sb = null; }
            return bReturn;
        }

        /// Compute the percentage of free disk space to total disk space. The input parameter must be a local path.
        /// Return value = Percentage of free disk space to total disk space. Return a negative number, e.g. -1, -2, -3, ... if errors exist.
        /// path = Path of the target drive.
        public static double ComputeFreeDiskSpacePercentage(string path)
        {
            string sDrive;
            System.IO.DriveInfo oDrive;
            try
            {
                if (string.IsNullOrEmpty(path) == false) { path = path.Trim(new char[]{ (char)9, ' ', (char)10, (char)13 }); }
                if (string.IsNullOrEmpty(path)) { return -2; }
                sDrive = System.IO.Path.GetPathRoot(path);
                if (string.IsNullOrEmpty(sDrive)) { return -3; }
                oDrive = new System.IO.DriveInfo(sDrive);
                return 100.0 * oDrive.TotalFreeSpace / oDrive.TotalSize;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return -1;
            }
            finally { oDrive = null; }
        }

        /// Check whether the switch exists or get the value of argument
        /// Return value = boolean value. True if the switch exists. Otherwise, false.
        /// args = string of arguments
        /// sSwitch = string of switch
        /// output = output string of value of switch
        public static bool GetArguments1(string[] args, string sSwitch) { string output; return GetArguments1(args, sSwitch, out output); }
        public static bool GetArguments1(string[] args, string sSwitch, out string output)
        {
            bool bReturn = false;
            int i;
            string s;
            output = "";
            try
            {
                if (args == null) { return bReturn; }
                if (sSwitch.Length > 0)
                {
                    i = 0;
                    while ((bReturn == false) && (i < args.Length))
                    {
                        s = args[i];
                        if (s.Length >= sSwitch.Length)
                        {
                            if (s.Substring(0, sSwitch.Length).ToLower().Equals(sSwitch.ToLower()))
                            {
                                bReturn = true;/// stop looping.
                                if (s.Length > sSwitch.Length) { output = s.Substring(sSwitch.Length); }
                            }
                        }
                        i += 1;
                    }
                }
            }
            catch (Exception ex) { Logger?.Error(ex); }
            return bReturn;
        }

        /// Check if the file is locked.
        /// Return value = true if the file is locked. Otherwise, false.
        /// file = file path
        /// isEnableDebugLog = whether enable debug log.
        /// https://stackoverflow.com/questions/876473/is-there-a-way-to-check-if-a-file-is-in-use
        public static bool IsFileLocked(string file) { return IsFileLocked(file, true); }
        public static bool IsFileLocked(string file, bool isEnableDebugLog)
        {
            bool bReturn = false;
            System.IO.FileStream oStream = null;
            try { oStream = System.IO.File.Open(file, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.None); }
            catch
            {
                /// the file is unavailable because it is:
                /// still being written to
                /// or being processed by another thread
                /// or does not exist (has already been processed)
                if (isEnableDebugLog) { Logger?.Debug("File is locked. {0}", file); }
                bReturn = true;
            }
            finally
            {
                if (oStream != null)
                {
                    try { oStream.Close(); }
                    catch (Exception ex2) { Logger?.Error(ex2); }
                    finally { oStream = null; }
                }
            }
            return bReturn;
        }

        /// Copy or move file with folder hierarchy.
        /// Return value = destination file path. Null if error.
        /// bIsCopied = Is the file copied or movied? True if copy file. Otherwise, move file.
        /// bReplaceIfExist = Whether replace the file in destination or not if it already exists. True = replace the file. False = Not replace the file.
        /// sSrcPath = source file path
        /// sSrcFolder = source folder
        /// sDestFolder = destination folder
        /// iSleepingIntervalInMS = sleeping interval in milli-second. No sleeping if negative. The default value is 50.
        /// iMaxTry = maximum number to try checking the file existance. No try if negative. The default value is 5.
        public static string FileActionWithFolderHierarchy(bool bIsCopied, bool bReplaceIfExist, string sSrcPath, string sSrcFolder, string sDestFolder) { return FileActionWithFolderHierarchy(bIsCopied, bReplaceIfExist, sSrcPath, sSrcFolder, sDestFolder, 50); }
        public static string FileActionWithFolderHierarchy(bool bIsCopied, bool bReplaceIfExist, string sSrcPath, string sSrcFolder, string sDestFolder, int iSleepingIntervalInMS) { return FileActionWithFolderHierarchy(bIsCopied, bReplaceIfExist, sSrcPath, sSrcFolder, sDestFolder, iSleepingIntervalInMS, 5); }
        public static string FileActionWithFolderHierarchy(bool bIsCopied, bool bReplaceIfExist, string sSrcPath, string sSrcFolder, string sDestFolder, int iSleepingIntervalInMS, int iMaxTry)
        {
            string sDestPath, sRelativePath, sParentFolder;
            int i;
            char[] cArrayTrim = { ' ', System.IO.Path.DirectorySeparatorChar };
            try
            {
                /// Check if the source file exists.
                sSrcPath = GetDefaultAbsolutePathIfRelative(sSrcPath);
                if (!System.IO.File.Exists(sSrcPath))
                {
                    Logger.Error(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + ". Cannot find file " + sSrcPath);
                    return null;
                }
                /// Check if the file is locked.
                i = 0;
                while (IsFileLocked(sSrcPath, false) && i < iMaxTry)
                {
                    if (iSleepingIntervalInMS >= 0) { System.Threading.Thread.Sleep(iSleepingIntervalInMS); }
                    i += 1;
                }
                if (IsFileLocked(sSrcPath)) { return null; }

                //if (sSrcPath.Contains(sSrcFolder)) { sRelativePath = sSrcPath.Substring(sSrcFolder.Length).Trim(cArrayTrim); }
                sSrcFolder = GetDefaultAbsolutePathIfRelative(sSrcFolder);
                if (sSrcPath.Length > sSrcFolder.Length && sSrcPath.IndexOf(sSrcFolder) == 0) { sRelativePath = sSrcPath.Substring(sSrcFolder.Length).Trim(cArrayTrim); }
                else { sRelativePath = System.IO.Path.GetFileName(sSrcPath); }
                sDestFolder = GetDefaultAbsolutePathIfRelative(sDestFolder);
                sDestPath = sDestFolder + System.IO.Path.DirectorySeparatorChar + sRelativePath;
                sParentFolder = System.IO.Path.GetDirectoryName(sDestPath);

                /// Check if the destination file exists.
                if (System.IO.File.Exists(sDestPath))
                {
                    if (bReplaceIfExist)
                    {
                        Logger?.Info("File already exists and delete it first. {0}", sDestPath);
                        System.IO.File.Delete(sDestPath);
                        i = 0;
                        while (System.IO.File.Exists(sDestPath) && i < iMaxTry)
                        {
                            if (iSleepingIntervalInMS >= 0) { System.Threading.Thread.Sleep(iSleepingIntervalInMS); }
                            i += 1;
                        }
                        if (System.IO.File.Exists(sDestPath))
                        {
                            Logger?.Error("Cannot delete file {0}", sDestPath);
                            return null;
                        }
                    }
                    else
                    {
                        Logger?.Info("File already exists and not replace it. {0}", sDestPath);
                        return sDestPath;
                    }
                }
                /// Check or create the destination folder.
                if (!FolderExistsOrCreateIt(sParentFolder))
                {
                    Logger?.Error("Cannot create folder {0}", sParentFolder);
                    return null;
                }
                /// File action.
                if (bIsCopied)
                {
                    Logger?.Debug("Copy file from '{0}' to '{1}'", sSrcPath, sDestPath);
                    i = 0;
                    while (IsFileLocked(sSrcPath, false) && i < iMaxTry)
                    {
                        if (iSleepingIntervalInMS >= 0) { System.Threading.Thread.Sleep(iSleepingIntervalInMS); }
                        i += 1;
                    }
                    System.IO.File.Copy(sSrcPath, sDestPath, true);
                    /// Check if the destination file exists.
                    if (!System.IO.File.Exists(sDestPath))
                    {
                        Logger?.Error("Fail to copy file. Cannot find file {0}", sDestPath);
                        return null;
                    }
                }
                else
                {
                    Logger?.Debug("Move file from '{0}' to '{1}'", sSrcPath, sDestPath);
                    i = 0;
                    while (IsFileLocked(sSrcPath, false) && i < iMaxTry)
                    {
                        if (iSleepingIntervalInMS >= 0) { System.Threading.Thread.Sleep(iSleepingIntervalInMS); }
                        i += 1;
                    }
                    System.IO.File.Move(sSrcPath, sDestPath);
                    /// Check if the destination file exists.
                    if (!System.IO.File.Exists(sDestPath))
                    {
                        Logger?.Error("Fail to move file. Cannot find file {0}", sDestPath);
                        return null;
                    }
                    /// Check if the source file exists on file moving.
                    if (System.IO.File.Exists(sSrcPath))
                    { Logger.Warn("The original file exists, which is unexpected. {0}", sSrcPath); }
                }
                /// Return the destination file path.
                return sDestPath;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return null;
            }
        }

        /// Delete a file and return the result.
        /// Return value = true if delete successfully. Otherwise, false.
        /// sPath = path of file to be deleted.
        public static bool FileDeletionReturnBool(string sPath)
        {
            try
            {
                if (!System.IO.File.Exists(sPath)) { return true; }
                System.IO.File.Delete(sPath);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// Delete a file with retry if the file is locked.
        /// Return value = true if delete successfully. Otherwise, false.
        /// sPath = path of file to be deleted.
        /// iSleepingIntervalInMS = sleeping interval in milli-second
        /// iTimeoutInMS = timeout in milli-second
        public static bool FileDeletionWithRetry(string sPath) { return FileDeletionWithRetry(sPath, 5); }
        public static bool FileDeletionWithRetry(string sPath, int iSleepingIntervalInMS) { return FileDeletionWithRetry(sPath, iSleepingIntervalInMS, 5000); }
        public static bool FileDeletionWithRetry(string sPath, int iSleepingIntervalInMS, int iTimeoutInMS)
        {
            bool b;
            DateTime t;
            try
            {
                if (!System.IO.File.Exists(sPath)) { return true; }
                b = true; t = DateTime.Now;
                do
                {
                    if (FileDeletionReturnBool(sPath)) { b = false; }
                    else { if (iSleepingIntervalInMS >= 0) { System.Threading.Thread.Sleep(iSleepingIntervalInMS); } }
                } while (b && (int)(DateTime.Now - t).TotalMilliseconds < iTimeoutInMS);
                if (System.IO.File.Exists(sPath)) { return false; }
                return true;
            }
            catch (Exception ex)
            {
                Logger?.Error(ex);
                return false;
            }
        }

        /// Check if all required files exist.
        /// Return Value = Whether all required files exist. True if all required files exist. Otherwise, false.
        /// filesRequired = array of file paths.
        public static bool ValidFilesExist(params string[] filesRequired)
        {
            bool bReturn;
            string sDefaultFolder;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            try
            {
                if (filesRequired == null || filesRequired.Length < 1) { return true; }
                sDefaultFolder = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
                bReturn = true;
                foreach (string s in filesRequired)
                {
                    if (System.IO.File.Exists(GetDefaultAbsolutePathIfRelative(s, sDefaultFolder)) == false)
                    {
                        bReturn = false;
                        Logger?.Error("Cannot find valid file {0}", s);
                    }
                }
                return bReturn;
            }
            catch (Exception ex)
            {
                Logger?.Error(ex);
                return false;
            }
            finally { sb = null; }
        }

        /// Check if all required services running.
        /// Return Value = Whether all required services running. True if all required services running. Otherwise, false.
        /// servicesRequired = array of services.
        public static bool ValidServicesRunning(params string[] servicesRequired)
        {
            bool bReturn;
            try
            {
                if (servicesRequired == null || servicesRequired.Length < 1) { return true; }
                bReturn = true;
                // foreach (string s in sArrayServicesRequired)
                // {
                // bReturn = bReturn && ServiceStart(s);
                // }
                return bReturn;
            }
            catch (Exception ex)
            {
                Logger?.Error(ex);
                return false;
            }
        }

        public static bool OtherSameProcessesExist()
        {
            int i;
            System.Diagnostics.Process pCurrent;
            System.Diagnostics.Process[] ps;
            try
            {
                /// Get the current process.
                pCurrent = System.Diagnostics.Process.GetCurrentProcess();
                /// Get the other same processes.
                ps = System.Diagnostics.Process.GetProcessesByName(pCurrent.ProcessName);
                i = 0;
                while ((i < ps.Length) && (ps[i].Id == pCurrent.Id)) { i += 1; }
                if (i < ps.Length)
                {
                    Logger?.Debug(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + ". Detect.");
                    return true;
                }
                else { return false; }
            }
            catch (Exception ex)
            {
                Logger?.Error(ex);
                return false;
            }
        }

        /// Kill same processes except the current process.
        public static void KillSameProcessesExceptCurrent()
        {
            System.Diagnostics.Process pCurrent;
            System.Diagnostics.Process[] ps;
            try
            {
                /// Get the current process.
                pCurrent = System.Diagnostics.Process.GetCurrentProcess();
                /// Get the other same processes.
                ps = System.Diagnostics.Process.GetProcessesByName(pCurrent.ProcessName);
                foreach (System.Diagnostics.Process p in ps)
                {
                    if (p.Id != pCurrent.Id)
                    {
                        if (p.Responding)
                        {
                            Logger?.Debug("Close the same process {0} with ID {1}", p.ProcessName, p.Id);
                            p.CloseMainWindow();
                        }
                        else
                        {
                            Logger?.Debug("Kill the same process {0} with ID {1}", p.ProcessName, p.Id);
                            p.Kill();
                        }
                    }
                }
                /// Get the other same processes again.
                ps = System.Diagnostics.Process.GetProcessesByName(pCurrent.ProcessName);
                foreach (System.Diagnostics.Process p in ps)
                {
                    if (p.Id != pCurrent.Id)
                    {
                        Logger?.Debug("Kill the same process {0} with ID {1}", p.ProcessName, p.Id);
                        p.Kill();
                    }
                }
            }
            catch (Exception ex) { Logger?.Error(ex); }
        }

        /// Abort a specific thread and set it to nothing.
        /// Return Value = Result whether abort the thread successfully. True if success. Otherwise, false.
        /// pThread = target thread that will be aborted.
        public static bool AbortThread(ref System.Threading.Thread pThread)
        {
            if (pThread == null) { return true; }
            try
            {
                if (pThread.IsAlive)
                {
                    Logger?.Debug("Force to abort the thread named {0}", pThread.Name);
                    pThread.Abort();
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger?.Error(ex);
                return false;
            }
            finally { pThread = null; }
        }

        /// 7-Zip operation.
        /// Return value = operation result. True if successful. Otherwise, fail.
        /// sevenZipPath = Path of 7-Zip EXE file.
        /// args = Arguments.
        /// output = Standard output of the process.
        /// error = Standard error of the process.
        /// waitingIntervalInMS = Waiting interval in millisecond.
        /// http://pramaire.pixnet.net/blog/post/13525145-vb.net-%E4%BD%BF%E7%94%A87-zip%E5%A3%93%E7%B8%AE%E6%8C%87%E5%AE%9A%E7%9B%AE%E9%8C%84%E6%89%80%E6%9C%89%E6%AA%94%E6%A1%88
        /// https://msdn.microsoft.com/en-us/library/system.diagnostics.processstartinfo.redirectstandardoutput(v=vs.110).aspx
        /// https://dotblogs.com.tw/yc421206/archive/2012/04/30/71911.aspx
        /// https://msdn.microsoft.com/en-us/library/ty0d8k56(v=vs.110).aspx
        public static bool SevenZipAction1(string sevenZipPath, string args, out string output, out string error) { return SevenZipAction1(sevenZipPath, args, out output, out error, 7000); }
        public static bool SevenZipAction1(string sevenZipPath, string args, out string output, out string error, int waitingIntervalInMS)
        {
            bool bReturn = false;
            System.Diagnostics.Process p = null;
            output = ""; error = "";
            try
            {
                if (System.IO.File.Exists(sevenZipPath) == false)
                {
                    Logger?.Error(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + ". Cannot find file " + sevenZipPath);
                    return false;
                }
                p = new System.Diagnostics.Process();
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
                return bReturn;
            }
            catch (Exception ex)
            {
                Logger?.Error(ex);
                return false;
            }
            finally
            {
                if (p != null)
                {
                    try { p.Close(); }
                    catch (Exception ex) { Logger?.Error(ex); }
                    finally { p = null; }
                }
            }
        }

        /// Compute hash from string
        /// Return value = hash string of the input parameter 'sSrc'
        /// input = input string
        /// encodingMethod = to define encoding method. Default method is UTF8. Can use other encoding methods including UTF8, UTF7, UTF32, ASCII, Unicode, BigEndianUnicode, Default (ANSI).
        /// algorithm = to define hash algorithm. Default method is SHA256. The hash algorithms includes HMACMD5, HMACRIPEMD160, HMACSHA1, HMACSHA256, HMACSHA384, HMACSHA512, MD5, SHA1, SHA256, SHA384, SHA512.
        /// https://stackoverflow.com/questions/3984138/hash-string-in-c-sharp
        /// https://support.microsoft.com/en-us/help/307020/how-to-compute-and-compare-hash-values-by-using-visual-c
        public static string ComputeHashFromString(string input) { return ComputeHashFromString(input, "UTF8"); }
        public static string ComputeHashFromString(string input, string encodingMethod) { return ComputeHashFromString(input, encodingMethod, "SHA256"); }
        public static string ComputeHashFromString(string input, string encodingMethod, string algorithm)
        {
            byte[] myBytes1;
            byte[] myBytes2 = null;
            System.Text.StringBuilder sb = null;
            try
            {
                switch (encodingMethod?.Trim().ToUpper())
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
                switch (algorithm?.Trim().ToUpper())
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
            catch (Exception ex)
            {
                Logger?.Error(ex);
                return null;
            }
            finally { myBytes1 = null; myBytes2 = null; sb = null; }
        }

        //public static string[] SearchFilesWithPatternInFolder(string folderPath, string searchPattern)
        //{
        //    try
        //    {
        //        if (string.IsNullOrWhiteSpace(folderPath) || string.IsNullOrWhiteSpace(searchPattern)) { return null; }
        //        if (!System.IO.Directory.Exists(folderPath)) { return null; }// Cannot find folder.
        //        // Get all files with search pattern in the folder.
        //        return System.IO.Directory.GetFiles(folderPath, searchPattern);
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger?.Error(ex);
        //        return null;
        //    }
        //}

        public static string[] GetFilesWithPatternAndExpiryDayCountInFolder(string folderPath, string fileNamePrefix, string fileNameSuffix, string fileExtension, int expiryDayCount)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(folderPath) || System.IO.Directory.Exists(folderPath) == false) { return null; }
                string filenameFormat = string.Format("{0}*{1}{2}", fileNamePrefix, fileNameSuffix, fileExtension);
                string[] filepaths = System.IO.Directory.GetFiles(folderPath, filenameFormat);
                List<string> list = new List<string>();
                foreach (string p in filepaths)
                {
                    try
                    {
                        string s = System.IO.Path.GetFileNameWithoutExtension(p);
                        if (!string.IsNullOrEmpty(s))
                        {
                            if (string.IsNullOrEmpty(fileNamePrefix) == false && fileNamePrefix.Length < s.Length) { s = s.Substring(fileNamePrefix.Length); }
                            if (string.IsNullOrEmpty(fileNameSuffix) == false && fileNameSuffix.Length < s.Length) { s = s.Substring(0, s.Length - fileNameSuffix.Length); }
                            if (string.IsNullOrEmpty(s) == false && s.Length > 0)
                            {
                                if (DateTime.TryParseExact(s, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime t))
                                {
                                    if ((int)(DateTime.Now - t).TotalDays > expiryDayCount)
                                    {
                                        list.Add(p);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex2)
                    {
                        Logger?.Error("File name = {0}", p);
                        Logger?.Error(ex2);
                    }
                }
                return list.ToArray();
            }
            catch (Exception ex)
            {
                Logger?.Error(ex);
                return null;
            }
        }
    }
}
