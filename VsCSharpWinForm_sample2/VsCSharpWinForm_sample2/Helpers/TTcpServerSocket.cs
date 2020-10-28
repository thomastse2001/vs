using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;

namespace VsCSharpWinForm_sample2.Helpers
{
    public class TTcpServerSocket
    {
        /// TCP Server by a synchronous socket in the threading model.
        /// Data unit is byte.
        /// Updated date: 2020-09-14

        /// http://csharp.net-informations.com/communications/csharp-server-socket.htm
        /// https://docs.microsoft.com/en-us/dotnet/framework/network-programming/synchronous-server-socket-example
        /// https://docs.microsoft.com/en-us/dotnet/framework/network-programming/asynchronous-server-socket-example
        /// http://www.yoda.arachsys.com/csharp/parameters.html
        /// https://blogs.msdn.microsoft.com/oldnewthing/20060801-19/?p=30273
        /// https://www.codeproject.com/Articles/990474/Scalable-Socket-Server
        /// http://www.tutorialsteacher.com/csharp/array-csharp

        public class TcpSocketData
        {
            public DateTime Timestamp = DateTime.MinValue;
            public string Host = "";
            public int Port = -1;
            //public bool IsFinishedReceiving = false;
            public byte[] ByteArray = null;
        }

        /// Parameters for private user.
        private bool IsExit = false;
        private bool IsReadyAccept = false;/// flag to indicate whether it is ready to accept client.
        private System.Net.Sockets.TcpListener ServerSocket = null;
        private System.Threading.Thread ThreadToAcceptClient = null;
        private System.Threading.Thread ThreadToSendData = null;
        //private System.Threading.Thread ThreadToTrimList = null;

        private Queue<TcpSocketData> OutgoingDataQueue = null;
        private readonly object OutgoingDataQueueLocker = new object();
        private Queue<TcpSocketData> FailedOutgoingDataQueue = null;/// queue of TCP outgoing data sent failed.
        private readonly object FailedOutgoingDataQueueLocker = new object();

        /// Properties.
        public static Helpers.TLog Logger { get; set; }
        public int ListeningPort { get; set; } = 8002;
        public int AcceptInterval { get; set; } = 1;/// Time interval in seconds that TCP server accepts client. If it is lesser than 1, the process will do immediately without waiting. The default value is 1.
        public bool ContainLengthAsHeader { get; set; } = true;/// flag to indicate if the data contains length (4-byte integer) as a header of package. The header is used to divide a package unit. The default value is true.
        public bool EnableAnalyzeIncomingData { get; set; } = true;/// flag to enable to analyze the incoming data from server. A package is preserved when adding to the queue of incoming data. Note that large package exceeding the buffer size is divided as several parts when receiving. The users may need to handle how to obtain the complete package if disable this flag. The default value is true.
        private byte[] _HeartbeatData = Encoding.UTF8.GetBytes("~");
        public byte[] HeartbeatData/// Data in byte array of heartbeat. This parameter will be used only if mbContainLengthAsHeader = false. Space and Tab will be neglect. Sending heartbeat is used to check if the client sockets are still connected. The default value is the byte array of ~.
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
        public int HeartbeatInterval { get; set; } = 15;/// Time interval in seconds that TCP server sends heartbeat to all clients. If it is negative, there is no heartbeat. Sending heartbeat is used to check if the connection still occurs. The default value is 15.
        public int MaxClient { get; set; } = 200;/// Maximum TCP clients allowed to be connected by the server. If it is 0, no client is allowed to be connected. If it is negative, no limitation to accept clients. The default value is 200.
        public int MaxConnectionDuration { get; set; } = 600;/// Maximum connection duration in seconds between server and client. If the time exceeds, it will disconnect automatically even the connection is normal. If it is negative, there is no maximum connection duration, hence the connection between server and client can preserve forever. The default value is 600.
        public int MaxIdleDuration { get; set; } = -1;/// Maximum idle duration in seconds between server and client. If the time exceeds, it will disconnect automatically. If it is negative, there is no maximum idle duration, hence it will not check the idle duration. The default value is -1.
        public int MaxDataSend { get; set; } = -1;/// Maximum unit of data sent from the TCP server each time. If it is 0, no data is sent. If it is negative, the process will send all data in the queue without waiting. The default value is -1.
        public int MaxDataSize { get; set; } = 2000000000;/// Maximum size of data in bytes. The default value is 2000000000.
        public int ProcessVerificationInterval { get; set; } = 600;/// Time interval in seconds to verify the process is still running or not. If it is running, a log is written. If the value is negative or zero, it does not verify. The default value is 600.
        public int ReceiveDataInterval { get; set; } = 0;/// Time interval in seconds that TCP server receives Data. If it is 0, the process will do immediately without waiting. If it is negative, no data is received. The default value is 0.
        public int ReceiveTotalBufferSize { get; set; } = 10485760;/// Total buffer size in bytes for receiving data from client, the minimum value is 10485760.
        public int SleepingIntervalInMS { get; set; } = 100;/// Sleeping interval in milliseconds. This sleeping interval helps to avoid the application too busy. If it is negative, no sleep. The default value is 100.

        public Queue<TcpSocketData> IncomingDataQueue = null;/// queue of TCP incoming data.
        public object IncomingDataQueueLocker = null;/// lock object for queue of TCP incoming data.

        public class InnerClient
        {
            /// Parameters for private user.
            private bool IsExit = false;/// flag to indicate if force to disconnect current client object.
            private bool IsIncomingDataFinished = true;/// Whether the incoming data is finished or continuous. True = finished, false = continuous.
            private bool IsIncomingDataLength = true;/// Whether the incoming data is length or content. True = length, false = content.
            private int IncomingContentSize = 0;/// Size of incoming content.
            private int IncomingContentIndex = 0;/// index of incoming content.
            private int IncomingLengthIndex = 0;/// index of incoming length.
            private byte[] IncomingContentBuffer = null;/// buffer for incoming content.
            private byte[] IncomingLengthBuffer = null;/// buffer for incoming length.
            private List<byte[]> IncomingBufferList = null;/// buffer list of TCP incoming data. Only used for mbEnableAnalyzeIncomingData = false.
            private System.Threading.Thread ThreadToAnalyzeIncomingBuffer = null;
            private System.Threading.Thread ThreadToReceiveData = null;
            private System.Net.Sockets.TcpClient ClientSocket = null;
            private Queue<byte[]> IncomingBufferQueue = null;/// buffer queue of TCP incoming data.
            private readonly object IncomingBufferQueueLocker = new object();

            /// Properties.
            public static Helpers.TLog Logger { get; set; }
            //private string _Host = "";
            //public string Host { get { return _Host; } }
            public readonly string Host;
            //private int _Port = -1;
            //public int Port { get { return _Port; } }
            public readonly int Port;
            public string ClientSocketString { get { return Host + ":" + Port.ToString(); } }
            //private DateTime _InitialDateTime = DateTime.MinValue;
            public readonly DateTime InitialDateTime;/// Date time of initialization.
            public DateTime LastReceivedDateTime { get; private set; }/// Date time of last received data. Can read and write it inside class, but read-only outside the class. https://stackoverflow.com/questions/4662180/c-sharp-public-variable-as-writeable-inside-the-class-but-readonly-outside-the-c
            public DateTime LastTransferDateTime { get; private set; }/// Date time of last transfer data. Can read and write it inside class, but read-only outside the class. https://stackoverflow.com/questions/4662180/c-sharp-public-variable-as-writeable-inside-the-class-but-readonly-outside-the-c
            public bool ContainLengthAsHeader { get; set; } = true;
            public bool EnableAnalyzeIncomingData { get; set; } = true;
            public int MaxDataSize { get; set; } = 2000000000;
            public int ProcessVerificationInterval { get; set; } = 600;
            public int ReceiveDataInterval { get; set; } = 0;/// time interval in seconds that TCP server receives data from client.
            public int ReceiveTotalBufferSize { get; set; } = 10485760;/// Total buffer size in bytes for receiving data from client, the default value is 10485760.
            public int SleepingIntervalInMS { get; set; } = 100;
            public bool IsConnected { get { return ClientSocket?.Connected ?? false; } }//{ get { return ClientSocket == null ? false : ClientSocket.Connected; } }
            public Queue<TcpSocketData> IncomingDataQueue = null;/// queue to store incoming data.
            public object IncomingDataQueueLocker = null;
            /// Other parameters.
            public string Username { get; set; }

