using System;
using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

namespace VsCSharpWinForm_sample2.Models
{
    public class Param
    {
        public static bool IsService = false;// variables for system.
        public class Login
        {
            public static int MaxRetry = 3;// Maximum number of retry when login. If the value is less than 1, it means that there is no limit to retry. The default value is 3.
            public static string FailMessage = "Either username or password is wrong.";// Message shown after login fails.
            public static string ExceedMaxRetryMessage = "Already exceed the maximum number of retry login. Force to exit.";// Message shown after exceed the maximum number of retry when login.
            public static string Username = "";
            public static string Hash = "";
        }

        public class TcpDataType
        {
            public const byte Text = 0;
            public const byte File = 1;
        }

        public class TcpClient
        {
            public static int Id = 0;
            public static List<Views.FrmTcpClient> FormList = new List<Views.FrmTcpClient>();
            public static readonly object FormListLocker = new object();
            public class DefaultValue
            {
                public static string ServerHost = "127.0.0.1";
                public static int ServerPort = 8001;
                public static bool ContainLengthAsHeader = true;
                public static bool EncryptData = true;
                public static string CryptPassword = "abc123";
                public static string IncomingDataFilePath = @"TcpClient\{0:yyyyMMdd_HHmmss}_{1}.dat";// {0} is date time, {1} is Id.
                public static string IncomingDataFolder = "TcpClient";
            }
        }

        public class TcpServer
        {
            //public static Queue<Helpers.TTcpServerSocket.DataPackage> IncomingDataQueue = new Queue<Helpers.TTcpServerSocket.DataPackage>();
            public static Queue<Helpers.TTcpSocket.DataPackage> IncomingDataQueue = new Queue<Helpers.TTcpSocket.DataPackage>();
            public static readonly object IncomingDataQueueLocker = new object();
            //public static Helpers.TTcpServerSocket ServerSocket = null;
            public static Helpers.TTcpSocket.Server ServerSocket = null;
            public static string CryptPassword = "abc123";
            public static string IncomingDataFilePath = @"TcpServer\{0:yyyyMMdd_HHmmss}_{1}_{2}.dat";// {0} is date time, {1} is host, {2} is port.
            public static string IncomingDataFolder = "TcpServer";
        }
    }
}
