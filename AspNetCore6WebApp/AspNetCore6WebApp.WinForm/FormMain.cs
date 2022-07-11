using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Microsoft.Extensions.Configuration;
using AspNetCore6WebApp.Entities;
using AspNetCore6WebApp.Business;

namespace AspNetCore6WebApp.WinForm
{
    public partial class FormMain : Form
    {
        private readonly TT.Logging Logger;

        private bool IsExit = false;/// flag to indicate if this application will exit. Set it to true, in order to release all memory and stop the threads.
        private string ExeFileNameWithoutExt = String.Empty;
        //private bool IsLockFileCreatedNormally = false;/// Whether the lock file is created normally. The default value is false.
        private bool IsLoginSuccess = false;/// Whether login successful. The default value is false.
        //private bool IsWarnIfFormClosing = true;/// Whether show a warning dialog box when form is closing. The default value is true.
        private bool IsRenameFilenamesEnd = false;/// Whether the process of renaming filenames is end. The default value is false

        private string _username = string.Empty;
        private string _password = string.Empty;
        private readonly Forms.FormWait _formWait = new();

        private int TcpClientLastId = 0;
        private readonly List<Forms.FormTcpClient> TcpClientList = new();
        private readonly object TcpClientListLocker = new();

        private TT.TcpSocket.Server? ServerSocket = null;
        private readonly Queue<TT.TcpSocket.DataPackage> IncomingDataQueue = new();
        private readonly object IncomingDataQueueLocker = new();

        public FormMain(TT.Logging tLogger)
        {
            this.Logger = tLogger;
            InitializeComponent();
        }

        private void LogToUi(string format, params object?[] args)
        {
            try
            {
                Invoke(new MethodInvoker(delegate
                {
                    try
                    {
                        int iMax = 100000;
                        TxtLog.AppendText((args == null || args.Length < 1) ? format : String.Format(format, args));
                        TxtLog.AppendText(Environment.NewLine);
                        if (TxtLog.TextLength > iMax) TxtLog.Text = TxtLog.Text[^iMax..];
                        TxtLog.SelectionStart = TxtLog.TextLength;
                        TxtLog.ScrollToCaret();
                    }
                    catch (Exception ex2)
                    {
                        try { Logger?.Error(ex2); }
                        catch (Exception ex3) { Console.WriteLine("[error] {0}.{1}. {2}", System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.Name, System.Reflection.MethodBase.GetCurrentMethod()?.Name, ex3.Message); }
                    }
                }));
            }
            catch (Exception ex)
            {
                try { Logger.Error(ex); }
                catch (Exception ex4) { Console.WriteLine("[error] {0}", ex4.ToString()); }
            }
        }

