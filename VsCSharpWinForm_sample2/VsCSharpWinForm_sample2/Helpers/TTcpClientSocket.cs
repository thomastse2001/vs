using System;
using System.Collections.Generic;
using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

namespace VsCSharpWinForm_sample2.Helpers
{
    public class TTcpClientSocket
    {
        /// TCP Client by a synchronous socket in the threading model.
        /// Data unit is byte.
        /// Updated date: 2020-09-11

        /// https://docs.microsoft.com/en-us/dotnet/framework/network-programming/asynchronous-client-socket-example
        /// https://docs.microsoft.com/en-us/dotnet/framework/network-programming/synchronous-client-socket-example
        /// http://csharp.net-informations.com/communications/csharp-socket-programming.htm

        /// Parameters for private user.
        private bool IsExit = false;/// flag to indicate if exit.
        private bool IsIncomingDataFinished = true;/// Whether the incoming data is finished or continuous. True = finished, false = continuous.
        private bool IsIncomingDataLength = true;/// Whether the incoming data is length or content. True = length, false = content.
        private int IncomingContentSize = 0;/// Size of incoming content.
        private int IncomingContentIndex = 0;/// index of incoming content.
        private int IncomingLengthIndex = 0;/// index of incoming length.
        private byte[] IncomingContentBuffer = null;/// buffer for incoming content.
        private byte[] IncomingLengthBuffer = null;/// buffer for incoming length.
        private List<byte[]> IncomingBufferList = null;/// buffer list of TCP incoming data. Only used for mbEnableAnalyzeIncomingData = false.
        private System.Threading.Thread ThreadToAnalyzeIncomingBuffer = null;
        private System.Threading.Thread ThreadToOtherProcesses = null;
        private System.Threading.Thread ThreadToReceiveData = null;
        private System.Net.Sockets.Socket ClientSocket = null;
        private Queue<byte[]> IncomingBufferQueue = null;/// buffer queue of TCP incoming data.
        private readonly object IncomingBufferQueueLocker = new object();

        public class TcpSocketData
        {
            public DateTime Timestamp = DateTime.MinValue;
            public string Host = "";
            public int Port = -1;
            //public bool IsFinishedReceiving = false;
            public byte[] ByteArray = null;
        }
        
        /// Properties.
        public static Helpers.TLog Logger { get; set; }
        public string ServerHost { get; set; } = "127.0.0.1";
        public int ServerPort { get; set; } = 8002;
        public readonly DateTime InitialDateTime;
        public DateTime LastReceivedDateTime { get; private set; }/// Date time of last received data. Can read and write it inside class, but read-only outside the class. https://stackoverflow.com/questions/4662180/c-sharp-public-variable-as-writeable-inside-the-class-but-readonly-outside-the-c
        public DateTime LastTransferDateTime { get; private set; }/// Date time of last transfer data. Can read and write it inside class, but read-only outside the class. https://stackoverflow.com/questions/4662180/c-sharp-public-variable-as-writeable-inside-the-class-but-readonly-outside-the-c
        public bool ContainLengthAsHeader { get; set; } = true;/// flag that if the data contains length (4-byte integer) as a header of package.
        public bool EnableAnalyzeIncomingData { get; set; } = true;/// flag to enable to analyze the incoming data from server. A package is preserved when adding to the queue of incoming data. Note that large package excceeding the buffer size is divided as several parts when receiving. The users may need to handle how to obtain the complete package. if disable this flag. The default value is true.
        private byte[] _HeartbeatData = System.Text.Encoding.UTF8.GetBytes("~");
        public byte[] HeartbeatData/// Data in byte array of heartbeat from server. This parameter will be used only if ContainLengthAsHeader = false. Space and Tab will be neglect. Sending heartbeat is used for server to check if the client sockets are still connected. The default value is the byte array of ~.
        {
            get { return _HeartbeatData; }
            set
            {
                if (value == null) { _HeartbeatData = null; }
                else
                {
                    _HeartbeatData = new byte[value.Length];
                    Array.Copy(value, 0, _HeartbeatData, 0, value.Length);
                }
            }
        }
        public int HeartbeatInterval { get; set; } = 15;/// Time interval in seconds that TCP client sends heartbeat to server. If it is negative, there is no heartbeat. Sending heartbeat is used to check if the connection still occurs. The default value is 15.
        public int MaxConnectionDuration { get; set; } = 600;/// Maximum connection duration in seconds between server and client. If the time exceeds, it will disconnect automatically even the connection is normal. If it is negative, there is no maximum connection duration, hence the connection between server and client can preserve forever. The default value is 600.
        public int MaxIdleDuration { get; set; } = -1;/// Maximum idle duration in seconds between server and client. If the time exceeds, it will disconnect automatically. If it is negative, there is no maximum idle duration, hence it will not check the idle duration. The default value is -1.
        private int _MaxDataSize = 2000000000;
        public int MaxDataSize { get { return _MaxDataSize; } set { _MaxDataSize = value < 0 ? 0 : value; } }/// Maximum size of data in bytes. The default value is 2000000000.
        
