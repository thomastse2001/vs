using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;

namespace VsCSharpWinForm_sample2.Helpers
{
    public class TTcpSocket
    {
        /// TCP Client and Server by a synchronous socket in the threading model.
        /// Able to send file with 100M bytes, and even 200M bytes, but not very good to send 200 M bytes. Cannot send 500M bytes file.
        /// Data unit is byte.
        /// Updated date: 2021-05-05
        /// 
        /// AddDataToIncomingDataQueue
        /// PackData
        /// TryDequeueAtIncomingBufferQueue
        /// AnalyzeIncomingBuffer
        /// ProcessAnalyzeIncomingBuffer
        /// 
        /// Example:
        /// void MyFunction(TTcpSocket.DataPackage o)
        /// {
        ///     // Do somethings.
        ///     Console.Write(o.Host + ":" + o.Port.ToString());
        /// }
        /// MySocket1 = new TTcpSocket.Client(..);
        /// MySocket1.ExternalActToHandleIncomingData = MyFunction(o);
        /// MySocket2 = new TTcpSocket.Server(..);
        /// MySocket2.ExternalActToHandleIncomingData = MyFunction(o);

        public class DataPackage
        {
            public DateTime Timestamp = DateTime.MinValue;/// Timestamp that this data package is generated.
            public string Host = "";/// Remote host.
            public int Port = -1;/// Remote port.
            public byte[] ByteArray = null;/// Data is stored as a byte array.
        }

        public class Client
        {
            /// TCP Client by a synchronous socket in the threading model.
            /// Data unit is byte.
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
            private Queue<byte[]> IncomingBufferQueue = new Queue<byte[]>();/// buffer queue of TCP incoming data.
            private readonly object IncomingBufferQueueLocker = new object();

            /// Properties.
            public static Helpers.TLog Logger { get; set; }
            //public string RemoteHost { get; set; } = "127.0.0.1";
            public readonly string RemoteHost;
            //public int RemotePort { get; set; } = 8002;
            public readonly int RemotePort;
            public readonly DateTime InitialDateTime;
            public DateTime LastReceivedDateTime { get; private set; }/// Date time of last received data. Can read and write it inside class, but read-only outside the class. https://stackoverflow.com/questions/4662180/c-sharp-public-variable-as-writeable-inside-the-class-but-readonly-outside-the-c
            public DateTime LastTransferDateTime { get; private set; }/// Date time of last transfer data. Can read and write it inside class, but read-only outside the class. https://stackoverflow.com/questions/4662180/c-sharp-public-variable-as-writeable-inside-the-class-but-readonly-outside-the-c
            public bool ContainLengthAsHeader { get; set; } = true;/// flag that if the data contains length (4-byte integer) as a header of package.
            public bool EnableAnalyzeIncomingData { get; set; } = true;/// flag to enable to analyze the incoming data from remote host. A package is preserved when adding to the queue of incoming data. Note that large package excceeding the buffer size is divided as several parts when receiving. The users may need to handle how to obtain the complete package. The default value is true.
            private byte[] _HeartbeatData = System.Text.Encoding.UTF8.GetBytes("~");
            public byte[] HeartbeatData/// Data in byte array of heartbeat from server. This parameter will be used only if ContainLengthAsHeader = false. Space and Tab will be neglect. Sending heartbeat is used for the remote host to check if the sockets are still connected. The default value is the byte array of ~.
            {
                get { return _HeartbeatData; }
                set
                {
                    if (value == null) _HeartbeatData = null;
                    else
                    {
                        _HeartbeatData = new byte[value.Length];
                        Array.Copy(value, 0, _HeartbeatData, 0, value.Length);
                    }
                }
            }
            public int HeartbeatInterval { get; set; } = -1;/// Time interval in seconds that the local host sends heartbeat to the remote host. If it is negative, there is no heartbeat. Sending heartbeat is used to check if the connection still occurs. The default value is -1.
            public int MaxConnectionDuration { get; set; } = 600;/// Maximum connection duration in seconds between the local host and remote host. If the time exceeds, it will disconnect automatically even the connection is normal. If it is negative, there is no maximum connection duration, hence the connection can preserve forever. The default value is 600.
            public int MaxIdleDuration { get; set; } = -1;/// Maximum idle duration in seconds between the local host and remote host. If the time exceeds, it will disconnect automatically. If it is negative, there is no maximum idle duration, hence it will not check the idle duration. The default value is -1.
            private int _MaxDataSize = 104857600;
            public int MaxDataSize { get { return _MaxDataSize; } set { _MaxDataSize = value < 0 ? 0 : value; } }/// Maximum size of data in bytes. The default value is 104857600.

            public int ProcessVerificationInterval { get; set; } = 600;
            public int ReceiveDataInterval { get; set; } = 0;/// Time interval in seconds to receive data. If it is 0, the process will do immediately without waiting. If it is negative, no data is received. The default value is 0.
            private int _ReceiveTotalBufferSize = 10485760;
            public int ReceiveTotalBufferSize { get { return _ReceiveTotalBufferSize; } set { _ReceiveTotalBufferSize = value < 10485760 ? 10485760 : value; } }/// Total buffer size in bytes for receiving data, the minimum value is 10485760.
            public int SleepingIntervalInMS { get; set; } = 100;/// Sleeping interval in milliseconds. This sleeping interval helps to avoid the application too busy. If it is negative, no sleep. The default value is 100.
            public bool IsConnected { get { return ClientSocket?.Connected ?? false; } }
            public string LocalEndPoint { get { return ClientSocket?.LocalEndPoint?.ToString(); } }
            public string RemoteEndPoint { get { return ClientSocket?.RemoteEndPoint?.ToString(); } }
            public Queue<DataPackage> IncomingDataQueue = null;/// queue of TCP incoming data.
            public object IncomingDataQueueLocker = null;

            /// External action to handle incoming data. This class loops each data package in the incoming data queue and pass the data package to the external action.
            /// Example:
            /// void MyFunction(TTcpSocket.DataPackage o)
            /// {
            ///     // Do somethings.
            ///     Console.Write(o.Host + ":" + o.Port.ToString());
            /// }
            /// MySocket.ExternalActToHandleIncomingData = MyFunction(o);
            public delegate void ExternalActionToHandleIncomingData(DataPackage o);
            public ExternalActionToHandleIncomingData ExternalActToHandleIncomingData { get; set; } = null;

            /// Other parameters.
            //public string Username { get; set; }

            /// Abort a specific thread and set it to null.
            /// Return Value = Result whether abort the thread successfully. True if success. Otherwise, false.
            /// th = target thread that will be aborted.
            private static bool AbortThread(ref System.Threading.Thread th)
            {
                if (th == null || th.IsAlive == false) return true;
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
                        throw new Exception(string.Format("Local host cannot put data to IncomingDataQueue as it is not initialized. Remote host = {0}, local host = {1}", RemoteEndPoint, LocalEndPoint));
                    }
                    DataPackage oData = new DataPackage()
                    {
                        Timestamp = t,
                        Host = RemoteHost,
                        Port = RemotePort,
                        ByteArray = data
                    };
                    if (IncomingDataQueueLocker == null) IncomingDataQueue.Enqueue(oData);
                    else { lock (IncomingDataQueueLocker) { IncomingDataQueue.Enqueue(oData); } }
                    Logger?.Debug("Socket adds the received data to IncomingDataQueue. Remote host = {0}, Byte Length = {1}", RemoteEndPoint, data == null ? 0 : data.Length);
                    return true;
                }
                catch (Exception ex)
                {
                    Logger?.Error("Remote host = {0}, local host = {1}", RemoteEndPoint, LocalEndPoint);
                    Logger?.Error(ex);
                    return false;
                }
            }

