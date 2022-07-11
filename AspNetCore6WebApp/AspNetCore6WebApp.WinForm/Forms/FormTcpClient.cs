using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using AspNetCore6WebApp.Entities;

namespace AspNetCore6WebApp.WinForm.Forms
{
    public partial class FormTcpClient : Form
    {
        private TT.TcpSocket.Client? MyClient;
        private readonly Queue<TT.TcpSocket.DataPackage> MyIncomingDataQueue = new();
        private readonly object MyIncomingDataQueueLocker = new();

        public TT.Logging? Logger { get; set; }
        public bool IsExit = false;
        public readonly int Id = -1;
        public string VersionString { get; set; } = string.Empty;
        public string CryptPassword { get; set; } = string.Empty;
        public string ServerHost { get; set; } = string.Empty;
        public int ServerPort { get; set; }
        public bool IsEncryptData { get; set; }

        public FormTcpClient(int id)
        {
            InitializeComponent();
            Id = id;
        }

        private void SetOtherParameters()
        {
            string[][] otherParameters = new string[][]
            {
                new string[] { "IsContainLengthAsHeader", "True" },
                new string[] { "IsEnableAnalyzeIncomingData", "True" },
                new string[] { "HeartbeatInterval", "15" },
                new string[] { "MaxConnectionDuration", "600" },
                new string[] { "MaxIdleDuration", "60" },
                new string[] { "MaxDataSize", "104857600" },
                new string[] { "ReceiveDataInterval", "0" },
                new string[] { "ReceiveTotalBufferSize", "10485760" },
                new string[] { "SleepingIntervalInMS", "100" }
            };
            foreach (string[] a in otherParameters)
            {
                DgvOtherParameters.Rows.Add(a);
            }
        }