        public int ReceiveDataInterval { get; set; } = 0;/// Time interval in seconds that TCP server receives Data. If it is 0, the process will do immediately without waiting. If it is negative, no data is received. The default value is 0.
        private int _ReceiveTotalBufferSize = 10485760;
        public int ReceiveTotalBufferSize { get { return _ReceiveTotalBufferSize; } set { _ReceiveTotalBufferSize = value < 10485760 ? 10485760 : value; } }/// Total buffer size in bytes for receiving data from client, the minimum value is 10485760.
        public int SleepingIntervalInMS { get; set; } = 100;/// Sleeping interval in milliseconds. This sleeping interval helps to avoid the application too busy. The default value is 100.
        //public bool IsConnected { get { return ClientSocket == null ? false : ClientSocket.Connected; } }
        public bool IsConnected { get { return ClientSocket?.Connected ?? false; } }
        //public string LocalEndPoint { get { return ClientSocket == null ? null : ClientSocket.LocalEndPoint == null ? null : ClientSocket.LocalEndPoint.ToString(); } }
        public string LocalEndPoint { get { return ClientSocket?.LocalEndPoint?.ToString(); } }
        //public string RemoteEndPoint { get { return ClientSocket == null ? null : ClientSocket.RemoteEndPoint == null ? null : ClientSocket.RemoteEndPoint.ToString(); } }
        public string RemoteEndPoint { get { return ClientSocket?.RemoteEndPoint?.ToString(); } }
        public Queue<TcpSocketData> IncomingDataQueue = null;/// queue of TCP incoming data.
        public object IncomingDataQueueLocker = null;

        /// Abort a specific thread and set it to null.
        /// Return Value = Result whether abort the thread successfully. True if success. Otherwise, false.
        /// th = target thread that will be aborted.
        private static bool AbortThread(ref System.Threading.Thread th)
        {
            if (th == null || th.IsAlive == false) { return true; }
            try
            {
                Logger?.Debug("Force to abort the thread named {0}", th.Name);
                th.Abort();
                return true;
            }
            catch (Exception ex)
            {
                Logger?.Error(ex);
                return false;
            }
            finally { th = null; }
        }

        /// Add data to the queue of incoming data.
        /// Return value = True if add data to the queue successfully. Otherwise, fail.
        //private bool AddDataToIncomingDataQueue(DateTime tTimestamp, bool bIsFinishedReceiving, ref byte[] oByteData)
        //private bool AddDataToIncomingDataQueue(DateTime tTimestamp, ref byte[] oByteData)
        private bool AddDataToIncomingDataQueue(DateTime t, byte[] data)
        {
            try
            {
                if (IncomingDataQueue == null)
                {
                    throw new Exception(string.Format("TCP Client cannot put data to IncomingDataQueue as it is not initialized. Server socket = {0}, local socket = {1}", RemoteEndPoint, LocalEndPoint));
                }
                TcpSocketData oData = new TcpSocketData()
                {
                    Timestamp = t,
                    Host = ServerHost,
                    Port = ServerPort,
                    //IsFinishedReceiving = bIsFinishedReceiving,
                    ByteArray = data
                };
                if (IncomingDataQueueLocker == null) { IncomingDataQueue.Enqueue(oData); }
                else { lock (IncomingDataQueueLocker) { IncomingDataQueue.Enqueue(oData); } }
                Logger?.Debug("TCP Client adds the received data to IncomingDataQueue. Server socket = {0}, Byte Length = {1}", RemoteEndPoint, data == null ? 0 : data.Length);
                return true;
            }
            catch (Exception ex)
            {
                Logger?.Error("Server socket = {0}, local socket = {1}", RemoteEndPoint, LocalEndPoint);
                Logger?.Error(ex);
                return false;
            }
        }

        /// Disconnect.
        public static void Disconnect(ref System.Net.Sockets.Socket o)
        {
            if (o == null) { return; }
            try
            {
                if (o?.Connected ?? false)
                {
                    o.Shutdown(System.Net.Sockets.SocketShutdown.Both);
                    o.Close();
                }
            }
            catch (Exception ex) { Logger?.Error(ex); }
            finally { o = null; }
        }
        public void Disconnect() { Disconnect(ref ClientSocket); }

        /// Pack the length of data into the beginning of data, in order to let the receiver easy to recognize each individual unit of data.
        public static byte[] PackData(int maxDataSize, byte[] data)
        {
            int iLength = 0;
            try
            {
                if (data != null) { iLength = data.Length; }
                if (iLength > maxDataSize)
                {
                    throw new Exception(string.Format("Exceed the maximum data size {0}. Data size = [1}", maxDataSize, iLength));
                }
                byte[] rByte = new byte[4 + iLength];
                BitConverter.GetBytes(iLength).CopyTo(rByte, 0);
                if (data != null) { data.CopyTo(rByte, 4); }
                return rByte;
            }
            catch (Exception ex)
            {
                Logger?.Error(ex);
                return null;
            }
        }
        public byte[] PackData(byte[] data) { return PackData(MaxDataSize, data); }

