using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VsCSharpWinForm_sample2.Views
{
    public partial class FrmTcpClient : Form
    {
        public static Helpers.TLog Logger { get; set; }
        public readonly int Id = -1;
        public bool IsExit = false;
        public string CryptPassword { get; set; }
        //private Helpers.TTcpClientSocket MyClient = null;
        //private Queue<Helpers.TTcpClientSocket.DataPackage> MyIncomingDataQueue = new Queue<Helpers.TTcpClientSocket.DataPackage>();
        private Helpers.TTcpSocket.Client MyClient = null;
        private Queue<Helpers.TTcpSocket.DataPackage> MyIncomingDataQueue = new Queue<Helpers.TTcpSocket.DataPackage>();
        private readonly object MyIncomingDataQueueLocker = new object();
        //private System.Threading.Thread MyThreadToAnalyzeIncomingDataQueue = null;
        //private int AnalyzeIncomingDataInterval = 1;

        public FrmTcpClient(int id)
        {
            InitializeComponent();
            Id = id;
        }

        private void AnalyzeIncomingDataRoutine1(Helpers.TTcpSocket.DataPackage o)
        {
            try
            {
                if (o == null) return;
                string s;
                byte[] decryptedData = ChkEncryptData.Checked ? (o.ByteArray == null ? o.ByteArray : Helpers.CyptoRijndaelT.Decrypt(o.ByteArray, CryptPassword)) : o.ByteArray;
                ///// Old method.
                //if ((decryptedData?.Length ?? 0) < 1)
                //{
                //    //throw new Exception("Length of decrypted data < 1, which is impossible.");
                //    Logger?.Debug("FrmTcpClient {0} meets decrypted data with 0 bytes.", Id);
                //    return;
                //}
                //switch (decryptedData[0])
                //{
                //    case Models.Param.TcpDataType.Text:
                //        string text = Encoding.UTF8.GetString(decryptedData, 1, decryptedData.Length - 1);
                //        Logger?.Debug("FrmTcpClient {3} receives text. Received Time = {0:yyyy-MM-dd HH:mm:ss}. Server = {1}:{2}. Text = {4}", o.Timestamp, o.Host, o.Port, Id, text);
                //        WriteLogToUI("{0:yyyy-MM-dd HH:mm:ss} {1}:{2}. Text = {3}", o.Timestamp, o.Host, o.Port, text);
                //        break;
                //    case Models.Param.TcpDataType.File:
                //        int i = decryptedData.Length - 1;
                //        if (i < 1)
                //        {
                //            s = string.Format("Length of data is {3} < 1. Received Time = {0:yyyy-MM-dd HH:mm:ss}. Server = {1}:{2}", o.Timestamp, o.Host, o.Port, i);
                //            Logger?.Warn("FrmTcpClient {0}: {1}", Id, s);
                //            WriteLogToUI(s);
                //            return;
                //        }
                //        byte[] data = new byte[i];
                //        Array.Copy(decryptedData, 1, data, 0, i);
                //        string filepath = string.Format(Models.Param.TcpClient.DefaultValue.IncomingDataFilePath, o.Timestamp, Id);
                //        filepath = Helpers.GeneralT.GetDefaultAbsolutePathIfRelative(filepath);
                //        if (!Helpers.GeneralT.FolderExistsOrCreateIt(System.IO.Path.GetDirectoryName(filepath)))
                //        {
                //            WriteLogToUI("Fail to create folder.");
                //            return;
                //        }
                //        System.IO.File.WriteAllBytes(filepath, data);
                //        Logger?.Debug("FrmTcpClient {3} receives file. Received Time = {0:yyyy-MM-dd HH:mm:ss}. Server = {1}:{2}. Length = {4}. File = {5}", o.Timestamp, o.Host, o.Port, Id, i, filepath);
                //        WriteLogToUI("{0:yyyy-MM-dd HH:mm:ss} {1}:{2}. Length = {3}. File = {4}", o.Timestamp, o.Host, o.Port, i, filepath);
                //        break;
                //    default:
                //        s = string.Format("Unclassified TCP data type. {0}", decryptedData[0]);
                //        Logger?.Error(s);
                //        WriteLogToUI(s);
                //        break;
                //}

                /// New method.
                Helpers.TTcpSocket.DeserializedData deserializedData = Helpers.TTcpSocket.Serialization.Deserialize(decryptedData);
                if (deserializedData == null)
                {
                    //throw new Exception("Length of decrypted data < 1, which is impossible.");
                    Logger?.Debug("FrmTcpClient {0} meets decrypted data with 0 bytes.", Id);
                    return;
                }
                switch (deserializedData.DataType)
                {
                    case Helpers.TTcpSocket.SerialDataType.Text:
                        s = string.Format("{0:yyyy-MM-dd HH:mm:ss}. Server = {1}:{2}. Text = {3}", o.Timestamp, o.Host, o.Port, deserializedData.Text);
                        Logger?.Debug("FrmTcpClient {0} receives text. Received Time = {1}", Id, s);
                        WriteLogToUI(s);
                        break;
                    case Helpers.TTcpSocket.SerialDataType.File:
                        s = string.Format("{0:yyyy-MM-dd HH:mm:ss}. Server = {1}:{2}. Last index of piece = {3}. Index of current piece = {4}. Piece length = {5}", o.Timestamp, o.Host, o.Port, deserializedData.LastIndexPiece, deserializedData.IndexPiece, deserializedData.FileContent?.Length);
                        Logger?.Debug("FrmTcpClient {0} receives a file piece. Received Time = {1}", Id, s);
                        WriteLogToUI(s);
                        if (!string.IsNullOrEmpty(deserializedData.ErrorMessage))
                        {
                            s = string.Format("{0:yyyy-MM-dd HH:mm:ss}. Server = {1}:{2}. Last index of piece = {3}. Index of current piece = {4}. Error Message = {5}", o.Timestamp, o.Host, o.Port, deserializedData.LastIndexPiece, deserializedData.IndexPiece, deserializedData.ErrorMessage);
                            Logger?.Debug("FrmTcpClient {0} receives error message. Received Time = {1}", Id, s);
                            WriteLogToUI(s);
                        }
                        deserializedData.DestFolder = Helpers.GeneralT.GetDefaultAbsolutePathIfRelative(string.Format(Models.Param.TcpClient.DefaultValue.IncomingDataFolder, Id));
                        if (string.IsNullOrWhiteSpace(deserializedData.Filename))
                        {
                            s = string.Format("{0:yyyy-MM-dd HH:mm:ss}. Server = {1}:{2}. Last index of piece = {3}. Index of current piece = {4}. Filename is empty.", o.Timestamp, o.Host, o.Port, deserializedData.LastIndexPiece, deserializedData.IndexPiece);
                            Logger?.Debug("FrmTcpClient {0} receives file piece. Received Time = {1}", Id, s);
                            WriteLogToUI(s);
                            deserializedData.Filename = string.Format(Models.Param.TcpClient.DefaultValue.IncomingDataFilename, o.Timestamp, Id);
                        }
                        s = Helpers.TTcpSocket.Serialization.AppendDeserializedDataToFile(deserializedData);
                        if (string.IsNullOrEmpty(s))
                        {
                            if (deserializedData.IndexPiece == deserializedData.LastIndexPiece)
                            {
                                s = string.Format("{0:yyyy-MM-dd HH:mm:ss}. Server = {1}:{2}. Last index of piece = {3}. Index of current piece = {4}. Output file path = {5}", o.Timestamp, o.Host, o.Port, deserializedData.LastIndexPiece, deserializedData.IndexPiece, deserializedData.DestFilepath);
                                Logger?.Debug("FrmTcpClient {0} completes to receive file. Received Time = {1}", Id, s);
                                WriteLogToUI(s);
                            }
                        }
                        else
                        {
                            s = string.Format("{0:yyyy-MM-dd HH:mm:ss}. Server = {1}:{2}. Last index of piece = {3}. Index of current piece = {4}. Output file path = {5}. Error = {6}", o.Timestamp, o.Host, o.Port, deserializedData.LastIndexPiece, deserializedData.IndexPiece, deserializedData.DestFilepath, s);
                            Logger?.Error("FrmTcpClient {0} has error when receiving file. Received Time = {1}", Id, s);
                            WriteLogToUI(s);
                        }
                        break;
                    default:
                        s = string.Format("FrmTcpClient {0} finds unclassified TCP data type. {1}", Id, decryptedData[0]);
                        Logger?.Error(s);
                        WriteLogToUI(s);
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger?.Error("Id = {0}", Id);
                Logger?.Error(ex);
            }
        }

        //private void AnalyzeIncomingDataRoutine0()
        //{
        //    List<Helpers.TTcpSocket.DataPackage> tempList = null;
        //    try
        //    {
        //        lock (MyIncomingDataQueueLocker)
        //        {
        //            if (MyIncomingDataQueue == null || MyIncomingDataQueue.Count < 1) return;
        //            int iMax = 5;
        //            int i = 0;
        //            tempList = new List<Helpers.TTcpSocket.DataPackage>();
        //            while ((MyIncomingDataQueue?.Count ?? 0) > 0 && i < iMax)
        //            {
        //                tempList.Add(MyIncomingDataQueue?.Dequeue());/// pass to temp list in order to unlock the list earlier.
        //                i += 1;
        //            }
        //        }
        //        if (tempList != null)
        //        {
        //            foreach (Helpers.TTcpSocket.DataPackage o in tempList)
        //            {
        //                AnalyzeIncomingDataRoutine1(o);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger?.Error("Id = {0}", Id);
        //        Logger?.Error(ex);
        //    }
        //    finally
        //    {
        //        if (tempList != null)
        //        {
        //            tempList.Clear();
        //            tempList = null;
        //        }
        //    }
        //}

        //private void ProcessAnalyzeIncomingDataQueue()
        //{
        //    try
        //    {
        //        DateTime tNow, tRef;
        //        Logger?.Debug("FrmTcpClient {0} begins to analyze the incoming data.", Id);
        //        tRef = DateTime.Now.AddHours(-1);
        //        while (IsExit == false)
        //        {
        //            tNow = DateTime.Now;
        //            if (AnalyzeIncomingDataInterval == 0 || (AnalyzeIncomingDataInterval > 0 && (int)(tNow - tRef).TotalSeconds >= AnalyzeIncomingDataInterval))
        //            {
        //                tRef = tNow;
        //                AnalyzeIncomingDataRoutine0();
        //            }
        //            System.Threading.Thread.Sleep(200);
        //        }
        //        Logger?.Debug("FrmTcpClient {0} stops to analyze the incoming data.", Id);
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger?.Error("Id = {0}", Id);
        //        Logger?.Error(ex);
        //    }
        //}

        public void FormClosedRoutine()
        {
            try
            {
                IsExit = true;
                Logger?.Debug("FrmTcpClient {0} closes.", Id);
                DisconnectRoutine();

                //DateTime tRef = DateTime.Now;
                //while ((MyThreadToAnalyzeIncomingDataQueue?.IsAlive ?? false) && (int)(DateTime.Now - tRef).TotalSeconds < 3)
                //{
                //    System.Threading.Thread.Sleep(100);
                //}
                //if (MyThreadToAnalyzeIncomingDataQueue?.IsAlive ?? false)
                //{
                //    Logger?.Debug("FrmTcpClient {0} aborts MyThreadToAnalyzeIncomingDataQueue", Id);
                //    MyThreadToAnalyzeIncomingDataQueue?.Abort();
                //}
                //MyThreadToAnalyzeIncomingDataQueue = null;

                lock (MyIncomingDataQueueLocker)
                {
                    MyIncomingDataQueue.Clear();
                    MyIncomingDataQueue = null;
                }
            }
            catch (Exception ex)
            {
                Logger?.Error("Id = {0}", Id);
                Logger?.Error(ex);
            }
        }

        private void FrmTcpClient_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                FormClosedRoutine();
            }
            catch (Exception ex)
            {
                Logger?.Error("Id = {0}", Id);
                Logger?.Error(ex);
            }
        }

        private void FrmTcpClient_Load(object sender, EventArgs e)
        {
            try
            {
                /// Version Information. Show the version in the GUI.
                /// Assume the product version (File version) is same as Assembly version.
                /// To update the version, in the menu bar, click "Project > [Project Name] Properties > Application > Assembly Information...", change the Assembly version and File version.
                LblVersion.Top = 9;
                LblVersion.Left = this.Width - LblVersion.Width - 24;/// set the position of label according to its width.

                UiConfig(false);
                //MyThreadToAnalyzeIncomingDataQueue = new System.Threading.Thread(new System.Threading.ThreadStart(ProcessAnalyzeIncomingDataQueue))
                //{
                //    Name = "ProcessAnalyzeIncomingDataQueue-" + Id.ToString()
                //};
                //MyThreadToAnalyzeIncomingDataQueue.Start();
            }
            catch (Exception ex)
            {
                Logger?.Error("Id = {0}", Id);
                Logger?.Error(ex);
            }
        }

        private void WriteLogToUI(string format, params object[] args)
        {
            try
            {
                if (IsExit) { return; }
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

        private void ChkHeartbeatInterval_CheckedChanged(object sender, EventArgs e)
        {
            NudHeartbeatInterval.Enabled = ChkHeartbeatInterval.Checked;
        }

        private void ChkMaxConnectionDuration_CheckedChanged(object sender, EventArgs e)
        {
            NudMaxConnectionDuration.Enabled = ChkMaxConnectionDuration.Checked;
        }

        private void ChkMaxIdleDuration_CheckedChanged(object sender, EventArgs e)
        {
            NudMaxIdleDuration.Enabled = ChkMaxIdleDuration.Checked;
        }

        private void ChkSleepingInterval_CheckedChanged(object sender, EventArgs e)
        {
            NudSleepingInterval.Enabled = ChkSleepingInterval.Checked;
        }

        private void UiConfig(bool isConnected)
        {
            try
            {
                if (IsExit) { return; }
                Invoke(new MethodInvoker(delegate
                {
                    try
                    {
                        //TxtServerHost.ReadOnly = isConnected;
                        //NudServerPort.ReadOnly = isConnected;
                        //ChkContainLengthAsHeader.Enabled = !isConnected;
                        //ChkEncryptData.Enabled = !isConnected;
                        ChkHeartbeatInterval_CheckedChanged(null, null);
                        ChkMaxConnectionDuration_CheckedChanged(null, null);
                        ChkMaxIdleDuration_CheckedChanged(null, null);
                        ChkSleepingInterval_CheckedChanged(null, null);
                        foreach (Control ctl in TPageMainParameters.Controls) { ctl.Enabled = !isConnected; }
                        foreach (Control ctl in TPageOtherParameters.Controls) { ctl.Enabled = !isConnected; }
                        BtnConnect.Enabled = !isConnected;
                        BtnDisconnect.Enabled = isConnected;
                        TxtInput.ReadOnly = !isConnected;
                        BtnSendText.Enabled = isConnected;
                        BtnSendFile.Enabled = isConnected;
                        this.Text = isConnected ? string.Format("TcpClient {0} - {1}", Id, MyClient?.LocalEndPoint) : string.Format("TcpClient {0}", Id);
                    }
                    catch (Exception ex2)
                    {
                        Logger?.Error("Id = {0}", Id);
                        Logger?.Error(ex2);
                    }
                }));
            }
            catch (Exception ex)
            {
                Logger?.Error("Id = {0}", Id);
                Logger?.Error(ex);
            }
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

        private void BtnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                DisconnectRoutine();
                //MyClient = new Helpers.TTcpClientSocket()
                //{
                //    ServerHost = TxtServerHost.Text.Trim(),
                //    ServerPort = (int)NudServerPort.Value,
                //    ContainLengthAsHeader = ChkContainLengthAsHeader.Checked,
                //    EnableAnalyzeIncomingData = Models.Param.TcpClient.DefaultValue.EnableAnalyzeIncomingData,
                //    HeartbeatInterval = ChkHeartbeatInterval.Checked ? (int)NudHeartbeatInterval.Value : -1,
                //    MaxConnectionDuration = ChkMaxConnectionDuration.Checked ? (int)NudMaxConnectionDuration.Value : -1,
                //    MaxIdleDuration = ChkMaxIdleDuration.Checked ? (int)NudMaxIdleDuration.Value : -1,
                //    MaxDataSize = (int)NudMaxDataSize.Value,
                //    ReceiveDataInterval = (int)NudReceiveDataInterval.Value,
                //    ReceiveTotalBufferSize = (int)NudReceiveTotalBufferSize.Value,
                //    SleepingIntervalInMS = ChkSleepingInterval.Checked ? (int)NudSleepingInterval.Value : -1,
                //    IncomingDataQueue = MyIncomingDataQueue,
                //    IncomingDataQueueLocker = MyIncomingDataQueueLocker
                //};
                MyClient = new Helpers.TTcpSocket.Client(TxtServerHost.Text.Trim(), (int)NudServerPort.Value, MyIncomingDataQueue, MyIncomingDataQueueLocker)
                {
                    ContainLengthAsHeader = ChkContainLengthAsHeader.Checked,
                    EnableAnalyzeIncomingData = true,
                    HeartbeatInterval = ChkHeartbeatInterval.Checked ? (int)NudHeartbeatInterval.Value : -1,
                    MaxConnectionDuration = ChkMaxConnectionDuration.Checked ? (int)NudMaxConnectionDuration.Value : -1,
                    MaxIdleDuration = ChkMaxIdleDuration.Checked ? (int)NudMaxIdleDuration.Value : -1,
                    MaxDataSize = (int)NudMaxDataSize.Value,
                    ReceiveDataInterval = (int)NudReceiveDataInterval.Value,
                    ReceiveTotalBufferSize = (int)NudReceiveTotalBufferSize.Value,
                    SleepingIntervalInMS = ChkSleepingInterval.Checked ? (int)NudSleepingInterval.Value : -1,
                    ExternalActToHandleIncomingData = AnalyzeIncomingDataRoutine1
                };
                string s = string.Format("FrmTcpClient {0} trys to connect.", Id);
                Logger?.Info(s);
                WriteLogToUI(s);
                if (MyClient.StartConnection())
                {
                    UiConfig(true);
                    BWorkerCheckConnected.RunWorkerAsync();
                }
                else
                {
                    s = string.Format("FrmTcpClient {0} fails to connect.", Id);
                    Logger?.Info(s);
                    WriteLogToUI(s);
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

        //private bool TcpClientSend(byte tcpDataType, byte[] data)
        //{
        //    List<byte> tempList = null;
        //    try
        //    {
        //        tempList = new List<byte>()
        //        {
        //            tcpDataType
        //        };
        //        tempList.AddRange(data);
        //        //byte[] encryptedData = null;
        //        //if (ChkEncryptData.Checked) encryptedData = Helpers.CyptoRijndaelT.Encrypt(tempList.ToArray(), CryptPassword);
        //        //else encryptedData = tempList.ToArray();
        //        byte[] encryptedData = ChkEncryptData.Checked ? Helpers.CyptoRijndaelT.Encrypt(tempList.ToArray(), CryptPassword) : tempList.ToArray();
        //        return MyClient?.SendByteArray(encryptedData) ?? false;
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger?.Error(ex);
        //        return false;
        //    }
        //    finally
        //    {
        //        if (tempList != null)
        //        {
        //            tempList.Clear();
        //            tempList = null;
        //        }
        //    }
        //}

        private bool TcpClientSend(byte[] serializedData)
        {
            return MyClient?.SendByteArray(
                ChkEncryptData.Checked ? Helpers.CyptoRijndaelT.Encrypt(serializedData, CryptPassword) : serializedData
                ) ?? false;
        }

        private void BtnSendText_Click(object sender, EventArgs e)
        {
            try
            {
                BtnSendText.Enabled = false;
                BtnSendFile.Enabled = false;
                TxtInput.ReadOnly = true;
                Logger?.Debug("FrmTcpClient {0} sends text: {1}", Id, TxtInput.Text);
                WriteLogToUI("{0:yyyy-MM-dd HH:mm:ss} Send text: {1}", DateTime.Now, TxtInput.Text);
                bool isSuccessful;
                ///// Old method.
                // isSuccessful = TcpClientSend(Models.Param.TcpDataType.Text, Encoding.UTF8.GetBytes(TxtInput.Text));
                /// New method.
                isSuccessful = TcpClientSend(Helpers.TTcpSocket.Serialization.SerializeText(TxtInput.Text));
                if (isSuccessful)
                {
                    TxtInput.Text = "";
                    TxtInput.ReadOnly = false;
                    BtnSendText.Enabled = true;
                    BtnSendFile.Enabled = true;
                    TxtInput.Focus();
                }
                else
                {
                    Logger?.Error("FrmTcpClient {0} fails to send text: {1}", Id, TxtInput.Text);
                    WriteLogToUI("Fail to send text: {0}", TxtInput.Text);
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
            if (e.KeyCode == Keys.Enter) BtnSendText_Click(null, null);
        }

        private bool HandleFile(string filepath)
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
                        isSuccessful = TcpClientSend(Helpers.TTcpSocket.Serialization.SerializeFilePiece(System.IO.Path.GetFileName(filepath), lastIndexPiece, indexPiece, buffer2));
                        indexPiece += 1;
                        buffer2 = null;
                    }
                    buffer = null;
                }
                string s;
                if (isSuccessful)
                {
                    s = string.Format("Client {0} succeeds to send file {1}", Id, filepath);
                    Logger?.Debug(s);
                }
                else
                {
                    s = string.Format("Client {0} fails to send file {1}", Id, filepath);
                    Logger?.Warn(s);
                }
                WriteLogToUI(s);
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
                BtnSendText.Enabled = false;
                BtnSendFile.Enabled = false;
                TxtInput.ReadOnly = true;
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
                    Logger?.Debug("FrmTcpClient {0} sends file {1}", Id, oDialog.FileName);
                    WriteLogToUI("{0:yyyy-MM-dd HH:mm:ss} Send file: {1}", DateTime.Now, oDialog.FileName);
                    bool isSuccessful;
                    ///// Old method.
                    //isSuccessful = TcpClientSend(Models.Param.TcpDataType.File, System.IO.File.ReadAllBytes(oDialog.FileName));
                    ///// New method.
                    //isSuccessful = TcpClientSend(Helpers.TTcpSocket.Serialization.SerializeSmallFile(oDialog.FileName));
                    /// New method again.
                    isSuccessful = HandleFile(oDialog.FileName);
                    if (!isSuccessful)
                    {
                        Logger?.Error("FrmTcpClient {0} fails to send file: {1}", Id, oDialog.FileName);
                        WriteLogToUI("Fail to send file: {0}", oDialog.FileName);
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
                BtnSendText.Enabled = true;
                BtnSendFile.Enabled = true;
            }
        }

        private void ProcessCheckConnected()
        {
            try
            {
                DateTime tRef = DateTime.Now.AddHours(-1);
                DateTime tNow;
                int interval = 1;
                Logger?.Debug("FrmTcpClient {0} begins to check connectivity.", Id);
                while (IsExit == false && (MyClient?.IsConnected ?? false))
                {
                    tNow = DateTime.Now;
                    if ((int)(tNow - tRef).TotalSeconds >= interval)
                    {
                        tRef = tNow;
                    }
                    System.Threading.Thread.Sleep(1000);
                }
                if (!(MyClient?.IsConnected ?? false)) DisconnectRoutine();
                Logger?.Debug("FrmTcpClient {0} stops to check connectivity.", Id);
            }
            catch (Exception ex)
            {
                Logger?.Error("Id = {0}", Id);
                Logger?.Error(ex);
            }
        }

        private void BWorkerCheckConnected_DoWork(object sender, DoWorkEventArgs e)
        {
            ProcessCheckConnected();
        }
    }
}