        private void LocalLogger(TT.Logging.LogLevel logLevel, string format, params object?[] args)
        {
            Logger.Log(logLevel, format, args);
            LogToUi(format, args);
        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                IsExit = true;/// set to exit

                TcpClientCloseAll();
                lock (TcpClientListLocker)
                {
                    if (TcpClientList != null) TcpClientList.Clear();
                }
                if (ServerSocket != null)
                {
                    ServerSocket.StopListening();
                    ServerSocket = null;
                }
                lock (IncomingDataQueueLocker)
                {
                    if (IncomingDataQueue != null) IncomingDataQueue.Clear();
                }

                //System.Threading.Thread.Sleep(500);
                new System.Threading.ManualResetEvent(false).WaitOne(500);

                //if (IsLockFileCreatedNormally) { }
                if (IsLoginSuccess) Logger.Debug("{0} ends. Version = {1}", ExeFileNameWithoutExt, Application.ProductVersion);
            }
            catch (Exception ex) { Logger?.Error(ex); }
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (IsLoginSuccess)
            {
                /// Before exit, show a dialog box to ask whether really want to exit
                /// http://msdn.microsoft.com/en-us/library/system.windows.forms.form.closing(v=vs.110).aspx
                /// Cancel the Closing event from closing the form
                if (MessageBox.Show("Do you really want to exit this application ?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.No)
                {
                    e.Cancel = true;
                }
            }
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            try
            {
                var a = System.Diagnostics.Process.GetCurrentProcess().MainModule;
                if (a != null)
                {
                    ExeFileNameWithoutExt = System.IO.Path.GetFileNameWithoutExtension(a.FileName) ?? string.Empty;
                }

                //_tLogger.Debug("hello");

                string? exeFolder = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly()?.Location);
                if (!string.IsNullOrEmpty(exeFolder)) System.IO.Directory.SetCurrentDirectory(exeFolder);

                /// Show the help message if run in the command line with proper switch
                string[] args = System.Environment.GetCommandLineArgs();
                if (TT.GeneralHelper.GetArguments(args, "-?") || TT.GeneralHelper.GetArguments(args, "/?"))
                {
                    StringBuilder sb = new();
                    sb.AppendLine("Syntax:");
                    sb.Append(ExeFileNameWithoutExt).AppendLine(" -u[Username] -p[Password]").AppendLine();
                    sb.AppendLine("Example:");
                    sb.Append(ExeFileNameWithoutExt).AppendLine(" -uThomas -pabc");
                    Console.WriteLine(sb.ToString());
                    MessageBox.Show(sb.ToString(), "Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                    return;
                }

                /// Version Information. Show the version in the GUI
                /// Assume the product version (File version) is same as Assembly version
                /// To update the version, in the menu bar, click "Project > [Project Name] Properties > Application > Assembly Information...", change the Assembly version and File version.
                LblVersion.Text = "Version: " + Application.ProductVersion;
                LblVersion.Top = 9;
                LblVersion.Left = this.Width - LblVersion.Width - 24;

                TT.GeneralHelper.GetArguments(args, "-u", out _username);
                TT.GeneralHelper.GetArguments(args, "-p", out _password);
                if (!string.IsNullOrEmpty(_username))
                {
                    _formWait.IsEndWaiting = false;
                    /// Authentication
                    BWorkerLogin.RunWorkerAsync();
                    _formWait.ShowDialog();
                    /// Exit if users click the EXIT button in the Waiting dialog
                    if (_formWait.IsExit)
                    {
                        IsLoginSuccess = false;
                        this.Close();
                        return;
                    }
                }

                /// If login fails by arguments, then prompt the login dialog
                if (!IsLoginSuccess)
                {
                    Forms.FormLogin frmLogin = new()
                    {
                        VersionString = LblVersion.Text,
                        Text = ExeFileNameWithoutExt + " - Login",
                        Username = _username,
                        Password = string.Empty,
                        Alert = string.Empty
                    };
                    int i = 0;
                    while (IsLoginSuccess == false && (Param.Login.MaxRetry < 1 || i < Param.Login.MaxRetry))
                    {
                        if (frmLogin.ShowDialog() == DialogResult.OK)
                        {
                            _username = frmLogin.Username;
                            _password = frmLogin.Password;
                            _formWait.IsEndWaiting = false;
                            /// Authentication
                            BWorkerLogin.RunWorkerAsync();
                            _formWait.ShowDialog();
                            /// Exit if users click the EXIT button in the Waiting dialog
                            if (_formWait.IsExit)
                            {
                                IsLoginSuccess = false;
                                this.Close();
                                return;
                            }
                        }
                        else
                        {
                            /// Click the "Cancel" button in the Login dialog
                            this.Close();
                            return;
                        }
                        frmLogin.Password = string.Empty;
                        frmLogin.Alert = Param.Login.FailMessage;
                        i++;
                    }
                }
                if (!IsLoginSuccess)
                {
                    Logger.Warn(Param.Login.ExceedMaxRetryMessage);
                    MessageBox.Show(Param.Login.ExceedMaxRetryMessage, "Warn", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    this.Close();
                    return;
                }

                HideMainDialogConfig(ExeFileNameWithoutExt);
                TcpServerSetOtherParameters();
                UiConfigForTcpServer(false);

                /// Main process
                Logger.Debug("{0} starts. Version = {1}", ExeFileNameWithoutExt, Application.ProductVersion);
                Logger.Debug("Current Working Directory = {0}", System.IO.Directory.GetCurrentDirectory());
            }
            catch (Exception ex) { Logger?.Error(ex); }
        }

        #region LoginRegion
        private void BWorkerLogin_DoWork(object sender, DoWorkEventArgs e)
        {
            //System.Threading.Thread.Sleep(1000);
            new System.Threading.ManualResetEvent(false).WaitOne(1000);
            /// for-test
            if (true)
            {
                IsLoginSuccess = "a".Equals(_username) && "a".Equals(_password);
                return;
            }
            /// other login method
        }

        private void BWorkerLogin_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _formWait.IsEndWaiting = true;
        }
        #endregion

        #region HideMainDialogRegion
        private void HideMainDialogConfig(string notifyIconText)
        {
            NotifyIconHide.ContextMenuStrip = ContextMenuStripHide;
            NotifyIconHide.Text = notifyIconText;
            NotifyIconHide.Icon = SystemIcons.Application;// https://stackoverflow.com/questions/16962639/why-isnt-my-notifyicon-showing-up
        }

        private void BtnHideMainDialog_Click(object sender, EventArgs e)
        {
            NotifyIconHide.Visible = true;
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            if (!ContextMenuStripHide.Enabled) ContextMenuStripHide.Enabled = true;
        }

        private void UnhideToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NotifyIconHide.Visible = false;
            this.ShowInTaskbar = true;
            this.WindowState = FormWindowState.Normal;
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        #region TcpClientRegion
        private void BtnNewTcpClient_Click(object sender, EventArgs e)
        {
            try
            {
                Forms.FormTcpClient o = new(TcpClientLastId++)
                {
                    Logger = Logger,
                    VersionString = LblVersion.Text,
                    CryptPassword = Param.DefaultValue.TcpSocket.CryptPassword,
                    ServerHost = Param.DefaultValue.TcpSocket.Client.ServerHost,
                    ServerPort = Param.DefaultValue.TcpSocket.Client.ServerPort,
                    IsEncryptData = Param.DefaultValue.TcpSocket.Client.IsEncryptData
                };
                lock (TcpClientListLocker)
                {
                    TcpClientList.Add(o);
                }
                o.Show();
            }
            catch (Exception ex) { Logger?.Error(ex); }
        }

        private void TcpClientCloseAll()
        {
            lock (TcpClientListLocker)
            {
                if (TcpClientList != null && TcpClientList.Count > 0)
                {
                    foreach (Forms.FormTcpClient o in TcpClientList)
                    {
                        if (o != null) o.Close();
                    }
                }
            }
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
        private void TcpServerIncomingDataHandler(TT.TcpSocket.DataPackage o)
        {
            try
            {
                if (o == null || string.IsNullOrEmpty(o.Host) || o.ByteArray == null) return;
                byte[] decryptedData = ChkTcpServerIsEncryptData.Checked ? (o.ByteArray == null ? Array.Empty<byte>() : TT.CryptoAES.Decrypt(o.ByteArray, Param.DefaultValue.TcpSocket.CryptPassword)) : o.ByteArray;
                /// New method
                TT.TcpSocket.DeserializedData deserializedData = TT.TcpSocket.Serialization.Deserialize(decryptedData);
                if (deserializedData == null)
                {
                    //throw new Exception("Length of decrypted data < 1, which is impossible.");
                    Logger?.Debug("TCP server meets decrypted data with 0 bytes");
                    return;
                }
                string s;
                switch (deserializedData.DataType)
                {
                    case TT.TcpSocket.SerialDataType.Heartbeat:
                        s = string.Format("{0:yyyy-MM-dd HH:mm:ss}. Client = {1}:{2}. Heartbeat", o.Timestamp, o.Host, o.Port);
                        Logger?.Debug("TCP server receices heartbeat. Received Time = {0}", s);
                        LogToUi(s);
                        break;
                    case TT.TcpSocket.SerialDataType.Text:
                        s = string.Format("{0:yyyy-MM-dd HH:mm:ss}. Client = {1}:{2}. Text = {3}", o.Timestamp, o.Host, o.Port, deserializedData.Text);
                        Logger?.Debug("TCP server receices text. Received Time = {0}", s);
                        LogToUi(s);
                        break;
                    case TT.TcpSocket.SerialDataType.File:
                        s = string.Format("{0:yyyy-MM-dd HH:mm:ss}. Client = {1}:{2}. Last index of piece = {3}. Index of current piece = {4}. Piece length = {5}", o.Timestamp, o.Host, o.Port, deserializedData.LastIndexPiece, deserializedData.IndexPiece, deserializedData.FileContent?.Length);
                        Logger?.Debug("TCP server receives a file piece. Received Time = {0}", s);
                        LogToUi(s);
                        if (!string.IsNullOrEmpty(deserializedData.ErrorMessage))
                        {
                            s = string.Format("{0:yyyy-MM-dd HH:mm:ss}. Client = {1}:{2}. Last index of pieces = {3}. Index of current piece = {4}. Error Message = {5}", o.Timestamp, o.Host, o.Port, deserializedData.LastIndexPiece, deserializedData.IndexPiece, deserializedData.ErrorMessage);
                            Logger?.Debug("TCP server receices error message. Received Time = {0}", s);
                            LogToUi(s);
                        }
                        deserializedData.DestFolder = TT.GeneralHelper.GetDefaultAbsolutePathIfRelative(Param.DefaultValue.TcpSocket.Server.IncomingDataFolder);
                        if (string.IsNullOrWhiteSpace(deserializedData.Filename))
                        {
                            s = string.Format("{0:yyyy-MM-dd HH:mm:ss}. Client = {1}:{2}. Last index of pieces = {3}. Index of current piece = {4}. Filename is empty", o.Timestamp, o.Host, o.Port, deserializedData.LastIndexPiece, deserializedData.IndexPiece);
                            Logger?.Debug("TCP server finds empty filename. Received Time = {0)");
                            LogToUi(s);
                            deserializedData.Filename = string.Format(Param.DefaultValue.TcpSocket.Server.IncomingDataFilenameFormat, o.Timestamp, o.Host, o.Port);
                        }
                        s = TT.TcpSocket.Serialization.AppendDeserializedDataToFile(deserializedData);
                        if (string.IsNullOrEmpty(s))
                        {
                            if (deserializedData.IndexPiece == deserializedData.LastIndexPiece)
                            {
                                s = string.Format("{0:yyyy-MM-dd HH:mm:ss}. Client = {1}:{2}. Last index of piece = {3}. Index of current piece = {4}. Output file path = {5}", o.Timestamp, o.Host, o.Port, deserializedData.LastIndexPiece, deserializedData.IndexPiece, deserializedData.DestFilepath);
                                Logger?.Debug("TCP server completes to receive file. Received Time = {0}", s);
                                LogToUi(s);
                            }
                        }
                        else
                        {
                            s = string.Format("{0:yyyy-MM-dd HH:mm:ss}. Client = {1}:{2}. Last index of piece = {3}. Index of current piece = {4}. Output file path = {5}", o.Timestamp, o.Host, o.Port, deserializedData.LastIndexPiece, deserializedData.IndexPiece, deserializedData.DestFilepath);
                            Logger?.Error("TCP server has error when receiving file. Received Time = {0}", s);
                            LogToUi(s);
                        }
                        break;
                    case TT.TcpSocket.SerialDataType.Error:
                        s = string.Format("TCP server finds error when handle incoming data. Data Type = {0}. Message = {1}", deserializedData.DataType, deserializedData.ErrorMessage);
                        Logger?.Error(s);
                        LogToUi(s);
                        break;
                    default:
                        s = string.Format("TCP server finds unknown serial data type when handle incoming data");
                        Logger?.Error(s);
                        LogToUi(s);
                        break;
                }
            }
            catch (Exception ex) { Logger?.Error(ex); }
        }

        private void BWorkerTcpServerIncomingDataHandler_DoWork(object sender, DoWorkEventArgs e)
        {
        }

        private void TcpServerSetOtherParameters()
        {
            string[][] otherParameters = new string[][]
            {
                new string[] { "IsContainLengthAsHeader", "true" },
                new string[] { "IsEnableAnalyzeIncomingData", "true" },
                new string[] { "HeartbeatInterval", "15" },
                new string[] { "AcceptInterval", "1" },
                new string[] { "MaxClient", "20" },
                new string[] { "MaxConnectionDuration", "600" },
                new string[] { "MaxIdleDuration", "60" },
                new string[] { "MaxDataSize", "104857600" },
                new string[] { "ReceiveDataInterval", "0" },
                new string[] { "ReceiveTotalBufferSize", "10485760" },
                new string[] { "SleepingIntervalInMS", "100" }
            };
            foreach (string[] a in otherParameters)
            {
                DgvTcpServerOtherParameters.Rows.Add(a);
            }
        }

        private void UiConfigForTcpServer(bool isListening)
        {
            /// https://stackoverflow.com/questions/418006/how-can-i-disable-a-tab-inside-a-tabcontrol
            foreach (Control ctl in TabPageTcpServerMainParameters.Controls) { ctl.Enabled = !isListening; }
            //foreach (Control ctl in TabPageTcpServerOtherParameters.Controls) { ctl.Enabled = !isListening; }

            BtnTcpServerStartListening.Enabled = TxtTcpServerInput.ReadOnly = !isListening;
            BtnTcpServerStopListening.Enabled = ChkTcpServerSelectAllClients.Enabled = BtnTcpServerDeselectAllClients.Enabled
                = ClbTcpServerClientList.Enabled = BtnTcpServerSendFile.Enabled = BtnTcpServerSendText.Enabled = DgvTcpServerOtherParameters.ReadOnly = isListening;
        }

        private void TcpServerClientListAddItem(string sItem, int index, bool isChecked)
        {
            try
            {
                Invoke(new MethodInvoker(delegate
                {
                    try
                    {
                        if (index < 0) ClbTcpServerClientList.Items.Add(sItem, isChecked);
                        else
                        {
                            if (index > ClbTcpServerClientList.Items.Count) index = ClbTcpServerClientList.Items.Count;
                            ClbTcpServerClientList.Items.Insert(index, sItem);
                            ClbTcpServerClientList.SetItemChecked(index, isChecked);
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
                        ClbTcpServerClientList.Items.Clear();
                    }
                    catch (Exception ex2)
                    {
                        Logger?.Error(ex2);
                    }
                }));
            }
            catch (Exception ex) { Logger?.Error(ex); }
        }

        private void TcpServerClientListRemoveItem(int index)
        {
            try
            {
                if (index < 0) return;
                Invoke(new MethodInvoker(delegate
                {
                    try
                    {
                        ClbTcpServerClientList.Items.RemoveAt(index);
                    }
                    catch (Exception ex2)
                    {
                        Logger?.Error(ex2);
                    }
                }));
            }
            catch (Exception ex) { Logger?.Error(ex); }
        }

        private void TcpServerUpdateClientList()
        {
            try
            {
                List<string>? tempList = ServerSocket?.ClientList()?.ToList();
                if (tempList == null || tempList.Count < 1)
                {
                    TcpServerClientListClear();
                }
                else
                {
                    /// loop the items on checked list box. If outdated, deleted it. Must loop from the last to the first one
                    int i = ClbTcpServerClientList.Items.Count - 1;
                    while (i > -1)
                    {
                        int idx = tempList.FindIndex(x => x.Equals(ClbTcpServerClientList.Items[i].ToString()));
                        if (idx < 0) TcpServerClientListRemoveItem(i);
                        else tempList.RemoveAt(idx);
                        i--;
                    }
                    /// new items
                    i = 0;
                    while (i < tempList.Count)
                    {
                        TcpServerClientListAddItem(tempList[i], -1, ChkTcpServerSelectAllClients.Checked);
                        i++;
                    }
                }
            }
            catch (Exception ex) { Logger?.Error(ex); }
        }

        private void BWorkerTcpServerUpdatingClientList_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                if (IsExit) BWorkerTcpServerUpdatingClientList.CancelAsync();
                BackgroundWorker? worker = sender as BackgroundWorker;
                if (worker == null || worker.CancellationPending)
                {
                    e.Cancel = true;
                    Logger?.Debug("BWorkerTcpServerUpdatingClientList Cancel");
                    return;
                }
                DateTime tNow;
                DateTime tRef = DateTime.Now.AddHours(-1);
                int interval = 1;
                bool b = true;
                Logger?.Debug("Start to update TCP server Client List");
                while (IsExit == false && worker.CancellationPending == false && b)
                {
                    tNow = DateTime.Now;
                    if ((int)(tNow - tRef).TotalSeconds >= interval)
                    {
                        tRef = tNow;
                        TcpServerUpdateClientList();
                        if (ServerSocket == null) b = false;
                    }
                    //System.Threading.Thread.Sleep(200);
                    new System.Threading.ManualResetEvent(false).WaitOne(200);
                }
                Logger?.Debug("Stop to update TCP server Client List");
            }
            catch (Exception ex) { Logger?.Error(ex); }
        }

        private string TcpServerGetStringFromDgv(string key)
        {
            foreach (DataGridViewRow row in DgvTcpServerOtherParameters.Rows)
            {
                if (key.Equals(row?.Cells?[0]?.Value?.ToString()))
                {
                    return row.Cells[1].Value.ToString() ?? string.Empty;
                }
            }
            return string.Empty;
        }

        private bool? TcpServerGetBoolFromDgv(string key)
        {
            if (bool.TryParse(TcpServerGetStringFromDgv(key), out bool b)) return b;
            return null;
        }

        private int? TcpServerGetIntFromDgv(string key)
        {
            if (int.TryParse(TcpServerGetStringFromDgv(key), out int i)) return i;
            return null;
        }

        private void BtnTcpServerStartListening_Click(object sender, EventArgs e)
        {
            BtnTcpServerStartListening.Enabled = false;
            try
            {
                if (NudTcpServerListeningPort.Value < 1025 && NudTcpServerListeningPort.Value > 65535)
                {
                    MessageBox.Show("The lower bound is 1025. The upper bound is 65535", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    BtnTcpServerStartListening.Enabled = true;
                    return;
                }
                if (ServerSocket == null)
                {
                    ServerSocket = new TT.TcpSocket.Server((int)NudTcpServerListeningPort.Value, IncomingDataQueue, IncomingDataQueueLocker, TcpServerIncomingDataHandler)
                    {
                        IsContainLengthAsHeader = TcpServerGetBoolFromDgv("IsContainLengthAsHeader") ?? true,
                        IsEnableAnalyzeIncomingData = TcpServerGetBoolFromDgv("IsEnableAnalyzeIncomingData") ?? true,
                        AcceptInterval = TcpServerGetIntFromDgv("AcceptInterval") ?? 1,
                        HeartbeatInterval = TcpServerGetIntFromDgv("HeartbeatInterval") ?? -1,
                        MaxClient = TcpServerGetIntFromDgv("MaxClient") ?? 20,
                        MaxConnectionDuration = TcpServerGetIntFromDgv("MaxConnectionDuration") ?? 600,
                        MaxIdleDuration = TcpServerGetIntFromDgv("MaxIdleDuration") ?? 60,
                        MaxDataSize = TcpServerGetIntFromDgv("MaxDataSize") ?? 104857600,
                        ReceiveDataInterval = TcpServerGetIntFromDgv("ReceiveDataInterval") ?? 0,
                        ReceiveTotalBufferSize = TcpServerGetIntFromDgv("ReceiveTotalBufferSize") ?? 10485760,
                        SleepingIntervalInMS = TcpServerGetIntFromDgv("SleepingIntervalInMS") ?? 100
                    };
                }
                string s;
                if (!ServerSocket.StartListening())
                {
                    s = "Fail to start TCP Server";
                    Logger?.Error(s);
                    LogToUi(s);
                    MessageBox.Show(s, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    BtnTcpServerStartListening.Enabled = true;
                    return;
                }
                BWorkerTcpServerUpdatingClientList.RunWorkerAsync();
                s = "TCP Server starts listening";
                Logger?.Debug(s);
                LogToUi(s);
                UiConfigForTcpServer(true);
            }
            catch (Exception ex) { Logger?.Error(ex); }
        }

        private void BtnTcpServerStopListening_Click(object sender, EventArgs e)
        {
            BtnTcpServerStopListening.Enabled = false;
            try
            {
                if (ServerSocket != null)
                {
                    ServerSocket.StopListening();
                    ServerSocket = null;
                }
                string s = "TCP Server stops listening";
                Logger?.Debug(s);
                LogToUi(s);
                UiConfigForTcpServer(false);
                LblTcpServerRestart.Visible = true;
            }
            catch (Exception ex) { Logger?.Error(ex); }
        }

        private void TcpClientListSetAllChecked(bool isChecked)
        {
            int i = 0;
            while (i < ClbTcpServerClientList.Items.Count)
            {
                ClbTcpServerClientList.SetItemChecked(i, isChecked);
                i++;
            }
        }

        private void ChkTcpServerSelectAllClients_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (ChkTcpServerSelectAllClients.Checked) TcpClientListSetAllChecked(true);
            }
            catch (Exception ex) { Logger?.Error(ex); }
        }

        private void BtnTcpServerDeselectAllClients_Click(object sender, EventArgs e)
        {
            try
            {
                ChkTcpServerSelectAllClients.Checked = false;
                TcpClientListSetAllChecked(false);
            }
            catch (Exception ex) { Logger?.Error(ex); }
        }

        private void TcpServerSend(byte[] serializedData)
        {
            try
            {
                byte[] encryptedData = ChkTcpServerIsEncryptData.Checked ? TT.CryptoAES.Encrypt(serializedData, Param.DefaultValue.TcpSocket.CryptPassword) : serializedData;
                if (ClbTcpServerClientList.Items.Count > 0)
                {
                    foreach (object o in ClbTcpServerClientList.CheckedItems)
                    {
                        string s = (string)o;
                        if (!string.IsNullOrEmpty(s))
                        {
                            int i = s.LastIndexOf(':');
                            if (i > 0)
                            {
                                string host = s[..i];
                                string s1 = s[(i + 1)..];
                                if (int.TryParse(s1, out i))
                                {
                                    ServerSocket?.QueueToSendData(host, i, ref encryptedData);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { Logger?.Error(ex); }
        }

        private void BtnTcpServerSendText_Click(object sender, EventArgs e)
        {
            try
            {
                if (ServerSocket == null)
                {
                    MessageBox.Show("Server socket is empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                BtnTcpServerSendFile.Enabled = BtnTcpServerSendText.Enabled = false;
                TxtTcpServerInput.ReadOnly = true;
                string s = string.Format("Send text: {0}", TxtTcpServerInput.Text);
                Logger?.Debug(s);
                LogToUi(s);
                TcpServerSend(TT.TcpSocket.Serialization.SerializeText(TxtTcpServerInput.Text));
            }
            catch (Exception ex) { Logger?.Error(ex); }
            finally
            {
                BtnTcpServerSendFile.Enabled = BtnTcpServerSendText.Enabled = true;
                TxtTcpServerInput.Text = string.Empty;
                TxtTcpServerInput.ReadOnly = false;
                TxtTcpServerInput.Focus();
            }
        }

        private void TxtTcpServerInput_KeyUp(object sender, KeyEventArgs e)
        {
            //if (e.KeyCode == Keys.Enter) BtnTcpServerSendText_Click(null, null);
            if (e.KeyCode == Keys.Enter) BtnTcpServerSendText.PerformClick();
        }

        private void TcpServerSendFile(string filepath)
        {
            /// https://stackoverflow.com/questions/2030847/best-way-to-read-a-large-file-into-a-byte-array-in-c
            /// https://stackoverflow.com/questions/2161895/reading-large-text-files-with-streams-in-c-sharp
            try
            {
                int pieceLength = 52428800;//50M bytes.//10485760;// 10M bytes
                long totalLength = (new System.IO.FileInfo(filepath)).Length;
                int lastIndexPiece = (int)Math.Ceiling(1.0m * totalLength / pieceLength) - 1;
                int indexPiece = 0;
                using System.IO.Stream st = System.IO.File.OpenRead(filepath);
                byte[] buffer = new byte[pieceLength];
                int bytesRead;
                while ((bytesRead = st.Read(buffer, 0, buffer.Length)) > 0)
                {
                    byte[] buffer2 = new byte[bytesRead];
                    System.Buffer.BlockCopy(buffer, 0, buffer2, 0, bytesRead);
                    TcpServerSend(TT.TcpSocket.Serialization.SerializeFilePiece(System.IO.Path.GetFileName(filepath), lastIndexPiece, indexPiece, buffer2));
                    indexPiece += 1;
                    //buffer2 = null;
                }
                //buffer = null;
            }
            catch (Exception ex) { Logger?.Error(ex); }
        }

        private void BtnTcpServerSendFile_Click(object sender, EventArgs e)
        {
            try
            {
                if (ServerSocket == null)
                {
                    MessageBox.Show("Server socket is empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                BtnTcpServerSendFile.Enabled = BtnTcpServerSendText.Enabled = false;
                TxtTcpServerInput.ReadOnly = true;
                OpenFileDialog oDialog = new()
                {
                    InitialDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly()?.Location),
                    Title = "Open File",
                    Filter = "All files (*.*)|*.*",
                    CheckFileExists = true,
                    CheckPathExists = true,
                    Multiselect = false
                };
                if (oDialog.ShowDialog() == DialogResult.OK)
                {
                    string s = string.Format("Send file: {0}", oDialog.FileName);
                    Logger?.Debug(s);
                    LogToUi(s);
                    /// New method again
                    TcpServerSendFile(oDialog.FileName);
                }
            }
            catch (Exception ex) { Logger?.Error(ex); }
            finally
            {
                BtnTcpServerSendFile.Enabled = BtnTcpServerSendText.Enabled = true;
                TxtTcpServerInput.ReadOnly = false;
            }
        }
        #endregion

        private void BtnCryptoAES_Click(object sender, EventArgs e)
        {
            try
            {
                string input = TxtFirstInput.Text;
                string password = "abc123abc123abc123";
                string encodingText = TT.CryptoAES.Encrypt(input, password);
                string decodingText = TT.CryptoAES.Decrypt(encodingText, password);
                string s = string.Format("Input = {1}{0}Encoding Text = {2}{0}Decoding Text = {3}", Environment.NewLine, input, encodingText, decodingText);
                Logger?.Debug(s);
                LogToUi(s);
            }
            catch (Exception ex) { Logger?.Error(ex); }
        }

        private void BtnTest1_Click(object sender, EventArgs e)
        {
            try
            {
                TimeSpan ts = DateTime.Now - new DateTime(2022, 5, 20, 0, 0, 0);
                LocalLogger(TT.Logging.LogLevel.DEBUG, "{0:0000000}", ts.Seconds);
            }
            catch (Exception ex) { Logger?.Error(ex); }
        }

        private void BtnPolygonShape2_Click(object sender, EventArgs e)
        {
            using Forms.FormPolygonShape2 frm = new();
            frm.ShowDialog();
        }

        private void BtnTicTacToe_Click(object sender, EventArgs e)
        {
            new Forms.FormTicTacToe() { VersionString = LblVersion.Text }.ShowDialog();
        }
    }
}