        /// Send byte array or string to TCP Server.
        private static bool SendByteArray(ref System.Net.Sockets.Socket oClientSocket, bool isContainLengthAsHeader, int maxDataSize, byte[] data)
        {
            int iSize = -1;
            int iSent = 0;/// how many bytes are already sent.
            byte[] byteData = null;
            string serverSocket = "";
            string localSocket = "";
            try
            {
                //if (oClientSocket == null || oClientSocket.Connected == false) { return false; }
                if ((oClientSocket?.Connected ?? false) == false) { return false; }
                //if (data == null) { data = new byte[0]; }
                if (isContainLengthAsHeader) { byteData = PackData(maxDataSize, data); }/// Pack data with the length as header.
                else { byteData = data; }// NOT pack data.
                /// Checking.
                if (byteData == null) { return false; }
                /// Send.
                iSize = byteData.Length;
                while (iSent < iSize)
                { iSent += oClientSocket.Send(byteData, iSent, iSize - iSent, System.Net.Sockets.SocketFlags.None); }
                localSocket = oClientSocket?.LocalEndPoint?.ToString();
                serverSocket = oClientSocket?.RemoteEndPoint?.ToString();
                Logger?.Debug("TCP Client sends data. Server socket = {0}, local socket = {1}, #Size = {2}, #Sent = {3}", serverSocket, localSocket, iSize, iSent);
                if (iSent == iSize) { return true; }
                else
                {
                    Logger?.Error("TCP Client sends incomplete data. Server socket = {0}, local socket = {1}, #Size = {2}, #Sent = {3}", serverSocket, localSocket, iSize, iSent);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Logger?.Error("Server socket = {0}, local socket = {1}", serverSocket, localSocket);
                Logger?.Error(ex);
                Disconnect(ref oClientSocket);
                return false;
            }
        }
        public bool SendByteArray(byte[] data)
        {
            bool b = SendByteArray(ref ClientSocket, ContainLengthAsHeader, MaxDataSize, data);
            if (b) { LastTransferDateTime = DateTime.Now; }
            return b;
        }

        /// Receive byte array from TCP Server.
        private void ReceiveByteArray()
        {
            int iReceived;/// how many bytes are total received.
            byte[] byteBuffer;
            byte[] byteData;
            string sRemoteEndPoint = "";
            string sLocalEndPoint = "";
            try
            {
                if (ClientSocket == null || ClientSocket.Connected == false) { return; }
                sRemoteEndPoint = RemoteEndPoint;
                sLocalEndPoint = LocalEndPoint;
                byteBuffer = new byte[ReceiveTotalBufferSize];/// 10M bytes.
                iReceived = ClientSocket.Receive(byteBuffer, 0, byteBuffer.Length, System.Net.Sockets.SocketFlags.None);
                try
                {
                    if (iReceived > 0)
                    {
                        byteData = new byte[iReceived];
                        Array.Copy(byteBuffer, 0, byteData, 0, iReceived);
                        byteBuffer = null;
                        LastReceivedDateTime = LastTransferDateTime = DateTime.Now;
                        Logger?.Debug("TCP Client receives data. Server socket = {0}, local socket = {1}, #Received = {2}", sRemoteEndPoint, sLocalEndPoint, iReceived);
                        if (EnableAnalyzeIncomingData)
                        {
                            /// Add data to the incoming buffer.
                            lock (IncomingBufferQueueLocker)
                            {
                                if (IncomingBufferQueue == null) { IncomingBufferQueue = new Queue<byte[]>(); }
                                IncomingBufferQueue.Enqueue(byteData);
                            }
                        }
                        else
                        {
                            /// Add data to the queue directly.
                            AddDataToIncomingDataQueue(DateTime.Now, byteData);
                        }
                    }
                    else
                    {
                        /// If receive empty bytes, do following.
                        // /// send a null string to the server, in order to verify the connection is still valid.
                        // SendByteArray(null);
                        /// If receive empty bytes if ContainLengthAsHeader = true, quit this function.
                        /// Only do the below if ContainLengthAsHeader = false.
                        if (!ContainLengthAsHeader)
                        {
                            Logger?.Debug("TCP Client receives 0 data. Server socket = {0}, local socket = {1}", sRemoteEndPoint, sLocalEndPoint);
                            if (EnableAnalyzeIncomingData)
                            {
                                /// Add a null data to buffer, to indicate it is the end of data.
                                lock (IncomingBufferQueueLocker)
                                {
                                    if (IncomingBufferQueue == null) { IncomingBufferQueue = new Queue<byte[]>(); }
                                    IncomingBufferQueue.Enqueue(null);
                                }
                            }
                            else
                            {
                                /// Add data to the list directly.
                                AddDataToIncomingDataQueue(DateTime.Now, null);
                            }
                        }
                        //return;
                    }
                }
                catch (Exception ex2)
                {
                    Logger?.Error("Server socket = {0}, local socket = {1}", sRemoteEndPoint, sLocalEndPoint);
                    Logger?.Error(ex2);
                }
            }
            catch
            {
                Disconnect(ref ClientSocket);
            }
            finally { byteBuffer = null; }
        }

        private bool TryDequeueAtIncomingBufferQueue(out byte[] byteOutput)
        {
            bool bReturn = false;
            try
            {
                lock (IncomingBufferQueueLocker)
                {
                    //if (IncomingBufferQueue == null || IncomingBufferQueue.Count < 1)
                    if ((IncomingBufferQueue?.Count ?? 0) < 1)
                    {
                        byteOutput = null;
                        bReturn = false;
                    }
                    else
                    {
                        byteOutput = IncomingBufferQueue.Dequeue();
                        bReturn = true;
                    }
                }
                return bReturn;
            }
            catch (Exception ex)
            {
                Logger?.Error("Server socket = {0}, local socket = {1}", RemoteEndPoint, LocalEndPoint);
                Logger?.Error(ex);
                byteOutput = null;
                return false;
            }
        }

        /// Analyze the incoming buffer.
        /// 3 statuses.
        /// State, IsIncomingDataFinished, IsIncomingDataLength.
        /// 1st state, True, True. The previous data is finished. And if there is an incoming data, should analyze the length first.
        /// 2nd state, False, True. set this state if the current incoming data arrives. And should analyze the length.
        /// 3rd state, False, False. After analyzing the length, set to analyze the content.
        private void AnalyzeIncomingBuffer()
        {
            byte[] byteDataInQueue;
            int iIndex1;/// Number of bytes that already analyzied in the data of the queue.
            int iCopy, iInputRemaining, iOutputRemaining;
            string remoteEndPoint = "";
            string localEndPoint = "";
            try
            {
                //if (mQueueOfIncomingBuffer == null || mQueueOfIncomingBuffer.Count < 1) { return; }
                remoteEndPoint = RemoteEndPoint;
                localEndPoint = LocalEndPoint;
                if (ContainLengthAsHeader)
                {
                    /// The case that data contains the length as header.
                    /// TT edited on 2018-08-21 to lock the common resource.
                    while (TryDequeueAtIncomingBufferQueue(out byteDataInQueue))
                    {
                        //if (byteDataInQueue == null) { return; }
                        if (byteDataInQueue != null)
                        {
                            Logger?.Debug("AnalyzeIncomingBuffer. byteDataInQueue.Length = {0}, Server socket = {1}, Local socket = {2}", byteDataInQueue.Length, remoteEndPoint, localEndPoint);
                            /// Loop through the buffer item of the queue.
                            iIndex1 = 0;
                            while (iIndex1 < byteDataInQueue.Length)
                            {
                                /// If they are bytes of length, do following.
                                if (IsIncomingDataLength)
                                {
                                    /// If the previous data is finished, do following.
                                    if (IsIncomingDataFinished)
                                    {
                                        IsIncomingDataFinished = false;/// Go to 2nd state.
                                        IncomingLengthIndex = 0;
                                        IncomingContentIndex = 0;
                                        IncomingContentSize = 0;
                                        /// Assign 4 byte to the Length buffer.
                                        if (IncomingLengthBuffer != null) { IncomingLengthBuffer = null; }
                                        IncomingLengthBuffer = new byte[4];
                                    }
                                    /// Determine the number of bytes to be read and copied.
                                    iInputRemaining = byteDataInQueue.Length - iIndex1;/// number of bytes that input remains to be read.
                                    iOutputRemaining = 4 - IncomingLengthIndex;/// number of bytes that output remains to be write.
                                    if (iInputRemaining >= iOutputRemaining)
                                    {
                                        /// If remaining data length is larger than or equal to 4, do following.
                                        iCopy = iOutputRemaining;
                                    }
                                    else
                                    {
                                        /// If the remaining data length is smaller than 4, do following.
                                        iCopy = iInputRemaining;
                                    }
                                    /// Copy.
                                    if (iCopy > 0) { Array.Copy(byteDataInQueue, iIndex1, IncomingLengthBuffer, IncomingLengthIndex, iCopy); }
                                    IncomingLengthIndex += iCopy;
                                    iIndex1 += iCopy;
                                    /// Check if finish to copy the length.
                                    if (IncomingLengthIndex < 4) { }/// do nothing and continue to loop.
                                    else
                                    {
                                        if (IncomingLengthIndex == 4)
                                        {
                                            IncomingContentSize = BitConverter.ToInt32(IncomingLengthBuffer, 0);
                                            if (IncomingContentBuffer != null) { IncomingContentBuffer = null; }
                                            if (IncomingContentSize > 0) { IncomingContentBuffer = new byte[IncomingContentSize]; }

                                            IsIncomingDataLength = false;/// go to 3rd state.
                                            IncomingContentIndex = 0;/// reset the index of content again.
                                        }
                                        else
                                        {
                                            Logger?.Error("AnalyzeIncomingBuffer. Too many bytes of length are copied. It is unexpected. Server socket = {0}, local socket = {1}", remoteEndPoint, localEndPoint);
                                            return;
                                        }
                                    }
                                }
                                else
                                {
                                    /// If they are bytes of content, do following.
                                    /// assume the memory is assigned for content.
                                    iInputRemaining = byteDataInQueue.Length - iIndex1;
                                    iOutputRemaining = IncomingContentSize - IncomingContentIndex;
                                    if (iInputRemaining >= iOutputRemaining)
                                    {
                                        /// if remaining data length is larger than or equal to the remaining data required to read to the buffer, do following.
                                        iCopy = iOutputRemaining;
                                    }
                                    else
                                    {
                                        /// if remaining data length is smaller than the remaining data required to read to the buffer, do following.
                                        iCopy = iInputRemaining;
                                    }
                                    /// Copy.
                                    if (iCopy > 0) { Array.Copy(byteDataInQueue, iIndex1, IncomingContentBuffer, IncomingContentIndex, iCopy); }
                                    IncomingContentIndex += iCopy;
                                    iIndex1 += iCopy;
                                    /// Check if finish to copy the content.
                                    if (IncomingContentIndex < IncomingContentSize) { }/// do nothing and continue to loop.
                                    else
                                    {
                                        if (IncomingContentIndex == IncomingContentSize)
                                        {
                                            AddDataToIncomingDataQueue(DateTime.Now, IncomingContentBuffer);
                                            /// go back to 1st state.
                                            IsIncomingDataFinished = true;
                                            IsIncomingDataLength = true;
                                        }
                                        else
                                        {
                                            Logger?.Error("AnalyzeIncomingBuffer. Too many bytes of content are copied. It is unexpected. Server socket = {0}, local socket = {1}", remoteEndPoint, localEndPoint);
                                            return;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    // /// The case that data does NOT contain the length as header.
                    // int iLengthContent = byteDataInQueue.Length;
                    // if (iLengthContent < 1) { return; }
                    // byte[] vbyteContent = new byte[iLengthContent];
                    // Array.Copy(byteDataInQueue, 0, vbyteContent, 0, iLengthContent);
                    // AddDataToIncomingDataQueue(DateTime.Now, true, ref vbyteContent);

                    /// TT edited on 2018-08-21 to lock the common resource.
                    while (TryDequeueAtIncomingBufferQueue(out byteDataInQueue))
                    {
                        //if (byteDataInQueue == null || byteDataInQueue.Length < 1)
                        if ((byteDataInQueue?.Length ?? 0) < 1)
                        {
                            /// if there is a zero-byte content, assume that it is the end of data.
                            if (IncomingBufferList != null)
                            {
                                /// https://stackoverflow.com/questions/4868113/convert-listbyte-to-one-byte-array
                                //byte[] byteFinalData = mListOfIncomingBuffer.SelectMany(x => x).ToArray();
                                byte[] byteFinalData = null;
                                System.Collections.Generic.IEnumerable<byte> oIEnumerable = null;
                                if (IncomingBufferList != null) { oIEnumerable = IncomingBufferList.SelectMany(x => x); }
                                if (oIEnumerable != null) { byteFinalData = oIEnumerable.ToArray(); }
                                AddDataToIncomingDataQueue(DateTime.Now, byteFinalData);
                                /// Release memory of the list.
                                IncomingBufferList.Clear();
                                IncomingBufferList = null;
                            }
                        }
                        else
                        {
                            /// add data into temp list.
                            if (IncomingBufferList == null) { IncomingBufferList = new List<byte[]>(); }
                            IncomingBufferList.Add(byteDataInQueue);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger?.Error("Server socket = {0}, local socket = {1}", remoteEndPoint, localEndPoint);
                Logger?.Error(ex);
            }
        }

        /// Thread to analyze the incoming buffer.
        private void ProcessAnalyzeIncomingBuffer()
        {
            DateTime tNow, tRef;
            string remoteEndPoint = "";
            string localEndPoint = "";
            try
            {
                tRef = DateTime.Now.AddHours(-1);
                remoteEndPoint = RemoteEndPoint;
                localEndPoint = LocalEndPoint;
                Logger?.Debug("TCP Client begins to analyze the incoming buffer. Server socket = {0}, local socket = {1}", remoteEndPoint, localEndPoint);
                //while (IsExit == false && ClientSocket != null && ClientSocket.Connected)
                while (IsExit == false && (ClientSocket?.Connected ?? false))
                {
                    tNow = DateTime.Now;
                    if (ReceiveDataInterval == 0 || (ReceiveDataInterval > 0 && (int)(tNow - tRef).TotalSeconds >= ReceiveDataInterval))
                    {
                        tRef = tNow;
                        if (EnableAnalyzeIncomingData) { AnalyzeIncomingBuffer(); }
                    }
                    if (SleepingIntervalInMS >= 0) { System.Threading.Thread.Sleep(SleepingIntervalInMS); }
                }
                Logger?.Debug("TCP Client stops to analyze the incoming buffer. Server socket = {0}, local socket = {1}", remoteEndPoint, localEndPoint);
            }
            catch (Exception ex)
            {
                Logger?.Error("Server socket = {0}, local socket = {1}", remoteEndPoint, localEndPoint);
                Logger?.Error(ex);
            }
        }

        /// Thread to receive data.
        private void ProcessReceiveData()
        {
            DateTime tNow, tRef;
            string remoteEndPoint = "";
            string localEndPoint = "";
            try
            {
                tRef = DateTime.Now.AddHours(-1);
                remoteEndPoint = RemoteEndPoint;
                localEndPoint = LocalEndPoint;
                Logger?.Debug("TCP Client begins to receive data. Server socket = {0}, local socket = {1}", remoteEndPoint, localEndPoint);
                //while (IsExit == false && ClientSocket != null && ClientSocket.Connected)
                while (IsExit == false && (ClientSocket?.Connected ?? false))
                {
                    tNow = DateTime.Now;
                    if (ReceiveDataInterval == 0 || (ReceiveDataInterval > 0 && (int)(tNow - tRef).TotalSeconds >= ReceiveDataInterval))
                    {
                        tRef = tNow;
                        ReceiveByteArray();
                    }
                    if (SleepingIntervalInMS >= 0) { System.Threading.Thread.Sleep(SleepingIntervalInMS); }
                }
                Logger?.Debug("TCP Client stops to receive data. Server socket = {0}, local socket = {1}", remoteEndPoint, localEndPoint);
            }
            catch (Exception ex)
            {
                Logger?.Error("Server socket = {0}, local socket = {1}", remoteEndPoint, localEndPoint);
                Logger?.Error(ex);
            }
        }

        /// Disconnect sockets which exceed maximum connection duration.
        private void DisconnectSocketExceedingMaxConnectionDuration(DateTime tNow)
        {
            try
            {
                if (MaxConnectionDuration <= 0) { return; }
                if ((int)(tNow - InitialDateTime).TotalSeconds > MaxConnectionDuration)
                {
                    Logger?.Debug("Disconnect socket as exceeding maximum connection duration {0} second(s). Server socket = {1}. Local socket = {2}", MaxConnectionDuration, RemoteEndPoint, LocalEndPoint);
                    Disconnect();
                }
            }
            catch (Exception ex)
            {
                Logger?.Error("Server socket = {0}, local socket = {1}", RemoteEndPoint, LocalEndPoint);
                Logger?.Error(ex);
            }
        }

        private void DisconnectSocketExceedingMaxIdleDuration(DateTime tNow)
        {
            try
            {
                if (MaxIdleDuration < 0) { return; }
                if ((int)(tNow - LastTransferDateTime).TotalSeconds > MaxIdleDuration)
                {
                    Logger?.Debug("Disconnect socket as exceeding maximum idle duration {0} second(s). Server socket = {1}. Local socket = {2}", MaxIdleDuration, RemoteEndPoint, LocalEndPoint);
                    Disconnect();
                }
            }
            catch (Exception ex)
            {
                Logger?.Error("Server socket = {0}, local socket = {1}", RemoteEndPoint, LocalEndPoint);
                Logger?.Error(ex);
            }
        }

        private void ProcessOtherProcesses()
        {
            DateTime tNow, tRef;
            string remoteEndPoint = "";
            string localEndPoint = "";
            byte[] oByteArrayHeartbeat = null;
            try
            {
                tRef = DateTime.Now.AddHours(-1);
                remoteEndPoint = RemoteEndPoint;
                localEndPoint = LocalEndPoint;
                Logger?.Debug("TCP Client begins to run other processes. Server socket = {0}, local socket = {1}", remoteEndPoint, localEndPoint);
                if (!this.ContainLengthAsHeader) { oByteArrayHeartbeat = this.HeartbeatData; }
                while (IsExit == false && (ClientSocket?.Connected ?? false))
                {
                    tNow = DateTime.Now;
                    /// Check if it exceeds maximum connection duration. If yes, disconnect it.
                    DisconnectSocketExceedingMaxConnectionDuration(tNow);
                    /// Check if it exceeds maximum idle duration. If yes, disconnect it.
                    DisconnectSocketExceedingMaxIdleDuration(tNow);
                    /// Send heartbeat to clients.
                    if (this.HeartbeatInterval == 0 || (this.HeartbeatInterval > 0 && ((int)(tNow-tRef).TotalSeconds >= this.HeartbeatInterval)))
                    {
                        tRef = tNow;
                        SendByteArray(oByteArrayHeartbeat);
                    }
                    if (SleepingIntervalInMS >= 0) { System.Threading.Thread.Sleep(SleepingIntervalInMS); }
                }
                Logger?.Debug("TCP Client stops to run other processes. Server socket = {0}, local socket = {1}", remoteEndPoint, localEndPoint);
            }
            catch (Exception ex)
            {
                Logger?.Error("Server socket = {0}, local socket = {1}", remoteEndPoint, localEndPoint);
                Logger?.Error(ex);
            }
        }

        /// Connect.
        /// Return value = socket if connect successfully. Otherwise, null.
        /// sServerHost = server host
        /// iServerPort = server port
        private static System.Net.Sockets.Socket Connect(string serverHost, int serverPort)
        {
            System.Net.IPAddress[] IPs;
            System.Net.Sockets.Socket oClientSocket = null;
            System.Net.IPAddress oIP = null;
            string localSocket = "";
            try
            {
                IPs = System.Net.Dns.GetHostAddresses(serverHost);
                //if (IPs == null || IPs.Length < 1)
                if ((IPs?.Length ?? 0) < 1)
                {
                    Logger?.Error("TCP Client cannot find IP address for {0}", serverHost);
                    return null;
                }
                /// Find the first IPv4 address, as IPv6 addresses are not supported.
                int i = 0; bool bLoop = true;
                while (bLoop && i < IPs.Length)
                {
                    oIP = IPs[i];
                    if (oIP != null && string.IsNullOrEmpty(oIP.ToString()) == false && oIP.ToString().Contains(":") == false)
                    { bLoop = false; }
                    else { i += 1; }
                }
                if (bLoop)
                {
                    Logger?.Error("TCP Client cannot find IPv4 address for {0}", serverHost);
                    return null;
                }
                Logger?.Debug("TCP Client finds IP address of server is {0}", oIP);
                /// Client socket.
                oClientSocket = new System.Net.Sockets.Socket(System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
                //oClientSocket.Connect(IPs[0], iServerPort);
                oClientSocket.Connect(oIP, serverPort);
                if ((oClientSocket?.Connected ?? false) == false)
                {
                    Logger?.Error("TCP Client cannot establish connection to the server {0}:{1}", serverHost, serverPort);
                    Disconnect(ref oClientSocket);
                    return null;
                }
                else
                {
                    //if (oClientSocket != null && oClientSocket.LocalEndPoint != null) { sLocalSocket = oClientSocket.LocalEndPoint.ToString(); }
                    localSocket = oClientSocket?.LocalEndPoint?.ToString();
                    Logger?.Debug("TCP Client connects successfully. Server socket = {0}:{1}, Local socket = {2}", serverHost, serverPort, localSocket);
                    return oClientSocket;
                }
            }
            catch (Exception ex)
            {
                Logger?.Error("Local socket = {0}:{1}", serverHost, serverPort);
                Logger?.Error(ex);
                Disconnect(ref oClientSocket);
                return null;
            }
        }

        /// Stop to connect TCP Server.
        public void StopConnection() { StopConnection(2); }
        public void StopConnection(int timeoutInSecond) { StopConnection(timeoutInSecond, this.SleepingIntervalInMS); }
        public void StopConnection(int timeoutInSecond, int sleepingIntervalInMS)
        {
            DateTime tRef;
            bool b;
            string remoteEndPoint = "";
            string localEndPoint = "";
            try
            {
                this.IsExit = true;
                remoteEndPoint = RemoteEndPoint;
                localEndPoint = LocalEndPoint;
                Disconnect(ref ClientSocket);
                if (timeoutInSecond < 0) { timeoutInSecond = 0; }
                tRef = DateTime.Now;
                b = true;
                while (b && (int)(DateTime.Now - tRef).TotalSeconds < timeoutInSecond)
                {
                    b = false;
                    if (ThreadToReceiveData?.IsAlive ?? false) { b = true; }
                    //else if (ThreadToAnalyzeIncomingBuffer != null || ThreadToAnalyzeIncomingBuffer.IsAlive) { b = true; }
                    else if (ThreadToAnalyzeIncomingBuffer?.IsAlive ?? false) { b = true; }
                    if (b && sleepingIntervalInMS >= 0) { System.Threading.Thread.Sleep(sleepingIntervalInMS); }
                }
                AbortThread(ref ThreadToReceiveData);
                AbortThread(ref ThreadToAnalyzeIncomingBuffer);
                /// Release the memory in buffer.
                lock (IncomingBufferQueueLocker)
                {
                    if (IncomingBufferQueue != null)
                    {
                        IncomingBufferQueue.Clear();
                        IncomingBufferQueue = null;
                    }
                }
                IncomingContentBuffer = null;
                IncomingLengthBuffer = null;
            }
            catch (Exception ex)
            {
                Logger?.Error("Server socket = {0}, local socket = {1}", remoteEndPoint, localEndPoint);
                Logger?.Error(ex);
            }
        }

        /// Start to connect TCP Server.
        public bool StartConnection()
        {
            try
            {
                /// disconnect first.
                Disconnect(ref ClientSocket);
                /// check if IncomingDataQueue is initialized.
                if (IncomingDataQueue == null)
                {
                    Logger?.Error("TCP Client finds that IncomingDataQueue is not initialized. Stop connection. Server socket = {0}:{1}", ServerHost, ServerPort);
                    return false;
                }
                /// connect.
                ClientSocket = Connect(ServerHost, ServerPort);
                if (ClientSocket == null || ClientSocket.Connected == false)
                {
                    Disconnect(ref ClientSocket);
                    return false;
                }
                /// thread to receive data.
                ThreadToReceiveData = new System.Threading.Thread(new System.Threading.ThreadStart(ProcessReceiveData))
                {
                    Name = "ProcessReceiveData-" + ServerHost + ":" + ServerPort.ToString()
                };
                ThreadToReceiveData.Start();/// This "start" statement can be divided from this function.
                /// thread to analyze the incoming buffer.
                ThreadToAnalyzeIncomingBuffer = new System.Threading.Thread(new System.Threading.ThreadStart(ProcessAnalyzeIncomingBuffer))
                {
                    Name = "ProcessAnalyzeIncomingBuffer-" + ServerHost + ":" + ServerPort.ToString()
                };
                ThreadToAnalyzeIncomingBuffer.Start();
                /// thread to other processes.
                ThreadToOtherProcesses = new System.Threading.Thread(new System.Threading.ThreadStart(ProcessOtherProcesses))
                {
                    Name = "ProcessOtherProcesses-" + ServerHost + ":" + ServerPort.ToString()
                };
                ThreadToOtherProcesses.Start();
                /// 
                return true;
            }
            catch (Exception ex)
            {
                Logger?.Error("Server socket = {0}:{1}", ServerHost, ServerPort);
                Logger?.Error(ex);
                return false;
            }
        }

        /// Connect to TCP Server and send the byte array. After sending, the socket is shutdown and closed immediately.
        /// Return value = true if success. Otherwise, fail.
        /// miDelayDisconnetInMS = Delay interval in milli-second to disconnect server. The delay disconnect to server is required. Otherwise, data is no time to be sent to the server.
        /// https://docs.microsoft.com/en-us/dotnet/framework/network-programming/synchronous-client-socket-example
        public static bool ConnectServerAndSendData(string serverHost, int serverPort, bool isContainLengthAsHeader, byte[] data) { return ConnectServerAndSendData(serverHost, serverPort, isContainLengthAsHeader, data, 5000); }
        public static bool ConnectServerAndSendData(string serverHost, int serverPort, bool isContainLengthAsHeader, byte[] data, int delayDisconnetInMS) { return ConnectServerAndSendData(serverHost, serverPort, isContainLengthAsHeader, data, delayDisconnetInMS, 2000000000); }
        public static bool ConnectServerAndSendData(string serverHost, int serverPort, bool isContainLengthAsHeader, byte[] data, int delayDisconnetInMS, int maxDataSize)
        {
            System.Net.Sockets.Socket oClientSocket = null;
            try
            {
                oClientSocket = Connect(serverHost, serverPort);
                if ((oClientSocket?.Connected ?? false) == false) { return false; }
                return SendByteArray(ref oClientSocket, isContainLengthAsHeader, maxDataSize, data);
            }
            catch (Exception ex)
            {
                Logger?.Error("Server socket = {0}:{1}", serverHost, serverPort);
                Logger?.Error(ex);
                return false;
            }
            finally
            {
                try
                {
                    if (delayDisconnetInMS >= 0) { System.Threading.Thread.Sleep(delayDisconnetInMS); }
                    Disconnect(ref oClientSocket);
                }
                catch (Exception ex2)
                {
                    Logger?.Error("Server socket = {0}:{1}", serverHost, serverPort);
                    Logger?.Error(ex2);
                }
            }
        }

        public TTcpClientSocket()
        {
            //ServerHost = "127.0.0.1";
            //ServerPort = 8002;
            InitialDateTime = LastReceivedDateTime = LastTransferDateTime = DateTime.Now;
            //ContainLengthAsHeader = true;
            //EnableAnalyzeIncomingData = true;
            //MaxDataSize = 2000000000;
            //HeartbeatData = System.Text.Encoding.UTF8.GetBytes("~");
            //ReceiveDataInterval = 0;
            //ReceiveTotalBufferSize = 10485760;
            //SleepingIntervalInMS = 100;
            //IncomingDataQueue = null;
            //IncomingDataQueueLocker = null;
        }
    }
}