        private void LogToUi(string format, params object?[] args)
        {
            try
            {
                if (IsExit) return;
                Invoke(new MethodInvoker(delegate
                {
                    try
                    {
                        int iMax = 100000;
                        TxtLog.AppendText((args == null || args.Length < 1) ? format : string.Format(format, args).Trim(new char[] { ' ', '\t', (char)10, (char)13 }));
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
                try { Logger?.Error(ex); }
                catch (Exception ex4) { Console.WriteLine("[error] {0}.{1}. {2}", System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.Name, System.Reflection.MethodBase.GetCurrentMethod()?.Name, ex4.Message); }
            }
        }

        private void LocalLoggerDebug(string s)
        {
            Logger?.Debug(s);
            LogToUi(s);
        }

        private void TcpClientIncomingDataHandler(TT.TcpSocket.DataPackage o)
        {
            try
            {
                if (o == null) return;
                string s;
                byte[] decryptedData = ChkIsEncryptData.Checked ? (o.ByteArray == null ? Array.Empty<byte>() : TT.CryptoAES.Decrypt(o.ByteArray, Param.DefaultValue.TcpSocket.CryptPassword)) : o.ByteArray;
                TT.TcpSocket.DeserializedData deserializedData = TT.TcpSocket.Serialization.Deserialize(decryptedData);
                if (deserializedData == null)
                {
                    LocalLoggerDebug(string.Format("FrmTcpClient {0} meets decrypted data with 0 bytes", Id));
                    return;
                }
                switch (deserializedData.DataType)
                {
                    case TT.TcpSocket.SerialDataType.Heartbeat:
                        s = string.Format("{0:yyyy-MM-dd HH:mm:ss}. Server = {1}:{2}. Heartbeat", o.Timestamp, o.Host, o.Port);
                        Logger?.Debug("FrmTcpClient {0} receives heartbeat. Received Time = {1}", Id, s);
                        LogToUi(s);
                        break;
                    case TT.TcpSocket.SerialDataType.Text:
                        s = string.Format("{0:yyyy-MM-dd HH:mm:ss}. Server = {1}:{2}. Text = {3}", o.Timestamp, o.Host, o.Port, deserializedData.Text);
                        Logger?.Debug("FrmTcpClient {0} receives text. Received Time = {1}", Id, s);
                        LogToUi(s);
                        break;
                    case TT.TcpSocket.SerialDataType.File:
                        s = string.Format("{0:yyyy-MM-dd HH:mm:ss}. Server = {1}:{2}. Last index of piece = {3}. Index of current piece = {4}. Piece length = {5}", o.Timestamp, o.Host, o.Port, deserializedData.LastIndexPiece, deserializedData.IndexPiece, deserializedData.FileContent?.Length);
                        Logger?.Debug("FrmTcpClient {0} receives a file piece. Received Time = {1}", Id, s);
                        LogToUi(s);
                        if (!string.IsNullOrEmpty(deserializedData.ErrorMessage))
                        {
                            s = string.Format("{0:yyyy-MM-dd HH:mm:ss}. Server = {1}:{2}. Last index of piece = {3}. Index of current piece = {4}. Error Message = {5}", o.Timestamp, o.Host, o.Port, deserializedData.LastIndexPiece, deserializedData.IndexPiece, deserializedData.ErrorMessage);
                            Logger?.Debug("FrmTcpClient {0} receives error message. Received Time = {1}", Id, s);
                            LogToUi(s);
                        }
                        deserializedData.DestFolder = TT.GeneralHelper.GetDefaultAbsolutePathIfRelative(string.Format(Param.DefaultValue.TcpSocket.Client.IncomingDataFolderFormat, Id));
                        if (string.IsNullOrWhiteSpace(deserializedData.Filename))
                        {
                            s = string.Format("{0:yyyy-MM-dd HH:mm:ss}. Server = {1}:{2}. Last index of piece = {3}. Index of current piece = {4}. Filename is empty", o.Timestamp, o.Host, o.Port, deserializedData.LastIndexPiece, deserializedData.IndexPiece);
                            Logger?.Debug("FrmTcpClient {0} receives file piece. Received Time = {1}", Id, s);
                            LogToUi(s);
                            deserializedData.Filename = string.Format(Param.DefaultValue.TcpSocket.Client.IncomingDataFilenameFormat, o.Timestamp, Id);
                        }
                        s = TT.TcpSocket.Serialization.AppendDeserializedDataToFile(deserializedData);
                        if (string.IsNullOrEmpty(s))
                        {
                            if (deserializedData.IndexPiece == deserializedData.LastIndexPiece)
                            {
                                s = string.Format("{0:yyyy-MM-dd HH:mm:ss}. Server = {1}:{2}. Last index of piece = {3}. Index of current piece = {4}. Output file path = {5}", o.Timestamp, o.Host, o.Port, deserializedData.LastIndexPiece, deserializedData.IndexPiece, deserializedData.DestFilepath);
                                Logger?.Debug("FrmTcpClient {0} completes to receive file. Received Time = {1}", Id, s);
                                LogToUi(s);
                            }
                        }
                        else
                        {
                            s = string.Format("{0:yyyy-MM-dd HH:mm:ss}. Server = {1}:{2}. Last index of piece = {3}. Index of current piece = {4}. Output file path = {5}. Error = {6}", o.Timestamp, o.Host, o.Port, deserializedData.LastIndexPiece, deserializedData.IndexPiece, deserializedData.DestFilepath, s);
                            Logger?.Error("FrmTcpClient {0} has error when receiving file. Received Time = {1}", Id, s);
                            LogToUi(s);
                        }
                        break;
                    case TT.TcpSocket.SerialDataType.Error:
                        s = string.Format("FrmTcpClient {0} finds error when handling incoming data. Data Type = {1}. Message = {2}", Id, deserializedData.DataType, deserializedData.ErrorMessage);
                        Logger?.Error(s);
                        LogToUi(s);
                        break;
                    default:
                        s = string.Format("FrmTcpClient {0} finds unknown serial data type when handling incoming data", Id);
                        Logger?.Error(s);
                        LogToUi(s);
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger?.Error("Id = {0}", Id);
                Logger?.Error(ex);
            }
        }

        private void UiConfig(bool isConnected)
        {
            if (IsExit) return;
            Invoke(new MethodInvoker(delegate
            {
                try
                {
                    foreach (Control ctl in TabPageMainParameters.Controls) { ctl.Enabled = !isConnected; }
                    BtnConnect.Enabled = TxtInput.ReadOnly = !isConnected;
                    BtnDisconnect.Enabled = BtnSendFile.Enabled = BtnSendText.Enabled = DgvOtherParameters.ReadOnly = isConnected;
                    this.Text = string.Format("TcpClient {0}{1}", Id, isConnected ? $" - {MyClient?.LocalEndPoint}" : string.Empty);
                }
                catch (Exception ex2)
                {
                    Logger?.Error("Id = {0}", Id);
                    Logger?.Error(ex2);
                }
            }));
        }

        private void DisconnectRoutine()
        {
            if (MyClient != null)
            {
                MyClient.StopConnection();
                MyClient = null;
            }
            UiConfig(false);
        }

        private void FormTcpClient_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                IsExit = true;
                Logger?.Debug("FrmTcpClient {0} closes.", Id);
                DisconnectRoutine();

                lock (MyIncomingDataQueueLocker)
                {
                    MyIncomingDataQueue.Clear();
                }
            }
            catch (Exception ex)
            {
                Logger?.Error("Id = {0}", Id);
                Logger?.Error(ex);
            }
        }

        private void FormTcpClient_Load(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(VersionString)) LblVersion.Text = string.Empty;
                else
                {
                    LblVersion.Text = VersionString;
                    LblVersion.Top = 9;
                    LblVersion.Left = this.Width - LblVersion.Width - 24;
                }

                TxtServerHost.Text = ServerHost;
                NudServerPort.Value = ServerPort;
                ChkIsEncryptData.Checked = IsEncryptData;

                SetOtherParameters();

                UiConfig(false);
            }
            catch (Exception ex)
            {
                Logger?.Error("Id = {0}", Id);
                Logger?.Error(ex);
            }
        }

        private string GetStringFromDgv(string key)
        {
            foreach (DataGridViewRow row in DgvOtherParameters.Rows)
            {
                if (key.Equals(row?.Cells?[0]?.Value?.ToString()))
                {
                    return row.Cells[1].Value.ToString() ?? string.Empty;
                }
            }
            return string.Empty;
        }

        private bool? GetBoolFromDgv(string key)
        {
            if (bool.TryParse(GetStringFromDgv(key), out bool b)) return b;
            return null;
        }

        private int? GetIntFromDgv(string key)
        {
            if (int.TryParse(GetStringFromDgv(key), out int i)) return i;
            return null;
        }

        private void BtnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                MyClient = new(TxtServerHost.Text.Trim(), (int)NudServerPort.Value, MyIncomingDataQueue, MyIncomingDataQueueLocker, TcpClientIncomingDataHandler)
                {
                    IsContainLengthAsHeader = GetBoolFromDgv("IsContainLengthAsHeader") ?? true,
                    IsEnableAnalyzeIncomingData = GetBoolFromDgv("IsEnableAnalyzeIncomingData") ?? true,
                    HeartbeatInterval = GetIntFromDgv("HeartbeatInterval") ?? -1,
                    MaxConnectionDuration = GetIntFromDgv("MaxConnectionDuration") ?? 600,
                    MaxIdleDuration = GetIntFromDgv("MaxIdleDuration") ?? -1,
                    MaxDataSize = GetIntFromDgv("MaxDataSize") ?? 104857600,
                    ReceiveDataInterval = GetIntFromDgv("ReceiveDataInterval") ?? 0,
                    ReceiveTotalBufferSize = GetIntFromDgv("ReceiveTotalBufferSize") ?? 10485760,
                    SleepingIntervalInMS = GetIntFromDgv("SleepingIntervalInMS") ?? 100
                };
                LocalLoggerDebug(string.Format("FrmTcpClient {0} trys to connect", Id));
                if (MyClient.Connect())
                {
                    UiConfig(true);
                    BWorkerCheckConnected.RunWorkerAsync();
                }
                else
                {
                    LocalLoggerDebug(string.Format("FrmTcpClient {0} fails to connect", Id));
                    DisconnectRoutine();
                }
            }
            catch (Exception ex)
            {
                Logger?.Error("Id = {0}", Id);
                Logger?.Error(ex);
            }
        }

