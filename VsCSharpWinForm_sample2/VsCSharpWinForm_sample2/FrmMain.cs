﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VsCSharpWinForm_sample2.Models;
using VsCSharpWinForm_sample2.Helpers;

namespace VsCSharpWinForm_sample2
{
    public partial class FrmMain : Form
    {
        /// ContainLengthAsHeader, EnableAnalyzeIncomingData 
        /// Server: true, true
        /// Client: true, true

        /// ContainLengthAsHeader, EnableAnalyzeIncomingData 
        /// Server: false, true
        /// Client: false, false

        private bool IsExit = false;/// flag to indicate if this application will exit. Set it to true, in order to release all memory and stop the threads.
        private string ExeFileNameWithoutExt = "";
        private bool IsLockFileCreatedNormally = false;/// Whether the lock file is created normally. The default value is false.
        private bool IsLoginSuccess = false;/// Whether login successful. The default value is false.
        private bool IsWarnIfFormClosing = true;/// Whether show a warning dialog box when form is closing. The default value is true.

        private static Helpers.TLog Logger = new Helpers.TLog();

        private Views.FrmWait MyFrmWait = null;

        public FrmMain()
        {
            InitializeComponent();
        }

        /// https://mutelight.org/using-the-invoke-design-pattern-with-anonymous-methods
        /// https://dotblogs.com.tw/shinli/2015/04/16/151076
        /// https://www.codeproject.com/Articles/37642/Avoiding-InvokeRequired
        // private static delegate void WriteLogToUICallback(string sMessage);
        //private static void WriteLogToUI(string sMessage)
        //{
        //    try
        //    {
        //        // https://docs.microsoft.com/en-us/dotnet/framework/winforms/controls/how-to-make-thread-safe-calls-to-windows-forms-controls
        //        if (InvokeRequired)
        //        {
        //            Invoke(new WriteLogToUICallback(WriteLogToUI), new object[] { sMessage });
        //            return;
        //        }
        //        char[] cArrayTrim = { ' ', '\t', (char)10, (char)13 };
        //        txtLog.AppendText(sMessage.Trim(cTrim) + Environment.NewLine);
        //        if (txtLog.TextLength > 100000) { txtLog.Text = txtLog.Text.Substring(txtLog.Text.Length - 100000); }
        //        txtLog.SelectionStart = txtLog.TextLength;
        //        txtLog.ScrollToCaret();
        //    }
        //    catch (Exception ex)
        //    {
        //        string s = "[error] " + System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + ". " + ex.Message;
        //        try
        //        {
        //            if (mLogger == null) { Console.WriteLine(s); }
        //            else { mLogger.WriteLog(s); }
        //        }
        //        catch (Exception ex2) { Console.WriteLine("[error] " + System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + ". " + ex2.Message); }
        //    }
        //}
        private void WriteLogToUI(string format, params object[] args)
        {
            try
            {
                Invoke(new MethodInvoker(delegate
                {
                    try
                    {
                        int iMax = 100000;
                        char[] cArrayTrim = { ' ', '\t', (char)10, (char)13 };
                        string s = (args?.Length ?? 0) < 1 ? format : string.Format(format, args);
                        TxtLog.AppendText(s.Trim(cArrayTrim) + Environment.NewLine);
                        if (TxtLog.TextLength > iMax) TxtLog.Text = TxtLog.Text.Substring(TxtLog.Text.Length - iMax);
                        TxtLog.SelectionStart = TxtLog.TextLength;
                        TxtLog.ScrollToCaret();
                    }
                    catch (Exception ex2)
                    {
                        try { Logger?.Error(ex2); }
                        catch (Exception ex3) { Console.WriteLine("[error] {0}.{1}. {2}", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex3.Message); }
                    }
                }));
            }
            catch (Exception ex)
            {
                try { Logger?.Error(ex); }
                catch (Exception ex4) { Console.WriteLine("[error] {0}.{1}. {2}", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex4.Message); }
            }
        }

        private void LocalLogger(TLog.LogLevel logLevel, string format, params object[] args)
        {
            try
            {
                Logger?.Log(logLevel, format, args);
                WriteLogToUI(format, args);
            }
            catch (Exception ex)
            {
                Logger?.Error(ex);
            }
        }

        private void FrmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                IsExit = true;/// set to exit.

                TcpClientCloseAll();
                lock (Param.TcpClient.FormListLocker)
                {
                    if (Param.TcpClient.FormList != null)
                    {
                        Param.TcpClient.FormList.Clear();
                        Param.TcpClient.FormList = null;
                    }
                }
                if (Param.TcpServer.ServerSocket != null)
                {
                    Param.TcpServer.ServerSocket.StopListening();
                    Param.TcpServer.ServerSocket = null;
                }
                lock (Param.TcpServer.IncomingDataQueueLocker)
                {
                    if (Param.TcpServer.IncomingDataQueue != null)
                    {
                        Param.TcpServer.IncomingDataQueue.Clear();
                        Param.TcpServer.IncomingDataQueue = null;
                    }
                }

                /// Remove the lock file.
                if (IsLockFileCreatedNormally) GeneralT.RemoveLockFile();
                if (IsLoginSuccess) Logger?.Info("{0} ends. Version = {1}", System.IO.Path.GetFileNameWithoutExtension(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName), Application.ProductVersion);
                /// Waiting dialog.
                if (MyFrmWait != null) MyFrmWait = null;
            }
            catch (Exception ex) { Logger?.Error(ex); }
            finally
            {
                if (Logger != null) Logger = null;
            }
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (IsLoginSuccess && IsWarnIfFormClosing)
                {
                    /// Before exit, show a dialog box to ask whether really want to exit.
                    /// http://msdn.microsoft.com/en-us/library/system.windows.forms.form.closing(v=vs.110).aspx
                    /// Cancel the Closing event from closing the form.
                    if (MessageBox.Show("Do you really want to exit this application?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.No)
                        e.Cancel = true;
                }
            }
            catch (Exception ex) { Logger?.Error(ex); }
        }

        /// Generate the key values of initial file.
        private string GenerateIni()
        {
            string sReturn = "";
            StringBuilder sb = new StringBuilder();
            try
            {
                /// system.
                sb.Append("IsService = ").Append(Param.IsService.ToString()).AppendLine();
                //sb.Append("MainLoopSleepingIntervalInMS = ").Append(miMainLoopSleepingIntervalInMS.ToString()).AppendLine();

                /// Logger.
                if (Logger != null)
                {
                    sb.Append("FilePathFormat = ").Append(Logger.FilePathFormat).AppendLine();
                    sb.Append("ContentFormat = ").Append(Logger.ContentFormat).AppendLine();
                    sb.Append("MinLogLevel = ").Append(Logger.MinLogLevel.ToString()).AppendLine();
                }

                /// Login.
                sb.Append("LoginMaxRetry = ").Append(Param.Login.MaxRetry.ToString()).AppendLine();
                sb.Append("LoginFailMessage = ").Append(Param.Login.FailMessage).AppendLine();
                sb.Append("LoginExceedMaxRetryMessage = ").Append(Param.Login.ExceedMaxRetryMessage).AppendLine();

                sReturn = sb.ToString();
            }
            catch (Exception ex) { Logger?.Error(ex); }
            finally { sb = null; }
            return sReturn;
        }