            public static void Disconnect(ref System.Net.Sockets.Socket o)
            {
                if (o == null) return;
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
            public void Disconnect()
            {
                Logger?.Debug("Socket disconnects. Remote host = {0}, local host = {1}", RemoteEndPoint, LocalEndPoint);
                Disconnect(ref ClientSocket);
            }

            /// Pack the length of data into the beginning of data, in order to let the receiver easy to recognize each individual unit of data.
            public static byte[] PackData(int maxDataSize, byte[] data)
            {
                int iLength = 0;
                try
                {
                    if (data != null) iLength = data.Length;
                    if (iLength > maxDataSize)
                    {
                        throw new Exception(string.Format("Exceed the maximum data size {0}. Data size = [1}", maxDataSize, iLength));
                    }
                    byte[] rByte = new byte[4 + iLength];
                    BitConverter.GetBytes(iLength).CopyTo(rByte, 0);
                    if (data != null) data.CopyTo(rByte, 4);
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
                string remoteSocket = "";
                string localSocket = "";
                try
                {
                    if ((oClientSocket?.Connected ?? false) == false) return false;
                    //if (data == null) data = new byte[0];
                    byteData = isContainLengthAsHeader ? PackData(maxDataSize, data) : data;
                    /// Checking.
                    if (byteData == null) return false;
                    /// Send.
                    iSize = byteData.Length;
                    while (iSent < iSize)
                    { iSent += oClientSocket.Send(byteData, iSent, iSize - iSent, System.Net.Sockets.SocketFlags.None); }
                    localSocket = oClientSocket?.LocalEndPoint?.ToString();
                    remoteSocket = oClientSocket?.RemoteEndPoint?.ToString();
                    Logger?.Debug("Socket sends data. Remote host = {0}, local host = {1}, #Size = {2}, #Sent = {3}", remoteSocket, localSocket, iSize, iSent);
                    if (iSent == iSize) return true;
                    else
                    {
                        Logger?.Error("Socket sends incomplete data. Remote host = {0}, local host = {1}, #Size = {2}, #Sent = {3}", remoteSocket, localSocket, iSize, iSent);
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    Logger?.Error("Remote host = {0}, local host = {1}", remoteSocket, localSocket);
                    Logger?.Error(ex);
                    Disconnect(ref oClientSocket);
                    return false;
                }
            }
            public bool SendByteArray(byte[] data)
            {
                bool b = SendByteArray(ref ClientSocket, ContainLengthAsHeader, MaxDataSize, data);
                if (b) LastTransferDateTime = DateTime.Now;
                return b;
            }

            /// Receive byte array from TCP Server.
            private void ReceiveByteArray()
            {
                int iReceived;/// how many bytes are total received.
                byte[] byteBuffer;
                byte[] byteData;
                try
                {
                    if (ClientSocket == null || ClientSocket.Connected == false) return;
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
                            Logger?.Debug("Socket receives data. Remote host = {0}, local host = {1}, #Received = {2}", RemoteEndPoint, LocalEndPoint, iReceived);
                            if (EnableAnalyzeIncomingData)
                            {
                                /// Add data to the incoming buffer.
                                lock (IncomingBufferQueueLocker)
                                {
                                    //if (IncomingBufferQueue == null) IncomingBufferQueue = new Queue<byte[]>();
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
                                Logger?.Debug("Socket receives 0 data. Remote host = {0}, local host = {1}", RemoteEndPoint, LocalEndPoint);
                                if (EnableAnalyzeIncomingData)
                                {
                                    /// Add a null data to buffer, to indicate it is the end of data.
                                    lock (IncomingBufferQueueLocker)
                                    {
                                        //if (IncomingBufferQueue == null) IncomingBufferQueue = new Queue<byte[]>();
                                        IncomingBufferQueue.Enqueue(null);
                                    }
                                }
                                else
                                {
                                    /// Add data to the list directly.
                                    AddDataToIncomingDataQueue(DateTime.Now, null);
                                }
                            }
                        }
                    }
                    catch (Exception ex2)
                    {
                        Logger?.Error("Remote host = {0}, local host = {1}", RemoteEndPoint, LocalEndPoint);
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
                    Logger?.Error("Remote host = {0}, local host = {1}", RemoteEndPoint, LocalEndPoint);
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
                try
                {
                    //if (mQueueOfIncomingBuffer == null || mQueueOfIncomingBuffer.Count < 1) { return; }
                    if (ContainLengthAsHeader)
                    {
                        /// The case that data contains the length as header.
                        /// TT edited on 2018-08-21 to lock the common resource.
                        while (TryDequeueAtIncomingBufferQueue(out byteDataInQueue))
                        {
                            //if (byteDataInQueue == null) { return; }
                            if (byteDataInQueue != null)
                            {
                                Logger?.Debug("AnalyzeIncomingBuffer. byteDataInQueue.Length = {0}, remote host = {1}, local host = {2}", byteDataInQueue.Length, RemoteEndPoint, LocalEndPoint);
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
                                            if (IncomingLengthBuffer != null) IncomingLengthBuffer = null;
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
                                        if (iCopy > 0) Array.Copy(byteDataInQueue, iIndex1, IncomingLengthBuffer, IncomingLengthIndex, iCopy);
                                        IncomingLengthIndex += iCopy;
                                        iIndex1 += iCopy;
                                        /// Check if finish to copy the length.
                                        if (IncomingLengthIndex < 4) { }/// do nothing and continue to loop.
                                        else
                                        {
                                            if (IncomingLengthIndex == 4)
                                            {
                                                IncomingContentSize = BitConverter.ToInt32(IncomingLengthBuffer, 0);
                                                if (IncomingContentBuffer != null) IncomingContentBuffer = null;
                                                if (IncomingContentSize > 0) IncomingContentBuffer = new byte[IncomingContentSize];

                                                IsIncomingDataLength = false;/// go to 3rd state.
                                                IncomingContentIndex = 0;/// reset the index of content again.
                                            }
                                            else
                                            {
                                                Logger?.Error("AnalyzeIncomingBuffer. Too many bytes of length are copied. It is unexpected. Remote host = {0}, local host = {1}", RemoteEndPoint, LocalEndPoint);
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
                                        if (iCopy > 0) Array.Copy(byteDataInQueue, iIndex1, IncomingContentBuffer, IncomingContentIndex, iCopy);
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
                                                Logger?.Error("AnalyzeIncomingBuffer. Too many bytes of content are copied. It is unexpected. Remote host = {0}, local host = {1}", RemoteEndPoint, LocalEndPoint);
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
                            if ((byteDataInQueue?.Length ?? 0) < 1)
                            {
                                /// if there is a zero-byte content, assume that it is the end of data.
                                if (IncomingBufferList != null)
                                {
                                    /// https://stackoverflow.com/questions/4868113/convert-listbyte-to-one-byte-array
                                    //byte[] byteFinalData = mListOfIncomingBuffer.SelectMany(x => x).ToArray();
                                    byte[] byteFinalData = null;
                                    System.Collections.Generic.IEnumerable<byte> oIEnumerable = null;
                                    if (IncomingBufferList != null) oIEnumerable = IncomingBufferList.SelectMany(x => x);
                                    if (oIEnumerable != null) byteFinalData = oIEnumerable.ToArray();
                                    AddDataToIncomingDataQueue(DateTime.Now, byteFinalData);
                                    /// Release memory of the list.
                                    IncomingBufferList.Clear();
                                    IncomingBufferList = null;
                                }
                            }
                            else
                            {
                                /// add data into temp list.
                                if (IncomingBufferList == null) IncomingBufferList = new List<byte[]>();
                                IncomingBufferList.Add(byteDataInQueue);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger?.Error("Remote host = {0}, local host = {1}", RemoteEndPoint, LocalEndPoint);
                    Logger?.Error(ex);
                }
            }

            /// Thread to analyze the incoming buffer.
            private void ProcessAnalyzeIncomingBuffer()
            {
                DateTime tNow, tRef, tRefLog;
                string remoteEndPoint = "";
                string localEndPoint = "";
                try
                {
                    tRefLog = DateTime.Now;
                    tRef = tRefLog.AddHours(-1);
                    remoteEndPoint = RemoteEndPoint;
                    localEndPoint = LocalEndPoint;
                    Logger?.Debug("Socket begins to analyze the incoming buffer. Remote host = {0}, local host = {1}", remoteEndPoint, localEndPoint);
                    //while (IsExit == false && (ClientSocket?.Connected ?? false))
                    while (IsExit == false && this.IsConnected)
                    {
                        tNow = DateTime.Now;
                        if (ReceiveDataInterval == 0 || (ReceiveDataInterval > 0 && (int)(tNow - tRef).TotalSeconds >= ReceiveDataInterval))
                        {
                            tRef = tNow;
                            if (EnableAnalyzeIncomingData) AnalyzeIncomingBuffer();
                        }
                        if (ProcessVerificationInterval > 0 && (int)(tNow - tRefLog).TotalSeconds >= ProcessVerificationInterval)
                        {
                            tRefLog = tNow;
                            Logger?.Debug(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + " is running. Remote host = {0}, local host = {1}", remoteEndPoint, localEndPoint);
                        }
                        if (SleepingIntervalInMS >= 0) System.Threading.Thread.Sleep(SleepingIntervalInMS);
                    }
                    Logger?.Debug("Socket stops to analyze the incoming buffer. Remote host = {0}, local host = {1}", remoteEndPoint, localEndPoint);
                }
                catch (Exception ex)
                {
                    Logger?.Error("Remote host = {0}, local host = {1}", remoteEndPoint, localEndPoint);
                    Logger?.Error(ex);
                }
            }

            /// Thread to receive data.
            private void ProcessReceiveData()
            {
                DateTime tNow, tRef, tRefLog;
                string remoteEndPoint = "";
                string localEndPoint = "";
                try
                {
                    tRefLog = DateTime.Now;
                    tRef = tRefLog.AddHours(-1);
                    remoteEndPoint = RemoteEndPoint;
                    localEndPoint = LocalEndPoint;
                    Logger?.Debug("Socket begins to receive data. Remote host = {0}, local host = {1}", remoteEndPoint, localEndPoint);
                    //while (IsExit == false && (ClientSocket?.Connected ?? false))
                    while (IsExit == false && this.IsConnected)
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
                            Logger?.Debug(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + " is running. Remote host = {0}, local host = {1}", remoteEndPoint, localEndPoint);
                        }
                        if (SleepingIntervalInMS >= 0) System.Threading.Thread.Sleep(SleepingIntervalInMS);
                    }
                    Logger?.Debug("Socket stops to receive data. Remote host = {0}, local host = {1}", remoteEndPoint, localEndPoint);
                }
                catch (Exception ex)
                {
                    Logger?.Error("Remote host = {0}, local host = {1}", remoteEndPoint, localEndPoint);
                    Logger?.Error(ex);
                }
            }

            /// Disconnect sockets which exceed maximum connection duration.
            private void DisconnectSocketExceedingMaxConnectionDuration(DateTime tNow)
            {
                try
                {
                    if (MaxConnectionDuration <= 0) return;
                    if ((int)(tNow - InitialDateTime).TotalSeconds > MaxConnectionDuration)
                    {
                        Logger?.Debug("Disconnect socket as exceeding maximum connection duration {0} second(s). Remote host = {0}, local host = {2}", MaxConnectionDuration, RemoteEndPoint, LocalEndPoint);
                        Disconnect();
                    }
                }
                catch (Exception ex)
                {
                    Logger?.Error("Remote host = {0}, local host = {1}", RemoteEndPoint, LocalEndPoint);
                    Logger?.Error(ex);
                }
            }

            private void DisconnectSocketExceedingMaxIdleDuration(DateTime tNow)
            {
                try
                {
                    if (MaxIdleDuration < 0) return;
                    if ((int)(tNow - LastTransferDateTime).TotalSeconds > MaxIdleDuration)
                    {
                        Logger?.Debug("Disconnect socket as exceeding maximum idle duration {0} second(s). Remote host = {0}, local host = {2}", MaxIdleDuration, RemoteEndPoint, LocalEndPoint);
                        Disconnect();
                    }
                }
                catch (Exception ex)
                {
                    Logger?.Error("Remote host = {0}, local host = {1}", RemoteEndPoint, LocalEndPoint);
                    Logger?.Error(ex);
                }
            }

            private void HandleIncomingDataQueue()
            {
                List<DataPackage> tempList = null;
                try
                {
                    lock (IncomingDataQueueLocker)
                    {
                        if (IncomingDataQueue == null || IncomingDataQueue.Count < 1) return;
                        int iMax = 5;
                        int i = 0;
                        tempList = new List<DataPackage>();
                        while ((IncomingDataQueue?.Count ?? 0) > 0 && i < iMax)
                        {
                            tempList.Add(IncomingDataQueue?.Dequeue());/// pass to temp list in order to unlock the list earlier.
                            i += 1;
                        }
                    }
                    if (tempList != null)
                    {
                        foreach (DataPackage o in tempList)
                        {
                            try { ExternalActToHandleIncomingData(o); }
                            catch (Exception ex2)
                            {
                                Logger?.Error("Remote host = {0}, local host = {1}", RemoteEndPoint, LocalEndPoint);
                                Logger?.Error(ex2);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger?.Error("Remote host = {0}, local host = {1}", RemoteEndPoint, LocalEndPoint);
                    Logger?.Error(ex);
                }
                finally
                {
                    if (tempList != null)
                    {
                        tempList.Clear();
                        tempList = null;
                    }
                }
            }

            private void ProcessOtherProcesses()
            {
                DateTime tNow, tRef, tRefIncomingData;
                string remoteEndPoint = "";
                string localEndPoint = "";
                byte[] oByteArrayHeartbeat = null;
                try
                {
                    tRef = DateTime.Now.AddHours(-1);
                    tRefIncomingData = tRef;
                    remoteEndPoint = RemoteEndPoint;
                    localEndPoint = LocalEndPoint;
                    Logger?.Debug("Socket begins to run other processes. Remote host = {0}, local host = {1}", remoteEndPoint, localEndPoint);
                    if (!this.ContainLengthAsHeader) oByteArrayHeartbeat = this.HeartbeatData;
                    //while (IsExit == false && (ClientSocket?.Connected ?? false))
                    while (IsExit == false && this.IsConnected)
                    {
                        tNow = DateTime.Now;
                        /// Check if it exceeds maximum connection duration. If yes, disconnect it.
                        DisconnectSocketExceedingMaxConnectionDuration(tNow);
                        /// Check if it exceeds maximum idle duration. If yes, disconnect it.
                        DisconnectSocketExceedingMaxIdleDuration(tNow);
                        /// Send heartbeat to clients.
                        if (this.HeartbeatInterval == 0 || (this.HeartbeatInterval > 0 && ((int)(tNow - tRef).TotalSeconds >= this.HeartbeatInterval)))
                        {
                            tRef = tNow;
                            SendByteArray(oByteArrayHeartbeat);
                        }
                        /// Handle the incoming data by the outter method.
                        if (this.ExternalActToHandleIncomingData != null && this.ReceiveDataInterval >= 0 && (int)(tNow - tRefIncomingData).TotalSeconds >= this.ReceiveDataInterval)
                        {
                            tRefIncomingData = tNow;
                            HandleIncomingDataQueue();
                        }
                        if (SleepingIntervalInMS >= 0) System.Threading.Thread.Sleep(SleepingIntervalInMS);
                    }
                    Logger?.Debug("Socket stops to run other processes. Remote host = {0}, local host = {1}", remoteEndPoint, localEndPoint);
                }
                catch (Exception ex)
                {
                    Logger?.Error("Remote host = {0}, local host = {1}", remoteEndPoint, localEndPoint);
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
                    if ((IPs?.Length ?? 0) < 1)
                    {
                        Logger?.Error("Socket cannot find IP address for {0}", serverHost);
                        return null;
                    }
                    /// Find the first IPv4 address, as IPv6 addresses are not supported.
                    int i = 0; bool bLoop = true;
                    while (bLoop && i < IPs.Length)
                    {
                        oIP = IPs[i];
                        if (oIP != null && string.IsNullOrEmpty(oIP.ToString()) == false && oIP.ToString().Contains(":") == false)
                            bLoop = false;
                        else i += 1;
                    }
                    if (bLoop)
                    {
                        Logger?.Error("Socket cannot find IPv4 address for {0}", serverHost);
                        return null;
                    }
                    Logger?.Debug("Socket finds IP address of server is {0}", oIP);
                    /// Client socket.
                    oClientSocket = new System.Net.Sockets.Socket(System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
                    //oClientSocket.Connect(IPs[0], iServerPort);
                    oClientSocket.Connect(oIP, serverPort);
                    if ((oClientSocket?.Connected ?? false) == false)
                    {
                        Logger?.Error("Socket cannot establish connection to the server {0}:{1}", serverHost, serverPort);
                        Disconnect(ref oClientSocket);
                        return null;
                    }
                    else
                    {
                        localSocket = oClientSocket?.LocalEndPoint?.ToString();
                        Logger?.Debug("Socket connects successfully. Remote host = {0}:{1}, local host = {2}", serverHost, serverPort, localSocket);
                        return oClientSocket;
                    }
                }
                catch (Exception ex)
                {
                    Logger?.Error("Remote host = {0}:{1}", serverHost, serverPort);
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
                //bool b;
                string remoteEndPoint = "";
                string localEndPoint = "";
                try
                {
                    this.IsExit = true;
                    remoteEndPoint = RemoteEndPoint;
                    localEndPoint = LocalEndPoint;
                    Disconnect(ref ClientSocket);
                    if (timeoutInSecond < 0) timeoutInSecond = 0;
                    tRef = DateTime.Now;
                    //b = true;
                    //while (b && (int)(DateTime.Now - tRef).TotalSeconds < timeoutInSecond)
                    //{
                    //    //b = false;
                    //    //if (ThreadToReceiveData?.IsAlive ?? false) b = true;
                    //    ////else if (ThreadToAnalyzeIncomingBuffer != null || ThreadToAnalyzeIncomingBuffer.IsAlive) b = true;
                    //    //else if (ThreadToAnalyzeIncomingBuffer?.IsAlive ?? false) b = true;
                    //    b = (ThreadToReceiveData?.IsAlive ?? false) || (ThreadToAnalyzeIncomingBuffer?.IsAlive ?? false);
                    //    if (b && sleepingIntervalInMS >= 0) System.Threading.Thread.Sleep(sleepingIntervalInMS);
                    //}
                    while ((int)(DateTime.Now - tRef).TotalSeconds < timeoutInSecond && (
                        (ThreadToReceiveData?.IsAlive ?? false) || (ThreadToAnalyzeIncomingBuffer?.IsAlive ?? false)
                        ))
                    {
                        if (sleepingIntervalInMS >= 0) System.Threading.Thread.Sleep(sleepingIntervalInMS);
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
                    Logger?.Error("Remote host = {0}, local host = {1}", remoteEndPoint, localEndPoint);
                    Logger?.Error(ex);
                }
            }

            public void StartReceivingData()
            {
                try
                {
                    /// thread to receive data.
                    ThreadToReceiveData = new System.Threading.Thread(new System.Threading.ThreadStart(ProcessReceiveData))
                    {
                        Name = "ProcessReceiveData-" + RemoteHost + ":" + RemotePort.ToString()
                    };
                    ThreadToReceiveData.Start();/// This "start" statement can be divided from this function.
                                                /// thread to analyze the incoming buffer.
                    ThreadToAnalyzeIncomingBuffer = new System.Threading.Thread(new System.Threading.ThreadStart(ProcessAnalyzeIncomingBuffer))
                    {
                        Name = "ProcessAnalyzeIncomingBuffer-" + RemoteHost + ":" + RemotePort.ToString()
                    };
                    ThreadToAnalyzeIncomingBuffer.Start();
                    /// thread to other processes.
                    ThreadToOtherProcesses = new System.Threading.Thread(new System.Threading.ThreadStart(ProcessOtherProcesses))
                    {
                        Name = "ProcessOtherProcesses-" + RemoteHost + ":" + RemotePort.ToString()
                    };
                    ThreadToOtherProcesses.Start();
                }
                catch (Exception ex){ Logger?.Error(ex); }
            }

            /// Start to connect TCP Server.
            public bool StartConnection()
            {
                try
                {
                    /// Disconnect first.
                    /// Check if IncomingDataQueue is initialized.
                    /// Connect.
                    /// Start the thread to receive data.
                    Disconnect(ref ClientSocket);
                    if (IncomingDataQueue == null)
                    {
                        Logger?.Error("Socket finds that IncomingDataQueue is not initialized. Stop connection. Remote host = {0}:{1}", RemoteHost, RemotePort);
                        return false;
                    }
                    ClientSocket = Connect(RemoteHost, RemotePort);
                    //if (ClientSocket == null || ClientSocket.Connected == false)
                    if (!this.IsConnected)
                    {
                        Disconnect(ref ClientSocket);
                        return false;
                    }
                    StartReceivingData();
                    return true;
                }
                catch (Exception ex)
                {
                    Logger?.Error("Remote host = {0}:{1}", RemoteHost, RemotePort);
                    Logger?.Error(ex);
                    return false;
                }
            }

            /// Connect to remote host and send the byte array. After sending, the socket is shutdown and closed after a delay.
            /// Return value = true if success. Otherwise, fail.
            /// delayDisconnetInMS = Delay interval in milli-second to disconnect server. The delay disconnect to remote host is required. Otherwise, data is no time to be sent to the remote host.
            /// https://docs.microsoft.com/en-us/dotnet/framework/network-programming/synchronous-client-socket-example
            public static bool ConnectServerAndSendData(string serverHost, int serverPort, bool isContainLengthAsHeader, byte[] data) { return ConnectServerAndSendData(serverHost, serverPort, isContainLengthAsHeader, data, 5000); }
            public static bool ConnectServerAndSendData(string serverHost, int serverPort, bool isContainLengthAsHeader, byte[] data, int delayDisconnetInMS) { return ConnectServerAndSendData(serverHost, serverPort, isContainLengthAsHeader, data, delayDisconnetInMS, 2000000000); }
            public static bool ConnectServerAndSendData(string serverHost, int serverPort, bool isContainLengthAsHeader, byte[] data, int delayDisconnetInMS, int maxDataSize)
            {
                System.Net.Sockets.Socket oClientSocket = null;
                try
                {
                    oClientSocket = Connect(serverHost, serverPort);
                    if ((oClientSocket?.Connected ?? false) == false) return false;
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
                        if (delayDisconnetInMS >= 0) System.Threading.Thread.Sleep(delayDisconnetInMS);
                        Disconnect(ref oClientSocket);
                    }
                    catch (Exception ex2)
                    {
                        Logger?.Error("Server socket = {0}:{1}", serverHost, serverPort);
                        Logger?.Error(ex2);
                    }
                }
            }

            private void ConstructorAction(Queue<DataPackage> incomingDataQueue, object incomingDataQueueLocker)
            {
                IncomingDataQueue = incomingDataQueue;
                IncomingDataQueueLocker = incomingDataQueueLocker;
            }

            public Client(string remoteHost, int remotePort, Queue<DataPackage> incomingDataQueue, object incomingDataQueueLocker)
            {
                RemoteHost = remoteHost;
                RemotePort = remotePort;
                InitialDateTime = LastReceivedDateTime = LastTransferDateTime = DateTime.Now;
                ConstructorAction(incomingDataQueue, incomingDataQueueLocker);
            }

            /// Initialize this class if the socket has been already connected.
            public Client(System.Net.Sockets.Socket clientSocket, Queue<DataPackage> incomingDataQueue, object incomingDataQueueLocker)
            {
                this.ClientSocket = clientSocket;
                System.Net.IPEndPoint iPEndPoint = (System.Net.IPEndPoint)this.ClientSocket?.RemoteEndPoint;
                RemoteHost = iPEndPoint?.Address?.ToString();
                RemotePort = iPEndPoint?.Port ?? -1;
                InitialDateTime = LastReceivedDateTime = LastTransferDateTime = DateTime.Now;
                ConstructorAction(incomingDataQueue, incomingDataQueueLocker);
                Logger?.Debug("TCP Server connects client {0}:{1}", RemoteHost, RemotePort);
            }
        }

        public class Server
        {
            /// TCP Server by a synchronous socket in the threading model.
            /// Data unit is byte.
            /// http://csharp.net-informations.com/communications/csharp-server-socket.htm
            /// https://docs.microsoft.com/en-us/dotnet/framework/network-programming/synchronous-server-socket-example
            /// https://docs.microsoft.com/en-us/dotnet/framework/network-programming/asynchronous-server-socket-example
            /// http://www.yoda.arachsys.com/csharp/parameters.html
            /// https://blogs.msdn.microsoft.com/oldnewthing/20060801-19/?p=30273
            /// https://www.codeproject.com/Articles/990474/Scalable-Socket-Server
            /// http://www.tutorialsteacher.com/csharp/array-csharp

            /// Parameters for private user.
            private bool IsExit = false;
            private bool IsReadyAccept = false;/// flag to indicate whether it is ready to accept client.
            private System.Net.Sockets.Socket ServerSocket = null;
            private System.Threading.Thread ThreadToAcceptClient = null;
            private System.Threading.Thread ThreadToOtherProcesses = null;
            private System.Threading.Thread ThreadToSendData = null;

            private Queue<DataPackage> OutgoingDataQueue = new Queue<DataPackage>();
            private readonly object OutgoingDataQueueLocker = new object();
            private Queue<DataPackage> FailedOutgoingDataQueue = new Queue<DataPackage>();/// queue of TCP outgoing data sent failed.
            private readonly object FailedOutgoingDataQueueLocker = new object();

            private List<Client> InnerClientList = null;/// list of TCP client objects.
            private object InnerClientListLocker = new object();

            /// Properties.
            public static Helpers.TLog Logger { get; set; }
            //public int ListeningPort { get; set; } = 8002;
            public readonly int ListeningPort;
            public int AcceptInterval { get; set; } = 1;/// Time interval in seconds that TCP server accepts client. If it is lesser than 1, the process will do immediately without waiting. The default value is 1.
            public bool ContainLengthAsHeader { get; set; } = true;/// flag to indicate if the data contains length (4-byte integer) as a header of package. The header is used to divide a package unit. The default value is true.
            public bool EnableAnalyzeIncomingData { get; set; } = true;/// flag to enable to analyze the incoming data from server. A package is preserved when adding to the queue of incoming data. Note that large package exceeding the buffer size is divided as several parts when receiving. The users may need to handle how to obtain the complete package if disable this flag. The default value is true.
            private byte[] _HeartbeatData = Encoding.UTF8.GetBytes("~");
            public byte[] HeartbeatData/// Data in byte array of heartbeat. This parameter will be used only if mbContainLengthAsHeader = false. Space and Tab will be neglect. Sending heartbeat is used to check if the sockets are still connected. The default value is the byte array of ~.
            {
                get { return _HeartbeatData; }
                set
                {
                    if (value == null) _HeartbeatData = null;
                    else
                    {
                        _HeartbeatData = new byte[value.Length];
                        Array.Copy(value, 0, _HeartbeatData, 0, value.Length);
                    }
                }
            }
            public int HeartbeatInterval { get; set; } = -1;/// Time interval in seconds that the local host sends heartbeat to the remote host. If it is negative, there is no heartbeat. Sending heartbeat is used to check if the connection still occurs. The default value is -1.
            public int MaxClient { get; set; } = 200;/// Maximum TCP clients allowed to be connected by the server. If it is 0, no client is allowed to be connected. If it is negative, no limitation to accept clients. The default value is 200.
            public int MaxConnectionDuration { get; set; } = 600;/// Maximum connection duration in seconds between the local host and remote host. If the time exceeds, it will disconnect automatically even the connection is normal. If it is negative, there is no maximum connection duration, hence the connection can preserve forever. The default value is 600.
            public int MaxIdleDuration { get; set; } = -1;/// Maximum idle duration in seconds between the local host and remote host. If the time exceeds, it will disconnect automatically. If it is negative, there is no maximum idle duration, hence it will not check the idle duration. The default value is -1.
            public int MaxDataSend { get; set; } = -1;/// Maximum unit of data sent from the local host each time. If it is 0, no data is sent. If it is negative, the process will send all data in the queue without waiting. The default value is -1.
            private int _MaxDataSize = 104857600;
            public int MaxDataSize { get { return _MaxDataSize; } set { _MaxDataSize = value < 0 ? 0 : value; } }/// Maximum size of data in bytes. The default value is 104857600.
            public int ProcessVerificationInterval { get; set; } = 600;/// Time interval in seconds to verify the process is still running or not. If it is running, a log is written. If the value is negative or zero, it does not verify. The default value is 600.
            public int ReceiveDataInterval { get; set; } = 0;/// Time interval in seconds to receive Data. If it is 0, the process will do immediately without waiting. If it is negative, no data is received. The default value is 0.
            private int _ReceiveTotalBufferSize = 10485760;
            public int ReceiveTotalBufferSize { get { return _ReceiveTotalBufferSize; } set { _ReceiveTotalBufferSize = value < 10485760 ? 10485760 : value; } }/// Total buffer size in bytes for receiving data, the minimum value is 10485760.
            public int SleepingIntervalInMS { get; set; } = 100;/// Sleeping interval in milliseconds. This sleeping interval helps to avoid the application too busy. If it is negative, no sleep. The default value is 100.

            public Queue<DataPackage> IncomingDataQueue = null;/// queue of TCP incoming data.
            public object IncomingDataQueueLocker = null;/// lock object for queue of TCP incoming data.

            /// External action to handle incoming data. This class loops each data package in the incoming data queue and pass the data package to the external action.
            /// This workflow should be done in this Server class and should not be done in the Client class, although there are same function in the Client class.
            /// Otherwise, various clients access one common incoming data queue, it makes the incoming data queue always be locked.
            /// Example:
            /// void MyFunction(TTcpSocket.DataPackage o)
            /// {
            ///     // Do somethings.
            ///     Console.Write(o.Host + ":" + o.Port.ToString());
            /// }
            /// MySocket.ExternalActToHandleIncomingData = MyFunction(o);
            public delegate void ExternalActionToHandleIncomingData(DataPackage o);
            public ExternalActionToHandleIncomingData ExternalActToHandleIncomingData { get; set; } = null;

            /// Abort a specific thread and set it to null.
            /// Return Value = Result whether abort the thread successfully. True if success. Otherwise, false.
            /// th = target thread that will be aborted.
            /// bIsDebugLog = Whether the log is a debug bug or not.
            private static bool AbortThread(ref System.Threading.Thread th)
            {
                if (th == null) return true;
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
            private static int NumberOfItemsInQueue(Queue<DataPackage> queue, object locker)
            {
                try
                {
                    if (locker == null) return queue?.Count ?? 0;
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
                    //        if (oItem != null) { rList.Add(oItem.RemoteEndPoint); }
                    //        i += 1;
                    //    }
                    //}
                    //string[] returnStringArray = rList.Count < 1 ? null : rList.ToArray();
                    //return returnStringArray;
                    lock (InnerClientListLocker)
                    {
                        return InnerClientList?.Select(x => x?.RemoteEndPoint)?.ToArray();
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
                    if ((array?.Length ?? 0) < 1) return "#Items = 0";
                    sb = new StringBuilder();
                    sb.Append("#Items = ");
                    if (array != null) i = array.Length;
                    sb.Append(i.ToString()).Append(recordSeparator);
                    if (array != null) sb.Append(string.Join(recordSeparator, array)).Append(recordSeparator);
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
                    host = host == null ? "" : host.Trim();

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
                        return InnerClientList?.FindIndex(x => x != null && port == x.RemotePort && host.Equals(x.RemoteHost)) ?? -1;
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
            private Client InnerClientTByHostAndPort(string host, int port)
            {
                try
                {
                    host = host == null ? "" : host.Trim();
                    lock (InnerClientListLocker)
                    {
                        return InnerClientList?.Find(x => x != null && port == x.RemotePort && host.Equals(x.RemoteHost));
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
                    host = host == null ? "" : host.Trim();

                    //if (InnerClientList == null || InnerClientList.Count < 1) return;
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
                        while (i < (InnerClientList?.Count ?? 0))
                        {
                            Client oItem = InnerClientList[i];
                            if (oItem != null)
                            {
                                if (oItem.RemotePort == port && host.Equals(oItem.RemoteHost))
                                    oItem.StopConnection(2);
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
                List<Client> vListOfDeleted = null;
                try
                {
                    lock (InnerClientListLocker)
                    {
                        if (InnerClientList != null)
                        {
                            InnerClientList.RemoveAll(x => x == null);
                            vListOfDeleted = InnerClientList.FindAll(x => x.IsConnected == false);
                            if ((vListOfDeleted?.Count ?? 0) > 0)
                            {
                                foreach (Client o in vListOfDeleted)
                                {
                                    if (o != null && InnerClientList != null)
                                    {
                                        if (string.IsNullOrEmpty(o.RemoteHost))
                                            InnerClientList.RemoveAll(x => x.RemotePort == o.RemotePort && string.IsNullOrEmpty(x.RemoteHost));
                                        else InnerClientList.RemoveAll(x => x.RemotePort == o.RemotePort && o.RemoteHost.Equals(x.RemoteHost));
                                        //o.ForceDisconnect();
                                    }
                                }
                            }
                        }
                    }
                    /// Disconnect the clients out of the LOCK block.
                    if ((vListOfDeleted?.Count ?? 0) > 0)
                    {
                        foreach (Client o in vListOfDeleted)
                        { if (o != null) o.StopConnection(); }
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
                        if (i >= 0) System.Threading.Thread.Sleep(i);
                    }
                    Logger?.Debug("TCP Server stops to trim list of clients.");
                }
                catch (Exception ex) { Logger?.Error(ex); }
            }

            private bool TryDequeueAtOutgoingDataQueue(out DataPackage oOutput)
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

            private bool TryDequeueAtFailedOutgoingDataQueue(out DataPackage oOutput)
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
                    if (string.IsNullOrEmpty(host)) return;
                    /// add data to queue.
                    lock (OutgoingDataQueueLocker)
                    {
                        //if (OutgoingDataQueue == null) OutgoingDataQueue = new Queue<TcpSocketData>();
                        OutgoingDataQueue.Enqueue(new DataPackage()
                        {
                            Timestamp = DateTime.Now,
                            Host = host,
                            Port = port,
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
            private void QueueToFailedOutgoingData(ref DataPackage vData)
            {
                try
                {
                    if (vData == null) return;
                    lock (FailedOutgoingDataQueueLocker)
                    {
                        //if (FailedOutgoingDataQueue == null) FailedOutgoingDataQueue = new Queue<TcpSocketData>();
                        FailedOutgoingDataQueue.Enqueue(vData);
                    }
                }
                catch (Exception ex) { Logger?.Error(ex); }
            }

            private void ProcessSendData_routine1(DataPackage vData)
            {
                try
                {
                    if (vData == null) return;
                    string sHost = "";
                    if (!string.IsNullOrEmpty(vData.Host)) sHost = vData.Host.ToUpper();
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
                                foreach (Client o in InnerClientList)
                                {
                                    if (o != null)
                                    { if (!o.SendByteArray(vData.ByteArray)) QueueToFailedOutgoingData(ref vData); }
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
                                foreach (Client o in InnerClientList)
                                {
                                    if (o != null && o.RemotePort == vData.Port && sHost.Equals(o.RemoteHost))
                                    {
                                        if (!o.SendByteArray(vData.ByteArray)) QueueToFailedOutgoingData(ref vData);
                                        b = false;/// set that it already finds InnerClient.
                                    }
                                }
                            }
                        }
                        if (b) Logger?.Debug("{0}.{1}. Cannot find the client in the list. Client socket = {2}:{3}", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, System.Reflection.MethodBase.GetCurrentMethod().Name, sHost, vData.Port);
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
                    //    if (oItem != null) oItem.SendByteArray(oByteArrayHeartbeat);
                    //    i += 1;
                    //}
                    lock (InnerClientListLocker)
                    {
                        if (InnerClientList != null)
                        {
                            foreach (Client o in InnerClientList)
                            { if (o != null) o.SendByteArray(oByteArrayHeartbeat); }
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
                            foreach (Client o in InnerClientList)
                            {
                                if (o != null)
                                {
                                    if ((int)(tNow - o.InitialDateTime).TotalSeconds > this.MaxConnectionDuration)
                                    {
                                        Logger?.Debug("Disconnect socket as exceeding maximum connection duration {0} second(s). Listening Port: {1}. Client: {2}:{3}", this.MaxConnectionDuration, this.ListeningPort, o.RemoteHost, o.RemotePort.ToString());
                                        o.StopConnection();
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
                            foreach (Client o in InnerClientList)
                            {
                                if (o != null)
                                {
                                    if ((int)(tNow - o.LastTransferDateTime).TotalSeconds > this.MaxIdleDuration)
                                    {
                                        Logger?.Debug("Disconnect socket as exceeding maximum idle duration {0} second(s). Listening Port: {1}. Client: {2}:{3}", this.MaxIdleDuration, this.ListeningPort, o.RemoteHost, o.RemotePort.ToString());
                                        o.StopConnection();
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
                    // if (mbContainLengthAsHeader) oByteArrayHeartbeat = new byte[0];
                    //else oByteArrayHeartbeat = System.Text.Encoding.UTF8.GetBytes("~");
                    if (!this.ContainLengthAsHeader) oByteArrayHeartbeat = this.HeartbeatData;
                    while (this.IsExit == false && this.ServerSocket != null)
                    {
                        tNow = DateTime.Now;
                        /// Send data to clients.
                        if (this.MaxDataSend < 0)
                        {
                            DataPackage oData = null;
                            while (TryDequeueAtOutgoingDataQueue(out oData))
                            { ProcessSendData_routine1(oData); }
                        }
                        else
                        {
                            DataPackage oData = null;
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
                            Logger?.Debug("{0}.{1} is running.", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
                        }
                        if (this.SleepingIntervalInMS >= 0) System.Threading.Thread.Sleep(this.SleepingIntervalInMS);
                    }
                    Logger?.Debug("TCP Server stops to send data to clients.");
                }
                catch (Exception ex) { Logger?.Error(ex); }
                finally { oByteArrayHeartbeat = null; }
            }

            private void HandleIncomingDataQueue()
            {
                List<DataPackage> tempList = null;
                try
                {
                    lock (IncomingDataQueueLocker)
                    {
                        if (IncomingDataQueue == null || IncomingDataQueue.Count < 1) return;
                        int iMax = 10;
                        int i = 0;
                        tempList = new List<DataPackage>();
                        while ((IncomingDataQueue?.Count ?? 0) > 0 && i < iMax)
                        {
                            tempList.Add(IncomingDataQueue?.Dequeue());/// pass to temp list in order to unlock the list earlier.
                            i += 1;
                        }
                    }
                    if (tempList != null)
                    {
                        foreach (DataPackage o in tempList)
                        {
                            try { ExternalActToHandleIncomingData(o); }
                            catch (Exception ex2) { Logger?.Error(ex2); }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger?.Error(ex);
                }
                finally
                {
                    if (tempList != null)
                    {
                        tempList.Clear();
                        tempList = null;
                    }
                }
            }

            private void ProcessOtherProcesses()
            {
                DateTime tNow, tRefIncomingData, tRefLog;
                try
                {
                    Logger?.Debug("TCP Server begins to run some processes.");
                    tRefIncomingData = DateTime.Now.AddHours(-1);
                    tRefLog = tRefIncomingData;
                    while (this.IsExit == false && this.ServerSocket != null)
                    {
                        tNow = DateTime.Now;
                        /// Handle the incoming data by the outter method.
                        if (this.ExternalActToHandleIncomingData != null && this.ReceiveDataInterval >= 0 && (int)(tNow - tRefIncomingData).TotalSeconds >= this.ReceiveDataInterval)
                        {
                            tRefIncomingData = tNow;
                            HandleIncomingDataQueue();
                        }
                        /// log.
                        if (this.ProcessVerificationInterval > 0 && (int)(tNow - tRefLog).TotalSeconds >= this.ProcessVerificationInterval)
                        {
                            tRefLog = tNow;
                            Logger?.Debug("{0}.{1} is running.", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
                        }
                        if (this.SleepingIntervalInMS >= 0) System.Threading.Thread.Sleep(this.SleepingIntervalInMS);
                    }
                    Logger?.Debug("TCP Server stops to run some processes.");
                }
                catch (Exception ex) { Logger?.Error(ex); }
            }

            /// Action after accept a client.
            private void AcceptClientCallback(IAsyncResult ar)
            {
                System.Net.Sockets.Socket oClientSocket = null;
                try
                {
                    if (this.IsExit || this.ServerSocket == null) return;
                    this.ServerSocket = (System.Net.Sockets.Socket)ar.AsyncState;
                    oClientSocket = this.ServerSocket.EndAccept(ar);
                    if (oClientSocket == null) return;
                    /// create the client instance.
                    //InnerClient oItem = new InnerClient(ref oClientSocket, this.IncomingDataQueue, this.IncomingDataQueueLocker)
                    //{
                    //    ContainLengthAsHeader = this.ContainLengthAsHeader,
                    //    EnableAnalyzeIncomingData = this.EnableAnalyzeIncomingData,
                    //    MaxDataSize = this.MaxDataSize,
                    //    ProcessVerificationInterval = this.ProcessVerificationInterval,
                    //    ReceiveDataInterval = this.ReceiveDataInterval,
                    //    ReceiveTotalBufferSize = this.ReceiveTotalBufferSize,
                    //    SleepingIntervalInMS = this.SleepingIntervalInMS
                    //};
                    //oItem.StartReceivingData();

                    Client oItem = new Client(oClientSocket, this.IncomingDataQueue, this.IncomingDataQueueLocker)
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
                        if (InnerClientList == null) InnerClientList = new List<Client>();
                        InnerClientList.Add(oItem);
                    }
                    /// set ready to accept client.
                    this.IsReadyAccept = true;
                }
                catch (Exception ex) { Logger?.Error(ex); }
            }

            /// This function should be run as a thread.
            /// https://msdn.microsoft.com/en-us/library/system.net.sockets.tcplistener.accepttcpclient(v=vs.110).aspx
            private void ProcessAcceptClient()
            {
                try
                {
                    if (this.IsExit) return;
                    Logger?.Debug("TCP Server begins to accept clients.");
                    this.IsReadyAccept = true;/// set ready to accept client.
                    DateTime tRefLog = DateTime.Now;
                    DateTime tRef = tRefLog.AddHours(-1);
                    DateTime tNow;
                    while (this.IsExit == false && this.ServerSocket != null)
                    {
                        tNow = DateTime.Now;
                        if (this.IsReadyAccept && this.ServerSocket != null && (this.AcceptInterval < 1 || (int)(tNow - tRef).TotalSeconds >= this.AcceptInterval))
                        {
                            int i = NumberOfConnectedClients();
                            if (this.MaxClient < 0 || i < this.MaxClient)
                            {
                                tRef = tNow;/// set the time.
                                this.IsReadyAccept = false;/// set NOT to accept client.
                                this.ServerSocket.BeginAccept(new AsyncCallback(AcceptClientCallback), ServerSocket);
                            }
                            else
                                Logger?.Warn("{0}.{1}. Number of Connected Clients exceeds maximum. Number of Connected Clients = {2}. Max = {3}", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, System.Reflection.MethodBase.GetCurrentMethod().Name, i, this.MaxClient);
                        }
                        if (this.ProcessVerificationInterval > 0 && (int)(tNow - tRefLog).TotalSeconds >= this.ProcessVerificationInterval)
                        {
                            tRefLog = tNow;
                            Logger?.Debug("{0}.{1} is running.", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
                        }
                        if (this.SleepingIntervalInMS >= 0) System.Threading.Thread.Sleep(this.SleepingIntervalInMS);
                    }
                    Logger?.Debug("TCP Server stops to accept clients.");
                }
                catch (Exception ex) { Logger?.Error(ex); }
            }

            /// Dequeue the outgoing data, which is sent failed.
            public DataPackage DequeueFailedOutgoingData()
            {
                try
                {
                    bool b = TryDequeueAtFailedOutgoingDataQueue(out DataPackage oOutput);
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
                try
                {
                    this.IsExit = true;/// set to close all clients.
                    if (timeoutInSecond < 0) timeoutInSecond = 0;
                    Logger?.Debug("TCP Server stops listening.");
                    this.ServerSocket = null;
                    /// close the threads in a time interval.
                    tRef = DateTime.Now;
                    //bLoop = true;
                    //while (bLoop && (DateTime.Now - tRef).TotalSeconds < timeoutInSecond)
                    //{
                    //    bLoop = false;
                    //    //if (this.ThreadToAcceptClient != null && this.ThreadToAcceptClient.IsAlive) { bLoop = true; }
                    //    //else if (this.ThreadToSendData != null && this.ThreadToSendData.IsAlive) { bLoop = true; }
                    //    if (ThreadToAcceptClient?.IsAlive ?? false) { bLoop = true; }
                    //    else if (ThreadToSendData?.IsAlive ?? false) { bLoop = true; }
                    //    if (bLoop && sleepingIntervalInMS >= 0) System.Threading.Thread.Sleep(sleepingIntervalInMS);
                    //}
                    while ((DateTime.Now - tRef).TotalSeconds < timeoutInSecond && (
                    (ThreadToAcceptClient?.IsAlive ?? false) || (ThreadToSendData?.IsAlive ?? false) || (ThreadToOtherProcesses?.IsAlive ?? false)
                    ))
                    {
                        if (sleepingIntervalInMS >= 0) System.Threading.Thread.Sleep(sleepingIntervalInMS);
                    }
                    /// abort threads.
                    AbortThread(ref this.ThreadToAcceptClient);
                    AbortThread(ref this.ThreadToSendData);
                    AbortThread(ref this.ThreadToOtherProcesses);
                    /// close all clients.
                    lock (InnerClientListLocker)
                    {
                        if (InnerClientList != null)
                        {
                            int i = 0;
                            while (i < (InnerClientList?.Count ?? 0))
                            {
                                Client oItem = InnerClientList[i];
                                if (oItem != null)
                                {
                                    oItem.StopConnection(timeoutInSecond, sleepingIntervalInMS);
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
                    //System.Net.IPHostEntry iPHostEntry = System.Net.Dns.GetHostEntry("localhost");
                    //System.Net.IPAddress iPAddress = iPHostEntry.AddressList[0];
                    System.Net.IPAddress iPAddress = System.Net.IPAddress.Any;
                    System.Net.IPEndPoint iPEndPoint = new System.Net.IPEndPoint(iPAddress, this.ListeningPort);
                    /// Create an instance.
                    if (this.ServerSocket != null) this.ServerSocket = null;
                    this.ServerSocket = new System.Net.Sockets.Socket(iPAddress.AddressFamily, System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
                    this.ServerSocket.Bind(iPEndPoint);
                    this.ServerSocket.Listen(100);
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
                    /// thread to other processes.
                    this.ThreadToOtherProcesses = new System.Threading.Thread(new System.Threading.ThreadStart(ProcessOtherProcesses))
                    {
                        Name = "ProcessOtherProcesses"
                    };
                    this.ThreadToOtherProcesses.Start();
                    return true;
                }
                catch (Exception ex)
                {
                    Logger?.Error(ex);
                    return false;
                }
            }

            public Server(int listeningPort, Queue<DataPackage> incomingDataQueue, object incomingDataQueueLocker)
            {
                ListeningPort = listeningPort;
                IncomingDataQueue = incomingDataQueue;
                IncomingDataQueueLocker = incomingDataQueueLocker;
            }
        }

        //public class DataPackageType
        //{
        //    public const byte Heartbeat = 0;
        //    public const byte Text = 1;
        //    public const byte File = 2;
        //}

        public enum SerialDataType : byte
        {
            Heartbeat = 0,
            Text = 1,
            File = 2
        }

        /// Return object after deserialization.
        public class DeserializedData
        {
            public SerialDataType DataType;
            public string ErrorMessage;
            public string Text;
            public string Filename;
            public int LastIndexPiece;
            public int IndexPiece;
            public byte[] FileContent;
            /// Destination setting.
            public string DestFolder;
            public string DestFilepath { get { return System.IO.Path.Combine(DestFolder, Filename); } }
        }

        public static class Serialization
        {
            /// This class is to convert a text or file to a byte array, and convert a byte array back to a text or file.
            /// 
            /// Format:
            /// [Serial Data Type][Data in byte array]
            /// 
            /// Text:
            /// [1][Data in byte array]
            /// 
            /// File:
            /// [2][Length of filename (4 bytes)][Filename in byte array][Last index of pieces (4 bytes)][Index of pieces starting from 0 (4 bytes)][File content]

            /// 101M bytes
            //public const int MaxByteLength = 105906176;

            public static byte[] SerializeText(string text)
            {
                if ((text?.Length ?? 0) < 1) return new byte[1] { (byte)SerialDataType.Text };
                byte[] textByteArray = Encoding.UTF8.GetBytes(text);
                //List<byte> tempList = tempList = new List<byte>()
                //{
                //    (byte)SerialDataType.Text
                //};
                //tempList.AddRange(textByteArray);
                //return tempList.ToArray();
                byte[] output = new byte[1 + textByteArray.Length];
                System.Buffer.SetByte(output, 0, (byte)SerialDataType.Text);
                System.Buffer.BlockCopy(textByteArray, 0, output, 1, textByteArray.Length);
                return output;
            }

            public static byte[] SerializeFilePiece(string filename, int lastIndexPiece, int indexPiece, byte[] piece)
            {
                if (string.IsNullOrWhiteSpace(filename)) return null;
                byte[] filenameByteArray = Encoding.UTF8.GetBytes(filename);
                int fileContentLength = piece?.Length ?? 0;
                byte[] output = new byte[1 + filenameByteArray.Length + 12 + fileContentLength];
                System.Buffer.SetByte(output, 0, (byte)SerialDataType.File);
                System.Buffer.BlockCopy(BitConverter.GetBytes(filenameByteArray.Length), 0, output, 1, 4);
                System.Buffer.BlockCopy(filenameByteArray, 0, output, 5, filenameByteArray.Length);
                int dstOffset = 5 + filenameByteArray.Length;
                System.Buffer.BlockCopy(BitConverter.GetBytes(lastIndexPiece), 0, output, dstOffset, 4);
                dstOffset += 4;
                System.Buffer.BlockCopy(BitConverter.GetBytes(indexPiece), 0, output, dstOffset, 4);
                dstOffset += 4;
                if (fileContentLength > 0) System.Buffer.BlockCopy(piece, 0, output, dstOffset, piece.Length);
                return output;
            }

            //public static byte[] SerializeSmallFile(string filename, byte[] fileContent)
            //{
            //    byte[] filenameByteArray = Encoding.UTF8.GetBytes(filename);
            //    //List<byte> tempList = new List<byte>();
            //    //tempList.AddRange(BitConverter.GetBytes(filenameByteArray.Length));
            //    //tempList.AddRange(filenameByteArray);
            //    //tempList.AddRange(fileContent);
            //    //return Serialize(SerialDataType.File, tempList.ToArray());
            //    byte[] filenameLengthByteArray = BitConverter.GetBytes(filenameByteArray.Length);
            //    int fileContentLength = fileContent?.Length ?? 0;
            //    byte[] output = new byte[filenameLengthByteArray.Length + filenameByteArray.Length + fileContentLength];
            //    System.Buffer.BlockCopy(filenameLengthByteArray, 0, output, 0, filenameLengthByteArray.Length);
            //    System.Buffer.BlockCopy(filenameByteArray, 0, output, filenameLengthByteArray.Length, filenameByteArray.Length);
            //    if (fileContentLength > 0) System.Buffer.BlockCopy(fileContent, 0, output, filenameLengthByteArray.Length + filenameByteArray.Length, fileContentLength);
            //    return Serialize(SerialDataType.File, output);
            //}

            /// Serialize small file with size not more than 50M bytes.
            public static byte[] SerializeSmallFile(string filepath)
            {
                //return SerializeSmallFile(System.IO.Path.GetFileName(filepath), System.IO.File.ReadAllBytes(filepath));
                if (string.IsNullOrWhiteSpace(filepath)) return null;
                return SerializeFilePiece(System.IO.Path.GetFileName(filepath), 0, 0, System.IO.File.ReadAllBytes(filepath));
            }

            /// Deserialize a byte array and output text or file.
            /// Output = DeserializedData
            /// DeserializedData.DataType indicates the type of the data whether it is text, file or heartbeat.
            /// DeserializedData.ErrorMessage stores the error message during running this function.
            /// DeserializedData.Text stores the text if the data type is text. Otherwise it is empty.
            /// DeserializedData.Filename stores the filename if the data type is file. Otherwise it is empty.
            /// DeserializedData.LastIndexPiece stores the last index of pieces of file.
            /// DeserializedData.IndexPiece stores the index of current pieces.
            /// DeserializedData.FileContent stores the file content if the data type is file. Otherwise it is empty.
            public static DeserializedData Deserialize(byte[] data)
            {
                if ((data?.Length ?? 0) < 1) return null;
                switch (data[0])
                {
                    case (byte)SerialDataType.Text:
                        return new DeserializedData()
                        {
                            DataType = SerialDataType.Text,
                            Text = Encoding.UTF8.GetString(data, 1, data.Length - 1)
                        };
                        //break;
                    case (byte)SerialDataType.File:
                        int filenameLength = BitConverter.ToInt32(data, 1);
                        if (filenameLength + 13 > data.Length)
                        {
                            return new DeserializedData()
                            {
                                DataType = SerialDataType.File,
                                ErrorMessage = string.Format("Length of unique ID + 13 is larger than piece length. Length of unique ID = {0}, piece length = {1}", filenameLength, data.Length)
                            };
                        }
                        byte[] filenameByteArray = null;
                        if (filenameLength > 0)
                        {
                            filenameByteArray = new byte[filenameLength];
                            System.Buffer.BlockCopy(data, 5, filenameByteArray, 0, filenameLength);
                        }
                        int srcOffset = 13 + filenameLength;
                        int contentLength = data.Length - srcOffset;
                        byte[] contentByteArray = null;
                        if (contentLength > 0)
                        {
                            contentByteArray = new byte[contentLength];
                            System.Buffer.BlockCopy(data, srcOffset, contentByteArray, 0, contentLength);
                        }
                        return new DeserializedData()
                        {
                            DataType = SerialDataType.File,
                            Filename = filenameByteArray == null ? null : Encoding.UTF8.GetString(filenameByteArray),
                            LastIndexPiece = BitConverter.ToInt32(data, 5 + filenameLength),
                            IndexPiece = BitConverter.ToInt32(data, 9 + filenameLength),
                            FileContent = contentByteArray
                        };
                        //break;
                    default:
                        return null;
                        //break;
                }
            }

            /// Append the deserialized data to file.
            /// Return value = error string. Null if success.
            /// deserializedData = object of deserialized data.
            /// deserializedData.Filename = file path. If it is not full path, the file will be saved in the current directory by default.
            public static string AppendDeserializedDataToFile(DeserializedData deserializedData)
            {
                if (deserializedData == null) return "Empty object of deserialized data";
                if (string.IsNullOrWhiteSpace(deserializedData.DestFilepath)) return "Empty destination filen path";
                string tempPath = deserializedData.DestFilepath + ".tmp";
                int fileContentLength = deserializedData.FileContent?.Length ?? 0;
                if (System.IO.File.Exists(tempPath))
                { if (deserializedData.IndexPiece == 0) System.IO.File.Delete(tempPath); }
                else
                {
                    string destFolder = deserializedData.DestFolder;
                    if (string.IsNullOrEmpty(destFolder)) destFolder = System.IO.Directory.GetCurrentDirectory();
                    if (!System.IO.Directory.Exists(destFolder)) System.IO.Directory.CreateDirectory(destFolder);
                    if (!System.IO.Directory.Exists(destFolder)) return string.Format("Fail to create folder {0}", destFolder);
                }
                if (fileContentLength > 0)
                {
                    using (System.IO.FileStream fs = new System.IO.FileStream(tempPath, System.IO.FileMode.Append))
                    { fs.Write(deserializedData.FileContent, 0, fileContentLength); }
                }
                else
                {
                    /// Create an empty file.
                    if (!System.IO.File.Exists(tempPath)) using (System.IO.File.Create(tempPath)) { }
                }
                if (deserializedData.IndexPiece == deserializedData.LastIndexPiece)
                {
                    if (System.IO.File.Exists(deserializedData.DestFilepath)) System.IO.File.Delete(deserializedData.DestFilepath);
                    if (System.IO.File.Exists(tempPath)) System.IO.File.Move(tempPath, deserializedData.DestFilepath);
                    else return string.Format("Cannot find temp file {0}", tempPath);
                }
                return null;
            }

            ///// Deserialize a byte array and output text or file.
            ///// Output = DeserializedData
            ///// DeserializedData.DataType indicates the type of the data whether it is text, file or heartbeat.
            ///// DeserializedData.Text stores the text if the data type is text. Otherwise it is empty.
            ///// DeserializedData.Filename stores the filename if the data type is file. Otherwise it is empty.
            ///// DeserializedData.FileContent stores the file content if the data type is file. Otherwise it is empty.
            ///// DeserializedData.ErrorMessage stores the error message during running this function.
            //public static DeserializedData Deserialize1(byte[] data)
            //{
            //    if ((data?.Length ?? 0) < 1) return null;
            //    switch (data[0])
            //    {
            //        case (byte)SerialDataType.Text:
            //            return new DeserializedData()
            //            {
            //                DataType = SerialDataType.Text,
            //                Text = Encoding.UTF8.GetString(data, 1, data.Length - 1)
            //            };
            //        //break;
            //        case (byte)SerialDataType.File:
            //            int totalLength = data.Length - 1;
            //            if (totalLength < 4)
            //            {
            //                return new DeserializedData()
            //                {
            //                    DataType = SerialDataType.File,
            //                    ErrorMessage = string.Format("Length of byte array is less than 4. Length = {0}", totalLength)
            //                };
            //            }
            //            byte[] filenameLengthByteArray = new byte[4];
            //            Array.Copy(data, 1, filenameLengthByteArray, 0, 4);
            //            int filenameLength = BitConverter.ToInt32(filenameLengthByteArray, 0);
            //            if (filenameLength > totalLength)
            //            {
            //                return new DeserializedData()
            //                {
            //                    DataType = SerialDataType.File,
            //                    ErrorMessage = string.Format("Filename length is larger than that of byte array length. Filename length = {0}, byte array length = {1}", filenameLength, totalLength)
            //                };
            //            }
            //            byte[] filenameByteArray = null;
            //            if (filenameLength > 0)
            //            {
            //                filenameByteArray = new byte[filenameLength];
            //                Array.Copy(data, 5, filenameByteArray, 0, filenameLength);
            //            }
            //            byte[] fileContentByteArray = null;
            //            int fileContentLength = totalLength - 4 - filenameLength;
            //            if (fileContentLength > 0)
            //            {
            //                fileContentByteArray = new byte[fileContentLength];
            //                Array.Copy(data, 5 + filenameLength, fileContentByteArray, 0, fileContentLength);
            //            }
            //            return new DeserializedData()
            //            {
            //                DataType = SerialDataType.File,
            //                Filename = filenameByteArray == null ? null : Encoding.UTF8.GetString(filenameByteArray),
            //                FileContent = fileContentByteArray
            //            };
            //        //break;
            //        default:
            //            return null;
            //            //break;
            //    }
            //}
        }
    }
}
