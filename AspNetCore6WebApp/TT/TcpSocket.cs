using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TT
{
    public class TcpSocket
    {
        /// TCP Client and Server by a synchronous socket in the threading model.
        /// Able to send file with 100M bytes, and even 200M bytes, but not very good to send 200 M bytes. Cannot send 500M bytes file.
        /// Data unit is byte.
        /// Updated date: 2022-06-16
        /// 
        /// AddDataToIncomingDataQueue
        /// PackData
        /// TryDequeueAtIncomingBufferQueue
        /// AnalyzeIncomingBuffer
        /// ProcessAnalyzeIncomingBuffer
        /// 
        /// Example:
        /// void MyFunction(TcpSocket.DataPackage o)
        /// {
        ///     // Do somethings.
        ///     Console.Write(o.Host + ":" + o.Port.ToString());
        /// }
        /// string remoteHost = "127.0.0.1";
        /// int remotePort = 8001;
        /// Queue<TcpSocket.DataPackage> incomingDataQueue = new();
        /// readonly object incomingDataQueueLocker = new();
        /// TcpSocket.Client clientSocket = new(remoteHost, remotePort, incomingDataQueue, incomingDataQueueLocker, MyFunction);

        public const int DefaultDataSize = 104857600;
        public const int DefaultBufferSize = 10485760;

        public class DataPackage
        {
            /// Timestamp that this data package is generated
            public DateTime Timestamp { get; set; } = DateTime.MinValue;
            /// Remote host
            public string Host { get; set; } = string.Empty;
            /// Remote port
            public int Port { get; set; } = -1;
            /// Data is stored as a byte array
            public byte[] ByteArray = Array.Empty<byte>();
        }

        public class Client
        {
            /// TCP Client by a synchronous socket in the threading model.
            /// Data unit is byte.
            /// https://docs.microsoft.com/en-us/dotnet/framework/network-programming/asynchronous-client-socket-example
            /// https://docs.microsoft.com/en-us/dotnet/framework/network-programming/synchronous-client-socket-example
            /// http://csharp.net-informations.com/communications/csharp-socket-programming.htm

            /// Parameters for private user
            private bool IsExit = false;/// flag to indicate if exit
            private bool IsIncomingDataFinished = true;/// Whether the incoming data is finished or continuous. True = finished, false = continuous
            private bool IsIncomingDataLength = true;/// Whether the incoming data is length or content. True = length, false = content
            private int IncomingContentSize = 0;/// Size of incoming content
            private int IncomingContentIndex = 0;/// index of incoming content
            private int IncomingLengthIndex = 0;/// index of incoming length
            private byte[]? IncomingContentBuffer = null;/// buffer for incoming content
            private byte[]? IncomingLengthBuffer = null;/// buffer for incoming length
            private List<byte[]>? IncomingBufferList = null;/// buffer list of TCP incoming data. Only used for mbEnableAnalyzeIncomingData = false
            private Thread? ThreadToAnalyzeIncomingBuffer = null;
            private Thread? ThreadToOtherProcesses = null;
            private Thread? ThreadToReceiveData = null;
            private System.Net.Sockets.Socket? ClientSocket = null;
            private readonly Queue<byte[]> IncomingBufferQueue = new();/// buffer queue of TCP incoming data.
            private readonly object IncomingBufferQueueLocker = new();

            //private readonly ManualResetEvent ConnectDone = new(false);
            //private readonly ManualResetEvent ReceiveDone = new(false);
            //private readonly ManualResetEvent SendDone = new(false);

            //private int ByteSend;
            //private int ByteReceive;

            public static Logging? Logger { get; set; }
            public readonly string RemoteHost = string.Empty;
            public readonly int RemotePort;
            public readonly DateTime InitialDateTime;
            public Queue<DataPackage> IncomingDataQueue;/// queue of TCP incoming data
            public readonly object IncomingDataQueueLocker;

            /// External action to handle incoming data. This class loops each data package in the incoming data queue and pass the data package to the external action
            /// Example:
            /// void MyFunction(TcpSocket.DataPackage o)
            /// {
            ///     // Do somethings.
            ///     Console.Write(o.Host + ":" + o.Port.ToString());
            /// }
            /// clientSocket.ExternalActToHandleIncomingData = MyFunction(o);
            public delegate void ExternalActionToHandleIncomingData(DataPackage o);
            public readonly ExternalActionToHandleIncomingData? ExternalActToHandleIncomingData;

            public DateTime LastReceivedDateTime { get; private set; }/// Date time of last received data. Can read and write it inside class, but read-only outside the class. https://stackoverflow.com/questions/4662180/c-sharp-public-variable-as-writeable-inside-the-class-but-readonly-outside-the-c
            public DateTime LastTransferDateTime { get; private set; }/// Date time of last transfer data. Can read and write it inside class, but read-only outside the class. https://stackoverflow.com/questions/4662180/c-sharp-public-variable-as-writeable-inside-the-class-but-readonly-outside-the-c
            private string _LocalEndPoint = string.Empty;
            public string LocalEndPoint
            {
                get
                {
                    // ClientSocket?.LocalEndPoint?.ToString() will null after disconnect
                    if (string.IsNullOrEmpty(_LocalEndPoint)) _LocalEndPoint = ClientSocket?.LocalEndPoint?.ToString() ?? string.Empty;
                    return _LocalEndPoint;
                }
            }
            public string RemoteEndPoint { get { return $"{RemoteHost}:{RemotePort}"; } }

            public bool IsContainLengthAsHeader { get; set; } = true;/// flag that if the data contains length (4-byte integer) as a header of package
            public bool IsEnableAnalyzeIncomingData { get; set; } = true;/// flag to enable to analyze the incoming data from remote endpoint. A package is preserved when adding to the queue of incoming data. Note that large package excceeding the buffer size is divided as several parts when receiving. The users may need to handle how to obtain the complete package. The default value is true
            private byte[] _HeartbeatData = System.Text.Encoding.UTF8.GetBytes("~");
            public byte[] HeartbeatData/// Data in byte array of heartbeat from server. This parameter will be used only if ContainLengthAsHeader = false. Space and Tab will be neglect. Sending heartbeat is used for the remote endpoint to check if the sockets are still connected. The default value is the byte array of ~.
            {
                get { return _HeartbeatData; }
                set
                {
                    if (value == null) _HeartbeatData = Array.Empty<byte>();
                    else
                    {
                        _HeartbeatData = new byte[value.Length];
                        Array.Copy(value, 0, _HeartbeatData, 0, value.Length);
                    }
                }
            }
            public int HeartbeatInterval { get; set; } = -1;/// Time interval in seconds that the local endpoint sends heartbeat to the remote endpoint. If it is negative, there is no heartbeat. Sending heartbeat is used to check if the connection still occurs. The default value is -1.
            public int MaxConnectionDuration { get; set; } = 600;/// Maximum connection duration in seconds between the local endpoint and remote endpoint. If the time exceeds, it will disconnect automatically even the connection is normal. If it is negative, there is no maximum connection duration, hence the connection can preserve forever. The default value is 600.
            public int MaxIdleDuration { get; set; } = -1;/// Maximum idle duration in seconds between the local endpoint and remote endpoint. If the time exceeds, it will disconnect automatically. If it is negative, there is no maximum idle duration, hence it will not check the idle duration. The default value is -1.
            private int _MaxDataSize = DefaultDataSize;
            public int MaxDataSize { get { return _MaxDataSize; } set { _MaxDataSize = value < 0 ? 0 : value; } }/// Maximum size of data in bytes. The default value is 104857600.
            public int ProcessVerificationInterval { get; set; } = 600;
            public int ReceiveDataInterval { get; set; } = 1;/// Time interval in seconds to receive data. If it is 0, the process will do immediately without waiting. If it is negative, no data is received. The default value is 0.
            private int _ReceiveTotalBufferSize = DefaultBufferSize;
            public int ReceiveTotalBufferSize { get { return _ReceiveTotalBufferSize; } set { _ReceiveTotalBufferSize = value < DefaultBufferSize ? DefaultBufferSize : value; } }/// Total buffer size in bytes for receiving data, the minimum value is 10485760.
            public int SleepingIntervalInMS { get; set; } = 100;/// Sleeping interval in milliseconds. This sleeping interval helps to avoid the application too busy. If it is negative, no sleep. The default value is 100.
            public bool IsConnected { get { return ClientSocket?.Connected ?? false; } }

            /// Other parameters.
            //public string Username { get; set; }

            public bool Connect()
            {
                try
                {
                    /// Disconnect first
                    /// Check if IncomingDataQueue is initialized
                    /// Connect
                    /// Start the thread to receive data
                    Disconnect();

                    if (IncomingDataQueue == null)
                    {
                        Logger?.Error("Socket finds that IncomingDataQueue is not initialized. Stop connection. Remote endpoint = {0}", RemoteEndPoint);
                        return false;
                    }

                    //System.Net.IPHostEntry ipHostInfo = System.Net.Dns.GetHostEntry(RemoteHost);
                    //System.Net.IPAddress ipAddress = ipHostInfo.AddressList[0];

                    System.Net.IPAddress[] IPs = System.Net.Dns.GetHostAddresses(RemoteHost);
                    if (IPs == null || IPs.Length < 1)
                    {
                        Logger?.Error("Socket cannot find IP address for {0}", RemoteHost);
                        return false;
                    }
                    /// Find the first IPv4 address, as IPv6 addresses are not supported
                    int i = 0; bool bLoop = true;
                    System.Net.IPAddress? ipAddress = null;
                    while (bLoop && i < IPs.Length)
                    {
                        ipAddress = IPs[i];
                        if (ipAddress != null && string.IsNullOrEmpty(ipAddress.ToString()) == false && ipAddress.ToString().Contains(':') == false)
                            bLoop = false;
                        else i++;
                    }
                    if (bLoop || ipAddress == null)
                    {
                        Logger?.Error("Socket cannot find IPv4 address for {0}", RemoteHost);
                        return false;
                    }
                    Logger?.Debug("Socket finds IP address of server is {0}", ipAddress);

                    ClientSocket = new System.Net.Sockets.Socket(ipAddress.AddressFamily, System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
                    
                    //System.Net.IPEndPoint remoteEndPoint = new(ipAddress, RemotePort);
                    //ClientSocket.BeginConnect(remoteEndPoint, new AsyncCallback(ConnectCallback), ClientSocket);
                    //ConnectDone.WaitOne();

                    ClientSocket.Connect(ipAddress, RemotePort);

                    if (IsConnected == false || IsExit)
                    {
                        Logger?.Debug("Socket cannot establish connection to the server {0}", RemoteEndPoint);
                        Disconnect();
                        return false;
                    }
                    Logger?.Debug("Socket connects successfully. Remote endpoint = {0}, local endpoint = {1}", RemoteEndPoint, LocalEndPoint);
                    StartReceivingData();
                    return true;
                }
                catch (Exception ex)
                {
                    Logger?.Error("Remote endpoint = {0}", RemoteEndPoint);
                    Logger?.Error(ex);
                    Disconnect();
                    return false;
                }
            }

            //private void ConnectCallback(IAsyncResult ar)
            //{
            //    try
            //    {
            //        System.Net.Sockets.Socket? socket = (System.Net.Sockets.Socket?)ar.AsyncState;
            //        if (socket == null)
            //        {
            //            Logger?.Error("Socket is empty. Cannot establish connection to remote endpoint {0}", RemoteEndPoint);
            //        }
            //        else
            //        {
            //            socket.EndConnect(ar);
            //            Logger?.Debug("Socket connects successfully. Remote endpoint = {0}, local endpoint = {1}", RemoteEndPoint, LocalEndPoint);
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        Logger?.Error("Remote endpoint = {0}", RemoteEndPoint);
            //        Logger?.Error(ex);
            //    }
            //    finally
            //    {
            //        ConnectDone.Set();
            //    }
            //}

            private void Disconnect()
            {
                _LocalEndPoint = string.Empty;
                if (ClientSocket == null) return;
                try
                {
                    if (ClientSocket.Connected)
                    {
                        ClientSocket.Shutdown(System.Net.Sockets.SocketShutdown.Both);
                        ClientSocket.Close();
                    }
                }
                catch (Exception ex) { Logger?.Error(ex); }
                finally { ClientSocket = null; }
            }

            private void DisconnectWithDebugLog()
            {
                Logger?.Debug("Socket disconnects. Remote endpoint = {0}, local endpoint = {1}", RemoteEndPoint, LocalEndPoint);
                Disconnect();
            }

            /// Pack the length of data into the beginning of data, in order to let the receiver easy to recognize each individual unit of data
            private static byte[] PackData(int maxDataSize, byte[] data)
            {
                try
                {
                    int length = data?.Length ?? 0;
                    if (length > maxDataSize)
                    {
                        throw new Exception(string.Format("Exceed the maximum data size {0}. Data size = {1}", maxDataSize, length));
                    }
                    byte[] rByte = new byte[4 + length];
                    BitConverter.GetBytes(length).CopyTo(rByte, 0);
                    if (data != null) data.CopyTo(rByte, 4);
                    return rByte;
                }
                catch (Exception ex)
                {
                    Logger?.Error(ex);
                    return Array.Empty<byte>();
                }
            }

            public bool SendByteArray(byte[] data)
            {
                try
                {
                    if (ClientSocket == null || IsConnected == false || IsExit) return false;
                    byte[] byteData = IsContainLengthAsHeader ? PackData(MaxDataSize, data) : data;
                    if (byteData == null || byteData.Length < 1) return false;
                    int totalSend = 0;

                    //while (totalSend < byteData.Length)
                    //{
                    //    ByteSend = 0;
                    //    ClientSocket.BeginSend(byteData, totalSend, byteData.Length - totalSend, System.Net.Sockets.SocketFlags.None, new AsyncCallback(SendByteArrayCallback), ClientSocket);
                    //    SendDone.WaitOne();
                    //    totalSend += ByteSend;
                    //    //Logger?.Debug("Socket sends data. Remote endpoint = {0}, local endpoint = {1}, #Size = {2}, #Sent = {3}", RemoteEndPoint, LocalEndPoint, byteData.Length, totalSend);
                    //}

                    while (totalSend < byteData.Length)
                    {
                        totalSend += ClientSocket.Send(byteData, totalSend, byteData.Length - totalSend, System.Net.Sockets.SocketFlags.None);
                    }

                    Logger?.Debug("Socket sends data. Remote endpoint = {0}, local endpoint = {1}, #Size = {2}, #Sent = {3}", RemoteEndPoint, LocalEndPoint, byteData.Length, totalSend);
                    if (totalSend == byteData.Length)
                    {
                        LastTransferDateTime = DateTime.Now;
                        return true;
                    }
                    else
                    {
                        Logger?.Error("Socket sends incomplete data. Remote endpoint = {0}, local endpoint = {1}, #Size = {2}, #Sent = {3}", RemoteEndPoint, LocalEndPoint, byteData.Length, totalSend);
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    Logger?.Error("Remote endpoint = {0}, local endpoint = {1}", RemoteEndPoint, LocalEndPoint);
                    Logger?.Error(ex);
                    DisconnectWithDebugLog();
                    return false;
                }
            }

            //private void SendByteArrayCallback(IAsyncResult ar)
            //{
            //    try
            //    {
            //        System.Net.Sockets.Socket? socket = (System.Net.Sockets.Socket?)ar.AsyncState;
            //        if (socket == null)
            //        {
            //            Logger?.Error("Socket is empty. Cannot send to Remote endpoint {0}", RemoteEndPoint);
            //        }
            //        else
            //        {
            //            ByteSend = socket.EndSend(ar);
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        Logger?.Error("Socket.SendByteArrayCallback. Remote endpoint = {0}, local endpoint = {1}", RemoteEndPoint, LocalEndPoint);
            //        Logger?.Error(ex);
            //    }
            //    finally
            //    {
            //        SendDone.Set();
            //    }
            //}

            /// Add data to the queue of incoming data
            /// Return value = True if add data to the queue successfully. Otherwise, fail
            //private bool AddDataToIncomingDataQueue(DateTime tTimestamp, bool bIsFinishedReceiving, ref byte[] oByteData)
            //private bool AddDataToIncomingDataQueue(DateTime tTimestamp, ref byte[] oByteData)
            private bool AddDataToIncomingDataQueue(DateTime t, byte[] data)
            {
                try
                {
                    if (IncomingDataQueue == null)
                    {
                        throw new Exception(string.Format("local endpoint cannot put data to IncomingDataQueue as it is not initialized. Remote endpoint = {0}, local endpoint = {1}", RemoteEndPoint, LocalEndPoint));
                    }
                    DataPackage oData = new()
                    {
                        Timestamp = t,
                        Host = RemoteHost,
                        Port = RemotePort,
                        ByteArray = data
                    };
                    if (IncomingDataQueueLocker == null) IncomingDataQueue.Enqueue(oData);
                    else { lock (IncomingDataQueueLocker) { IncomingDataQueue.Enqueue(oData); } }
                    Logger?.Debug("Socket adds the received data to IncomingDataQueue. Remote endpoint = {0}, Byte Length = {1}", RemoteEndPoint, data?.Length ?? 0);
                    return true;
                }
                catch (Exception ex)
                {
                    Logger?.Error("Remote endpoint = {0}, local endpoint = {1}", RemoteEndPoint, LocalEndPoint);
                    Logger?.Error(ex);
                    return false;
                }
            }

            /// Receive byte array from TCP Server
            private void ReceiveByteArray()
            {
                int iReceived;/// how many bytes are total received
                byte[] byteBuffer;
                byte[] byteData;
                try
                {
                    if (ClientSocket == null || ClientSocket.Connected == false || IsExit) return;
                    byteBuffer = new byte[ReceiveTotalBufferSize];/// 10M bytes
                    iReceived = ClientSocket.Receive(byteBuffer, 0, byteBuffer.Length, System.Net.Sockets.SocketFlags.None);
                    try
                    {
                        if (iReceived > 0)
                        {
                            byteData = new byte[iReceived];
                            Array.Copy(byteBuffer, 0, byteData, 0, iReceived);
                            byteBuffer = Array.Empty<byte>();
                            LastReceivedDateTime = LastTransferDateTime = DateTime.Now;
                            Logger?.Debug("Socket receives data. Remote endpoint = {0}, local endpoint = {1}, #Received = {2}", RemoteEndPoint, LocalEndPoint, iReceived);
                            if (IsEnableAnalyzeIncomingData)
                            {
                                /// Add data to the incoming buffer
                                lock (IncomingBufferQueueLocker)
                                {
                                    IncomingBufferQueue.Enqueue(byteData);
                                }
                            }
                            else
                            {
                                /// Add data to the queue directly
                                AddDataToIncomingDataQueue(DateTime.Now, byteData);
                            }
                        }
                        else
                        {
                            /// If receive empty bytes, do following
                            // /// send a null string to the server, in order to verify the connection is still valid
                            // SendByteArray(null);
                            /// If receive empty bytes if ContainLengthAsHeader = true, quit this function
                            /// Only do the below if ContainLengthAsHeader = false
                            if (!IsContainLengthAsHeader)
                            {
                                Logger?.Debug("Socket receives 0 data. Remote endpoint = {0}, local endpoint = {1}", RemoteEndPoint, LocalEndPoint);
                                if (IsEnableAnalyzeIncomingData)
                                {
                                    /// Add a null data to buffer, to indicate it is the end of data
                                    lock (IncomingBufferQueueLocker)
                                    {
                                        IncomingBufferQueue.Enqueue(Array.Empty<byte>());
                                    }
                                }
                                else
                                {
                                    /// Add data to the list directly.
                                    AddDataToIncomingDataQueue(DateTime.Now, Array.Empty<byte>());
                                }
                            }
                        }
                    }
                    catch (Exception ex2)
                    {
                        Logger?.Error("Remote endpoint = {0}, local endpoint = {1}", RemoteEndPoint, LocalEndPoint);
                        Logger?.Error(ex2);
                    }
                }
                catch
                {
                    Disconnect();
                }
                //finally { byteBuffer = Array.Empty<byte>(); }
            }

            //private void ReceiveByteArray()
            //{
            //    string localEndPoint = string.Empty;
            //    string remoteEndPoint = string.Empty;
            //    try
            //    {
            //        if (ClientSocket == null || IsConnected == false || IsExit) return;
            //        localEndPoint = LocalEndPoint;
            //        remoteEndPoint = RemoteEndPoint;
            //        ByteReceive = 0;
            //        byte[] byteBuffer = new byte[ReceiveTotalBufferSize];
            //        ClientSocket.BeginReceive(byteBuffer, 0, byteBuffer.Length, System.Net.Sockets.SocketFlags.None, new AsyncCallback(ReceiveByteArrayCallBack), ClientSocket);
            //        ReceiveDone.WaitOne();
            //        if (ClientSocket == null || IsConnected == false || IsExit) return;
            //        try
            //        {
            //            if (ByteReceive > 0)
            //            {
            //                Logger?.Debug("ReceiveByteArray. >0 xxxxxxx. Remote endpoint = {0}, local endpoint = {1}", remoteEndPoint, localEndPoint);

            //                byte[] byteData = new byte[ByteReceive];
            //                Array.Copy(byteBuffer, 0, byteData, 0, ByteReceive);
            //                byteBuffer = Array.Empty<byte>();
            //                LastReceivedDateTime = LastTransferDateTime = DateTime.Now;
            //                Logger?.Debug("Socket receives data. Remote endpoint = {0}, local endpoint = {1}, #Received = {2}", remoteEndPoint, localEndPoint, ByteReceive);
            //                if (IsEnableAnalyzeIncomingData)
            //                {
            //                    /// Add data to the incoming buffer
            //                    lock (IncomingBufferQueueLocker) { IncomingBufferQueue.Enqueue(byteData); }
            //                }
            //                else
            //                {
            //                    /// Add data to the queue directly
            //                    AddDataToIncomingDataQueue(DateTime.Now, byteData);
            //                }
            //            }
            //            else
            //            {
            //                Logger?.Debug("ReceiveByteArray. = 0 xxxxxxx. Remote endpoint = {0}, local endpoint = {1}", remoteEndPoint, localEndPoint);

            //                /// If receive empty bytes, do following
            //                // /// send a null string to the server, in order to verify the connection is still valid.
            //                // SendByteArray(null);
            //                /// If receive empty bytes if ContainLengthAsHeader = true, quit this function
            //                /// Only do the below if ContainLengthAsHeader = false.
            //                if (!IsContainLengthAsHeader)
            //                {
            //                    Logger?.Debug("Socket receives 0 data. Remote endpoint = {0}, local endpoint = {1}", remoteEndPoint, localEndPoint);
            //                    if (IsEnableAnalyzeIncomingData)
            //                    {
            //                        /// Add a null data to buffer, to indicate it is the end of data.
            //                        lock (IncomingBufferQueueLocker) { IncomingBufferQueue.Enqueue(Array.Empty<byte>()); }
            //                    }
            //                    else
            //                    {
            //                        /// Add data to the list directly.
            //                        AddDataToIncomingDataQueue(DateTime.Now, Array.Empty<byte>());
            //                    }
            //                }
            //            }
            //        }
            //        catch (Exception ex2)
            //        {
            //            Logger?.Error("Socket.ReceiveByteArray. Remote endpoint = {0}, local endpoint = {1}", remoteEndPoint, localEndPoint);
            //            Logger?.Error(ex2);
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        Logger?.Error("Socket.ReceiveByteArray0. Remote endpoint = {0}, local endpoint = {1}", RemoteEndPoint, localEndPoint);
            //        Logger?.Error(ex);
            //    }
            //}

            //private void ReceiveByteArrayCallBack(IAsyncResult ar)
            //{
            //    try
            //    {
            //        System.Net.Sockets.Socket? socket = (System.Net.Sockets.Socket?)ar.AsyncState;
            //        if (socket == null)
            //        {
            //            Logger?.Error("Socket is empty. Cannot receive from Remote endpoint {0}", RemoteEndPoint);
            //        }
            //        else
            //        {
            //            ByteReceive = socket.EndReceive(ar);
            //        }
            //    }
            //    catch
            //    {
            //        /// do nothing
            //    }
            //    //catch (Exception ex)
            //    //{
            //    //    Logger?.Error("Socket.ReceiveByteArrayCallBack. Remote endpoint = {0}, local endpoint = {1}", RemoteEndPoint, LocalEndPoint);
            //    //    Logger?.Error(ex);
            //    //}
            //    finally
            //    {
            //        ReceiveDone.Set();
            //    }
            //}

            private bool TryDequeueAtIncomingBufferQueue(out byte[] byteOutput)
            {
                bool bReturn = false;
                try
                {
                    lock (IncomingBufferQueueLocker)
                    {
                        if (IncomingBufferQueue.Count < 1)
                        {
                            byteOutput = Array.Empty<byte>();
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
                    Logger?.Error("Remote endpoint = {0}, local endpoint = {1}", RemoteEndPoint, LocalEndPoint);
                    Logger?.Error(ex);
                    byteOutput = Array.Empty<byte>();
                    return false;
                }
            }

            /// Analyze the incoming buffer
            /// 3 statuses
            /// State, IsIncomingDataFinished, IsIncomingDataLength
            /// 1st state, True, True. The previous data is finished. And if there is an incoming data, should analyze the length first
            /// 2nd state, False, True. set this state if the current incoming data arrives. And should analyze the length
            /// 3rd state, False, False. After analyzing the length, set to analyze the content
            private void AnalyzeIncomingBuffer()
            {
                byte[] byteDataInQueue;
                int iIndex1;/// Number of bytes that already analyzied in the data of the queue
                int iCopy, iInputRemaining, iOutputRemaining;
                try
                {
                    //if (mQueueOfIncomingBuffer == null || mQueueOfIncomingBuffer.Count < 1) { return; }
                    if (IsContainLengthAsHeader)
                    {
                        /// The case that data contains the length as header
                        /// TT edited on 2018-08-21 to lock the common resource
                        while (TryDequeueAtIncomingBufferQueue(out byteDataInQueue))
                        {
                            if (byteDataInQueue != null && byteDataInQueue.Length > 0)
                            {
                                Logger?.Debug("AnalyzeIncomingBuffer. byteDataInQueue.Length = {0}, Remote endpoint = {1}, local endpoint = {2}", byteDataInQueue.Length, RemoteEndPoint, LocalEndPoint);
                                /// Loop through the buffer item of the queue
                                iIndex1 = 0;
                                while (iIndex1 < byteDataInQueue.Length)
                                {
                                    /// If they are bytes of length, do following
                                    if (IsIncomingDataLength)
                                    {
                                        /// If the previous data is finished, do following
                                        if (IsIncomingDataFinished)
                                        {
                                            IsIncomingDataFinished = false;/// Go to 2nd state
                                            IncomingLengthIndex = 0;
                                            IncomingContentIndex = 0;
                                            IncomingContentSize = 0;
                                            /// Assign 4 byte to the Length buffer
                                            if (IncomingLengthBuffer != null) IncomingLengthBuffer = null;
                                            IncomingLengthBuffer = new byte[4];
                                        }
                                        /// Determine the number of bytes to be read and copied
                                        iInputRemaining = byteDataInQueue.Length - iIndex1;/// number of bytes that input remains to be read
                                        iOutputRemaining = 4 - IncomingLengthIndex;/// number of bytes that output remains to be write
                                        /// Set iCopy according to the remaining data length whether it is larger than or equal to 4
                                        iCopy = iInputRemaining >= iOutputRemaining ? iOutputRemaining : iInputRemaining;
                                        /// Copy
                                        if (iCopy > 0) Array.Copy(byteDataInQueue, iIndex1, IncomingLengthBuffer!, IncomingLengthIndex, iCopy);
                                        IncomingLengthIndex += iCopy;
                                        iIndex1 += iCopy;
                                        /// Check if finish to copy the length
                                        if (IncomingLengthIndex == 4)
                                        {
                                            IncomingContentSize = BitConverter.ToInt32(IncomingLengthBuffer!, 0);
                                            if (IncomingContentBuffer != null) IncomingContentBuffer = null;
                                            if (IncomingContentSize > 0) IncomingContentBuffer = new byte[IncomingContentSize];

                                            IsIncomingDataLength = false;/// go to 3rd state
                                            IncomingContentIndex = 0;/// reset the index of content again
                                        }
                                        else if (IncomingLengthIndex > 4)
                                        {
                                            Logger?.Error("AnalyzeIncomingBuffer. Too many bytes of length are copied. It is unexpected. Remote endpoint = {0}, local endpoint = {1}", RemoteEndPoint, LocalEndPoint);
                                            return;
                                        }
                                        /// If IncomingLengthIndex < 4, do nothing and continue to loop
                                    }
                                    else
                                    {
                                        /// If they are bytes of content, do following
                                        /// assume the memory is assigned for content
                                        iInputRemaining = byteDataInQueue.Length - iIndex1;
                                        iOutputRemaining = IncomingContentSize - IncomingContentIndex;
                                        /// Set iCopy according to the remaining data length whether it is larger than or equal to 4
                                        iCopy = iInputRemaining >= iOutputRemaining ? iOutputRemaining : iInputRemaining;
                                        /// Copy
                                        if (iCopy > 0) Array.Copy(byteDataInQueue, iIndex1, IncomingContentBuffer!, IncomingContentIndex, iCopy);
                                        IncomingContentIndex += iCopy;
                                        iIndex1 += iCopy;
                                        /// Check if finish to copy the content
                                        if (IncomingContentIndex == IncomingContentSize)
                                        {
                                            AddDataToIncomingDataQueue(DateTime.Now, IncomingContentBuffer!);
                                            /// go back to 1st state
                                            IsIncomingDataFinished = true;
                                            IsIncomingDataLength = true;
                                        }
                                        else if (IncomingContentIndex > IncomingContentSize)
                                        {
                                            Logger?.Error("AnalyzeIncomingBuffer. Too many bytes of content are copied. It is unexpected. Remote endpoint = {0}, local endpoint = {1}", RemoteEndPoint, LocalEndPoint);
                                            return;
                                        }
                                        /// If IncomingContentIndex < IncomingContentSize, do nothing and continue to loop
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        // /// The case that data does NOT contain the length as header
                        // int iLengthContent = byteDataInQueue.Length;
                        // if (iLengthContent < 1) { return; }
                        // byte[] vbyteContent = new byte[iLengthContent];
                        // Array.Copy(byteDataInQueue, 0, vbyteContent, 0, iLengthContent);
                        // AddDataToIncomingDataQueue(DateTime.Now, true, ref vbyteContent);

                        /// TT edited on 2018-08-21 to lock the common resource
                        while (TryDequeueAtIncomingBufferQueue(out byteDataInQueue))
                        {
                            if (byteDataInQueue == null || byteDataInQueue.Length < 1)
                            {
                                /// if there is a zero-byte content, assume that it is the end of data
                                if (IncomingBufferList != null)
                                {
                                    /// https://stackoverflow.com/questions/4868113/convert-listbyte-to-one-byte-array
                                    byte[] byteFinalData = IncomingBufferList.SelectMany(x => x)?.ToArray() ?? Array.Empty<byte>();
                                    AddDataToIncomingDataQueue(DateTime.Now, byteFinalData);
                                    /// Release memory of the list
                                    IncomingBufferList.Clear();
                                    IncomingBufferList = null;
                                }
                            }
                            else
                            {
                                /// add data into temp list
                                if (IncomingBufferList == null) IncomingBufferList = new List<byte[]>();
                                IncomingBufferList.Add(byteDataInQueue);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger?.Error("Remote endpoint = {0}, local endpoint = {1}", RemoteEndPoint, LocalEndPoint);
                    Logger?.Error(ex);
                }
            }

            private static void Sleeping(int sleepingIntervalInMS)
            {
                //if (sleepingIntervalInMS >= 0) System.Threading.Thread.Sleep(sleepingIntervalInMS);
                if (sleepingIntervalInMS >= 0) new System.Threading.ManualResetEvent(false).WaitOne(sleepingIntervalInMS);
            }

            /// Thread to analyze the incoming buffer
            private void ProcessAnalyzeIncomingBuffer()
            {
                string localEndPoint = string.Empty;
                string remoteEndPoint = string.Empty;
                DateTime tNow, tRef, tRefLog;
                try
                {
                    localEndPoint = LocalEndPoint;
                    remoteEndPoint = RemoteEndPoint;
                    tRefLog = DateTime.Now;
                    tRef = tRefLog.AddHours(-1);
                    Logger?.Debug("Socket begins to analyze the incoming buffer. Remote endpoint = {0}, local endpoint = {1}", remoteEndPoint, localEndPoint);
                    while (IsExit == false && IsConnected)
                    {
                        tNow = DateTime.Now;
                        if (ReceiveDataInterval == 0 || (ReceiveDataInterval > 0 && (int)(tNow - tRef).TotalSeconds >= ReceiveDataInterval))
                        {
                            tRef = tNow;
                            if (IsEnableAnalyzeIncomingData) AnalyzeIncomingBuffer();
                        }
                        if (ProcessVerificationInterval > 0 && (int)(tNow - tRefLog).TotalSeconds >= ProcessVerificationInterval)
                        {
                            tRefLog = tNow;
                            Logger?.Debug(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.Name + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name + " is running. Remote endpoint = {0}, local endpoint = {1}", remoteEndPoint, localEndPoint);
                        }
                        Sleeping(SleepingIntervalInMS);
                    }
                    Logger?.Debug("Socket stops to analyze the incoming buffer. Remote endpoint = {0}, local endpoint = {1}", remoteEndPoint, localEndPoint);
                }
                catch (Exception ex)
                {
                    Logger?.Error("Socket.ProcessAnalyzeIncomingBuffer. Remote endpoint = {0}, local endpoint = {1}", remoteEndPoint, localEndPoint);
                    Logger?.Error(ex);
                }
            }

            /// Thread to receive data
            private void ProcessReceiveData()
            {
                string localEndPoint = string.Empty;
                string remoteEndPoint = string.Empty;
                DateTime tNow, tRef, tRefLog;
                try
                {
                    localEndPoint = LocalEndPoint;
                    remoteEndPoint = RemoteEndPoint;
                    tRefLog = DateTime.Now;
                    tRef = tRefLog.AddHours(-1);
                    Logger?.Debug("Socket begins to receive data. Remote endpoint = {0}, local endpoint = {1}", remoteEndPoint, localEndPoint);
                    while (IsExit == false && IsConnected)
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
                            Logger?.Debug(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.Name + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name + " is running. Remote endpoint = {0}, local endpoint = {1}", remoteEndPoint, localEndPoint);
                        }
                        Sleeping(SleepingIntervalInMS);
                    }
                    Logger?.Debug("Socket stops to receive data. Remote endpoint = {0}, local endpoint = {1}", remoteEndPoint, localEndPoint);
                }
                catch (Exception ex)
                {
                    Logger?.Error("Socket.ProcessReceiveData. Remote endpoint = {0}, local endpoint = {1}", remoteEndPoint, localEndPoint);
                    Logger?.Error(ex);
                }
            }

            /// Disconnect sockets which exceed maximum connection duration
            private void DisconnectSocketExceedingMaxConnectionDuration(DateTime tNow)
            {
                try
                {
                    if (MaxConnectionDuration <= 0) return;
                    if ((int)(tNow - InitialDateTime).TotalSeconds > MaxConnectionDuration)
                    {
                        Logger?.Debug("Disconnect socket as exceeding maximum connection duration {0} second(s). Remote endpoint = {0}, local endpoint = {2}", MaxConnectionDuration, RemoteEndPoint, LocalEndPoint);
                        DisconnectWithDebugLog();
                    }
                }
                catch (Exception ex)
                {
                    Logger?.Error("Remote endpoint = {0}, local endpoint = {1}", RemoteEndPoint, LocalEndPoint);
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
                        Logger?.Debug("Disconnect socket as exceeding maximum idle duration {0} second(s). Remote endpoint = {0}, local endpoint = {2}", MaxIdleDuration, RemoteEndPoint, LocalEndPoint);
                        DisconnectWithDebugLog();
                    }
                }
                catch (Exception ex)
                {
                    Logger?.Error("Remote endpoint = {0}, local endpoint = {1}", RemoteEndPoint, LocalEndPoint);
                    Logger?.Error(ex);
                }
            }

            private void HandleIncomingDataQueue()
            {
                if (ExternalActToHandleIncomingData == null) return;
                List<DataPackage> tempList = new();
                try
                {
                    lock (IncomingDataQueueLocker)
                    {
                        if (IncomingDataQueue == null || IncomingDataQueue.Count < 1) return;
                        int iMax = 5;
                        int i = 0;
                        while (IncomingDataQueue.Count > 0 && i < iMax)
                        {
                            tempList.Add(IncomingDataQueue.Dequeue());/// pass to temp list in order to unlock the list earlier
                            i += 1;
                        }
                    }
                    foreach (DataPackage o in tempList)
                    {
                        try { ExternalActToHandleIncomingData(o); }
                        catch (Exception ex2)
                        {
                            Logger?.Error("Remote endpoint = {0}, local endpoint = {1}", RemoteEndPoint, LocalEndPoint);
                            Logger?.Error(ex2);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger?.Error("Remote endpoint = {0}, local endpoint = {1}", RemoteEndPoint, LocalEndPoint);
                    Logger?.Error(ex);
                }
                finally
                {
                    tempList.Clear();
                }
            }

            private void ProcessOtherProcesses()
            {
                string localEndPoint = string.Empty;
                string remoteEndPoint = string.Empty;
                DateTime tNow, tRef, tRefIncomingData;
                byte[] oByteArrayHeartbeat = Array.Empty<byte>();
                try
                {
                    localEndPoint = LocalEndPoint;
                    remoteEndPoint = RemoteEndPoint;
                    tRef = DateTime.Now.AddHours(-1);
                    tRefIncomingData = tRef;
                    Logger?.Debug("Socket begins to run other processes. Remote endpoint = {0}, local endpoint = {1}", remoteEndPoint, localEndPoint);
                    if (!IsContainLengthAsHeader) oByteArrayHeartbeat = HeartbeatData;
                    while (IsExit == false && IsConnected)
                    {
                        tNow = DateTime.Now;
                        /// Check if it exceeds maximum connection duration. If yes, disconnect it
                        DisconnectSocketExceedingMaxConnectionDuration(tNow);
                        /// Check if it exceeds maximum idle duration. If yes, disconnect it
                        DisconnectSocketExceedingMaxIdleDuration(tNow);
                        /// Send heartbeat to clients.
                        if (HeartbeatInterval == 0 || (HeartbeatInterval > 0 && ((int)(tNow - tRef).TotalSeconds >= HeartbeatInterval)))
                        {
                            tRef = tNow;
                            SendByteArray(oByteArrayHeartbeat);
                        }
                        /// Handle the incoming data by the outter method
                        if (ReceiveDataInterval >= 0 && (int)(tNow - tRefIncomingData).TotalSeconds >= ReceiveDataInterval)
                        {
                            tRefIncomingData = tNow;
                            HandleIncomingDataQueue();
                        }
                        Sleeping(SleepingIntervalInMS);
                    }
                    Logger?.Debug("Socket stops to run other processes. Remote endpoint = {0}, local endpoint = {1}", remoteEndPoint, localEndPoint);
                }
                catch (Exception ex)
                {
                    Logger?.Error("Socket.ProcessOtherProcesses. Remote endpoint = {0}, local endpoint = {1}", remoteEndPoint, localEndPoint);
                    Logger?.Error(ex);
                }
            }

            /// Stop to connect TCP Server
            public void StopConnection() { StopConnection(2, SleepingIntervalInMS); }
            public void StopConnection(int timeoutInSecond, int sleepingIntervalInMS)
            {
                string localEndPoint = string.Empty;
                string remoteEndPoint = string.Empty;
                try
                {
                    IsExit = true;
                    localEndPoint = LocalEndPoint;
                    remoteEndPoint = RemoteEndPoint;
                    DisconnectWithDebugLog();

                    if (timeoutInSecond < 0) timeoutInSecond = 0;
                    DateTime tRef = DateTime.Now;
                    while ((int)(DateTime.Now - tRef).TotalSeconds < timeoutInSecond && (
                        (ThreadToReceiveData?.IsAlive ?? false) || (ThreadToAnalyzeIncomingBuffer?.IsAlive ?? false) || (ThreadToOtherProcesses?.IsAlive ?? false)
                        ))
                    {
                        Sleeping(sleepingIntervalInMS);
                    }
                    if (ThreadToReceiveData?.IsAlive ?? false)
                    {
                        Logger?.Debug("Force to abort the thread named {0}", ThreadToReceiveData.Name);
                        ThreadToReceiveData.Interrupt();
                    }
                    if (ThreadToAnalyzeIncomingBuffer?.IsAlive ?? false)
                    {
                        Logger?.Debug("Force to abort the thread named {0}", ThreadToAnalyzeIncomingBuffer.Name);
                        ThreadToAnalyzeIncomingBuffer.Interrupt();
                    }
                    if (ThreadToOtherProcesses?.IsAlive ?? false)
                    {
                        Logger?.Debug("Force to abort the thread named {0}", ThreadToOtherProcesses.Name);
                        ThreadToOtherProcesses.Interrupt();
                    }
                    ThreadToReceiveData = ThreadToAnalyzeIncomingBuffer = ThreadToOtherProcesses = null;

                    /// Release the memory in buffer
                    lock (IncomingBufferQueueLocker)
                    {
                        IncomingBufferQueue.Clear();
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
                    Logger?.Error("Socket.StopConnection. Remote endpoint = {0}, local endpoint = {1}", remoteEndPoint, localEndPoint);
                    Logger?.Error(ex);
                }
            }

            public void StartReceivingData()
            {
                try
                {
                    if (ThreadToReceiveData == null)
                    {
                        ThreadToReceiveData = new Thread(new ThreadStart(ProcessReceiveData)) { Name = $"ProcessReceiveData-{RemoteEndPoint}" };
                        ThreadToReceiveData.Start();/// This "start" statement can be divided from this function
                    }
                    if (ThreadToAnalyzeIncomingBuffer == null)
                    {
                        ThreadToAnalyzeIncomingBuffer = new Thread(new ThreadStart(ProcessAnalyzeIncomingBuffer)) { Name = $"ProcessAnalyzeIncomingBuffer-{RemoteEndPoint}" };
                        ThreadToAnalyzeIncomingBuffer.Start();
                    }
                    if (ThreadToOtherProcesses == null)
                    {
                        ThreadToOtherProcesses = new Thread(new ThreadStart(ProcessOtherProcesses)) { Name = $"ProcessOtherProcesses-{RemoteEndPoint}" };
                        ThreadToOtherProcesses.Start();
                    }
                }
                catch (Exception ex) { Logger?.Error(ex); }
            }

            ///// Connect to remote endpoint and send the byte array. After sending, the socket is shutdown and closed after a delay
            ///// Return value = true if success. Otherwise, fail
            ///// delayDisconnetInMS = Delay interval in milli-second to disconnect server. The delay disconnect to remote endpoint is required. Otherwise, data is no time to be sent to the remote endpoint
            ///// https://docs.microsoft.com/en-us/dotnet/framework/network-programming/synchronous-client-socket-example
            //public static bool ConnectServerAndSendData(string serverHost, int serverPort, bool isContainLengthAsHeader, byte[] data) { return ConnectServerAndSendData(serverHost, serverPort, isContainLengthAsHeader, data, 5000); }
            //public static bool ConnectServerAndSendData(string serverHost, int serverPort, bool isContainLengthAsHeader, byte[] data, int delayDisconnetInMS) { return ConnectServerAndSendData(serverHost, serverPort, isContainLengthAsHeader, data, delayDisconnetInMS, 2000000000); }
            //public static bool ConnectServerAndSendData(string serverHost, int serverPort, bool isContainLengthAsHeader, byte[] data, int delayDisconnetInMS, int maxDataSize)
            //{
            //    System.Net.Sockets.Socket oClientSocket = null;
            //    try
            //    {
            //        oClientSocket = Connect(serverHost, serverPort);
            //        if ((oClientSocket?.Connected ?? false) == false) return false;
            //        return SendByteArray(ref oClientSocket, isContainLengthAsHeader, maxDataSize, data);
            //    }
            //    catch (Exception ex)
            //    {
            //        Logger?.Error("Server socket = {0}:{1}", serverHost, serverPort);
            //        Logger?.Error(ex);
            //        return false;
            //    }
            //    finally
            //    {
            //        try
            //        {
            //            if (delayDisconnetInMS >= 0) System.Threading.Thread.Sleep(delayDisconnetInMS);
            //            Disconnect(ref oClientSocket);
            //        }
            //        catch (Exception ex2)
            //        {
            //            Logger?.Error("Server socket = {0}:{1}", serverHost, serverPort);
            //            Logger?.Error(ex2);
            //        }
            //    }
            //}

            //public Client(string remoteHost, int remotePort, Queue<DataPackage> incomingDataQueue, object incomingDataQueueLocker)
            public Client(string remoteHost, int remotePort, Queue<DataPackage> incomingDataQueue, object incomingDataQueueLocker, ExternalActionToHandleIncomingData? externalActToHandleIncomingData)
            {
                RemoteHost = remoteHost;
                RemotePort = remotePort;
                IncomingDataQueue = incomingDataQueue;
                IncomingDataQueueLocker = incomingDataQueueLocker;
                ExternalActToHandleIncomingData = externalActToHandleIncomingData;
                InitialDateTime = LastReceivedDateTime = LastTransferDateTime = DateTime.Now;
            }

            /// Initialize this class if the socket has been already connected
            //public Client(System.Net.Sockets.Socket clientSocket, Queue<DataPackage> incomingDataQueue, object incomingDataQueueLocker)
            public Client(System.Net.Sockets.Socket clientSocket, Queue<DataPackage> incomingDataQueue, object incomingDataQueueLocker, ExternalActionToHandleIncomingData? externalActToHandleIncomingData)
            {
                this.ClientSocket = clientSocket;
                System.Net.IPEndPoint? iPEndPoint = (System.Net.IPEndPoint?)ClientSocket?.RemoteEndPoint;
                RemoteHost = iPEndPoint?.Address?.ToString() ?? string.Empty;
                RemotePort = iPEndPoint?.Port ?? -1;
                IncomingDataQueue = incomingDataQueue;
                IncomingDataQueueLocker = incomingDataQueueLocker;
                ExternalActToHandleIncomingData = externalActToHandleIncomingData;
                InitialDateTime = LastReceivedDateTime = LastTransferDateTime = DateTime.Now;
                Logger?.Debug("TCP Server connects client {0}:{1}", RemoteHost, RemotePort);
            }
        }

        public class Server
        {
            /// TCP Server by a synchronous socket in the threading model
            /// Data unit is byte
            /// http://csharp.net-informations.com/communications/csharp-server-socket.htm
            /// https://docs.microsoft.com/en-us/dotnet/framework/network-programming/synchronous-server-socket-example
            /// https://docs.microsoft.com/en-us/dotnet/framework/network-programming/asynchronous-server-socket-example
            /// http://www.yoda.arachsys.com/csharp/parameters.html
            /// https://blogs.msdn.microsoft.com/oldnewthing/20060801-19/?p=30273
            /// https://www.codeproject.com/Articles/990474/Scalable-Socket-Server
            /// http://www.tutorialsteacher.com/csharp/array-csharp
            /// 

            /// Parameters for private user
            private bool IsExit = false;
            //private bool IsReadyAccept = false;/// flag to indicate whether it is ready to accept client
            private readonly ManualResetEvent AcceptDone = new(false);
            private System.Net.Sockets.Socket? ServerSocket = null;
            private System.Threading.Thread? ThreadToAcceptClient = null;
            private System.Threading.Thread? ThreadToOtherProcesses = null;
            private System.Threading.Thread? ThreadToSendData = null;

            private readonly Queue<DataPackage> OutgoingDataQueue = new();
            private readonly object OutgoingDataQueueLocker = new();
            private readonly Queue<DataPackage> FailedOutgoingDataQueue = new();/// queue of TCP outgoing data sent failed
            private readonly object FailedOutgoingDataQueueLocker = new();

            private readonly List<Client> InnerClientList = new();/// list of TCP client objects
            private readonly object InnerClientListLocker = new();

            /// Properties.
            public static Logging? Logger { get; set; }
            public readonly int ListeningPort;
            public Queue<DataPackage> IncomingDataQueue;/// queue of TCP incoming data
            public readonly object IncomingDataQueueLocker;/// lock object for queue of TCP incoming data

            /// External action to handle incoming data. This class loops each data package in the incoming data queue and pass the data package to the external action.
            /// This workflow should be done in this Server class and should not be done in the Client class, although there are same function in the Client class.
            /// Otherwise, various clients access one common incoming data queue, it makes the incoming data queue always be locked
            /// Example:
            /// void MyFunction(TcpSocket.DataPackage o)
            /// {
            ///     // Do somethings.
            ///     Console.Write(o.Host + ":" + o.Port.ToString());
            /// }
            /// MySocket.ExternalActToHandleIncomingData = MyFunction(o);
            public delegate void ExternalActionToHandleIncomingData(DataPackage o);
            public readonly ExternalActionToHandleIncomingData? ExternalActToHandleIncomingData;

            public int AcceptInterval { get; set; } = 1;/// Time interval in seconds that TCP server accepts client. If it is lesser than 1, the process will do immediately without waiting. The default value is 1.
            public bool IsContainLengthAsHeader { get; set; } = true;/// flag to indicate if the data contains length (4-byte integer) as a header of package. The header is used to divide a package unit. The default value is true.
            public bool IsEnableAnalyzeIncomingData { get; set; } = true;/// flag to enable to analyze the incoming data from server. A package is preserved when adding to the queue of incoming data. Note that large package exceeding the buffer size is divided as several parts when receiving. The users may need to handle how to obtain the complete package if disable this flag. The default value is true.
            private byte[] _HeartbeatData = Encoding.UTF8.GetBytes("~");
            public byte[] HeartbeatData/// Data in byte array of heartbeat. This parameter will be used only if mbContainLengthAsHeader = false. Space and Tab will be neglect. Sending heartbeat is used to check if the sockets are still connected. The default value is the byte array of ~.
            {
                get { return _HeartbeatData; }
                set
                {
                    if (value == null) _HeartbeatData = Array.Empty<byte>();
                    else
                    {
                        _HeartbeatData = new byte[value.Length];
                        Array.Copy(value, 0, _HeartbeatData, 0, value.Length);
                    }
                }
            }
            public int HeartbeatInterval { get; set; } = -1;/// Time interval in seconds that the local endpoint sends heartbeat to the remote endpoint. If it is negative, there is no heartbeat. Sending heartbeat is used to check if the connection still occurs. The default value is -1.
            public int MaxClient { get; set; } = 200;/// Maximum TCP clients allowed to be connected by the server. If it is 0, no client is allowed to be connected. If it is negative, no limitation to accept clients. The default value is 200.
            public int MaxConnectionDuration { get; set; } = 600;/// Maximum connection duration in seconds between local endpoint and remote endpoint. If the time exceeds, it will disconnect automatically even the connection is normal. If it is negative, there is no maximum connection duration, hence the connection can preserve forever. The default value is 600.
            public int MaxIdleDuration { get; set; } = -1;/// Maximum idle duration in seconds between local endpoint and remote endpoint. If the time exceeds, it will disconnect automatically. If it is negative, there is no maximum idle duration, hence it will not check the idle duration. The default value is -1.
            public int MaxDataSend { get; set; } = -1;/// Maximum unit of data sent from the local endpoint each time. If it is 0, no data is sent. If it is negative, the process will send all data in the queue without waiting. The default value is -1.
            private int _MaxDataSize = DefaultDataSize;
            public int MaxDataSize { get { return _MaxDataSize; } set { _MaxDataSize = value < 0 ? 0 : value; } }/// Maximum size of data in bytes. The default value is 104857600.
            public int ProcessVerificationInterval { get; set; } = 600;/// Time interval in seconds to verify the process is still running or not. If it is running, a log is written. If the value is negative or zero, it does not verify. The default value is 600.
            public int ReceiveDataInterval { get; set; } = 1;/// Time interval in seconds to receive Data. If it is 0, the process will do immediately without waiting. If it is negative, no data is received. The default value is 0.
            private int _ReceiveTotalBufferSize = DefaultBufferSize;
            public int ReceiveTotalBufferSize { get { return _ReceiveTotalBufferSize; } set { _ReceiveTotalBufferSize = value < DefaultBufferSize ? DefaultBufferSize : value; } }/// Total buffer size in bytes for receiving data, the minimum value is 10485760.
            public int SleepingIntervalInMS { get; set; } = 100;/// Sleeping interval in milliseconds. This sleeping interval helps to avoid the application too busy. If it is negative, no sleep. The default value is 100.

            /// Get the number of items in the queue
            private static int NumberOfItemsInQueue(Queue<DataPackage> queue, object? locker)
            {
                if (locker == null) return queue?.Count ?? 0;
                else { lock (locker) { return queue?.Count ?? 0; } }
            }

            /// Get the number of outgoing data in the queue
            public int NumberOfOutgoingData() { return NumberOfItemsInQueue(OutgoingDataQueue, OutgoingDataQueueLocker); }

            /// Get the number of failed outgoing data in the queue, which is sent failed
            public int NumberOfFailedOutgoingData() { return NumberOfItemsInQueue(FailedOutgoingDataQueue, FailedOutgoingDataQueueLocker); }

            /// Get the number of connected clients
            public int NumberOfConnectedClients()
            {
                lock (InnerClientListLocker) { return InnerClientList?.Count ?? 0; }
            }

            /// Get a list of Host:Port of clients
            public string[] ClientList()
            {
                try
                {
                    lock (InnerClientListLocker)
                    {
                        return InnerClientList?.Select(x => x?.RemoteEndPoint ?? string.Empty)?.ToArray() ?? Array.Empty<string>();
                    }
                }
                catch (Exception ex)
                {
                    Logger?.Error(ex);
                    return Array.Empty<string>();
                }
            }

            public string PrintAllClients() { return PrintAllClients(Environment.NewLine); }
            public string PrintAllClients(string recordSeparator)
            {
                string[] array;
                StringBuilder sb;
                try
                {
                    array = ClientList();
                    if (array == null || array.Length < 1) return "#Items = 0";
                    sb = new StringBuilder();
                    sb.Append("#Items = ").Append(array.Length).Append(recordSeparator);
                    sb.Append(string.Join(recordSeparator, array)).Append(recordSeparator);
                    return sb.ToString();
                }
                catch (Exception ex)
                {
                    Logger?.Error(ex);
                    return string.Empty;
                }
            }

            ///// Get the index of clients in the list by host and port
            ///// Return value = index of clients in the list by host and port
            ///// host = client host
            ///// port = client port
            //private int IndexOfInnerClientByHostAndPort(string host, int port)
            //{
            //    try
            //    {
            //        host = host == null ? string.Empty : host.Trim();

            //        //int i = 0;
            //        //lock (mListInnerClientLocker)
            //        //{
            //        //    while (mListInnerClientT != null && i < mListInnerClientT.Count)
            //        //    {
            //        //        InnerClient oItem = mListInnerClientT[i];
            //        //        if (oItem != null)
            //        //        {
            //        //            if (sHost.Equals(oItem.Host) && oItem.Port == iPort)
            //        //            { return i; }
            //        //        }
            //        //        i += 1;
            //        //    }
            //        //}
            //        //return -1;

            //        /// Not use the below lambda expression because worry that the mListInnerClientT is always changed, and the iteration may go exception.
            //        // if (string.IsNullOrEmpty(sHost)) { return mListInnerClientT.FindIndex(x => x != null && iPort == x.Port && string.IsNullOrEmpty(x.Host)); }
            //        // else
            //        // {
            //        // sHost = sHost.Trim();
            //        // return mListInnerClientT.FindIndex(x => x != null && iPort == x.Port && sHost.Equals(x.Host));
            //        // }

            //        lock (InnerClientListLocker)
            //        {
            //            return InnerClientList?.FindIndex(x => x != null && port == x.RemotePort && host.Equals(x.RemoteHost)) ?? -1;
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        Logger?.Error("Client socket = {0}:{1}", host, port);
            //        Logger?.Error(ex);
            //        return -1;
            //    }
            //}

            /// Get the client item in the list by host and port
            /// Return value = client item in the list by host and port
            /// host = client host
            /// port = client port
            private Client? InnerClientTByHostAndPort(string host, int port)
            {
                try
                {
                    host = host == null ? string.Empty : host.Trim();
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

            /// Check if the specific client is connected to this server
            /// Return value = true if connected. Otherwise, false
            /// host = client host
            /// port = client port
            public bool IsClientConnected(string host, int port)
            {
                return InnerClientTByHostAndPort(host, port)?.IsConnected ?? false;
            }

            /// Disconnect a specific client
            public void DisconnectClient(string host, int port)
            {
                // System.Collections.Generic.List<InnerClientT> vListOfDisconnect = null;
                try
                {
                    host = host == null ? string.Empty : host.Trim();

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
                            Client? oItem = InnerClientList?[i];
                            if (oItem != null)
                            {
                                if (oItem.RemotePort == port && host.Equals(oItem.RemoteHost))
                                    oItem.StopConnection();
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

            /// Trim the list of InnerClient
            private void TrimList()
            {
                List<Client>? vListOfDeleted = null;
                try
                {
                    lock (InnerClientListLocker)
                    {
                        if (InnerClientList != null)
                        {
                            InnerClientList.RemoveAll(x => x == null);
                            vListOfDeleted = InnerClientList.FindAll(x => x.IsConnected == false);
                            if (vListOfDeleted != null && vListOfDeleted.Count > 0)
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
                    /// Disconnect the clients out of the LOCK block
                    if (vListOfDeleted != null && vListOfDeleted.Count > 0)
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
                    }
                }
            }

            ///// This function should be run as a thread
            //private void ProcessTrimList()
            //{
            //    try
            //    {
            //        int i = this.SleepingIntervalInMS * 2;
            //        Logger?.Debug("TCP Server begins to trim list of clients.");
            //        while (!this.IsExit)
            //        {
            //            TrimList();
            //            if (i >= 0) System.Threading.Thread.Sleep(i);
            //        }
            //        Logger?.Debug("TCP Server stops to trim list of clients.");
            //    }
            //    catch (Exception ex) { Logger?.Error(ex); }
            //}

            private bool TryDequeueAtOutgoingDataQueue(out DataPackage? oOutput)
            {
                try
                {
                    lock (OutgoingDataQueueLocker)
                    {
                        if (OutgoingDataQueue.Count < 1)
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

            private bool TryDequeueAtFailedOutgoingDataQueue(out DataPackage? oOutput)
            {
                try
                {
                    lock (FailedOutgoingDataQueueLocker)
                    {
                        if (FailedOutgoingDataQueue.Count < 1)
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

            /// Queue to send data
            public void QueueToSendData(string host, int port, ref byte[] data)
            {
                try
                {
                    if (string.IsNullOrEmpty(host)) return;
                    /// add data to queue
                    lock (OutgoingDataQueueLocker)
                    {
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

            /// Queue to failed outgoing data
            private void QueueToFailedOutgoingData(ref DataPackage vData)
            {
                try
                {
                    if (vData == null) return;
                    lock (FailedOutgoingDataQueueLocker)
                    {
                        FailedOutgoingDataQueue.Enqueue(vData);
                    }
                }
                catch (Exception ex) { Logger?.Error(ex); }
            }

            private void ProcessSendData_routine1(DataPackage? data)
            {
                try
                {
                    if (data == null) return;
                    string host = string.Empty;
                    if (!string.IsNullOrEmpty(data.Host)) host = data.Host.ToUpper();
                    //switch (host)
                    //{
                    //    case "255.255.255.255":
                    //    case "ALL":
                    //        // Don't use foreach
                    //        i = 0;
                    //        while (mListInnerClientT != null && i < mListInnerClientT.Count)
                    //        {
                    //            InnerClient oItem = mListInnerClientT[i];
                    //            if (oItem != null)
                    //            { if (!oItem.SendByteArray(data.ByteArray)) { QueueToFailedOutgoingData(ref data); } }
                    //            i += 1;
                    //        }
                    //        break;
                    //    default: // find which client in the list
                    //        bool b = true;
                    //        i = 0;
                    //        sHost = vData.Host;
                    //        if (string.IsNullOrEmpty(host)) { sHost = ""; }
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
                    if ("ALL".Equals(host.ToUpper()) || "255.255.255.255".Equals(host))
                    {
                        lock (InnerClientListLocker)
                        {
                            if (InnerClientList != null)
                            {
                                foreach (Client o in InnerClientList)
                                {
                                    if (o != null)
                                    { if (!o.SendByteArray(data.ByteArray)) QueueToFailedOutgoingData(ref data); }
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
                                    if (o != null && o.RemotePort == data.Port && host.Equals(o.RemoteHost))
                                    {
                                        if (!o.SendByteArray(data.ByteArray)) QueueToFailedOutgoingData(ref data);
                                        b = false;/// set that it already finds InnerClient
                                    }
                                }
                            }
                        }
                        if (b) Logger?.Debug("{0}.{1}. Cannot find the client in the list. Client socket = {2}:{3}", System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.Name, System.Reflection.MethodBase.GetCurrentMethod()?.Name, host, data.Port);
                    }
                }
                catch (Exception ex) { Logger?.Error(ex); }
            }

            /// Send heartbeat to all clients
            private void SendHeartbeat(byte[] oByteArrayHeartbeat)
            {
                try
                {
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

            /// Disconnect sockets which exceed maximum connection duration
            private void DisconnectSocketExceedingMaxConnectionDuration(DateTime tNow)
            {
                try
                {
                    if (MaxConnectionDuration <= 0) return;
                    lock (InnerClientListLocker)
                    {
                        if (InnerClientList != null)
                        {
                            foreach (Client o in InnerClientList)
                            {
                                if (o != null)
                                {
                                    if ((int)(tNow - o.InitialDateTime).TotalSeconds > MaxConnectionDuration)
                                    {
                                        Logger?.Debug("TCP Server disconnects socket as exceeding maximum connection duration {0} second(s). Listening Port: {1}. Client: {2}", MaxConnectionDuration, ListeningPort, o.RemoteEndPoint);
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
                    if (MaxIdleDuration < 0) return;
                    lock (InnerClientListLocker)
                    {
                        if (InnerClientList != null)
                        {
                            foreach (Client o in InnerClientList)
                            {
                                if (o != null)
                                {
                                    if ((int)(tNow - o.LastTransferDateTime).TotalSeconds > MaxIdleDuration)
                                    {
                                        Logger?.Debug("TCP Server disconnects socket as exceeding maximum idle duration {0} second(s). Listening Port: {1}. Client: {2}", MaxIdleDuration, ListeningPort, o.RemoteEndPoint);
                                        o.StopConnection();
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex) { Logger?.Error(ex); }
            }

            private static void Sleeping(int sleepingIntervalInMS)
            {
                //if (sleepingIntervalInMS >= 0) Thread.Sleep(sleepingIntervalInMS);
                if (sleepingIntervalInMS >= 0) new System.Threading.ManualResetEvent(false).WaitOne(sleepingIntervalInMS);
            }

            /// Process to send data
            private void ProcessSendData()
            {
                DateTime tNow;
                byte[] oByteArrayHeartbeat = Array.Empty<byte>();
                try
                {
                    Logger?.Debug("TCP Server begins to send data to clients");
                    /// looping
                    DateTime tRefLog = DateTime.Now;
                    DateTime tRefHeartbeat = tRefLog.AddHours(-1);

                    if (!IsContainLengthAsHeader) oByteArrayHeartbeat = HeartbeatData;
                    while (IsExit == false && ServerSocket != null)
                    {
                        tNow = DateTime.Now;
                        /// Send data to clients
                        if (MaxDataSend < 0)
                        {
                            while (TryDequeueAtOutgoingDataQueue(out DataPackage? oData))
                            { ProcessSendData_routine1(oData); }
                        }
                        else
                        {
                            int i = 0;
                            while (i < MaxDataSend && TryDequeueAtOutgoingDataQueue(out DataPackage? oData))
                            {
                                ProcessSendData_routine1(oData);
                                i += 1;
                            }
                        }
                        /// Check if clients exceed maximum connection duration. If yes, disconnect it
                        DisconnectSocketExceedingMaxConnectionDuration(tNow);
                        /// Check if clients exceed maximum idle duration. If yes, disconnect it
                        DisconnectSocketExceedingMaxIdleDuration(tNow);
                        /// Send heartbeat to clients
                        if (HeartbeatInterval == 0 || (HeartbeatInterval > 0 && ((int)(tNow - tRefHeartbeat).TotalSeconds >= this.HeartbeatInterval)))
                        {
                            tRefHeartbeat = tNow;
                            SendHeartbeat(oByteArrayHeartbeat);
                            Logger?.Debug("List of Inner clients: " + PrintAllClients());
                        }
                        /// Trim the list
                        TrimList();
                        /// log
                        if (ProcessVerificationInterval > 0 && (int)(tNow - tRefLog).TotalSeconds >= ProcessVerificationInterval)
                        {
                            tRefLog = tNow;
                            Logger?.Debug("{0}.{1} is running", System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.Name, System.Reflection.MethodBase.GetCurrentMethod()?.Name);
                        }
                        Sleeping(SleepingIntervalInMS);
                    }
                    Logger?.Debug("TCP Server stops to send data to clients");
                }
                catch (Exception ex)
                {
                    Logger?.Error("ProcessSendData");
                    Logger?.Error(ex);
                }
            }

            private void HandleIncomingDataQueue()
            {
                if (ExternalActToHandleIncomingData == null) return;
                List<DataPackage> tempList = new();
                try
                {
                    lock (IncomingDataQueueLocker)
                    {
                        if (IncomingDataQueue == null || IncomingDataQueue.Count < 1) return;
                        int iMax = 10;
                        int i = 0;
                        if (IncomingDataQueue != null)
                        {
                            while (IncomingDataQueue.Count > 0 && i < iMax)
                            {
                                tempList.Add(IncomingDataQueue.Dequeue());/// pass to temp list in order to unlock the list earlier.
                                i += 1;
                            }
                        }
                    }
                    foreach (DataPackage o in tempList)
                    {
                        try { ExternalActToHandleIncomingData(o); }
                        catch (Exception ex2) { Logger?.Error(ex2); }
                    }
                }
                catch (Exception ex)
                {
                    Logger?.Error(ex);
                }
                finally
                {
                    tempList.Clear();
                }
            }

            private void ProcessOtherProcesses()
            {
                DateTime tNow, tRefIncomingData, tRefLog;
                try
                {
                    Logger?.Debug("TCP Server begins to run some processes");
                    tRefIncomingData = DateTime.Now.AddHours(-1);
                    tRefLog = tRefIncomingData;
                    while (IsExit == false && ServerSocket != null)
                    {
                        tNow = DateTime.Now;
                        /// Handle the incoming data by the outter method
                        if (ReceiveDataInterval >= 0 && (int)(tNow - tRefIncomingData).TotalSeconds >= this.ReceiveDataInterval)
                        {
                            tRefIncomingData = tNow;
                            HandleIncomingDataQueue();
                        }
                        /// log
                        if (ProcessVerificationInterval > 0 && (int)(tNow - tRefLog).TotalSeconds >= ProcessVerificationInterval)
                        {
                            tRefLog = tNow;
                            Logger?.Debug("{0}.{1} is running", System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.Name, System.Reflection.MethodBase.GetCurrentMethod()?.Name);
                        }
                        Sleeping(SleepingIntervalInMS);
                    }
                    Logger?.Debug("TCP Server stops to run some processes");
                }
                catch (Exception ex) { Logger?.Error(ex); }
            }

            /// Action after accept a client
            private void AcceptClientCallback(IAsyncResult ar)
            {
                try
                {
                    if (IsExit || ServerSocket == null) return;
                    ServerSocket = (System.Net.Sockets.Socket?)ar.AsyncState;
                    if (ServerSocket == null)
                    {
                        Logger?.Error("Socket is empty. Server cannot accept the client");
                        return;
                    }
                    System.Net.Sockets.Socket? oClientSocket = ServerSocket.EndAccept(ar);
                    if (oClientSocket == null) return;
                    /// create the client instance
                    Client oItem = new(oClientSocket, IncomingDataQueue, IncomingDataQueueLocker, null)
                    {
                        IsContainLengthAsHeader = this.IsContainLengthAsHeader,
                        IsEnableAnalyzeIncomingData = this.IsEnableAnalyzeIncomingData,
                        MaxDataSize = this.MaxDataSize,
                        ProcessVerificationInterval = this.ProcessVerificationInterval,
                        ReceiveDataInterval = this.ReceiveDataInterval,
                        ReceiveTotalBufferSize = this.ReceiveTotalBufferSize,
                        SleepingIntervalInMS = this.SleepingIntervalInMS
                    };
                    oItem.StartReceivingData();
                    /// add client socket to list
                    lock (InnerClientListLocker)
                    {
                        InnerClientList.Add(oItem);
                    }
                    /// set ready to accept client
                    //this.IsReadyAccept = true;
                }
                catch (Exception ex) { Logger?.Error(ex); }
                finally
                {
                    AcceptDone.Set();
                }
            }

            /// This function should be run as a thread
            /// https://msdn.microsoft.com/en-us/library/system.net.sockets.tcplistener.accepttcpclient(v=vs.110).aspx
            private void ProcessAcceptClient()
            {
                try
                {
                    if (IsExit) return;
                    Logger?.Debug("TCP Server begins to accept clients");
                    //IsReadyAccept = true;// set ready to accept client
                    DateTime tRefLog = DateTime.Now;
                    DateTime tRef = tRefLog.AddHours(-1);
                    DateTime tNow;
                    while (IsExit == false && ServerSocket != null)
                    {
                        tNow = DateTime.Now;
                        //if (IsReadyAccept && ServerSocket != null && (AcceptInterval < 1 || (int)(tNow - tRef).TotalSeconds >= AcceptInterval))
                        if (ServerSocket != null && (AcceptInterval < 1 || (int)(tNow - tRef).TotalSeconds >= AcceptInterval))
                        {
                            int i = NumberOfConnectedClients();
                            if (MaxClient < 0 || i < MaxClient)
                            {
                                tRef = tNow;/// set the time
                                //IsReadyAccept = false;// set NOT to accept client.
                                ServerSocket.BeginAccept(new AsyncCallback(AcceptClientCallback), ServerSocket);
                                AcceptDone.WaitOne();
                            }
                            else
                                Logger?.Warn("{0}.{1}. Number of Connected Clients exceeds maximum. Number of Connected Clients = {2}. Max = {3}", System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.Name, System.Reflection.MethodBase.GetCurrentMethod()?.Name, i, MaxClient);
                        }
                        if (ProcessVerificationInterval > 0 && (int)(tNow - tRefLog).TotalSeconds >= ProcessVerificationInterval)
                        {
                            tRefLog = tNow;
                            Logger?.Debug("{0}.{1} is running", System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.Name, System.Reflection.MethodBase.GetCurrentMethod()?.Name);
                        }
                        Sleeping(SleepingIntervalInMS);
                    }
                    Logger?.Debug("TCP Server stops to accept clients");
                }
                catch (Exception ex) { Logger?.Error(ex); }
            }

            /// Dequeue the outgoing data, which is sent failed
            public DataPackage? DequeueFailedOutgoingData()
            {
                try
                {
                    bool b = TryDequeueAtFailedOutgoingDataQueue(out DataPackage? oOutput);
                    return oOutput;
                }
                catch (Exception ex)
                {
                    Logger?.Error(ex);
                    return null;
                }
            }

            /// Stop this server to listen clients
            /// timeoutInSecond = Timeout in second. The default value is 5
            /// sleepingIntervalInMS = Sleeping interval in milli-second. The default value 100
            public void StopListening() { StopListening(5, 100); }
            public void StopListening(int timeoutInSecond, int sleepingIntervalInMS)
            {
                try
                {
                    IsExit = true;/// set to close all clients
                    if (timeoutInSecond < 0) timeoutInSecond = 0;
                    Logger?.Debug("TCP Server stops listening");
                    ServerSocket = null;

                    /// close the threads in a time interval
                    DateTime tRef = DateTime.Now;
                    while ((DateTime.Now - tRef).TotalSeconds < timeoutInSecond && (
                    (ThreadToAcceptClient?.IsAlive ?? false) || (ThreadToSendData?.IsAlive ?? false) || (ThreadToOtherProcesses?.IsAlive ?? false)
                    ))
                    {
                        Sleeping(sleepingIntervalInMS);
                    }
                    if (ThreadToAcceptClient != null && ThreadToAcceptClient.IsAlive)
                    {
                        Logger?.Debug("Force to abort the thread named {0}", ThreadToAcceptClient.Name);
                        ThreadToAcceptClient.Interrupt();
                    }
                    if (ThreadToSendData != null && ThreadToSendData.IsAlive)
                    {
                        Logger?.Debug("Force to abort the thread named {0}", ThreadToSendData.Name);
                        ThreadToSendData.Interrupt();
                    }
                    if (ThreadToOtherProcesses != null && ThreadToOtherProcesses.IsAlive)
                    {
                        Logger?.Debug("Force to abort the thread named {0}", ThreadToOtherProcesses.Name);
                        ThreadToOtherProcesses.Interrupt();
                    }

                    /// Close all clients
                    lock (InnerClientListLocker)
                    {
                        if (InnerClientList != null)
                        {
                            int i = 0;
                            while (i < InnerClientList.Count)
                            {
                                Client oItem = InnerClientList[i];
                                if (oItem != null)
                                {
                                    oItem.StopConnection(timeoutInSecond, sleepingIntervalInMS);
                                    //oItem = null;
                                    //InnerClientList[i] = null;
                                }
                                i += 1;
                            }
                            InnerClientList.Clear();
                            //InnerClientList = null;
                        }
                    }

                }
                catch (Exception ex) { Logger?.Error(ex); }
            }

            /// Start this server to listen clients
            public bool StartListening()
            {
                try
                {
                    IsExit = false;
                    /// check if the queue of incoming data is initialized
                    if (IncomingDataQueue == null)
                    {
                        Logger?.Warn("TCP Server refuses to start listening as the queue is not initialized");
                        return false;
                    }
                    //System.Net.IPHostEntry ipHostInfo = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
                    //System.Net.IPAddress ipAddress = ipHostInfo.AddressList[0];
                    System.Net.IPAddress ipAddress = System.Net.IPAddress.Any;
                    System.Net.IPEndPoint localEndPoint = new(ipAddress, ListeningPort);
                    ServerSocket = new(ipAddress.AddressFamily, System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
                    ServerSocket.Bind(localEndPoint);
                    ServerSocket.Listen(100);
                    /// Threads
                    ThreadToAcceptClient = new Thread(new ThreadStart(ProcessAcceptClient)) { Name = "ProcessAcceptClient" };
                    ThreadToAcceptClient.Start();
                    ThreadToSendData = new Thread(new ThreadStart(ProcessSendData)) { Name = "ProcessSendData" };
                    ThreadToSendData.Start();
                    ThreadToOtherProcesses = new Thread(new ThreadStart(ProcessOtherProcesses)) { Name = "ProcessOtherProcesses" };
                    ThreadToOtherProcesses.Start();
                    return true;
                }
                catch (Exception ex)
                {
                    Logger?.Error(ex);
                    return false;
                }
            }

            public Server(int listeningPort, Queue<DataPackage> incomingDataQueue, object incomingDataQueueLocker, ExternalActionToHandleIncomingData? externalActToHandleIncomingData)
            {
                ListeningPort = listeningPort;
                IncomingDataQueue = incomingDataQueue;
                IncomingDataQueueLocker = incomingDataQueueLocker;
                ExternalActToHandleIncomingData = externalActToHandleIncomingData;
            }
        }

        public enum SerialDataType : byte
        {
            Heartbeat = 0,
            Text = 1,
            File = 2,
            Error = 255
        }

        /// Return object after deserialization
        public class DeserializedData
        {
            public SerialDataType DataType { get; set; }
            public string? ErrorMessage { get; set; }
            public string Text { get; set; } = string.Empty;
            public string Filename { get; set; } = string.Empty;
            public int LastIndexPiece { get; set; }
            public int IndexPiece { get; set; }
            public byte[] FileContent { get; set; } = Array.Empty<byte>();
            /// Destination setting
            public string DestFolder { get; set; } = string.Empty;
            public string DestFilepath { get { return System.IO.Path.Combine(DestFolder, Filename); } }
        }

        public static class Serialization
        {
            /// This class is to convert a text or file to a byte array, and convert a byte array back to a text or file
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
                if (string.IsNullOrEmpty(text) || text.Length < 1) return new byte[1] { (byte)SerialDataType.Text };
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
                if (string.IsNullOrWhiteSpace(filename)) return Array.Empty<byte>();
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
                if (fileContentLength > 0 && piece != null) System.Buffer.BlockCopy(piece, 0, output, dstOffset, piece.Length);
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

            /// Serialize small file with size not more than 50M bytes
            public static byte[] SerializeSmallFile(string filepath)
            {
                //return SerializeSmallFile(System.IO.Path.GetFileName(filepath), System.IO.File.ReadAllBytes(filepath));
                if (string.IsNullOrWhiteSpace(filepath)) return Array.Empty<byte>();
                return SerializeFilePiece(System.IO.Path.GetFileName(filepath), 0, 0, System.IO.File.ReadAllBytes(filepath));
            }

            /// Deserialize a byte array and output text or file
            /// Output = DeserializedData
            /// DeserializedData.DataType indicates the type of the data whether it is text, file or heartbeat
            /// DeserializedData.ErrorMessage stores the error message during running this function
            /// DeserializedData.Text stores the text if the data type is text. Otherwise it is empty
            /// DeserializedData.Filename stores the filename if the data type is file. Otherwise it is empty
            /// DeserializedData.LastIndexPiece stores the last index of pieces of file
            /// DeserializedData.IndexPiece stores the index of current pieces
            /// DeserializedData.FileContent stores the file content if the data type is file. Otherwise it is empty
            public static DeserializedData Deserialize(byte[] data)
            {
                if (data == null || data.Length < 1) return new DeserializedData() { DataType = SerialDataType.Error, ErrorMessage = "No data" };
                switch (data[0])
                {
                    case (byte)SerialDataType.Text:
                        return new DeserializedData()
                        {
                            DataType = SerialDataType.Text,
                            Text = Encoding.UTF8.GetString(data, 1, data.Length - 1)
                        };
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
                        byte[]? filenameByteArray = null;
                        if (filenameLength > 0)
                        {
                            filenameByteArray = new byte[filenameLength];
                            System.Buffer.BlockCopy(data, 5, filenameByteArray, 0, filenameLength);
                        }
                        int srcOffset = 13 + filenameLength;
                        int contentLength = data.Length - srcOffset;
                        byte[]? contentByteArray = null;
                        if (contentLength > 0)
                        {
                            contentByteArray = new byte[contentLength];
                            System.Buffer.BlockCopy(data, srcOffset, contentByteArray, 0, contentLength);
                        }
                        return new DeserializedData()
                        {
                            DataType = SerialDataType.File,
                            Filename = filenameByteArray == null ? string.Empty : Encoding.UTF8.GetString(filenameByteArray),
                            LastIndexPiece = BitConverter.ToInt32(data, 5 + filenameLength),
                            IndexPiece = BitConverter.ToInt32(data, 9 + filenameLength),
                            FileContent = contentByteArray ?? Array.Empty<byte>()
                        };
                    case (byte)SerialDataType.Heartbeat:
                        return new DeserializedData()
                        {
                            DataType = SerialDataType.Heartbeat
                        };
                    default:
                        return new DeserializedData() { DataType = SerialDataType.Error, ErrorMessage = string.Format("Unknown serial data type {0}", data[0]) };
                }
            }

            /// Append the deserialized data to file
            /// Return value = error string. Null or empty if success
            /// deserializedData = object of deserialized data
            /// deserializedData.Filename = file path. If it is not full path, the file will be saved in the current directory by default
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
                if (fileContentLength > 0 && deserializedData.FileContent != null)
                {
                    using System.IO.FileStream fs = new(tempPath, System.IO.FileMode.Append);
                    fs.Write(deserializedData.FileContent, 0, fileContentLength);
                }
                else
                {
                    /// Create an empty file
                    if (!System.IO.File.Exists(tempPath)) using (System.IO.File.Create(tempPath)) { }
                }
                if (deserializedData.IndexPiece == deserializedData.LastIndexPiece)
                {
                    if (System.IO.File.Exists(deserializedData.DestFilepath)) System.IO.File.Delete(deserializedData.DestFilepath);
                    if (System.IO.File.Exists(tempPath)) System.IO.File.Move(tempPath, deserializedData.DestFilepath);
                    else return string.Format("Cannot find temp file {0}", tempPath);
                }
                return string.Empty;
            }

            ///// Deserialize a byte array and output text or file
            ///// Output = DeserializedData
            ///// DeserializedData.DataType indicates the type of the data whether it is text, file or heartbeat
            ///// DeserializedData.Text stores the text if the data type is text. Otherwise it is empty
            ///// DeserializedData.Filename stores the filename if the data type is file. Otherwise it is empty
            ///// DeserializedData.FileContent stores the file content if the data type is file. Otherwise it is empty
            ///// DeserializedData.ErrorMessage stores the error message during running this function
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