        /// Read initial file.
        private void ReadIni()
        {
            string sIni = "";
            try
            {
                /// Define the initial filepath.
                string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + System.IO.Path.DirectorySeparatorChar + System.IO.Path.GetFileNameWithoutExtension(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + ".ini";
                /// Read the content of initial file.
                sIni = Helpers.GeneralT.ReadTextFromFile(path);
                if (string.IsNullOrEmpty(sIni))
                {
                    sIni = GenerateIni();
                    if (!Helpers.GeneralT.WriteTextToFile(path, sIni))
                        Logger?.Error("{0}.{1}. Cannot save the initial file {2}", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, System.Reflection.MethodBase.GetCurrentMethod().Name, path);
                }
                else
                {
                    char[] cArrayTrim = { ' ', '\t', (char)10, (char)13 };

                    /// system.
                    Param.IsService = TIniFile.GetBool(TIniFile.DoIniParaInBuffer(false, sIni, "IsService", "", Param.IsService.ToString()).Trim(cArrayTrim).ToUpper());
                    //GeneralT.DoIniParaInBufferGetInteger(ref miMainLoopSleepingIntervalInMS, sIni, "MainLoopSleepingIntervalInMS", "", miMainLoopSleepingIntervalInMS);

                    /// Log.
                    if (Logger != null)
                    {
                        Logger.FilePathFormat = GeneralT.GetDefaultAbsolutePathIfRelative(TIniFile.DoIniParaInBuffer(false, sIni, "FilePathFormat", "", Logger.FilePathFormat).Trim(cArrayTrim));
                        Logger.ContentFormat = TIniFile.DoIniParaInBuffer(false, sIni, "ContentFormat", "", Logger.ContentFormat).Trim(cArrayTrim);
                    }

                    /// Login.
                    Param.Login.MaxRetry = TIniFile.GetInt(TIniFile.DoIniParaInBuffer(false, sIni, "LoginMaxRetry", "", Param.Login.MaxRetry.ToString()), 0) ?? 0;
                    Param.Login.FailMessage = TIniFile.DoIniParaInBuffer(false, sIni, "LoginFailMessage", "", Param.Login.FailMessage).Trim(cArrayTrim);
                    Param.Login.ExceedMaxRetryMessage = TIniFile.DoIniParaInBuffer(false, sIni, "LoginExceedMaxRetryMessage", "", Param.Login.ExceedMaxRetryMessage).Trim(cArrayTrim);
                }
            }
            catch (Exception ex) { Logger?.Error(ex); }
        }

        #region Login
        private void BWorkerLogin_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                System.Threading.Thread.Sleep(1000);
                /// for-test.
                if (true)
                {
                    IsLoginSuccess = "a".Equals(Param.Login.Username) && "a".Equals(Param.Login.Hash);
                    return;
                }
                /// other login method.
            }
            catch (Exception ex) { Logger?.Error(ex); }
        }

        private void BWorkerLogin_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (MyFrmWait == null) return;
                MyFrmWait.EndWaiting = true;
            }
            catch (Exception ex) { Logger?.Error(ex); }
        }
        #endregion

        private void FrmMain_Load(object sender, EventArgs e)
        {
            string s;
            string[] args;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            try
            {
                /// EXE file name without extension.
                ExeFileNameWithoutExt = System.IO.Path.GetFileNameWithoutExtension(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                /// Set the current working directory. The current working directory may be different if the starting directory is not set when running in the Task Scheduler.
                string exeFolder = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
                System.IO.Directory.SetCurrentDirectory(exeFolder);

                TIniFile.Logger = Logger;
                /// Declare instances of the objects.

                ReadIni();/// Read initial file.
                /// Set the logger of objects.
                CyptoRijndaelT.Logger = Logger;
                GeneralT.Logger = Logger;
                TTcpClientSocket.Logger = Logger;
                TTcpServerSocket.Logger = Logger;
                //TTcpServerSocket.InnerClient.Logger = Logger;/// Declare in the static constructor of InnerClient.
                Views.FrmWait.Logger = Logger;
                Views.FrmTcpClient.Logger = Logger;
                MailHelper.Logger = Logger;
                ZipHelper.Logger = Logger;
                FileTransferHelper.Logger = Logger;
                TCsvFile.Logger = Logger;
                ExcelHelper.Logger = Logger;

                /// Show the help message if run in the command line with proper switch.
                args = System.Environment.GetCommandLineArgs();
                if (GeneralT.GetArguments1(args, "-?") || GeneralT.GetArguments1(args, "/?"))
                {
                    sb.Append("Syntax:").AppendLine();
                    sb.Append(ExeFileNameWithoutExt).Append(" -u[Username] -p[Password]").AppendLine().AppendLine();
                    sb.Append("Example:").AppendLine();
                    sb.Append(ExeFileNameWithoutExt).Append(" -uThomas -pabc").AppendLine();
                    s = sb.ToString();
                    Console.WriteLine(s);
                    MessageBox.Show(s, "Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();// http://net-informations.com/faq/general/close-exit.htm
                    return;
                }

                /// Version Information. Show the version in the GUI.
                /// Assume the product version (File version) is same as Assembly version.
                /// To update the version, in the menu bar, click "Project > [Project Name] Properties > Application > Assembly Information...", change the Assembly version and File version.
                LblVersion.Text = "Version: " + Application.ProductVersion;/// set the version text to the product version.
                LblVersion.Top = 9;
                LblVersion.Left = this.Width - LblVersion.Width - 24;/// set the position of label according to its width.

                /// create a lock file if not a service.
                IsLockFileCreatedNormally = GeneralT.WriteLockFile();
                if (!IsLockFileCreatedNormally)
                {
                    this.Close();
                    return;
                }

                /// Check free disk space percentage.
                double dFreeSpacePercentage = GeneralT.ComputeFreeDiskSpacePercentage(exeFolder);
                if (dFreeSpacePercentage < 99.0)
                {
                    s = string.Format("Too few free space. Free Space Percentage = {0:0.00}%", dFreeSpacePercentage);
                    Logger?.Warn(s);
                    MessageBox.Show(s, "Warn", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                /// Login directly with arguments.
                GeneralT.GetArguments1(args, "-u", out s);
                Param.Login.Username = s;
                GeneralT.GetArguments1(args, "-p", out s);
                Param.Login.Hash = s;
                if (!string.IsNullOrEmpty(Param.Login.Username))
                {
                    if (MyFrmWait == null) { MyFrmWait = new Views.FrmWait(); }
                    MyFrmWait.EndWaiting = false;
                    /// Authentication.
                    BWorkerLogin.RunWorkerAsync();
                    MyFrmWait.ShowDialog();
                    /// Exit if users click the EXIT button in the Waiting dialog.
                    if (MyFrmWait?.ExitNow ?? false)
                    {
                        IsLoginSuccess = false;
                        this.Close();
                        return;
                    }
                }

                /// if login fails by arguments, then prompt the login dialog.
                if (!IsLoginSuccess)
                {
                    Views.FrmLogin frmLogin = new Views.FrmLogin()
                    {
                        Text = ExeFileNameWithoutExt + " - Login"
                    };
                    frmLogin.LblVersion.Text = this.LblVersion.Text;
                    frmLogin.LblVersion.Top = 9;
                    frmLogin.LblVersion.Left = frmLogin.Width - frmLogin.LblVersion.Width - 24;
                    frmLogin.LblMessage.Text = "";
                    frmLogin.TxtUsername.Text = Param.Login.Username;
                    frmLogin.TxtPassword.Text = Param.Login.Hash;

                    int i = 0;
                    while (IsLoginSuccess == false && (Param.Login.MaxRetry < 1 || i < Param.Login.MaxRetry))
                    {
                        /// select text in the textbox.
                        frmLogin.TxtUsername.Select();
                        if (!string.IsNullOrEmpty(frmLogin.TxtUsername.Text))
                        {
                            frmLogin.TxtUsername.SelectionStart = 0;
                            frmLogin.TxtUsername.SelectionLength = frmLogin.TxtUsername.Text.Length;
                        }
                        frmLogin.TxtPassword.Clear();/// set the password textbox empty.
                        frmLogin.ShowDialog();
                        if (frmLogin.DialogResult == DialogResult.OK)
                        {
                            Param.Login.Username = frmLogin.TxtUsername.Text;
                            Param.Login.Hash = frmLogin.TxtPassword.Text;
                            if (MyFrmWait == null) MyFrmWait = new Views.FrmWait();
                            MyFrmWait.EndWaiting = false;
                            /// Authentication.
                            BWorkerLogin.RunWorkerAsync();
                            MyFrmWait.ShowDialog();
                            /// Exit if users click the EXIT button in the Waiting dialog.
                            if (MyFrmWait?.ExitNow ?? false)
                            {
                                IsLoginSuccess = false;
                                this.Close();
                                return;
                            }
                        }
                        else
                        {
                            /// Click the "Cancel" button in the Login dialog.
                            this.Close();
                            return;
                        }
                        frmLogin.LblMessage.Text = Param.Login.FailMessage;/// set the message whenever login sucessfully.
                        i += 1;
                    }
                }
                if (!IsLoginSuccess)
                {
                    Logger?.Warn(Param.Login.ExceedMaxRetryMessage);
                    MessageBox.Show(Param.Login.ExceedMaxRetryMessage, "Warn", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    this.Close();
                    return;
                }

                HideMainDialogConfig();
                UiConfigForTcpServer(false);

                /// Main process.
                Logger?.Info("{0} starts. Version = {1}", ExeFileNameWithoutExt, Application.ProductVersion);
                Logger?.Debug("Current Working Directory = {0}", System.IO.Directory.GetCurrentDirectory());
                Logger?.Debug("Key values of the initial file:{0}{1}", Environment.NewLine, GenerateIni());
                BWorkerExitFile.RunWorkerAsync();
            }
            catch (Exception ex) { Logger?.Error(ex); }
        }

        private void BtnWriteLogToUI_Click(object sender, EventArgs e)
        {
            try
            {
                WriteLogToUI("1234984 64633 d2g");
                WriteLogToUI("abcdef {0:HH:mm:ss.fff}", DateTime.Now);
            }
            catch (Exception ex) { Logger?.Error(ex); }
        }

        private void BtnCyptoRijndaelT_Click(object sender, EventArgs e)
        {
            try
            {
                string input, password, encodingText, decodingText;
                input = TxtInput1.Text;
                password = "abHe1HD2AH02534*";
                encodingText = CyptoRijndaelT.Encrypt(input, password);
                decodingText = CyptoRijndaelT.Decrypt(encodingText, password);

                WriteLogToUI("Input = {0}", input);
                WriteLogToUI("Encoding Text = {0}", encodingText);
                WriteLogToUI("Decoding Text = {0}", decodingText);
            }
            catch (Exception ex) { Logger?.Error(ex); }
        }

        private void BtnGetFilesWithPatternAndExpiryDayCountInFolder_Click(object sender, EventArgs e)
        {
            try
            {
                string[] files = GeneralT.GetFilesWithPatternAndExpiryDayCountInFolder("log", null, null, ".log", 2);
                if ((files?.Length ?? 0) > 0)
                {
                    LocalLogger(TLog.LogLevel.INFO, "FileCount = {0}", files.Length);
                    foreach (string f in files)
                    {
                        LocalLogger(TLog.LogLevel.INFO, "File: {0}", f);
                    }
                }
            }
            catch (Exception ex) { Logger?.Error(ex); }
        }

        private void BtnComputeHash_Click(object sender, EventArgs e)
        {
            try
            {
                string input = "abc123";
                string hash = GeneralT.ComputeHashFromString(input);
                LocalLogger(TLog.LogLevel.INFO, "Input: {0}", input);
                LocalLogger(TLog.LogLevel.INFO, "Hash: {0}", hash);
                /// https://coderwall.com/p/oea7uq/convert-simple-int-array-to-string-c
                int[] intArray = new int[] { 1, 4, 9 };
                string s = string.Join(",", intArray.Select(i=>i.ToString()).ToArray());
                LocalLogger(TLog.LogLevel.INFO, "s: {0}", s);
            }
            catch (Exception ex) { Logger?.Error(ex); }
        }

        private void BtnSevenZipAction1_Click(object sender, EventArgs e)
        {
            try
            {
                string targetfile = "log.7z";
                if (System.IO.File.Exists(targetfile))
                {
                    LocalLogger(TLog.LogLevel.INFO, "Delete file {0}", targetfile);
                    System.IO.File.Delete(targetfile);
                }
                string sevenZipPath = @"C:\Program Files\7-Zip\7z.exe";
                string argsText = string.Format("a {0} {1}", targetfile, "log");
                bool b = GeneralT.SevenZipAction1(sevenZipPath, argsText, out string outputText, out string errorText);
                if (b) LocalLogger(TLog.LogLevel.INFO, "7zip success.");
                else LocalLogger(TLog.LogLevel.INFO, "7zip fail.");
                LocalLogger(TLog.LogLevel.INFO, "output text = {0}", outputText);
                LocalLogger(TLog.LogLevel.INFO, "error text = {0}", errorText);
            }
            catch (Exception ex) { Logger?.Error(ex); }
        }

        #region HideMainDialogRegion
        private void HideMainDialogConfig()
        {
            try
            {
                NotifyIconHide.ContextMenuStrip = ContextMenuStripHide;
                NotifyIconHide.Text = ExeFileNameWithoutExt;
                NotifyIconHide.Icon = SystemIcons.Application;/// https://stackoverflow.com/questions/16962639/why-isnt-my-notifyicon-showing-up
            }
            catch (Exception ex) { Logger?.Error(ex); }
        }

        private void BtnHideMainDialog_Click(object sender, EventArgs e)
        {
            try
            {
                NotifyIconHide.Visible = true;
                this.WindowState = FormWindowState.Minimized;
                this.ShowInTaskbar = false;
                if (!ContextMenuStripHide.Enabled) ContextMenuStripHide.Enabled = true;
            }
            catch (Exception ex) { Logger?.Error(ex); }
        }

        private void UnhideToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                NotifyIconHide.Visible = false;
                this.ShowInTaskbar = true;
                this.WindowState = FormWindowState.Normal;
            }
            catch (Exception ex) { Logger?.Error(ex); }
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception ex) { Logger?.Error(ex); }
        }
        #endregion

        #region ExitFileRegion
        private void BWorkerExitFile_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                int ExitFileCheckingInterval = 2;
                int iSleepingDurationInMS = 500;
                DateTime tNow;
                DateTime tRef = DateTime.Now;
                bool bLoop = true;
                while (IsExit == false && bLoop)
                {
                    tNow = DateTime.Now;
                    if ((int)(tNow - tRef).TotalSeconds > ExitFileCheckingInterval)
                    {
                        tRef = tNow;
                        if (GeneralT.ExitFileExists(true))
                        {
                            bLoop = false;
                        }
                    }
                    System.Threading.Thread.Sleep(iSleepingDurationInMS);
                }
                if (!bLoop)
                {
                    IsWarnIfFormClosing = false;
                    Application.Exit();
                }
            }
            catch (Exception ex) { Logger?.Error(ex); }
        }

        private void BtnCreateExitFile_Click(object sender, EventArgs e)
        {
            try
            {
                string sPath = string.Format("{0}{1}{2}{3}",
                    System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location),
                    System.IO.Path.DirectorySeparatorChar,
                    ExeFileNameWithoutExt,
                    ".end"
                    );
                if (!System.IO.File.Exists(sPath))
                {
                    using (System.IO.FileStream fs = System.IO.File.Create(sPath)) { }
                }
            }
            catch (Exception ex) { Logger?.Error(ex); }
        }
        #endregion

        #region TcpClientRegion
        private void BtnNewTcpClient_Click(object sender, EventArgs e)
        {
            try
            {
                Views.FrmTcpClient frmTcpClient = new Views.FrmTcpClient(Param.TcpClient.Id)
                {
                    CryptPassword = Param.TcpClient.DefaultValue.CryptPassword
                };
                frmTcpClient.LblVersion.Text = this.LblVersion.Text;
                frmTcpClient.TxtServerHost.Text = Param.TcpClient.DefaultValue.ServerHost;
                frmTcpClient.NudServerPort.Value = Param.TcpClient.DefaultValue.ServerPort;
                frmTcpClient.ChkContainLengthAsHeader.Checked = Param.TcpClient.DefaultValue.ContainLengthAsHeader;
                frmTcpClient.ChkEncryptData.Checked = Param.TcpClient.DefaultValue.EncryptData;
                lock (Param.TcpClient.FormListLocker)
                { Param.TcpClient.FormList.Add(frmTcpClient); }
                Param.TcpClient.Id += 1;
                frmTcpClient.Show();
            }
            catch (Exception ex) { Logger?.Error(ex); }
        }

        private void TcpClientCloseAll()
        {
            try
            {
                lock (Param.TcpClient.FormListLocker)
                {
                    if ((Param.TcpClient.FormList?.Count ?? 0) > 0)
                    {
                        foreach (Views.FrmTcpClient frmTcpClient in Param.TcpClient.FormList)
                        {
                            if (frmTcpClient != null)
                                frmTcpClient.Close();
                        }
                    }
                }
            }
            catch (Exception ex) { Logger?.Error(ex); }
        }

        private void BtnCloseAllTcpClients_Click(object sender, EventArgs e)
        {
            try
            {
                TcpClientCloseAll();
            }
            catch (Exception ex) { Logger?.Error(ex); }
        }
        #endregion

        #region TcpServerRegion
        public static void AppendBytesToFile(string path, byte[] byteArray)
        {
            try
            {
                if (byteArray == null || string.IsNullOrWhiteSpace(path)) return;
                using (System.IO.FileStream stream = new System.IO.FileStream(path, System.IO.FileMode.Append))
                { stream.Write(byteArray, 0, byteArray.Length); }
            }
            catch (Exception ex) { Logger?.Error(ex); }
        }

        private void TcpServerIncomingDataHander1(TTcpServerSocket.TcpSocketData o)
        {
            try
            {
                if (o == null || string.IsNullOrEmpty(o.Host) || o.ByteArray == null) return;
                DateTime tRef = DateTime.Now;
                //if (oData.ByteArray != null) { AppendBytesToFile(IncomingDataFilePath, oData.ByteArray); }
                byte[] decryptedData;
                if (ChkTcpServerEncryptData.Checked) decryptedData = o.ByteArray == null ? null : CyptoRijndaelT.Decrypt(o.ByteArray, Param.TcpServer.CryptPassword);
                else decryptedData = o.ByteArray;
                if ((decryptedData?.Length ?? 0) < 1)
                {
                    //throw new Exception("Length of decrypted data < 1, which is impossible.");
                    Logger?.Debug("TCP server meets decrypted data with 0 bytes.");
                    return;
                }
                string s;
                switch (decryptedData[0])
                {
                    case Param.TcpDataType.Text:
                        string text = Encoding.UTF8.GetString(decryptedData, 1, decryptedData.Length - 1);
                        Logger?.Debug("Receice text on TCP server. Received Time = {0:yyyy-MM-dd HH:mm:ss}. Client = {1}:{2}. Text = {3}", o.Timestamp, o.Host, o.Port, text);
                        WriteLogToUI("{0:yyyy-MM-dd HH:mm:ss} Receive text from {1}:{2}. Text = {3}", o.Timestamp, o.Host, o.Port, text);
                        break;
                    case Param.TcpDataType.File:
                        int i = decryptedData.Length - 1;
                        if (i < 1)
                        {
                            s = string.Format("Length of data is {3} < 1. Received Time = {0:yyyy-MM-dd HH:mm:ss}. Client = {1}:{2}", o.Timestamp, o.Host, o.Port, i);
                            Logger?.Warn(s);
                            WriteLogToUI(s);
                            return;
                        }
                        byte[] data = new byte[i];
                        Array.Copy(decryptedData, 1, data, 0, i);
                        string filepath = string.Format(Param.TcpServer.IncomingDataFilePath, o.Timestamp, o.Host, o.Port);
                        filepath = GeneralT.GetDefaultAbsolutePathIfRelative(filepath);
                        if (!GeneralT.FolderExistsOrCreateIt(System.IO.Path.GetDirectoryName(filepath)))
                        {
                            WriteLogToUI("Fail to create folder.");
                            return;
                        }
                        System.IO.File.WriteAllBytes(filepath, data);
                        Logger?.Debug("Receive file on TCP server. Received Time = {0:yyyy-MM-dd HH:mm:ss}. Client = {1}:{2}. Length = {3}. File = {4}", o.Timestamp, o.Host, o.Port, i, filepath);
                        WriteLogToUI("{0:yyyy-MM-dd HH:mm:ss} Receive file from {1}:{2}. Length = {3}. File = {4}", o.Timestamp, o.Host, o.Port, i, filepath);
                        break;
                    default:
                        s = string.Format("Unclassified TCP data type. {0}", decryptedData[0]);
                        Logger?.Error(s);
                        WriteLogToUI(s);
                        break;
                }
            }
            catch (Exception ex) { Logger?.Error(ex); }
        }

        private void TcpServerIncomingDataHandler0()
        {
            List<TTcpServerSocket.TcpSocketData> tempList = null;
            try
            {
                lock (Param.TcpServer.IncomingDataQueueLocker)
                {
                    if ((Param.TcpServer.IncomingDataQueue?.Count ?? 0) < 1) return;
                    int iMax = 10;
                    int i = 0;
                    tempList = new List<TTcpServerSocket.TcpSocketData>();
                    while ((Param.TcpServer.IncomingDataQueue?.Count ?? 0) > 0 && i < iMax)
                    {
                        tempList.Add(Param.TcpServer.IncomingDataQueue.Dequeue());/// pass to list in order to unlock the list earlier.
                        i += 1;
                    }
                }
                if (tempList != null)
                {
                    foreach (TTcpServerSocket.TcpSocketData o in tempList)
                    {
                        TcpServerIncomingDataHander1(o);
                    }
                }
            }
            catch (Exception ex) { Logger?.Error(ex); }
            finally
            {
                if (tempList != null)
                {
                    tempList.Clear();
                    tempList = null;
                }
            }
        }

        private void ProcessTcpServerIncomingData()
        {
            try
            {
                Logger?.Debug("Start to handle TCP server incoming data.");
                DateTime tRef = DateTime.Now.AddHours(-1);
                DateTime tNow;
                int interval = 0;
                while (IsExit == false && Param.TcpServer.ServerSocket != null)
                {
                    tNow = DateTime.Now;
                    if (interval == 0 || (int)(tNow - tRef).TotalSeconds >= interval)
                    {
                        tRef = tNow;
                        TcpServerIncomingDataHandler0();
                    }
                    System.Threading.Thread.Sleep(200);
                }
                Logger?.Debug("Stop to handle TCP server incoming data.");
            }
            catch (Exception ex) { Logger?.Error(ex); }
        }

        private void BWorkerTcpServerIncomingDataHandler_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                ProcessTcpServerIncomingData();
            }
            catch (Exception ex) { Logger?.Error(ex); }
        }

        private void ChkTcpServerHeartbeatInterval_CheckedChanged(object sender, EventArgs e)
        {
            NudTcpServerHeartbeatInterval.Enabled = ChkTcpServerHeartbeatInterval.Checked;
        }

        private void ChkTcpServerMaxConnectionDuration_CheckedChanged(object sender, EventArgs e)
        {
            NudTcpServerMaxConnectionDuration.Enabled = ChkTcpServerMaxConnectionDuration.Checked;
        }

        private void ChkTcpServerMaxIdleDuration_CheckedChanged(object sender, EventArgs e)
        {
            NudTcpServerMaxIdleDuration.Enabled = ChkTcpServerMaxIdleDuration.Checked;
        }

        private void ChkTcpServerSleepingInterval_CheckedChanged(object sender, EventArgs e)
        {
            NudTcpServerSleepingInterval.Enabled = ChkTcpServerSleepingInterval.Checked;
        }

        private void UiConfigForTcpServer(bool isListening)
        {
            try
            {
                //NudTcpServerListeningPort.ReadOnly = isListening;
                //ChkTcpServerContainLengthAsHeader.Enabled = !isListening;
                //ChkTcpServerEncryptData.Enabled = !isListening;
                //ChkTcpServerHeartbeatInterval.Enabled = !isListening;
                ChkTcpServerHeartbeatInterval_CheckedChanged(null, null);
                ChkTcpServerMaxConnectionDuration_CheckedChanged(null, null);
                ChkTcpServerMaxIdleDuration_CheckedChanged(null, null);
                ChkTcpServerSleepingInterval_CheckedChanged(null, null);
                /// https://stackoverflow.com/questions/418006/how-can-i-disable-a-tab-inside-a-tabcontrol
                foreach (Control ctl in TPageTcpServerMainParameters.Controls) { ctl.Enabled = !isListening; }
                foreach (Control ctl in TPageTcpServerOtherParameters.Controls) { ctl.Enabled = !isListening; }
                BtnTcpServerStartListening.Enabled = !isListening;
                BtnTcpServerStopListening.Enabled = isListening;
                ChkTcpServerSelectAllClients.Enabled = isListening;
                BtnTcpServerDeselectAllClients.Enabled = isListening;
                ClbTcpClientList.Enabled = isListening;
                TxtTcpServerInput.ReadOnly = !isListening;
                BtnTcpServerSendText.Enabled = isListening;
                BtnTcpServerSendFile.Enabled = isListening;
            }
            catch (Exception ex) { Logger?.Error(ex); }
        }

        private void TcpServerClientListAddItem(string sItem, int iIndex, bool bChecked)
        {
            try
            {
                Invoke(new MethodInvoker(delegate
                {
                    try
                    {
                        if (iIndex < 0) { ClbTcpClientList.Items.Add(sItem, bChecked); }
                        else
                        {
                            if (iIndex > ClbTcpClientList.Items.Count) iIndex = ClbTcpClientList.Items.Count;
                            ClbTcpClientList.Items.Insert(iIndex, sItem);
                            ClbTcpClientList.SetItemChecked(iIndex, bChecked);
                        }
                    }
                    catch (Exception ex2) { Logger?.Error(ex2); }
                }));
            }
            catch (Exception ex) { Logger?.Error(ex); }
        }

        private void TcpServerClientListClear()
        {
            try
            {
                Invoke(new MethodInvoker(delegate
                {
                    try
                    {
                        ClbTcpClientList.Items.Clear();
                    }
                    catch (Exception ex2) { Logger?.Error(ex2); }
                }));
            }
            catch (Exception ex) { Logger?.Error(ex); }
        }

        private void TcpServerClientListRemoveItem(int iIndex)
        {
            try
            {
                if (iIndex < 0) return;
                Invoke(new MethodInvoker(delegate
                {
                    try
                    {
                        ClbTcpClientList.Items.RemoveAt(iIndex);
                    }
                    catch (Exception ex2) { Logger?.Error(ex2); }
                }));
            }
            catch (Exception ex) { Logger?.Error(ex); }
        }

        private void TcpServerUpdateClientList()
        {
            try
            {
                /// get list.
                List<string> tempList = Param.TcpServer.ServerSocket?.TcpClientList()?.ToList();
                if ((tempList?.Count ?? 0) > 0)
                {
                    /// loop the items on checked list box. If outdated, deleted it. Must loop from the last to the first one.
                    int i = ClbTcpClientList.Items.Count - 1;
                    while (i > -1)
                    {
                        int idx = tempList.FindIndex(x => ClbTcpClientList.Items[i].ToString().Equals(x));
                        if (idx < 0) TcpServerClientListRemoveItem(i);/// remove it in the CheckedListBox.
                        else tempList.RemoveAt(idx);/// remove it in the list, so that the list remains the new items.
                        i -= 1;
                    }
                    /// new items.
                    i = 0;
                    while (i < tempList.Count)
                    {
                        TcpServerClientListAddItem(tempList[i], -1, ChkTcpServerSelectAllClients.Checked);
                        i += 1;
                    }
                }
                else
                {
                    TcpServerClientListClear();
                }
            }
            catch (Exception ex) { Logger?.Error(ex); }
        }

        private void ProcessTcpServerUpdateClientList()
        {
            try
            {
                DateTime tRef = DateTime.Now.AddHours(-1);
                DateTime tNow;
                int interval = 1;
                Logger?.Debug("Start to update TCP server Client List.");
                while (IsExit == false)
                {
                    tNow = DateTime.Now;
                    if ((int)(tNow - tRef).TotalSeconds >= interval)
                    {
                        tRef = tNow;
                        TcpServerUpdateClientList();
                    }
                    System.Threading.Thread.Sleep(200);
                }
                Logger?.Debug("Stop to update TCP server Client List.");
            }
            catch (Exception ex) { Logger?.Error(ex); }
        }

        private void BWorkerTcpServerUpdatingClientList_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                ProcessTcpServerUpdateClientList();
            }
            catch (Exception ex) { Logger?.Error(ex); }
        }

        private void BtnTcpServerStartListening_Click(object sender, EventArgs e)
        {
            BtnTcpServerStartListening.Enabled = false;
            try
            {
                if (NudTcpServerListeningPort.Value < 1025 && NudTcpServerListeningPort.Value > 65535)
                {
                    MessageBox.Show("The lower bound is 1025. The upper bound is 65535.", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    BtnTcpServerStartListening.Enabled = true;
                    return;
                }
                lock (Param.TcpServer.IncomingDataQueueLocker)
                {
                    if (Param.TcpServer.IncomingDataQueue == null) { Param.TcpServer.IncomingDataQueue = new Queue<TTcpServerSocket.TcpSocketData>(); }
                }
                if (Param.TcpServer.ServerSocket == null)
                {
                    Param.TcpServer.ServerSocket = new TTcpServerSocket()
                    {
                        ListeningPort = (int)NudTcpServerListeningPort.Value,
                        AcceptInterval = (int)NudTcpServerAcceptInterval.Value,
                        ContainLengthAsHeader = ChkTcpServerContainLengthAsHeader.Checked,
                        EnableAnalyzeIncomingData = true,
                        HeartbeatInterval = ChkTcpServerHeartbeatInterval.Checked ? (int)NudTcpServerHeartbeatInterval.Value : -1,
                        MaxClient = (int)NudTcpServerMaxClient.Value,
                        MaxConnectionDuration = ChkTcpServerMaxConnectionDuration.Checked ? (int)NudTcpServerMaxConnectionDuration.Value : -1,
                        MaxIdleDuration = ChkTcpServerMaxIdleDuration.Checked ? (int)NudTcpServerMaxIdleDuration.Value : -1,
                        ReceiveDataInterval = (int)NudTcpServerReceiveDataInterval.Value,
                        ReceiveTotalBufferSize = (int)NudTcpServerReceiveTotalBufferSize.Value,
                        SleepingIntervalInMS = (int)NudTcpServerSleepingInterval.Value,
                        IncomingDataQueue = Param.TcpServer.IncomingDataQueue,
                        IncomingDataQueueLocker = Param.TcpServer.IncomingDataQueueLocker
                    };
                }
                string s;
                if ((Param.TcpServer.ServerSocket?.StartListening() ?? false) == false)
                {
                    s = "Fail to start TCP Server.";
                    Logger?.Error(s);
                    MessageBox.Show(s, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    BtnTcpServerStartListening.Enabled = true;
                    return;
                }
                BWorkerTcpServerIncomingDataHandler.RunWorkerAsync();
                BWorkerTcpServerUpdatingClientList.RunWorkerAsync();
                s = "Start listening.";
                Logger?.Debug(s);
                WriteLogToUI(s);
                UiConfigForTcpServer(true);
            }
            catch (Exception ex) { Logger?.Error(ex); }
        }

        private void BtnTcpServerStopListening_Click(object sender, EventArgs e)
        {
            BtnTcpServerStopListening.Enabled = false;
            try
            {
                if (Param.TcpServer.ServerSocket != null)
                {
                    Param.TcpServer.ServerSocket.StopListening();
                    Param.TcpServer.ServerSocket = null;
                }
                string s = "Stop listening.";
                Logger?.Debug(s);
                WriteLogToUI(s);
                UiConfigForTcpServer(false);
                //BtnTcpServerStopListening.Enabled = false;
                LblTcpServerRestart.Visible = true;
            }
            catch (Exception ex) { Logger?.Error(ex); }
        }

        private void TcpClientListSetItemChecked(bool isChecked)
        {
            try
            {
                int i = 0;
                while (i < ClbTcpClientList.Items.Count)
                {
                    ClbTcpClientList.SetItemChecked(i, isChecked);
                    i += 1;
                }
            }
            catch (Exception ex) { Logger?.Error(ex); }
        }

        private void ChkTcpServerSelectAllClients_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (ChkTcpServerSelectAllClients.Checked)
                    TcpClientListSetItemChecked(true);
            }
            catch (Exception ex) { Logger?.Error(ex); }
        }

        private void BtnTcpServerDeselectAllClients_Click(object sender, EventArgs e)
        {
            try
            {
                ChkTcpServerSelectAllClients.Checked = false;
                TcpClientListSetItemChecked(false);
            }
            catch (Exception ex) { Logger?.Error(ex); }
        }

        private void TcpServerSend(byte tcpDataType, byte[] data)
        {
            List<byte> tempList = null;
            try
            {
                tempList = new List<byte>()
                {
                    tcpDataType
                };
                tempList.AddRange(data);
                byte[] encryptedData = null;
                if (ChkTcpServerEncryptData.Checked) encryptedData = CyptoRijndaelT.Encrypt(tempList.ToArray(), Param.TcpServer.CryptPassword);
                else encryptedData = tempList.ToArray();
                if (ClbTcpClientList.Items.Count > 0)
                {
                    foreach (object o in ClbTcpClientList.CheckedItems)
                    {
                        string s = (string)o;
                        if (!string.IsNullOrEmpty(s))
                        {
                            int i = s.LastIndexOf(':');
                            if (i > 0)
                            {
                                string host = s.Substring(0, i);
                                string s1 = s.Substring(i + 1);
                                if (int.TryParse(s1, out i))
                                    Param.TcpServer.ServerSocket?.QueueToSendData(host, i, ref encryptedData);
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { Logger?.Error(ex); }
            finally
            {
                if (tempList != null)
                {
                    tempList.Clear();
                    tempList = null;
                }
            }
        }

        private void BtnTcpServerSendText_Click(object sender, EventArgs e)
        {
            try
            {
                if (Param.TcpServer.ServerSocket == null)
                {
                    MessageBox.Show("Server socket is empty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                BtnTcpServerSendText.Enabled = false;
                BtnTcpServerSendFile.Enabled = false;
                TxtTcpServerInput.ReadOnly = true;
                Logger?.Debug("Send text: {0}", TxtTcpServerInput.Text);
                WriteLogToUI("Send text: {0}", TxtTcpServerInput.Text);
                TcpServerSend(Param.TcpDataType.Text, Encoding.UTF8.GetBytes(TxtTcpServerInput.Text));
            }
            catch (Exception ex) { Logger?.Error(ex); }
            finally
            {
                BtnTcpServerSendText.Enabled = true;
                BtnTcpServerSendFile.Enabled = true;
                TxtTcpServerInput.Text = "";
                TxtTcpServerInput.ReadOnly = false;
                TxtTcpServerInput.Focus();
            }
        }

        private void TxtTcpServerInput_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) BtnTcpServerSendText_Click(null, null);
        }

        private void BtnTcpServerSendFile_Click(object sender, EventArgs e)
        {
            try
            {
                if (Param.TcpServer.ServerSocket == null)
                {
                    MessageBox.Show("Server socket is empty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                BtnTcpServerSendText.Enabled = false;
                BtnTcpServerSendFile.Enabled = false;
                TxtTcpServerInput.ReadOnly = true;
                OpenFileDialog oDialog = new OpenFileDialog()
                {
                    InitialDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location),
                    Title = "Open File",
                    Filter = "All files (*.*)|*.*",
                    CheckFileExists = true,
                    CheckPathExists = true,
                    Multiselect = false
                };
                if (oDialog.ShowDialog() == DialogResult.OK)
                {
                    //MessageBox.Show(oDialog.FileName);
                    Logger?.Debug("Send file: {0}", oDialog.FileName);
                    WriteLogToUI("Send file: {0}", oDialog.FileName);
                    byte[] data = System.IO.File.ReadAllBytes(oDialog.FileName);
                    TcpServerSend(Param.TcpDataType.File, data);
                }
            }
            catch (Exception ex) { Logger?.Error(ex); }
            finally
            {
                BtnTcpServerSendText.Enabled = true;
                BtnTcpServerSendFile.Enabled = true;
                TxtTcpServerInput.ReadOnly = false;
            }
        }
        #endregion

        #region SQLiteRegion
        private static string PrintStudent(Models.Student o)
        {
            if (o == null) return "";
            return string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}", o.StudentId, o.UniqueName, o.DisplayName, o.Phone, o.Email, o.Gender, o.EnrollmentFee, o.IsNewlyEnrolled, o.Birthday, o.CreatedDate, o.UpdatedDate);
        }

        private void BtnSqliteInsert1_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime tRef = DateTime.Now;
                //Models.Student o = new Models.Student()
                //{
                //    UniqueName = "apple.chan",
                //    DisplayName = "Apple Chan",
                //    Phone = "11111111",
                //    Email = "apple.chan@abc.com",
                //    Gender = 'F',
                //    CreatedDate = tRef,
                //    UpdatedDate = tRef
                //};
                //Models.Student o = new Models.Student()
                //{
                //    UniqueName = "orange.lee",
                //    DisplayName = "Orange Lee",
                //    Phone = "22222222",
                //    Email = "orange.lee@abc.com",
                //    Gender = 'M',
                //    EnrollmentFee = 100,
                //    IsNewlyEnrolled = false,
                //    CreatedDate = tRef,
                //    UpdatedDate = tRef
                //};
                Models.Student o = new Models.Student()
                {
                    UniqueName = "pear.ho",
                    DisplayName = "Pear Ho",
                    Phone = "33333333",
                    Email = "pear.ho@abc.com",
                    Gender = 'M',
                    EnrollmentFee = 130,
                    IsNewlyEnrolled = false,
                    CreatedDate = tRef,
                    UpdatedDate = tRef
                };
                LocalLogger(TLog.LogLevel.DEBUG, PrintStudent(o));
                ////int i = DbHelper.InsertStudent(o);
                ////WriteLogToUI("i = {0}", i);
                //DbHelper.InsertStudent2(o);
                //LocalLogger(TLog.LogLevel.DEBUG, "Succeed to add student.");
            }
            catch (Exception ex) { Logger?.Error(ex); }
        }

        private void BtnSqliteSelect1_Click(object sender, EventArgs e)
        {
            List<Models.Student> list = null;
            IQueryable<Models.Student> list2 = null;
            try
            {
                ////list = DbHelper.GetStudentList();
                ////LocalLogger(TLog.LogLevel.DEBUG, "Student Count = {0}", list?.Count ?? 0);
                ////if (list != null)
                ////{
                ////    foreach (Student o in list)
                ////    {
                ////        LocalLogger(TLog.LogLevel.DEBUG, PrintStudent(o));
                ////    }
                ////}
                //DbHelper.InitializeSqliteDb();
                //list2 = DbHelper.GetStudentList2();
                LocalLogger(TLog.LogLevel.DEBUG, "Student Count = {0}", list2?.Count() ?? 0);
                if (list2 != null)
                {
                    foreach (Student o in list2)
                    {
                        LocalLogger(TLog.LogLevel.DEBUG, PrintStudent(o));
                    }
                }
            }
            catch (Exception ex) { Logger?.Error(ex); }
            finally
            {
                if (list != null)
                {
                    list.Clear();
                    list = null;
                }
            }
        }
        #endregion

        #region ChangeFilenameByModifiedDateTime
        private void ChangeFilenameByModifiedDateTimeCore(string filepath)
        {
            try
            {
                if (!System.IO.File.Exists(filepath))
                {
                    LocalLogger(TLog.LogLevel.WARN, "Cannot find file {0}", filepath);
                    return;
                }
                DateTime dt = System.IO.File.GetLastWriteTime(filepath);
                string changedFilenameNoExt = string.Format("{0:yyyyMMdd_HHmmss}", dt);
                string changedFilepath = System.IO.Path.GetDirectoryName(filepath) + System.IO.Path.DirectorySeparatorChar + changedFilenameNoExt + System.IO.Path.GetExtension(filepath);
                LocalLogger(TLog.LogLevel.INFO, "File: {0}, modified date time: {1:yyyy-MM-dd HH:mm:ss}, changed file path = {2}", filepath, dt, changedFilepath);
                System.IO.File.Move(filepath, changedFilepath);
            }
            catch (Exception ex) { Logger?.Error(ex); }
        }

        private void ChangeFilenameByModifiedDateTimeRoutine(string folderpath)
        {
            if (string.IsNullOrEmpty(folderpath)) return;
            if (!System.IO.Directory.Exists(folderpath))
            {
                LocalLogger(TLog.LogLevel.INFO, "Cannot find folder {0}", folderpath);
                return;
            }
            string[] filepaths = System.IO.Directory.GetFiles(folderpath);
            if ((filepaths?.Length ?? 0) > 0)
            {
                foreach (string f in filepaths)
                {
                    ChangeFilenameByModifiedDateTimeCore(f);
                }
            }
        }

        private void BtnChangeFilenameByDateTime_Click(object sender, EventArgs e)
        {
            try
            {
                FolderBrowserDialog dialog = new FolderBrowserDialog()
                {
                    SelectedPath = @"D:\temp",
                    ShowNewFolderButton = false
                };
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string folderpath = dialog.SelectedPath;
                    LocalLogger(TLog.LogLevel.INFO, "Selected path = {0}", folderpath);
                    ChangeFilenameByModifiedDateTimeRoutine(folderpath);
                }
            }
            catch (Exception ex) { Logger?.Error(ex); }
        }
        #endregion

        #region SingletonRegion
        private void BtnSingleton_Click(object sender, EventArgs e)
        {
            BtnSingleton.Enabled = false;
            try
            {
                Singleton s1 = Singleton.GetInstance();
                Singleton s2 = Singleton.GetInstance();
                Singleton s3 = Singleton.GetInstance();
                s1.Value1 = 100;
                WriteLogToUI("s1.Value1 = {0}", s1.Value1);
                WriteLogToUI("s2.Value1 = {0}", s2.Value1);
                WriteLogToUI("s3.Value1 = {0}", s3.Value1);
            }
            catch (Exception ex) { LocalLogger(TLog.LogLevel.ERROR, ex.ToString()); }
            finally { BtnSingleton.Enabled = true; }
        }
        #endregion

        #region ZipRegion
        private void BtnZip_Click(object sender, EventArgs e)
        {
            BtnZip.Enabled = false;
            try
            {
                ///// To use the ZipFile class, you must reference the System.IO.Compression.FileSystem assembly in your project.
                //string sourcePath = GeneralT.GetDefaultAbsolutePathIfRelative("log");
                //string zipPath = GeneralT.GetDefaultAbsolutePathIfRelative("a1.zip");
                //System.IO.Compression.ZipFile.CreateFromDirectory(sourcePath, zipPath);
                string password = "abc123";
                string zipFilepath = GeneralT.GetDefaultAbsolutePathIfRelative("a1.zip");
                string[] sourcePaths = new string[]
                {
                    GeneralT.GetDefaultAbsolutePathIfRelative("log"),
                    GeneralT.GetDefaultAbsolutePathIfRelative("VsCSharpWinForm_sample2.ini"),
                    GeneralT.GetDefaultAbsolutePathIfRelative(@"x86\SQLite.Interop.dll"),
                    GeneralT.GetDefaultAbsolutePathIfRelative("x64")
                };
                LocalLogger(TLog.LogLevel.DEBUG, "Source Paths:{0}{1}", Environment.NewLine, string.Join(Environment.NewLine, sourcePaths));
                if (ZipHelper.DotNet.Zip(zipFilepath, password, sourcePaths))
                    LocalLogger(TLog.LogLevel.DEBUG, "Succeed to zip items.");
                else
                {
                    LocalLogger(TLog.LogLevel.DEBUG, "Fail to zip items.");
                    return;
                }
                string extractedFolder = GeneralT.GetDefaultAbsolutePathIfRelative("extract");
                if (ZipHelper.DotNet.Unzip(zipFilepath, password, extractedFolder))
                    LocalLogger(TLog.LogLevel.DEBUG, "Succeed to unzip items.");
                else
                {
                    LocalLogger(TLog.LogLevel.DEBUG, "Fail to unzip items.");
                }
                /// Seven Zip.
                LocalLogger(TLog.LogLevel.DEBUG, "7 zip");
                LocalLogger(TLog.LogLevel.DEBUG, "Source Paths:{0}{1}", Environment.NewLine, string.Join(Environment.NewLine, sourcePaths));
                zipFilepath = GeneralT.GetDefaultAbsolutePathIfRelative("a2.zip");
                if (ZipHelper.SevenZip.Zip(zipFilepath, password, out string output, out string error, sourcePaths))
                    LocalLogger(TLog.LogLevel.DEBUG, "Succeed to zip items.");
                else
                {
                    LocalLogger(TLog.LogLevel.DEBUG, "Fail to zip items.");
                    return;
                }
                extractedFolder = GeneralT.GetDefaultAbsolutePathIfRelative("extract2");
                if (ZipHelper.SevenZip.Unzip(zipFilepath, password, extractedFolder, out output, out error))
                    LocalLogger(TLog.LogLevel.DEBUG, "Succeed to unzip items.");
                else
                {
                    LocalLogger(TLog.LogLevel.DEBUG, "Fail to unzip items.");
                }
            }
            catch (Exception ex) { LocalLogger(TLog.LogLevel.ERROR, ex.ToString()); }
            finally { BtnZip.Enabled = true; }
        }
        #endregion

        #region JsonRegion
        private void BtnJson_Click(object sender, EventArgs e)
        {
            /// Need to install Newtonsoft.Json in the NuGet Package Manager.
            /// https://www.c-sharpcorner.com/article/working-with-json-in-C-Sharp/
            /// https://www.newtonsoft.com/json
            try
            {
                List<Student> list = new List<Student>()
                {
                    new Student()
                    {
                        StudentId = 1,
                        UniqueName = "apple.chan",
                        DisplayName = "Apple Chan",
                        Phone = "11111111",
                        Email = "apple.chan@abc.com",
                        Gender = 'F',
                        EnrollmentFee = 100,
                        IsNewlyEnrolled = false,
                        Birthday = new DateTime(1990, 2, 16)
                    },
                    new Student()
                    {
                        StudentId = 2,
                        UniqueName = "orange.lee",
                        DisplayName = "Orange Lee",
                        Phone = "22222222",
                        Email = "orange.lee@abc.com",
                        Gender = 'M',
                        EnrollmentFee = 120,
                        IsNewlyEnrolled = true,
                        Birthday = new DateTime(1989, 12, 25)
                    },
                    new Student()
                    {
                        StudentId = 3,
                        UniqueName = "mango.wong",
                        DisplayName = "Mango, \"Ann\" Wong",
                        Phone = "33333333",
                        Email = "mango.wong@abc.com",
                        Gender = 'F',
                        EnrollmentFee = 250
                    }
                };
                LocalLogger(TLog.LogLevel.DEBUG, "Convert to JSON.");
                string s = "";
                //s = Newtonsoft.Json.JsonConvert.SerializeObject(list);/// Need to install Newtonsoft.Json in the NuGet Package Manager.
                LocalLogger(TLog.LogLevel.DEBUG, s);
                LocalLogger(TLog.LogLevel.DEBUG, "Convert from JSON to object.");
                List<Student> result = null;
                //result = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Student>>(s);/// Need to install Newtonsoft.Json in the NuGet Package Manager.
                if (result != null)
                {
                    foreach (Student o in result)
                    {
                        if (o != null)
                        {
                            LocalLogger(TLog.LogLevel.DEBUG, "{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}", o.StudentId, o.UniqueName, o.DisplayName, o.Phone, o.Email, o.Gender, o.EnrollmentFee, o.IsNewlyEnrolled, o.Birthday);
                        }
                    }
                }
            }
            catch (Exception ex) { LocalLogger(TLog.LogLevel.ERROR, ex.ToString()); }
        }
        #endregion

        #region CsvRegion
        private void CsvHandlingMethod1(string[] array)
        {
            try
            {
                if ((array?.Length ?? 0) < 1) return;
                LocalLogger(TLog.LogLevel.DEBUG, "----------Record");
                int idx = 0;
                foreach (string s in array)
                {
                    LocalLogger(TLog.LogLevel.DEBUG, "Field {0} = -->{1}<--", idx, s);
                    idx++;
                }
            }
            catch (Exception ex)
            {
                Logger?.Error("Array = {0}", array);
                Logger?.Error(ex);
            }
        }

        private void BtnReadCsvFile_Click(object sender, EventArgs e)
        {
            BtnReadCsvFile.Enabled = false;
            try
            {
                /// Read.
                string path = GeneralT.GetDefaultAbsolutePathIfRelative("testCsvFile.csv");
                if (!System.IO.File.Exists(path))
                {
                    LocalLogger(TLog.LogLevel.ERROR, "Cannot find file {0}", path);
                    return;
                }
                ///// https://joshclose.github.io/CsvHelper/getting-started
                //using (System.IO.StreamReader sr = new System.IO.StreamReader(path))
                //{
                //    using (CsvHelper.CsvReader csv = new CsvHelper.CsvReader(sr, System.Globalization.CultureInfo.InvariantCulture))
                //    {
                //        csv.Configuration.MissingFieldFound = null;
                //        IEnumerable<Student> records = csv.GetRecords<Student>();
                //        if ((records?.Count() ?? 0) > 0)
                //        {
                //            foreach (Student o in records)
                //            {
                //                if (o != null)
                //                {
                //                    LocalLogger(TLog.LogLevel.INFO, "{0}, {1}, {2}, {3}, {4}, {5}, {6}", o.StudentId, o.UniqueName, o.DisplayName, o.Phone, o.EnrollmentFee, o.IsNewlyEnrolled, o.Birthday);
                //                }
                //            }
                //        }
                //    }
                //}

                //List<List<string>> list = TCsvFile.GetListOfStringList(path);
                //LocalLogger(TLog.LogLevel.DEBUG, "Number of records = {0}", list?.Count);
                //if ((list?.Count ?? 0) > 0)
                //{
                //    foreach (List<string> r in list)
                //    {
                //        if ((r?.Count ?? 0) > 0)
                //        {
                //            LocalLogger(TLog.LogLevel.DEBUG, "----------Record:");
                //            foreach (string s in r)
                //            {
                //                LocalLogger(TLog.LogLevel.DEBUG, "Field = -->{0}<--", s);
                //            }
                //        }
                //    }
                //}

                ///// Example 1.
                //int recordCount = TCsvFile.ReadFileWithImportedAction(path, CsvHandlingMethod1);
                //LocalLogger(TLog.LogLevel.DEBUG, "Number of records = {0}", recordCount);
                /// Example 2.
                string[][] arrayOfStringArray = TCsvFile.ReadFileAndGetArrayOfStringArray(path);
                LocalLogger(TLog.LogLevel.DEBUG, "Number of records = {0}", arrayOfStringArray?.Length);
                if ((arrayOfStringArray?.Length ?? 0) > 0)
                {
                    foreach (string[] a2 in arrayOfStringArray)
                    {
                        if ((a2?.Length ?? 0) > 0)
                        {
                            LocalLogger(TLog.LogLevel.DEBUG, "----------Record:");
                            foreach (string s in a2)
                            {
                                LocalLogger(TLog.LogLevel.DEBUG, "Field = -->{0}<--", s);
                            }
                        }
                    }
                }

                //List<Student> students = TCsvFile.Read1<Student>(path);

                /// Write.
                path = GeneralT.GetDefaultAbsolutePathIfRelative("testCsvFile1.csv");
                if (System.IO.File.Exists(path))
                {
                    LocalLogger(TLog.LogLevel.DEBUG, "Delete file {0}", path);
                    System.IO.File.Delete(path);
                }
                /// Example 1.
                //using (System.IO.StreamWriter sw = new System.IO.StreamWriter(path))
                //{
                //    int recordCount1 = arrayOfStringArray?.Length ?? 0;
                //    if (recordCount1 > 0)
                //    {
                //        int i = 0;
                //        while (i < recordCount1)
                //        {
                //            if (TCsvFile.WriteStringArrayToStreamWriter(sw, arrayOfStringArray[i], false))
                //            { LocalLogger(TLog.LogLevel.DEBUG, "Succeed to write the record {0}", i); }
                //            else { LocalLogger(TLog.LogLevel.DEBUG, "Fail to write the record {0}", i); }
                //            i++;
                //        }
                //    }
                //    sw.Flush();
                //}
                /// Example 2.
                int recordCount2 = arrayOfStringArray?.Length ?? 0;
                if (recordCount2 > 0)
                {
                    int i = 0;
                    while (i < recordCount2)
                    {
                        if (TCsvFile.WriteToFile(path, arrayOfStringArray[i], false))
                            LocalLogger(TLog.LogLevel.DEBUG, "Succeed to write the record {0}", i);
                        else LocalLogger(TLog.LogLevel.DEBUG, "Fail to write the record {0}", i);
                        i++;
                    }
                }
            }
            catch (Exception ex) { LocalLogger(TLog.LogLevel.ERROR, ex.ToString()); }
            finally { BtnReadCsvFile.Enabled = true; }
        }
        #endregion

        #region XmlRegion
        private void XmlRoutine2(System.Xml.XmlWriter writer, int aneNum, decimal? spe, decimal? ma, int? dire, int? qty, string sta)
        {
            try
            {
                if (writer == null) return;
                writer.WriteStartElement("Rec");

                string NumberFormat = "0.00";

                //writer.WriteStartElement("Dev");
                //writer.WriteString("ANE/" + aneNum.ToString("00") + "/A");
                //writer.WriteEndElement();
                writer.WriteElementString("Dev", "ANE/" + aneNum.ToString("00") + "/A");
                //writer.WriteElementString("Spe", spe.GetValueOrDefault().ToString("0.00"));
                writer.WriteElementString("Spe", string.Format("{0:" + NumberFormat + "}", spe));
                //writer.WriteElementString("Max", ma.GetValueOrDefault().ToString("0.00"));
                writer.WriteElementString("Max", ma == null ? null : ma.GetValueOrDefault().ToString(NumberFormat));
                writer.WriteElementString("Dir", dire?.ToString(NumberFormat));
                writer.WriteElementString("Qty", qty?.ToString());
                writer.WriteElementString("Sta", sta);

                writer.WriteEndElement();
            }
            catch (Exception ex) { LocalLogger(TLog.LogLevel.ERROR, ex.ToString()); }
        }

        private void XmlRoutine1(System.Xml.XmlWriter writer, int per)
        {
            try
            {
                if (writer == null) return;
                writer.WriteStartElement("RSet");
                writer.WriteAttributeString("per", per.ToString());

                XmlRoutine2(writer, 1, 2.8m, 2.87m, 134, 211, "Normal");
                XmlRoutine2(writer, 2, 2.98m, 3.05m, 144, 213, "Normal");
                XmlRoutine2(writer, 3, null, null, null, null, "Normal");

                writer.WriteEndElement();
            }
            catch (Exception ex) { LocalLogger(TLog.LogLevel.ERROR, ex.ToString()); }
        }

        private void BtnXml_Click(object sender, EventArgs e)
        {
            BtnXml.Enabled = false;
            try
            {
                string path = GeneralT.GetDefaultAbsolutePathIfRelative("abc.xml");
                if (System.IO.File.Exists(path))
                {
                    LocalLogger(TLog.LogLevel.DEBUG, "Delete file {0}", path);
                    System.IO.File.Delete(path);
                }
                using (System.Xml.XmlWriter writer = System.Xml.XmlWriter.Create(path))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("Ex");
                    writer.WriteAttributeString("name", "WeData");

                    XmlRoutine1(writer, 1);

                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
                LocalLogger(TLog.LogLevel.DEBUG, "Finish to generate XML file {0}", path);
            }
            catch (Exception ex) { LocalLogger(TLog.LogLevel.ERROR, ex.ToString()); }
            finally { BtnXml.Enabled = true; }
        }
        #endregion

        #region ExcelRegion
        private void BtnExcel_Click(object sender, EventArgs e)
        {
            BtnExcel.Enabled = false;
            try
            {
                ///// DLL method.
                //string path = GeneralT.GetDefaultAbsolutePathIfRelative("abc.xls");
                //if (System.IO.File.Exists(path))
                //{
                //    LocalLogger(TLog.LogLevel.DEBUG, "Delete file {0}", path);
                //    System.IO.File.Delete(path);
                //}
                //else
                //{
                //    string folder = System.IO.Path.GetDirectoryName(path);
                //    if (!System.IO.Directory.Exists(folder))
                //    {
                //        LocalLogger(TLog.LogLevel.DEBUG, "Create folder {0}", folder);
                //        System.IO.Directory.CreateDirectory(folder);
                //    }
                //}
                //if (ExcelHelper.Dll.ExportFile(path)) { LocalLogger(TLog.LogLevel.DEBUG, "Succeed to export Excel file {0}", path); }
                //else { LocalLogger(TLog.LogLevel.DEBUG, "Fail to export Excel file {0}", path); }
                ///// OpenXml method.
                //string path2 = GeneralT.GetDefaultAbsolutePathIfRelative("abc.xlsx");
                //if (System.IO.File.Exists(path2))
                //{
                //    LocalLogger(TLog.LogLevel.DEBUG, "Delete file {0}", path2);
                //    System.IO.File.Delete(path2);
                //}
                //else
                //{
                //    string folder = System.IO.Path.GetDirectoryName(path2);
                //    if (!System.IO.Directory.Exists(folder))
                //    {
                //        LocalLogger(TLog.LogLevel.DEBUG, "Create folder {0}", folder);
                //        System.IO.Directory.CreateDirectory(folder);
                //    }
                //}
                //if (ExcelHelper.OpenXml.ExportFile(path2)) { LocalLogger(TLog.LogLevel.DEBUG, "Succeed to export Excel file {0}", path2); }
                //else { LocalLogger(TLog.LogLevel.DEBUG, "Fail to export Excel file {0}", path2); }

                //string path = GeneralT.GetDefaultAbsolutePathIfRelative("QM_0173_HRSM.xlsx");
                //string s = ExcelHelper.ClosedXML.DeleteRows(path, "QMData");
                //LocalLogger(TLog.LogLevel.DEBUG, "Result = {0}", s);

                //string path = GeneralT.GetDefaultAbsolutePathIfRelative("QM_0173_HRSM.xlsx");
                //string s = ExcelHelper.EPPlus.DeleteRows(path, "QMData");
                //LocalLogger(TLog.LogLevel.DEBUG, "Result = {0}", s);

                //string path = GeneralT.GetDefaultAbsolutePathIfRelative("QM_0173_HRSM.xlsx");
                //string s = ExcelHelper.EPPlusCore.DeleteRows(path, "QMData");
                //LocalLogger(TLog.LogLevel.DEBUG, "Result = {0}", s);
            }
            catch (Exception ex) { Logger?.Error(ex); }
            finally { BtnExcel.Enabled = true; }
        }
        #endregion

        private void BtnMail_Click(object sender, EventArgs e)
        {
            BtnMail.Enabled = false;
            try
            {
                /// https://stackoverflow.com/questions/32260/sending-email-in-net-through-gmail
                MailHelper.Send(new MailItem()
                {
                    From = "thomastse2001@gmail.com",
                    To = new string[] { "thomas_tse2001@hotmail.com" },
                    Subject = "Test3",
                    Body = "Testing mail 3.",
                    SmtpHost = "smtp.gmail.com",
                    SmtpUserName = "thomastse2001@gmail.com",
                    SmtpPassword = "password"
                });
                LocalLogger(TLog.LogLevel.DEBUG, "Finish to send mail.");
            }
            catch (Exception ex) { LocalLogger(TLog.LogLevel.ERROR, ex.ToString()); }
            finally { BtnMail.Enabled = true; }
        }

        /// Get the n repeated <br/> or new-line.
        private static string GetLineSeparatorString(bool isHtml, int n)
        {
            if (n < 1) return null;
            if (n == 1) return isHtml ? "<br/>" : Environment.NewLine;
            return isHtml ?
                string.Concat(Enumerable.Repeat("<br/>", n)) :
                string.Concat(Enumerable.Repeat(Environment.NewLine, n));
        }

        private void BtnLineSeparator_Click(object sender, EventArgs e)
        {
            BtnLineSeparator.Enabled = false;
            try
            {
                int n = 2;
                string s = string.Format("n = {0}{1}", n, Environment.NewLine);
                s += GetLineSeparatorString(false, n);
                LocalLogger(TLog.LogLevel.DEBUG, s + "---");
            }
            catch (Exception ex) { LocalLogger(TLog.LogLevel.ERROR, ex.ToString()); }
            finally { BtnLineSeparator.Enabled = true; }
        }
    }
}