            static InnerClient() { Logger = TTcpServerSocket.Logger; }

            private bool AbortThread(ref System.Threading.Thread th)
            {
                if (th == null) { return true; }
                try
                {
                    if (th.IsAlive)
                    {
                        Logger?.Debug("Force to abort the thread named {0}", th.Name);
                        th.Abort();
                    }
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
            //private bool AddDataToListOfIncomingData(DateTime tTimestamp, bool bIsFinishedReceiving, ref byte[] oByteData)
            //private bool AddDataToIncomingDataQueue(DateTime tTimestamp, ref byte[] oByteData)
            private bool AddDataToIncomingDataQueue(DateTime t, byte[] data)
            {
                try
                {
                    if (IncomingDataQueue == null)
                    {
                        Logger?.Error("Client in TCP Server meets NULL outter server. Client socket = {0}", ClientSocketString);
                        return false;
                    }
                    // if (mOutterServer.IncomingDataQueue == null)
                    // {
                    // LocalLogger("[error] TCP Server cannot put data to IncomingDataQueue as it is NOT initialized. [" + _Host + ":" + _Port.ToString() + "].");
                    // return false;
                    // }
                    /// TT edited on 2018-08-21 to lock the common resource.
                    bool b = true;
                    if (IncomingDataQueueLocker == null) { b = IncomingDataQueue == null; }
                    else { lock (IncomingDataQueueLocker) { b = IncomingDataQueue == null; } }
                    if (b)
                    {
                        Logger?.Error("TCP Server cannot put data to IncomingDataQueue as it is NOT initialized. Client socket = {0}", ClientSocketString);
                        return false;
                    }
                    TcpSocketData oData = new TcpSocketData()
                    {
                        Timestamp = t,
                        Host = this.Host,
                        Port = this.Port,
                        //IsFinishedReceiving = bIsFinishedReceiving,
                        ByteArray = data
                    };
                    int iLength = 0;
                    if (data != null) { iLength = data.Length; }
                    if (IncomingDataQueueLocker == null) { IncomingDataQueue.Enqueue(oData); }
                    else { lock (IncomingDataQueueLocker) { IncomingDataQueue.Enqueue(oData); } }
                    Logger?.Debug("TCP Server adds the received data to IncomingDataQueue. Client socket = {0}, Byte Length = {1}", ClientSocketString, iLength);
                    return true;
                }
                catch (Exception ex)
                {
                    Logger?.Error("Client socket = {0}", ClientSocketString);
                    Logger?.Error(ex);
                    return false;
                }
            }

            /// Close and stop the TCP client socket. Set it to nothing.
            public void Disconnect()
            {
                if (ClientSocket == null) { return; }
                try
                {
                    Logger?.Debug("TCP Server disconnects. Client socket = {0}", ClientSocketString);
                    if (ClientSocket.Connected) { ClientSocket.Close(); }
                }
                catch (Exception ex)
                {
                    Logger?.Error("Client socket = {0}", ClientSocketString);
                    Logger?.Error(ex);
                }
                finally { ClientSocket = null; }
            }

            /// Pack the length of data into the beginning of data, in order to let the receiver easy to recognize each individual unit of data.
            public byte[] PackData(byte[] data)
            {
                try
                {
                    int iLength = 0;
                    if (data != null) { iLength = data.Length; }
                    if (iLength > MaxDataSize)
                    {
                        Logger?.Error("Exceed the maximum data size {0}. Client socket = {1}, Data size = {2}", MaxDataSize, ClientSocketString, iLength);
                        return null;
                    }
                    byte[] rByte = new byte[4 + iLength];
                    BitConverter.GetBytes(iLength).CopyTo(rByte, 0);
                    if (data != null) { data.CopyTo(rByte, 4); }
                    return rByte;
                }
                catch (Exception ex)
                {
                    Logger?.Error("Client socket = {0}", ClientSocketString);
                    Logger?.Error(ex);
                    return null;
                }
            }

            // /// Analyze and pack the incoming data for ContainLengthAsHeader = false.
            // /// Not well test, need to test for large file.
            // private void AnalyzeAndPackIncomingData()
            // {
            // int iTotalLength = 0;
            // int iLength = 0;
            // byte[] vByteArrayFinal = null; // final result of byte array.
            // byte[] vByteArrayTemp = null; // temp buffer.
            // byte[] vByteArrayInQueue = null; // data in queue.
            // try
            // {
            // if (mIncomingBufferQueue == null && mIncomingBufferQueue.Count < 1) { return; }
            // // 
            // while (mIncomingBufferQueue.Count > 0)
            // {
            // vByteArrayInQueue = mIncomingBufferQueue.Dequeue();
            // if (vByteArrayInQueue == null || vByteArrayInQueue.Length < 1)
            // {
            // // if there is a zero-byte content, assume that it is the end of data.
            // if (vByteArrayFinal != null)
            // //{ AddDataToIncomingDataQueue(DateTime.Now, ref vByteArrayFinal); }
            // { AddDataToIncomingDataQueue(DateTime.Now, vByteArrayFinal); }
            // // reset the parameters.
            // iTotalLength = 0;
            // vByteArrayFinal = null;
            // }
            // else
            // {
            // /// https://stackoverflow.com/questions/4868113/convert-listbyte-to-one-byte-array
            // /// This below code should be re-written, because the coping process may be time consuming.
            // iLength = vByteArrayInQueue.Length;
            // // Set a temp buffer pointing to the final byte array.
            // //if (vByteArrayFinal == null) { vByteArrayTemp = null; }
            // //else { vByteArrayTemp = vByteArrayFinal; }
            // vByteArrayTemp = vByteArrayFinal;
            // // Assign new memory with new length for final byte array.
            // vByteArrayFinal = new byte[iTotalLength + iLength];
            // // Copy data from temp buffer (previous data) to final byte array.
            // if (vByteArrayTemp != null) { Array.Copy(vByteArrayTemp, 0, vByteArrayFinal, 0, iTotalLength); }
            // // Copy data from byte array in queue (new data) to final byte array.
            // Array.Copy(vByteArrayInQueue, 0, vByteArrayFinal, iTotalLength, iLength);
            // // Set the new length.
            // iTotalLength += iLength;
            // // Release the temp buffer.
            // vByteArrayTemp = null;
            // }
            // }
            // }
            // catch (Exception ex) { LocalLogger("[error] " + System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + ". [" + _Host + ":" + _Port.ToString() + "]. " + ex.Message); }
            // }

            /// Send data to client socket.
            /// Return value = true if send data successfully. Otherwise, fail.
            /// data = byte array that will be sent
            public bool SendByteArray(byte[] data)
            {
                byte[] byteData = null;
                try
                {
                    if (ClientSocket == null || ClientSocket.Connected == false) { return false; }
                    if (data == null) { data = new byte[0]; }
                    System.Net.Sockets.NetworkStream oStream = ClientSocket.GetStream();
                    if (!oStream.CanWrite)
                    {
                        Logger?.Error("TCP Server sends data which cannot write to client {0}", ClientSocketString);
                        return false;
                    }
                    if (ContainLengthAsHeader) { byteData = PackData(data); }/// Pack data with the length as header.
                    else { byteData = data; }/// NOT pack data.
                    if (byteData == null) { return false; }/// Checking.
                    oStream.Write(byteData, 0, byteData.Length);/// Send.
                    oStream.Flush();
                    /// Must not use oStream.Close() at the end. Otherwise, it will affect the "Receive" function that data cannot be received.
                    //int iLengthOrig = 0;
                    //int iLengthSent = 0;
                    //if (vByteArray != null) { iLengthOrig = vByteArray.Length; }
                    //if (byteData != null) { iLengthSent = byteData.Length; }
                    this.LastTransferDateTime = DateTime.Now;
                    Logger?.Debug("TCP Server sends data to client {0}, #Original = {1}, #Sent = {2}", ClientSocketString, data?.Length ?? 0, byteData?.Length ?? 0);
                    return true;
                }
                catch (Exception ex)
                {
                    Logger?.Error("Client socket = {0}", ClientSocketString);
                    Logger?.Error(ex);
                    Disconnect();
                    return false;
                }
                //finally { oStream = null; }
            }

            /// Receive data from client socket.
            /// http://www.overclock.net/t/1376406/c-tcp-proper-way-to-receive-byte
            public void ReceiveByteArray()
            {
                System.Net.Sockets.NetworkStream oStream;
                int iReceived;/// how many bytes are total received.
                byte[] byteBuffer;
                byte[] byteData;
                //int iOffset;
                //int i;
                //int iCurrReceived;/// how many bytes are received currently.
                //int iUpperLimit;/// Number of bytes of upper limit that stop receiving data.
                //byte[] byteTemp;
                //TcpSocketData oData;
                try
                {
                    //if (ClientSocket == null || ClientSocket.Connected == false) { return; }
                    if ((ClientSocket?.Connected ?? false) == false) { return; }
                    oStream = ClientSocket.GetStream();
                    if (!oStream.CanRead)
                    {
                        Logger?.Error("TCP Server receives data which cannot read from client {0}", ClientSocketString);
                        return;
                    }
                    if (oStream.DataAvailable)
                    {
                        byteBuffer = new byte[ReceiveTotalBufferSize];/// 10M bytes.
                        //i = (int)mTcpClient.ReceiveBufferSize;
                        //if (i > miReceiveTotalBufferSize) { i = miReceiveTotalBufferSize; }
                        //iUpperLimit = miReceiveTotalBufferSize - i + 1;
                        //byteTemp = new byte[i]; // 65536.
                        // Must use a loop to receive data.
                        //iCurrReceived = 0; iReceived = 0;
                        //do {
                        //    iCurrReceived = oStream.Read(byteTemp, 0, byteTemp.Length);
                        //    Array.Copy(byteTemp, 0, byteBuffer, iReceived, iCurrReceived);
                        //    iReceived += iCurrReceived;
                        //    // LocalLogger(true, "[debug] TCP Server receives data from client [" + _Host + ":" + _Port.ToString() + "]. #Received = " + iReceived.ToString() + ". #CurrentlyReceived " + iCurrReceived.ToString());
                        //} while ((iReceived < iUpperLimit) && oStream.DataAvailable);
                        //if ((iReceived < miReceiveTotalBufferSize) && oStream.DataAvailable)
                        //{
                        //    iCurrReceived = oStream.Read(byteTemp, 0, );
                        //}

                        iReceived = 0;
                        while (iReceived < byteBuffer.Length && oStream.DataAvailable)
                        { iReceived += oStream.Read(byteBuffer, iReceived, byteBuffer.Length - iReceived); }
                        /// If receive empty bytes, quit this function.
                        if (iReceived < 1) { return; }
                        byteData = new byte[iReceived];
                        Array.Copy(byteBuffer, 0, byteData, 0, iReceived);
                        //byteTemp = null;
                        byteBuffer = null;
                        this.LastReceivedDateTime = this.LastTransferDateTime = DateTime.Now;
                        Logger?.Debug("TCP Server receives data. Client socket = {0}, #Received = {1}", ClientSocketString, iReceived);
                        if (EnableAnalyzeIncomingData)
                        {
                            /// Add data to the incoming buffer for analyzing.
                            lock (IncomingBufferQueueLocker)
                            {
                                if (IncomingBufferQueue == null) { IncomingBufferQueue = new Queue<byte[]>(); }
                                IncomingBufferQueue.Enqueue(byteData);
                            }
                        }
                        else
                        {
                            AddDataToIncomingDataQueue(DateTime.Now, byteData);/// Add data to the list directly.
                        }
                        /// Must not use oStream.Close() at the end. Otherwise, data cannot be received.
                    }
                    else
                    {
                        // LocalLogger(true, "[debug] TCP Server receives EMPTY data from client [" + _Host + ":" + _Port.ToString() + "].");
                        // byteData = null;
                        // /// Add data to the list. IsFinishedReceiving = true.
                        // AddDataToListOfIncomingData(DateTime.Now, true, ref byteData);
                        // //Disconnect();

                        //LocalLogger(true, "[debug] TCP Server receives EMPTY data from client [" + _Host + ":" + _Port.ToString() + "].");
                        // if (!mbContainLengthAsHeader)
                        // {
                        // /// Add a null data to the list, to indicate it is the end of data.
                        // if (mIncomingBufferQueue == null) { mIncomingBufferQueue = new System.Collections.Generic.Queue<byte[]>(); }
                        // mIncomingBufferQueue.Enqueue(null);
                        // /// analyze and pack them.
                        // AnalyzeAndPackIncomingData();
                        // }

                        /// Only do the below if mbContainLengthAsHeader = false.
                        if (!ContainLengthAsHeader)
                        {
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
                                AddDataToIncomingDataQueue(DateTime.Now, null);/// Add data to the list directly.
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger?.Error("Client socket = {0}", ClientSocketString);
                    Logger?.Error(ex);
                    Disconnect();
                }
                finally { oStream = null; byteBuffer = null; }
            }

            private bool TryDequeueAtIncomingBufferQueue(out byte[] byteOutput)
            {
                bool bReturn = false;
                try
                {
                    lock (IncomingBufferQueueLocker)
                    {
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
                    Logger?.Error("Client socket = {0}", ClientSocketString);
                    Logger?.Error(ex);
                    byteOutput = null;
                    return false;
                }
            }

            /// Analyze the incoming buffer.
            /// 3 statuses.
            /// State, mbIncomingDataFinished, mbIncomingDataIsLength.
            /// 1st state, True, True. The previous data is finished. And if there is an incoming data, should analyze the length first.
            /// 2nd state, False, True. set this state if the current incoming data arrives. And should analyze the length.
            /// 3rd state, False, False. After analyzing the length, set to analyze the content.
            private void AnalyzeIncomingBuffer()
            {
                byte[] byteDataInQueue;
                int iIndex1;/// Number of bytes that already analyzied in the data of the queue.
                int iCopy, iInputRemaining, iOutputRemaining;
                try
                {
                    //if (mIncomingBufferQueue == null) { return; }
                    //LocalLogger(true, "[debug] " + System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + ". [" + _Host + ":" + _Port.ToString() + "]. ContainLengthAsHeader = " + mbContainLengthAsHeader.ToString());
                    if (ContainLengthAsHeader)
                    {
                        /// The case that data contains the length as header.
                        // while (IncomingBufferQueue.Count > 0)
                        // {
                        // byteDataInQueue = IncomingBufferQueue.Dequeue();

                        /// TT edited on 2018-08-21 to lock the common resource.
                        while (TryDequeueAtIncomingBufferQueue(out byteDataInQueue))
                        {
                            if (byteDataInQueue != null)
                            {
                                Logger?.Debug("AnalyzeIncomingBuffer. Client socket = {0}, byteDataInQueue.Length = {1}", ClientSocketString, byteDataInQueue.Length);
                                /// Loop through the buffer item of the queue.
                                iIndex1 = 0;
                                while (iIndex1 < byteDataInQueue.Length)
                                {
                                    /// If they are bytes of length, do following.
                                    if (IsIncomingDataLength)
                                    {
                                        /// if the previous data is finished, do following.
                                        if (IsIncomingDataFinished)
                                        {
                                            IsIncomingDataFinished = false;/// go to 2nd state.
                                            IncomingLengthIndex = 0;
                                            IncomingContentIndex = 0;
                                            IncomingContentSize = 0;
                                            /// Assign 4 bytes to the Length buffer.
                                            if (IncomingLengthBuffer != null) { IncomingLengthBuffer = null; }
                                            IncomingLengthBuffer = new byte[4];
                                        }
                                        /// Determine the number of bytes to be read and copied.
                                        iInputRemaining = byteDataInQueue.Length - iIndex1;/// number of bytes that input remains to be read.
                                        iOutputRemaining = 4 - IncomingLengthIndex;/// number of bytes that output remains to be write.
                                        if (iInputRemaining >= iOutputRemaining)
                                        {
                                            /// if remaining data length is larger than or equal to 4, do following.
                                            iCopy = iOutputRemaining;
                                        }
                                        else
                                        {
                                            /// if the remaining data length is smaller than 4, do following.
                                            iCopy = iInputRemaining;
                                        }
                                        /// Copy.
                                        Logger?.Debug("AnalyzeIncomingBuffer. Copy bytes from queue to length buffer. Client socket = {0}, iIndex1 = {2}, miIncomingLengthIndex = {3}, iCopy = {4}", ClientSocketString, iIndex1, IncomingLengthIndex, iCopy);
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
                                                Logger?.Debug("AnalyzeIncomingBuffer. Client socket = {0}, Get content length = {1}", ClientSocketString, IncomingContentSize);
                                                if (IncomingContentBuffer != null) { IncomingContentBuffer = null; }
                                                if (IncomingContentSize > 0) { IncomingContentBuffer = new byte[IncomingContentSize]; }

                                                IsIncomingDataLength = false;/// go to 3rd state.
                                                IncomingContentIndex = 0;/// reset the index of content again.
                                            }
                                            else
                                            {
                                                Logger?.Debug("AnalyzeIncomingBuffer. Too many bytes of length are copied. It is unexpected. Client socket = {0}", ClientSocketString);
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
                                        Logger?.Debug("AnalyzeIncomingBuffer. Copy bytes from queue to content buffer. Client socket = {0}, iIndex1 = {1}, miIncomingContentIndex = {2}, iCopy = {3}", ClientSocketString, iIndex1, IncomingLengthIndex, iCopy);
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
                                                Logger?.Debug("AnalyzeIncomingBuffer. Too many bytes of content are copied. It is unexpected. Client socket = {0}", ClientSocketString);
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
                        // //AddDataToListOfIncomingData(DateTime.Now, true, ref vbyteContent);
                        // AddDataToIncomingDataQueue(DateTime.Now, ref vbyteContent);

                        // while (mIncomingBufferQueue != null && mIncomingBufferQueue.Count > 0)
                        // {
                        // byteDataInQueue = mIncomingBufferQueue.Dequeue();

                        /// TT edited on 2018-08-21 to lock the common resource.
                        while (TryDequeueAtIncomingBufferQueue(out byteDataInQueue))
                        {
                            if (byteDataInQueue == null || byteDataInQueue.Length < 1)
                            {
                                /// if there is a zero-byte content, assume that it is the end of data.
                                //if (IncomingBufferList != null && IncomingBufferList.Count > 0)
                                if (IncomingBufferList != null)
                                {
                                    // https://stackoverflow.com/questions/4868113/convert-listbyte-to-one-byte-array
                                    //byte[] byteFinalData = IncomingBufferList.SelectMany(x => x).ToArray();
                                    byte[] byteFinalData = null;
                                    IEnumerable<byte> oIEnumerable = null;
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
                    Logger?.Error("Client socket = {0}, ContainLengthAsHeader = {1}", ClientSocketString, ContainLengthAsHeader);
                    Logger?.Error(ex);
                }
            }

            /// Thread to analyze the incoming buffer.
            private void ProcessAnalyzeIncomingBuffer()
            {
                DateTime tNow, tRef, tRefLog;
                try
                {
                    Logger?.Debug("TCP Server begins to analyze the incoming buffer. Client socket = {0}", ClientSocketString);
                    tRefLog = DateTime.Now;
                    tRef = tRefLog.AddHours(-1);
                    while (IsExit == false && (this.ClientSocket?.Connected ?? false))
                    {
                        tNow = DateTime.Now;
                        if (ReceiveDataInterval == 0 || (ReceiveDataInterval > 0 && (int)(tNow - tRef).TotalSeconds >= ReceiveDataInterval))
                        {
                            tRef = tNow;
                            if (EnableAnalyzeIncomingData) { AnalyzeIncomingBuffer(); }
                        }
                        if (ProcessVerificationInterval > 0 && (int)(tNow - tRefLog).TotalSeconds >= ProcessVerificationInterval)
                        {
                            tRefLog = tNow;
                            Logger?.Debug(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + " is running. Client socket = {0}:{1}", Host, Port);
                        }
                        if (SleepingIntervalInMS >= 0) { System.Threading.Thread.Sleep(SleepingIntervalInMS); }
                    }
                    Logger?.Debug("TCP Server stops to analyze the incoming buffer. Client socket = {0}", ClientSocketString);
                }
                catch (Exception ex)
                {
                    Logger?.Error("Client socket = {0}, ContainLengthAsHeader = {1}", ClientSocketString, ContainLengthAsHeader);
                    Logger?.Error(ex);
                }
            }

            /// Thread to receive data.
            private void ProcessReceiveData()
            {
                DateTime tNow, tRef, tRefLog;
                try
                {
                    Logger?.Debug("TCP Server begins to receive data. Client socket = {0}", ClientSocketString);
                    tRefLog = DateTime.Now;
                    tRef = tRefLog.AddHours(-1);
                    //while (IsExit == false && ClientSocket != null && ClientSocket.Connected)
                    while (IsExit == false && (ClientSocket?.Connected ?? false))
                    {
                        tNow = DateTime.Now;
                        if (ReceiveDataInterval == 0 || (ReceiveDataInterval > 0 && (int)(tNow - tRef).TotalSeconds >= ReceiveDataInterval))
                        {
                            tRef = tNow;
                            ReceiveByteArray();
                        }
                        if (ProcessVerificationInterval > 0 && (int)(tNow - tRefLog).TotalSeconds >= ProcessVerificationInterval)
                        {
                            tRefLog = tNow;
                            Logger?.Debug(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + " is running. Client socket = {0}", ClientSocketString);
                        }
                        if (SleepingIntervalInMS >= 0) { System.Threading.Thread.Sleep(SleepingIntervalInMS); }
                    }
                    // while (mbForceDisconnect == false && mOutterServer.mbForceStop == false && mTcpClient != null && mTcpClient.Connected)
                    // { ReceiveByteArray(); }
                    Logger?.Debug("TCP Server stops to receive data. Client socket = {0}", ClientSocketString);
                }
                catch (Exception ex)
                {
                    Logger?.Error("Client socket = {0}, ContainLengthAsHeader = {1}", ClientSocketString, ContainLengthAsHeader);
                    Logger?.Error(ex);
                }
            }

            /// Set to force disconnect.
            public void Exit()
            {
                try
                {
                    IsExit = true;
                    Disconnect();
                }
                catch (Exception ex)
                {
                    Logger?.Error("Client socket = {0}, ContainLengthAsHeader = {2}", ClientSocketString, ContainLengthAsHeader);
                    Logger?.Error(ex);
                }
            }

            /// Force disconnecting.
            /// iTimeoutInSecond = Timeout in second. The default value is 5.
            /// iSleepingIntervalInMS = Sleeping interval in milli-second. The default value is 100.
            public void ForceDisconnect() { ForceDisconnect(5); }
            public void ForceDisconnect(int timeoutInSecond) { ForceDisconnect(timeoutInSecond, 100); }
            public void ForceDisconnect(int timeoutInSecond, int sleepingIntervalInMS)
            {
                DateTime tRef;
                bool b;
                try
                {
                    Exit();
                    if (timeoutInSecond < 0) { timeoutInSecond = 0; }
                    tRef = DateTime.Now; b = true;
                    // while ((mThreadReceiveData != null) && mThreadReceiveData.IsAlive && ((int)(DateTime.Now-tRef).TotalSeconds<iTimeoutInSecond))
                    // { if(iSleepingIntervalInMS >= 0) { System.Threading.Thread.Sleep(iSleepingIntervalInMS); } }
                    while (b && (int)(DateTime.Now - tRef).TotalSeconds < timeoutInSecond)
                    {
                        b = false;
                        //if (ThreadToReceiveData != null && ThreadToReceiveData.IsAlive) { b = true; }
                        //else { if (ThreadToAnalyzeIncomingBuffer != null && ThreadToAnalyzeIncomingBuffer.IsAlive) { b = true; } }
                        if (ThreadToReceiveData?.IsAlive ?? false) { b = true; }
                        else { if (ThreadToAnalyzeIncomingBuffer?.IsAlive ?? false) { b = true; } }
                        if (sleepingIntervalInMS >= 0) { System.Threading.Thread.Sleep(sleepingIntervalInMS); }
                    }
                    AbortThread(ref ThreadToReceiveData);
                    AbortThread(ref ThreadToAnalyzeIncomingBuffer);
                    /// Release the memory in buffer.
                    if (IncomingBufferQueue != null)
                    {
                        IncomingBufferQueue.Clear();
                        IncomingBufferQueue = null;
                    }
                    if (IncomingBufferList != null)
                    {
                        IncomingBufferList.Clear();
                        IncomingBufferList = null;
                    }
                    IncomingContentBuffer = null;
                    IncomingLengthBuffer = null;
                }
                catch (Exception ex)
                {
                    Logger?.Error("Client socket = {0}, ContainLengthAsHeader = {1}", ClientSocketString, ContainLengthAsHeader);
                    Logger?.Error(ex);
                }
            }

            /// Start the thread to receive messages.
            public void StartReceivingData()
            {
                try
                {
                    if (ThreadToReceiveData == null)
                    {
                        ThreadToReceiveData = new System.Threading.Thread(new System.Threading.ThreadStart(ProcessReceiveData))
                        {
                            Name = "ProcessReceiveData-" + ClientSocketString
                        };
                    }
                    if (ThreadToReceiveData == null) { Logger?.Debug(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + ". Cannot start the thread of receiving data. Client socket = {0}", ClientSocketString); }
                    else { if (ThreadToReceiveData.IsAlive == false) { ThreadToReceiveData.Start(); } }
                    if (ThreadToAnalyzeIncomingBuffer == null)
                    {
                        ThreadToAnalyzeIncomingBuffer = new System.Threading.Thread(new System.Threading.ThreadStart(ProcessAnalyzeIncomingBuffer))
                        {
                            Name = "ProcessAnalyzeIncomingBuffer-" + ClientSocketString
                        };
                    }
                    if (ThreadToAnalyzeIncomingBuffer == null) { Logger?.Debug(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + ". Cannot start the thread of analyzing incoming data. Client socket = {0}", ClientSocketString); }
                    else { if (ThreadToAnalyzeIncomingBuffer.IsAlive == false) { ThreadToAnalyzeIncomingBuffer.Start(); } }
                }
                catch (Exception ex)
                {
                    Logger?.Error("Client socket = {0}, ContainLengthAsHeader = {1}", ClientSocketString, ContainLengthAsHeader);
                    Logger?.Error(ex);
                }
            }

            public InnerClient(ref System.Net.Sockets.TcpClient pTcpClient, Queue<TcpSocketData> incomingDataQueue, object incomingDataQueueLocker)
            {
                try
                {
                    this.ClientSocket = pTcpClient;
                    if (this.ClientSocket == null || ClientSocket.Connected == false) { return; }
                    /// IP and Port.
                    System.Net.IPEndPoint oIPEndPoint = (System.Net.IPEndPoint)ClientSocket.Client.RemoteEndPoint;
                    //this._Host = oIPEndPoint.Address.ToString();
                    //this._Port = oIPEndPoint.Port;
                    this.Host = oIPEndPoint.Address.ToString();
                    this.Port = oIPEndPoint.Port;
                    this.InitialDateTime = this.LastReceivedDateTime = this.LastTransferDateTime = DateTime.Now;
                    /// Other parameters.
                    //this.ContainLengthAsHeader = true;
                    //this.EnableAnalyzeIncomingData = true;
                    //this.MaxDataSize = 2000000000;
                    //this.ProcessVerificationInterval = 600;
                    //this.ReceiveDataInterval = 0;
                    //this.ReceiveTotalBufferSize = 10485760;
                    //this.SleepingIntervalInMS = 100;
                    this.IncomingDataQueueLocker = incomingDataQueueLocker;
                    this.IncomingDataQueue = incomingDataQueue;
                    this.Username = "";
                    Logger?.Debug("TCP Server connects client {0}", ClientSocketString);
                }
                catch (Exception ex)
                {
                    Logger?.Error("Client socket = {0}, ContainLengthAsHeader = {1}", ClientSocketString, ContainLengthAsHeader);
                    Logger?.Error(ex);
                }
            }
        }
        private List<InnerClient> InnerClientList = null;/// list of TCP client objects.
        private object InnerClientListLocker = new object();

        /// Abort a specific thread and set it to null.
        /// Return Value = Result whether abort the thread successfully. True if success. Otherwise, false.
        /// th = target thread that will be aborted.
        /// bIsDebugLog = Whether the log is a debug bug or not.
        private static bool AbortThread(ref System.Threading.Thread th)
        {
            if (th == null) { return true; }
            try
            {
                if (th.IsAlive)
                {
                    Logger?.Debug("Force to abort the thread named {0}", th.Name);
                    th.Abort();
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger?.Error(ex);
                return false;
            }
            finally { th = null; }
        }

        /// Get the number of items in the queue.
        private static int NumberOfItemsInQueue(Queue<TcpSocketData> queue, object locker)
        {
            try
            {
                //if (locker == null)
                //{
                //    return vQueue == null ? 0 : vQueue.Count;
                //}
                //else
                //{
                //    lock (locker)
                //    {
                //        return vQueue == null ? 0 : vQueue.Count;
                //    }
                //}
                if (locker == null) { return queue?.Count ?? 0; }
                else { lock (locker) { return queue?.Count ?? 0; } }
            }
            catch (Exception ex)
            {
                Logger?.Error(ex);
                return -1;
            }
        }

        /// Get the number of outgoing data in the queue.
        public int NumberOfOutgoingData() { return NumberOfItemsInQueue(OutgoingDataQueue, OutgoingDataQueueLocker); }

        /// Get the number of failed outgoing data in the queue, which is sent failed.
        public int NumberOfFailedOutgoingData() { return NumberOfItemsInQueue(FailedOutgoingDataQueue, FailedOutgoingDataQueueLocker); }

        /// Get the number of connected clients.
        public int NumberOfConnectedClients()
        {
            try
            {
                lock (InnerClientListLocker) { return InnerClientList?.Count ?? 0; }
            }
            catch (Exception ex)
            {
                Logger?.Error(ex);
                return -1;
            }
        }

        /// Get a list of Host:Port of clients.
        public string[] TcpClientList()
        {
            //List<string> rList = new List<string>();
            try
            {
                //lock (InnerClientListLocker)
                //{
                //    int i = 0;
                //    //while (InnerClientList != null && i < InnerClientList.Count)
                //    while (i < (InnerClientList?.Count ?? 0))
                //    {
                //        InnerClient oItem = InnerClientList[i];
                //        if (oItem != null) { rList.Add(oItem.ClientSocketString); }
                //        i += 1;
                //    }
                //}
                //string[] returnStringArray = rList.Count < 1 ? null : rList.ToArray();
                //return returnStringArray;
                lock (InnerClientListLocker)
                {
                    return InnerClientList?.Select(x => x?.ClientSocketString)?.ToArray();
                }
            }
            catch (Exception ex)
            {
                Logger?.Error(ex);
                return null;
            }
            //finally
            //{
            //    if (rList != null)
            //    {
            //        try { rList.Clear(); }
            //        catch (Exception ex2) { Logger?.Error(ex2); }
            //        rList = null;
            //    }
            //}
        }

        public string PrintAllTcpClients() { return PrintAllTcpClients(Environment.NewLine); }
        public string PrintAllTcpClients(string recordSeparator)
        {
            string[] array = null;
            int i = 0;
            StringBuilder sb = null;
            try
            {
                array = TcpClientList();
                //if (sArray == null || sArray.Length < 1) { return "#Items = 0"; }
                if ((array?.Length ?? 0) < 1) { return "#Items = 0"; }
                sb = new StringBuilder();
                sb.Append("#Items = ");
                if (array != null) { i = array.Length; }
                sb.Append(i.ToString()).Append(recordSeparator);
                if (array != null) { sb.Append(string.Join(recordSeparator, array)).Append(recordSeparator); }
                string sReturn = sb.ToString();
                return sReturn;
            }
            catch (Exception ex)
            {
                Logger?.Error(ex);
                return "";
            }
            finally { sb = null; }
        }

        /// Get the index of clients in the list by host and port.
        /// Return value = index of clients in the list by host and port
        /// host = client host
        /// port = client port
        private int IndexOfInnerClientByHostAndPort(string host, int port)
        {
            try
            {
                if (host == null) { host = ""; }
                else { host = host.Trim(); }

                //int i = 0;
                //lock (mListInnerClientLocker)
                //{
                //    while (mListInnerClientT != null && i < mListInnerClientT.Count)
                //    {
                //        InnerClient oItem = mListInnerClientT[i];
                //        if (oItem != null)
                //        {
                //            if (sHost.Equals(oItem.Host) && oItem.Port == iPort)
                //            { return i; }
                //        }
                //        i += 1;
                //    }
                //}
                //return -1;

                /// Not use the below lambda expression because worry that the mListInnerClientT is always changed, and the iteration may go exception.
                // if (string.IsNullOrEmpty(sHost)) { return mListInnerClientT.FindIndex(x => x != null && iPort == x.Port && string.IsNullOrEmpty(x.Host)); }
                // else
                // {
                // sHost = sHost.Trim();
                // return mListInnerClientT.FindIndex(x => x != null && iPort == x.Port && sHost.Equals(x.Host));
                // }

                lock (InnerClientListLocker)
                {
                    return InnerClientList?.FindIndex(x => x != null && port == x.Port && host.Equals(x.Host)) ?? -1;
                }
            }
            catch (Exception ex)
            {
                Logger?.Error("Client socket = {0}:{1}", host, port);
                Logger?.Error(ex);
                return -1;
            }
        }

        /// Get the client item in the list by host and port.
        /// Return value = client item in the list by host and port
        /// host = client host
        /// port = client port
        private InnerClient InnerClientTByHostAndPort(string host, int port)
        {
            try
            {
                if (host == null) { host = ""; }
                else { host = host.Trim(); }
                lock (InnerClientListLocker)
                {
                    return InnerClientList?.Find(x => x != null && port == x.Port && host.Equals(x.Host));
                }
            }
            catch (Exception ex)
            {
                Logger?.Error("Client socket = {0}:{1}", host, port);
                Logger?.Error(ex);
                return null;
            }
        }

        /// Check if the specific client is connected to this server.
        /// Return value = true if connected. Otherwise, false.
        /// host = client host
        /// port = client port
        public bool IsClientConnected(string host, int port)
        {
            try
            {
                return InnerClientTByHostAndPort(host, port)?.IsConnected ?? false;
            }
            catch (Exception ex)
            {
                Logger?.Error("Client socket = {0}:{1}", host, port);
                Logger?.Error(ex);
                return false;
            }
        }

        /// Disconnect a specific client.
        public void DisconnectClient(string host, int port)
        {
            // System.Collections.Generic.List<InnerClientT> vListOfDisconnect = null;
            try
            {
                if (host == null) { host = ""; }
                else { host = host.Trim(); }

                //if (InnerClientList == null || InnerClientList.Count < 1) { return; }
                // if (string.IsNullOrEmpty(sHost)) { vListOfDisconnect = mListInnerClientT.FindAll(x => x != null && iPort == x.Port && string.IsNullOrEmpty(x.Host)); }
                // else
                // {
                // sHost = sHost.Trim();
                // vListOfDisconnect = mListInnerClientT.FindAll(x => x != null && iPort == x.Port && sHost.Equals(x.Host));
                // }
                // if (vListOfDisconnect != null)
                // {
                // int i = 0;
                // while (i < vListOfDisconnect.Count)
                // {
                // InnerClientT oItem = vListOfDisconnect[i];
                // if (oItem != null) { oItem.ForceDisconnect(2); }
                // i += 1;
                // }
                // }

                /// Not use FindAll().
                lock (InnerClientListLocker)
                {
                    int i = 0;
                    //while (InnerClientList != null && i < InnerClientList.Count)
                    while (i < (InnerClientList?.Count ?? 0))
                    {
                        InnerClient oItem = InnerClientList[i];
                        if (oItem != null)
                        {
                            if (oItem.Port == port && host.Equals(oItem.Host))
                            { oItem.ForceDisconnect(2); }
                        }
                        i += 1;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger?.Error("Client socket = {0}:{1}", host, port);
                Logger?.Error(ex);
            }
        }

        /// Trim the list of InnerClient.
        private void TrimList()
        {
            List<InnerClient> vListOfDeleted = null;
            try
            {
                lock (InnerClientListLocker)
                {
                    if (InnerClientList != null)
                    {
                        InnerClientList.RemoveAll(x => x == null);
                        vListOfDeleted = InnerClientList.FindAll(x => x.IsConnected == false);
                        //if (vListOfDeleted != null && vListOfDeleted.Count > 0)
                        if ((vListOfDeleted?.Count ?? 0) > 0)
                        {
                            foreach (InnerClient o in vListOfDeleted)
                            {
                                if (o != null && InnerClientList != null)
                                {
                                    if (string.IsNullOrEmpty(o.Host))
                                    { InnerClientList.RemoveAll(x => x.Port == o.Port && string.IsNullOrEmpty(x.Host)); }
                                    else { InnerClientList.RemoveAll(x => x.Port == o.Port && o.Host.Equals(x.Host)); }
                                    //o.ForceDisconnect();
                                }
                            }
                        }
                    }
                }
                /// Disconnect the clients out of the LOCK block.
                //if (vListOfDeleted != null && vListOfDeleted.Count > 0)
                if ((vListOfDeleted?.Count ?? 0) > 0)
                {
                    foreach (InnerClient o in vListOfDeleted)
                    { if (o != null) { o.ForceDisconnect(); } }
                }
            }
            catch (Exception ex) { Logger?.Error(ex); }
            finally
            {
                if (vListOfDeleted != null)
                {
                    try { vListOfDeleted.Clear(); }
                    catch (Exception ex2) { Logger?.Error(ex2); }
                    finally { vListOfDeleted = null; }
                }
            }
        }

        /// This function should be run as a thread.
        private void ProcessTrimList()
        {
            try
            {
                int i = this.SleepingIntervalInMS * 2;
                Logger?.Debug("TCP Server begins to trim list of clients.");
                while (!this.IsExit)
                {
                    TrimList();
                    if (i >= 0) { System.Threading.Thread.Sleep(i); }
                }
                Logger?.Debug("TCP Server stops to trim list of clients.");
            }
            catch (Exception ex) { Logger?.Error(ex); }
        }

        private bool TryDequeueAtOutgoingDataQueue(out TcpSocketData oOutput)
        {
            try
            {
                lock (OutgoingDataQueueLocker)
                {
                    if ((OutgoingDataQueue?.Count ?? 0) < 1)
                    {
                        oOutput = null;
                        return false;
                    }
                    else
                    {
                        oOutput = OutgoingDataQueue.Dequeue();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger?.Error(ex);
                oOutput = null;
                return false;
            }
        }

        private bool TryDequeueAtFailedOutgoingDataQueue(out TcpSocketData oOutput)
        {
            try
            {
                lock (FailedOutgoingDataQueueLocker)
                {
                    if ((FailedOutgoingDataQueue?.Count ?? 0) < 1)
                    {
                        oOutput = null;
                        return false;
                    }
                    else
                    {
                        oOutput = FailedOutgoingDataQueue.Dequeue();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger?.Error(ex);
                oOutput = null;
                return false;
            }
        }

        /// Queue to send data.
        public void QueueToSendData(string host, int port, ref byte[] data)
        {
            try
            {
                if (string.IsNullOrEmpty(host)) { return; }
                /// add data to queue.
                lock (OutgoingDataQueueLocker)
                {
                    if (OutgoingDataQueue == null) { OutgoingDataQueue = new Queue<TcpSocketData>(); }
                    OutgoingDataQueue.Enqueue(new TcpSocketData()
                    {
                        Timestamp = DateTime.Now,
                        Host = host,
                        Port = port,
                        //IsFinishedReceiving = bIsFinishedReceiving,
                        ByteArray = data
                    });
                }
            }
            catch (Exception ex)
            {
                Logger?.Error("Client socket = {0}:{1}", host, port);
                Logger?.Error(ex);
            }
        }

        /// Queue to failed outgoing data.
        private void QueueToFailedOutgoingData(ref TcpSocketData vData)
        {
            try
            {
                if (vData == null) { return; }
                lock (FailedOutgoingDataQueueLocker)
                {
                    if (FailedOutgoingDataQueue == null) { FailedOutgoingDataQueue = new Queue<TcpSocketData>(); }
                    FailedOutgoingDataQueue.Enqueue(vData);
                }
            }
            catch (Exception ex) { Logger?.Error(ex); }
        }

        private void ProcessSendData_routine1(TcpSocketData vData)
        {
            try
            {
                if (vData == null) { return; }
                //if (vData == null || InnerClientList == null) { return; }
                string sHost = "";
                if (!string.IsNullOrEmpty(vData.Host)) { sHost = vData.Host.ToUpper(); }
                //switch (sHost)
                //{
                //    case "255.255.255.255":
                //    case "ALL":
                //        // Don't use foreach.
                //        i = 0;
                //        while (mListInnerClientT != null && i < mListInnerClientT.Count)
                //        {
                //            InnerClient oItem = mListInnerClientT[i];
                //            if (oItem != null)
                //            { if (!oItem.SendByteArray(vData.ByteArray)) { QueueToFailedOutgoingData(ref vData); } }
                //            i += 1;
                //        }
                //        break;
                //    default: // find which client in the list.
                //        bool b = true;
                //        i = 0;
                //        sHost = vData.Host;
                //        if (string.IsNullOrEmpty(sHost)) { sHost = ""; }
                //        while (b && mListInnerClientT != null && i < mListInnerClientT.Count)
                //        {
                //            InnerClient oClient = mListInnerClientT[i];
                //            if (oClient != null)
                //            {
                //                if (oClient.Port == vData.Port && sHost.Equals(oClient.Host))
                //                {
                //                    if (!oClient.SendByteArray(vData.ByteArray)) { QueueToFailedOutgoingData(ref vData); }
                //                    b = false; // set that it already finds InnerClientT.
                //                }
                //            }
                //            i += 1;
                //        }
                //        if (b) { LocalLogger(true, "[debug] " + System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + ". [" + vData.Host + ":" + vData.Port.ToString() + "]. Cannot find the client in the list."); }
                //        break;
                //}
                if ("ALL".Equals(sHost.ToUpper()) || "255.255.255.255".Equals(sHost))
                {
                    lock (InnerClientListLocker)
                    {
                        if (InnerClientList != null)
                        {
                            foreach (InnerClient o in InnerClientList)
                            {
                                if (o != null)
                                { if (!o.SendByteArray(vData.ByteArray)) { QueueToFailedOutgoingData(ref vData); } }
                            }
                        }
                    }
                }
                else
                {
                    bool b = true;
                    lock (InnerClientListLocker)
                    {
                        if (InnerClientList != null)
                        {
                            foreach (InnerClient o in InnerClientList)
                            {
                                if (o != null && o.Port == vData.Port && sHost.Equals(o.Host))
                                {
                                    if (!o.SendByteArray(vData.ByteArray)) { QueueToFailedOutgoingData(ref vData); }
                                    b = false;/// set that it already finds InnerClient.
                                }
                            }
                        }
                    }
                    if (b) { Logger?.Debug(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "Cannot find the client in the list. Client socket = {0}:{1}", sHost, vData.Port); }
                }
            }
            catch (Exception ex) { Logger?.Error(ex); }
        }

        /// Send heartbeat to all clients.
        private void SendHeartbeat(byte[] oByteArrayHeartbeat)
        {
            try
            {
                //int i = 0;
                //while (mListInnerClientT != null && i < mListInnerClientT.Count)
                //{
                //    InnerClient oItem = mListInnerClientT[i];
                //    if (oItem != null) { oItem.SendByteArray(oByteArrayHeartbeat); }
                //    i += 1;
                //}
                lock (InnerClientListLocker)
                {
                    if (InnerClientList != null)
                    {
                        foreach (InnerClient o in InnerClientList)
                        { if (o != null) { o.SendByteArray(oByteArrayHeartbeat); } }
                    }
                }
            }
            catch (Exception ex) { Logger?.Error(ex); }
        }

        /// Disconnect sockets which exceed maximum connection duration.
        private void DisconnectSocketExceedingMaxConnectionDuration(DateTime tNow)
        {
            try
            {
                if (this.MaxConnectionDuration <= 0) { return; }
                lock (InnerClientListLocker)
                {
                    if (InnerClientList != null)
                    {
                        foreach (InnerClient o in InnerClientList)
                        {
                            if (o != null)
                            {
                                if ((int)(tNow - o.InitialDateTime).TotalSeconds > this.MaxConnectionDuration)
                                {
                                    Logger?.Debug("Disconnect socket as exceeding maximum connection duration {0} second(s). Listening Port: {1}. Client: {2}:{3}", this.MaxConnectionDuration, this.ListeningPort, o.Host, o.Port.ToString());
                                    o.ForceDisconnect();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { Logger?.Error(ex); }
        }

        private void DisconnectSocketExceedingMaxIdleDuration(DateTime tNow)
        {
            try
            {
                if (this.MaxIdleDuration < 0) { return; }
                lock (InnerClientListLocker)
                {
                    if (InnerClientList != null)
                    {
                        foreach (InnerClient o in InnerClientList)
                        {
                            if (o != null)
                            {
                                if ((int)(tNow - o.LastTransferDateTime).TotalSeconds > this.MaxIdleDuration)
                                {
                                    Logger?.Debug("Disconnect socket as exceeding maximum idle duration {0} second(s). Listening Port: {1}. Client: {2}:{3}", this.MaxIdleDuration, this.ListeningPort, o.Host, o.Port.ToString());
                                    o.ForceDisconnect();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { Logger?.Error(ex); }
        }

        /// Process to send data.
        private void ProcessSendData()
        {
            DateTime tNow;
            byte[] oByteArrayHeartbeat = null;
            try
            {
                Logger?.Debug("TCP Server begins to send data to clients.");
                /// looping.
                DateTime tRefLog = DateTime.Now;
                DateTime tRefHeartbeat = tRefLog.AddHours(-1);
                //tRefData = tRefHeartbeat;
                //oByteArrayHeartbeat = new byte[0];
                // if (mbContainLengthAsHeader) { oByteArrayHeartbeat = new byte[0]; }
                //else { oByteArrayHeartbeat = System.Text.Encoding.UTF8.GetBytes("~"); }
                if (!this.ContainLengthAsHeader) { oByteArrayHeartbeat = this.HeartbeatData; }
                while (this.IsExit == false && this.ServerSocket != null)
                {
                    tNow = DateTime.Now;
                    /// Send data to clients.
                    if (this.MaxDataSend < 0)
                    {
                        TcpSocketData oData = null;
                        while (TryDequeueAtOutgoingDataQueue(out oData))
                        { ProcessSendData_routine1(oData); }
                    }
                    else
                    {
                        TcpSocketData oData = null;
                        int i = 0;
                        while (i < this.MaxDataSend && TryDequeueAtOutgoingDataQueue(out oData))
                        {
                            ProcessSendData_routine1(oData);
                            i += 1;
                        }
                    }
                    /// Check if clients exceed maximum connection duration. If yes, disconnect it.
                    DisconnectSocketExceedingMaxConnectionDuration(tNow);
                    /// Check if clients exceed maximum idle duration. If yes, disconnect it.
                    DisconnectSocketExceedingMaxIdleDuration(tNow);
                    /// Send heartbeat to clients.
                    if (this.HeartbeatInterval == 0 || (this.HeartbeatInterval > 0 && ((int)(tNow - tRefHeartbeat).TotalSeconds >= this.HeartbeatInterval)))
                    {
                        tRefHeartbeat = tNow;
                        SendHeartbeat(oByteArrayHeartbeat);
                        Logger?.Debug("List of Inner clients: " + PrintAllTcpClients());
                    }
                    /// Trim the list.
                    TrimList();
                    /// log.
                    if (this.ProcessVerificationInterval > 0 && (int)(tNow - tRefLog).TotalSeconds >= this.ProcessVerificationInterval)
                    {
                        tRefLog = tNow;
                        Logger?.Debug(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + " is running.");
                    }
                    if (this.SleepingIntervalInMS >= 0) { System.Threading.Thread.Sleep(this.SleepingIntervalInMS); }
                }
                Logger?.Debug("TCP Server stops to send data to clients.");
            }
            catch (Exception ex) { Logger?.Error(ex); }
            finally { oByteArrayHeartbeat = null; }
        }

        /// Action after accept a client.
        private void AcceptClientCallback(IAsyncResult ar)
        {
            System.Net.Sockets.TcpClient oClientSocket = null;
            try
            {
                if (this.IsExit || this.ServerSocket == null) { return; }
                oClientSocket = this.ServerSocket.EndAcceptTcpClient(ar);
                if (oClientSocket == null) { return; }
                /// create the client instance.
                InnerClient oItem = new InnerClient(ref oClientSocket, this.IncomingDataQueue, this.IncomingDataQueueLocker)
                {
                    ContainLengthAsHeader = this.ContainLengthAsHeader,
                    EnableAnalyzeIncomingData = this.EnableAnalyzeIncomingData,
                    MaxDataSize = this.MaxDataSize,
                    ProcessVerificationInterval = this.ProcessVerificationInterval,
                    ReceiveDataInterval = this.ReceiveDataInterval,
                    ReceiveTotalBufferSize = this.ReceiveTotalBufferSize,
                    SleepingIntervalInMS = this.SleepingIntervalInMS
                };
                oItem.StartReceivingData();
                /// add client socket to list.
                lock (InnerClientListLocker)
                {
                    if (InnerClientList == null) { InnerClientList = new List<InnerClient>(); }
                    InnerClientList.Add(oItem);
                }
                /// set ready to accept client.
                this.IsReadyAccept = true;
            }
            catch (Exception ex) { Logger?.Error(ex); }
        }

        //private void ProcessAcceptClient_routine1()
        //{
        //    System.Net.Sockets.TcpClient oClientSocket = null;
        //    InnerClientT oItem = null;
        //    try
        //    {
        //        if (mbForceStop || mServerSocket == null) { return; }
        //        oClientSocket = mServerSocket.AcceptTcpClient();
        //        if (oClientSocket == null) { return; }
        //        // create the client instance.
        //        oItem = new InnerClientT(ref oClientSocket, this);
        //        oItem.Logger = mLogger;
        //        oItem.ContainLengthAsHeader = mbContainLengthAsHeader;
        //        oItem.MaxDataSize = miMaxDataSend;
        //        oItem.ReceiveDataInterval = miReceiveDataInterval;
        //        oItem.ReceiveTotalBufferSize = miReceiveTotalBufferSize;
        //        oItem.SleepingIntervalInMS = miSleepingIntervalInMS;
        //        //oItem.Username = "";
        //        oItem.StartReceivingData();
        //        // add client socket to list.
        //        if (mListInnerClientT == null) { mListInnerClientT = new System.Collections.Generic.List<InnerClientT>(); }
        //        mListInnerClientT.Add(oItem);
        //        // set ready to accept client.
        //        mbReadyAccept = true;
        //    }
        //    catch (Exception ex) { LocalLogger("[error] " + System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + ". " + ex.Message); }
        //}

        /// This function should be run as a thread.
        /// https://msdn.microsoft.com/en-us/library/system.net.sockets.tcplistener.accepttcpclient(v=vs.110).aspx
        private void ProcessAcceptClient()
        {
            try
            {
                if (this.IsExit) { return; }
                Logger?.Debug("TCP Server begins to accept clients.");
                this.IsReadyAccept = true;/// set ready to accept client.
                DateTime tRefLog = DateTime.Now;
                DateTime tRef = tRefLog.AddHours(-1);
                DateTime tNow;
                while (this.IsExit == false && this.ServerSocket != null)
                {
                    tNow = DateTime.Now;
                    //if (this.IsReadyAccept && this.ServerSocket != null && (this.MaxClient < 0 || NumberOfConnectedClients() < this.MaxClient) && (this.AcceptInterval < 1 || (int)(tNow - tRef).TotalSeconds >= this.AcceptInterval))
                    if (this.IsReadyAccept && this.ServerSocket != null && (this.AcceptInterval < 1 || (int)(tNow - tRef).TotalSeconds >= this.AcceptInterval))
                    {
                        int i = NumberOfConnectedClients();
                        if (this.MaxClient < 0 || i < this.MaxClient)
                        {
                            tRef = tNow;/// set the time.
                            this.IsReadyAccept = false;/// set NOT to accept client.
                            this.ServerSocket.BeginAcceptTcpClient(new AsyncCallback(AcceptClientCallback), null);
                        }
                        else
                        {
                            Logger?.Warn(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + " Number of Connected Clients exceeds maximum. Number of Connected Clients = " + i.ToString() + ". Max = " + this.MaxClient.ToString());
                        }
                    }
                    if (this.ProcessVerificationInterval > 0 && (int)(tNow - tRefLog).TotalSeconds >= this.ProcessVerificationInterval)
                    {
                        tRefLog = tNow;
                        Logger?.Debug(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + " is running.");
                    }
                    if (this.SleepingIntervalInMS >= 0) { System.Threading.Thread.Sleep(this.SleepingIntervalInMS); }
                }
                //while (mbForceStop == false)
                //{
                //    if ((mServerSocket != null) && (miMaxClient < 0 || NumberOfConnectedClients() < miMaxClient))
                //    {
                //        ProcessAcceptClient_routine1();
                //    }
                //}
                Logger?.Debug("TCP Server stops to accept clients.");
            }
            catch (Exception ex) { Logger?.Error(ex); }
        }

        /// Dequeue the outgoing data, which is sent failed.
        public TcpSocketData DequeueFailedOutgoingData()
        {
            try
            {
                bool b = TryDequeueAtFailedOutgoingDataQueue(out TcpSocketData oOutput);
                return oOutput;
            }
            catch (Exception ex)
            {
                Logger?.Error(ex);
                return null;
            }
        }

        /// Stop this server to listen clients.
        /// timeoutInSecond = Timeout in second. The default value is 5.
        /// sleepingIntervalInMS = Sleeping interval in milli-second. The default value 100.
        public void StopListening() { StopListening(5); }
        public void StopListening(int timeoutInSecond) { StopListening(timeoutInSecond, 100); }
        public void StopListening(int timeoutInSecond, int sleepingIntervalInMS)
        {
            DateTime tRef;
            bool bLoop;
            try
            {
                this.IsExit = true;/// set to close all clients.
                if (timeoutInSecond < 0) { timeoutInSecond = 0; }
                Logger?.Debug("TCP Server stops listening.");
                this.ServerSocket = null;
                /// close the threads in a time interval.
                tRef = DateTime.Now;
                bLoop = true;
                while (bLoop && (DateTime.Now - tRef).TotalSeconds < timeoutInSecond)
                {
                    bLoop = false;
                    //if (this.ThreadToAcceptClient != null && this.ThreadToAcceptClient.IsAlive) { bLoop = true; }
                    //else if (this.ThreadToSendData != null && this.ThreadToSendData.IsAlive) { bLoop = true; }
                    if (ThreadToAcceptClient?.IsAlive ?? false) { bLoop = true; }
                    else if (ThreadToSendData?.IsAlive ?? false) { bLoop = true; }
                    if (bLoop && sleepingIntervalInMS >= 0) { System.Threading.Thread.Sleep(sleepingIntervalInMS); }
                }
                /// abort threads.
                AbortThread(ref this.ThreadToAcceptClient);
                AbortThread(ref this.ThreadToSendData);
                /// close all clients.
                lock (InnerClientListLocker)
                {
                    if (InnerClientList != null)
                    {
                        int i = 0;
                        //while (InnerClientList != null && i < InnerClientList.Count)
                        while (i < (InnerClientList?.Count ?? 0))
                        {
                            InnerClient oItem = InnerClientList[i];
                            if (oItem != null)
                            {
                                oItem.ForceDisconnect(timeoutInSecond, sleepingIntervalInMS);
                                oItem = null;
                                InnerClientList[i] = null;
                            }
                            i += 1;
                        }
                        InnerClientList.Clear();
                        InnerClientList = null;
                    }
                }
            }
            catch (Exception ex) { Logger?.Error(ex); }
        }

        /// Start this server to listen clients.
        public bool StartListening()
        {
            try
            {
                this.IsExit = false;
                /// check if the queue of incoming data is initialized.
                if (IncomingDataQueue == null)
                {
                    Logger?.Warn("TCP Server refuses to start listening as the queue is not initialized.");
                    return false;
                }
                /// Create an instance.
                if (this.ServerSocket == null) { this.ServerSocket = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Any, this.ListeningPort); }
                /// debug log.
                Logger?.Debug("TCP Server starts listening. Listening Port = {0}", this.ListeningPort);
                /// start listening.
                this.ServerSocket.Start();
                /// thread to accept clients.
                this.ThreadToAcceptClient = new System.Threading.Thread(new System.Threading.ThreadStart(ProcessAcceptClient))
                {
                    Name = "ProcessAcceptClient"
                };
                this.ThreadToAcceptClient.Start();
                /// thread to send data to clients.
                this.ThreadToSendData = new System.Threading.Thread(new System.Threading.ThreadStart(ProcessSendData))
                {
                    Name = "ProcessSendData"
                };
                this.ThreadToSendData.Start();
                return true;
            }
            catch (Exception ex)
            {
                Logger?.Error(ex);
                return false;
            }
        }

        //static TTcpServerSocket() { InnerClient.Logger = Logger; }/// This line is no effect.

        //public TTcpServerSocket()
        //{
        //    ListeningPort = 8002;
        //    AcceptInterval = 1;
        //    ContainLengthAsHeader = true;
        //    EnableAnalyzeIncomingData = true;
        //    HeartbeatInterval = 15;
        //    HeartbeatData = Encoding.UTF8.GetBytes("~");
        //    MaxClient = 200;
        //    MaxConnectionDuration = 600;
        //    MaxIdleDuration = -1;
        //    MaxDataSend = -1;
        //    MaxDataSize = 2000000000;
        //    ProcessVerificationInterval = 600;
        //    ReceiveDataInterval = 0;
        //    ReceiveTotalBufferSize = 10485760;
        //    SleepingIntervalInMS = 100;
        //}
    }
}