        private void BtnDisconnect_Click(object sender, EventArgs e)
        {
            try
            {
                DisconnectRoutine();
            }
            catch (Exception ex)
            {
                Logger?.Error("Id = {0}", Id);
                Logger?.Error(ex);
            }
        }

        private bool TcpClientSend(byte[] serializedData)
        {
            return MyClient?.SendByteArray(
                ChkIsEncryptData.Checked ? TT.CryptoAES.Encrypt(serializedData, CryptPassword) : serializedData
                ) ?? false;
        }

        private void BtnSendText_Click(object sender, EventArgs e)
        {
            try
            {
                BtnSendText.Enabled = BtnSendFile.Enabled = false;
                TxtInput.ReadOnly = true;
                Logger?.Debug("FrmTcpClient {0} sends text: {1}", Id, TxtInput.Text);
                LogToUi("{0:yyyy-MM-dd HH:mm:ss} Send text: {1}", DateTime.Now, TxtInput.Text);
                if (TcpClientSend(TT.TcpSocket.Serialization.SerializeText(TxtInput.Text)))
                {
                    TxtInput.Text = string.Empty;
                    TxtInput.ReadOnly = false;
                    BtnSendFile.Enabled = BtnSendText.Enabled = true;
                    TxtInput.Focus();
                }
                else
                {
                    Logger?.Error("FrmTcpClient {0} fails to send text: {1}", Id, TxtInput.Text);
                    LogToUi("Fail to send text: {0}", TxtInput.Text);
                    DisconnectRoutine();
                }
            }
            catch (Exception ex)
            {
                Logger?.Error("Id = {0}", Id);
                Logger?.Error(ex);
            }
        }

