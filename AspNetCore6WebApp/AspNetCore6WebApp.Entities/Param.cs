using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore6WebApp.Entities
{
    public static class Param
    {
        public static class AppSettings
        {
            public const string Logging = "Logging";
            public const string SqliteDatabaseFilePath = "SqliteDatabaseFilePath";

            public static class ConnectionStrings
            {
                public const string SQLServer = "SQLServer";
                public const string SQLite = "SQLite";
            }

            public static class Login
            {
                public const string MaxRetry = "Login:MaxRetry";
                public const string FailMessage = "Login:FailMessage";
                public const string ExceedMaxRetryMessage = "Login:ExceedMaxRetryMessage";
            }

            public static class TT
            {
                public static class Logging
                {
                    public const string ContentFormat = "TT:Logging:ContentFormat";
                    public const string FilePathFormat = "TT:Logging:FilePathFormat";
                }
            }

            public static class Console
            {
                public const string DbMinBatchSize = "DbMinBatchSize";
                public const string DbMaxBatchSize = "DbMaxBatchSize";
                public static class DbExcelFile
                {
                    public const string Path = "DbExcelFile:Path";
                    public static class Sheet
                    {
                        public const string Product = "DbExcelFile:Sheet:Product";
                        public const string Vendor = "DbExcelFile:Sheet:Vendor";
                    }
                }
            }
        }

        public static class Excel
        {
            public static class ColumnHeader
            {
                public static class User
                {
                    public const string UserId = "UserId";
                    public const string LoginName = "LoginName";
                    public const string DisplayName = "DisplayName";
                    public const string Hash = "Hash";
                    public const string Password = "Password";
                    public const string DepartmentId = "DepartmentId";
                    public const string CategoryId = "CategoryId";
                    public const string SubCategoryId = "SubCategoryId";
                    public const string Birthday = "Birthday";
                    public const string RegistrationFee = "RegistrationFee";
                    public const string IsDisabled = "IsDisabled";
                    public const string Description = "Description";
                }

                public static class Product
                {
                    public const string Code = "Code";
                    public const string Name = "Name";
                    public const string Description = "Description";
                    public const string ProductTypeId = "ProductTypeId";
                    public const string Price = "Price";
                    public const string Price2 = "Price2";
                    public const string DiscountRate = "DiscountRate";
                    public const string Discount = "Discount";
                }

                public static class Vendor
                {
                    public const string Name = "Name";
                    public const string Description = "Description";
                }
            }
        }

        public class Login
        {
            public static int MaxRetry = 5;
            public static string FailMessage = "Fail";
            public static string ExceedMaxRetryMessage = "Exceed";
        }

        public static class MaxLength
        {
            public static class AppFunction
            {
                public const int UniqueName = 255;
                public const int DisplayName = 255;
                public const int ControllerName = 255;
                public const int ActionName = 255;
                public const int Description = 1023;
            }

            public static class Role
            {
                public const int UniqueName = 255;
                public const int DisplayName = 255;
                public const int Description = 1023;
            }

            public static class User
            {
                public const int LoginName = 255;
                public const int DisplayName = 255;
                public const int Hash = 255;
                public const int Password = 255;
                public const int Description = 1023;
                public const int RetypedPassword = 255;
            }
        }

        public class DefaultValue
        {
            public class TcpSocket
            {
                public const string CryptPassword = "abc123";

                public class Client
                {
                    public const string ServerHost = "127.0.0.1";
                    public const int ServerPort = 8001;
                    public const bool IsEncryptData = true;
                    public const string IncomingDataFilenameFormat = "{0:yyyyMMdd_HHmmss}_{1}.dat";// {0} is date time, {1} is Id.
                    public const string IncomingDataFolderFormat = "TcpClient{0}";
                }

                public class Server
                {
                    public const string IncomingDataFilenameFormat = "{0:yyyyMMdd_HHmmss}_{1}_{2}.dat";// {0} is date time, {1} is host, {2} is port.
                    public const string IncomingDataFolder = "TcpServer";
                }
            }
        }
    }
}