        private void TxtInput_KeyUp(object sender, KeyEventArgs e)
        {
            //if (e.KeyCode == Keys.Enter) BtnSendText_Click(null, null);
            if (e.KeyCode == Keys.Enter) BtnSendText.PerformClick();
        }

        private bool SendFile(string filepath)
        {
            /// https://stackoverflow.com/questions/2030847/best-way-to-read-a-large-file-into-a-byte-array-in-c
            /// https://stackoverflow.com/questions/2161895/reading-large-text-files-with-streams-in-c-sharp
            try
            {
                bool isSuccessful = true;
                int pieceLength = 52428800;//50M bytes.//10485760;// 10M bytes.
                long totalLength = (new System.IO.FileInfo(filepath)).Length;
                int lastIndexPiece = (int)Math.Ceiling(1.0m * totalLength / pieceLength) - 1;
                int indexPiece = 0;
                using (System.IO.Stream st = System.IO.File.OpenRead(filepath))
                {
                    byte[] buffer = new byte[pieceLength];
                    int bytesRead;
                    while (isSuccessful && (bytesRead = st.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        byte[] buffer2 = new byte[bytesRead];
                        System.Buffer.BlockCopy(buffer, 0, buffer2, 0, bytesRead);
                        isSuccessful = TcpClientSend(TT.TcpSocket.Serialization.SerializeFilePiece(System.IO.Path.GetFileName(filepath), lastIndexPiece, indexPiece, buffer2));
                        indexPiece += 1;
                        //buffer2 = null;
                    }
                    //buffer = null;
                }
                string s;
                if (isSuccessful)
                {
                    LocalLoggerDebug(string.Format("Client {0} succeeds to send file {1}", Id, filepath));
                }
                else
                {
                    s = string.Format("Client {0} fails to send file {1}", Id, filepath);
                    Logger?.Warn(s);
                    LogToUi(s);
                }
                return isSuccessful;
            }
            catch (Exception ex)
            {
                Logger?.Error("Id = {0}", Id);
                Logger?.Error(ex);
                return false;
            }
        }

        private void BtnSendFile_Click(object sender, EventArgs e)
        {
            try
            {
                BtnSendFile.Enabled = BtnSendText.Enabled = false;
                TxtInput.ReadOnly = true;
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
                    Logger?.Debug("FrmTcpClient {0} sends file {1}", Id, oDialog.FileName);
                    LogToUi("{0:yyyy-MM-dd HH:mm:ss} Send file: {1}", DateTime.Now, oDialog.FileName);
                    /// New method again.
                    if (!SendFile(oDialog.FileName))
                    {
                        Logger?.Error("FrmTcpClient {0} fails to send file: {1}", Id, oDialog.FileName);
                        LogToUi("Fail to send file: {0}", oDialog.FileName);
                        DisconnectRoutine();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger?.Error("Id = {0}", Id);
                Logger?.Error(ex);
            }
            finally
            {
                TxtInput.ReadOnly = false;
                BtnSendFile.Enabled = BtnSendText.Enabled = true;
            }
        }

        private void BWorkerCheckConnected_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                if (IsExit) BWorkerCheckConnected.CancelAsync();
                BackgroundWorker? worker = sender as BackgroundWorker;
                if (worker == null || worker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
                Logger?.Debug("FrmTcpClient {0} begins to check connectivity", Id);
                while (IsExit == false && worker.CancellationPending == false && (MyClient?.IsConnected ?? false))
                {
                    //System.Threading.Thread.Sleep(1000);
                    new System.Threading.ManualResetEvent(false).WaitOne(1000);
                }
                if (!(MyClient?.IsConnected ?? false)) DisconnectRoutine();
                Logger?.Debug("FrmTcpClient {0} stops to check connectivity", Id);
            }
            catch (Exception ex)
            {
                Logger?.Error("Id = {0}", Id);
                Logger?.Error(ex);
            }
        }
    }
}
