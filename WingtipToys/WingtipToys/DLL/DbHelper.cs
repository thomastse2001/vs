using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace WingtipToys.DLL
{
    public class DbHelper
    {
        public static object GetObjectToDb<T>(T input)
        {
            if (input == null) { return DBNull.Value; }
            return input;
        }

        public static object GetObjectFromDb(DataRow dr, string fieldName)
        {
            if (string.IsNullOrEmpty(fieldName) || (dr?.Table.Columns.Contains(fieldName) ?? false) == false || DBNull.Value.Equals(dr[fieldName]))
            { return null; }
            return dr[fieldName];
        }

        /// Need to install System.Data.SQLite in NuGet Package Manager.
        /// The NuGet Package Manager will install the below packages automatically.
        /// EntityFramework
        /// System.Data.SQLite.Core
        /// System.Data.SQLite.EF6
        /// System.Data.SQLite.Linq
        /// System.Data.SQLite
        /// 
        /// Create SQLite Database and table
        /// https://stackoverflow.com/questions/15292880/create-sqlite-database-and-table
        /// How to Use and Connect To Sqlite in a Windows Application
        /// https://www.c-sharpcorner.com/UploadFile/5d065a/how-to-use-and-connect-sqlite-in-a-window-application/
        /// Using SQLite in your C# Application
        /// https://www.codeproject.com/Articles/22165/Using-SQLite-in-your-C-Application
        public class SQLite
        {
            public static string DatabaseFilePath { get; set; } = @"C:\temp\WingtipToys.sqlite";
            public static string ConnectionStringTemplate = "Data Source={0};Version=3";

            public static string GetConnectionString()
            {
                //return string.Format(ConnectionStringTemplate, System.IO.Path.GetFileName(DatabaseFilePath));
                return string.Format(ConnectionStringTemplate, DatabaseFilePath);
            }

            /// Create database file if it does not exist.
            public static void CreateDatabaseFileIfNotExist()
            {
                if (!System.IO.File.Exists(DatabaseFilePath))
                {
                    string folder = System.IO.Path.GetDirectoryName(DatabaseFilePath);
                    if (!System.IO.Directory.Exists(folder))
                    {
                        System.IO.Directory.CreateDirectory(folder);
                    }
                    System.Data.SQLite.SQLiteConnection.CreateFile(DatabaseFilePath);
                }
            }

            public static int ExecuteNonQuery(string sql, params System.Data.SQLite.SQLiteParameter[] arrayOfParameters)
            {
                if (string.IsNullOrEmpty(sql)) { return 0; }
                CreateDatabaseFileIfNotExist();
                using (System.Data.SQLite.SQLiteConnection cn = new System.Data.SQLite.SQLiteConnection(GetConnectionString()))
                {
                    cn.Open();
                    using (System.Data.SQLite.SQLiteCommand com = new System.Data.SQLite.SQLiteCommand(sql, cn))
                    {
                        if ((arrayOfParameters?.Length ?? 0) > 0)
                        { com.Parameters.AddRange(arrayOfParameters); }
                        return com.ExecuteNonQuery();
                    }
                }
            }

            public static int ExecuteNonQueryWithTransaction(string sql, params System.Data.SQLite.SQLiteParameter[] arrayOfParameters)
            {
                if (string.IsNullOrEmpty(sql)) { return 0; }
                CreateDatabaseFileIfNotExist();
                using (System.Data.SQLite.SQLiteConnection cn = new System.Data.SQLite.SQLiteConnection(GetConnectionString()))
                {
                    cn.Open();
                    int iReturn = -2;
                    cn.Open();
                    System.Data.SQLite.SQLiteTransaction trans = cn.BeginTransaction();
                    try
                    {
                        using (System.Data.SQLite.SQLiteCommand com = new System.Data.SQLite.SQLiteCommand(sql, cn, trans))
                        {
                            if ((arrayOfParameters?.Length ?? 0) > 0)
                            { com.Parameters.AddRange(arrayOfParameters); }
                            iReturn = com.ExecuteNonQuery();
                        }
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        iReturn = -2;
                        throw;
                    }
                    return iReturn;
                }
            }

            /// Return value = Number of rows affected.
            /// sConnectionString = Connection string.
            /// arrayOfSqlItems = Array of SQL items.
            /// KeyValuePair<string, SQLiteParameter[]> o1 = new KeyValuePair<string, SQLiteParameter[]>
            ///    (
            ///        "INSERT INTO SpeedStage (SpeedStageValue, Description) VALUES (@SpeedStageValue, @Description)",
            ///        new SQLiteParameter[]
            ///        {
            ///           new SQLiteParameter("@SpeedStageValue", SqlDbType.Int) { Value = 1001 },
            ///           new SQLiteParameter("@Description", SqlDbType.VarChar) { Value = "test1" }
            ///        }
            ///    );
            /// KeyValuePair<string, SQLiteParameter[]> o2 = new KeyValuePair<string, SQLiteParameter[]>
            ///    (
            ///        "INSERT INTO SpeedStage (SpeedStageValue, Description) VALUES (@SpeedStageValue, @Description)",
            ///        new SQLiteParameter[]
            ///        {
            ///           new SQLiteParameter("@SpeedStageValue", SqlDbType.Int) { Value = 1002 },
            ///           new SQLiteParameter("@Description", SqlDbType.VarChar) { Value = "test2" }
            ///        }
            ///    );
            /// KeyValuePair<string, SQLiteParameter[]> o3 = new KeyValuePair<string, SQLiteParameter[]>
            ///    (
            ///        "INSERT INTO SpeedStage (SpeedStageValue, Description) VALUES (@SpeedStageValue, @Description)",
            ///        new SQLiteParameter[]
            ///        {
            ///           new SQLiteParameter("@SpeedStageValue", SqlDbType.Int) { Value = 1003 },
            ///           new SQLiteParameter("@Description", SqlDbType.VarChar) { Value = "test3" }
            ///        }
            ///    );
            /// Example 1:
            /// int i = ExecuteNonQuery(ConnectionString, o1, o2);
            /// Example 2:
            /// KeyValuePair<string, SQLiteParameter[]>[] arrayOfSqlItems = new KeyValuePair<string, SQLiteParameter[]>[] { o1, o2, o3 };
            /// int i = ExecuteNonQuery(ConnectionString, arrayOfSqlItems);
            public static int ExecuteNonQuery(params KeyValuePair<string, System.Data.SQLite.SQLiteParameter[]>[] arrayOfSqlItems)
            {
                if (arrayOfSqlItems == null) { return 0; }
                CreateDatabaseFileIfNotExist();
                using (System.Data.SQLite.SQLiteConnection cn = new System.Data.SQLite.SQLiteConnection(GetConnectionString()))
                {
                    cn.Open();
                    int iReturn = 0;
                    System.Data.SQLite.SQLiteTransaction trans = cn.BeginTransaction();
                    try
                    {
                        foreach (KeyValuePair<string, System.Data.SQLite.SQLiteParameter[]> o in arrayOfSqlItems)
                        {
                            if (string.IsNullOrEmpty(o.Key) == false)/// o.Key is SQL.
                            {
                                using (System.Data.SQLite.SQLiteCommand com = new System.Data.SQLite.SQLiteCommand(o.Key, cn, trans))
                                {
                                    if ((o.Value?.Length ?? 0) > 0)/// o.Value is array of parameters.
                                    { com.Parameters.AddRange(o.Value); }
                                    iReturn += com.ExecuteNonQuery();
                                }
                            }
                        }
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        iReturn = -2;
                        throw;
                    }
                    return iReturn;
                }
            }

            /// Get a single value from database.
            public static object ExecuteScalar(string sql, params System.Data.SQLite.SQLiteParameter[] arrayOfParameters)
            {
                CreateDatabaseFileIfNotExist();
                using (System.Data.SQLite.SQLiteConnection cn = new System.Data.SQLite.SQLiteConnection(GetConnectionString()))
                {
                    using (System.Data.SQLite.SQLiteCommand com = new System.Data.SQLite.SQLiteCommand(sql, cn))
                    {
                        if ((arrayOfParameters?.Length ?? 0) > 0)
                        { com.Parameters.AddRange(arrayOfParameters); }
                        com.Connection.Open();
                        return com.ExecuteScalar();
                    }
                }
            }

            /// Get a table from database.
            public static System.Data.DataTable SelectDataTable(string sql, params System.Data.SQLite.SQLiteParameter[] arrayOfParameters)
            {
                if (!System.IO.File.Exists(DatabaseFilePath)) { return null; }
                using (System.Data.SQLite.SQLiteConnection cn = new System.Data.SQLite.SQLiteConnection(GetConnectionString()))
                {
                    System.Data.DataTable dtReturn = new System.Data.DataTable();
                    using (System.Data.SQLite.SQLiteDataAdapter da = new System.Data.SQLite.SQLiteDataAdapter(sql, cn))
                    {
                        if ((arrayOfParameters?.Length ?? 0) > 0)
                        { da.SelectCommand.Parameters.AddRange(arrayOfParameters); }
                        cn.Open();
                        da.Fill(dtReturn);
                    }
                    /// Cannot use SQLiteDataReader in some cases.
                    //using (System.Data.SQLite.SQLiteCommand com = new System.Data.SQLite.SQLiteCommand(sql, cn))
                    //{
                    //    if ((arrayOfParameters?.Length ?? 0) > 0)
                    //    { com.Parameters.AddRange(arrayOfParameters); }
                    //    com.Connection.Open();
                    //    using (System.Data.SQLite.SQLiteDataReader rdr = com.ExecuteReader())
                    //    {
                    //        dtReturn.Load(rdr);
                    //    }
                    //}
                    return dtReturn;
                }
            }

            /// Check if table exists.
            /// Return value = true if exists. Otherwise, false.
            public static bool TableExists(string tableName)
            {
                return Convert.ToInt32(ExecuteScalar(
                    "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name=@name",
                    new System.Data.SQLite.SQLiteParameter("@name", System.Data.DbType.String) { Value = tableName })) > 0;
            }

            public static bool CheckConnected()
            {
                try
                {
                    CreateDatabaseFileIfNotExist();
                    using (System.Data.SQLite.SQLiteConnection cn = new System.Data.SQLite.SQLiteConnection(GetConnectionString()))
                    {
                        cn.Open();
                        return true;
                    }
                }
                catch
                {
                    return false;
                }
            }
        }

        public static int InsertCategory(Models.Category o)
        {
            if (o == null) return 0;
            return DbHelper.SQLite.ExecuteNonQuery("INSERT INTO Category (CategoryID,CategoryName) VALUES (@CategoryID,@CategoryName)",
                new System.Data.SQLite.SQLiteParameter("@CategoryID", System.Data.DbType.Int32) { Value = o.CategoryID },
                new System.Data.SQLite.SQLiteParameter("@CategoryName", System.Data.DbType.AnsiString) { Value = o.CategoryName });
        }

        private static void InitializeCategoryTableIfNotExists()
        {
            if (DbHelper.SQLite.TableExists("Category")) return;
            int i = DbHelper.SQLite.ExecuteNonQuery(
                "CREATE TABLE Category ("
                + "CategoryID INTEGER PRIMARY KEY NOT NULL,"
                + "CategoryName varchar(100) NOT NULL,"
                + "Description varchar(1023) NULL"
                + ");");
            InitializeCategoryList().ForEach(x => InsertCategory(x));
        }

        public static int InsertProduct(Models.Product o)
        {
            if (o == null) return 0;
            return DbHelper.SQLite.ExecuteNonQuery("INSERT INTO Product (ProductID,ProductName,Description,ImagePath,UnitPrice,CategoryID) VALUES (@ProductID,@ProductName,@Description,@ImagePath,@UnitPrice,@CategoryID)",
                new System.Data.SQLite.SQLiteParameter("@ProductID", System.Data.DbType.Int32) { Value = o.ProductID },
                new System.Data.SQLite.SQLiteParameter("@ProductName", System.Data.DbType.AnsiString) { Value = o.ProductName },
                new System.Data.SQLite.SQLiteParameter("@Description", System.Data.DbType.AnsiString) { Value = o.Description },
                new System.Data.SQLite.SQLiteParameter("@ImagePath", System.Data.DbType.AnsiString) { Value = o.ImagePath },
                new System.Data.SQLite.SQLiteParameter("@UnitPrice", System.Data.DbType.Decimal) { Value = o.UnitPrice },
                new System.Data.SQLite.SQLiteParameter("@CategoryID", System.Data.DbType.Int32) { Value = o.CategoryID });
        }

        private static void InitializeProductTableIfNotExists()
        {
            if (DbHelper.SQLite.TableExists("Product")) return;
            int i = DbHelper.SQLite.ExecuteNonQuery(
                "CREATE TABLE Product ("
                + "ProductID INTEGER PRIMARY KEY NOT NULL,"
                + "ProductName varchar(100) NOT NULL,"
                + "Description varchar(10000) NOT NULL,"
                + "ImagePath varchar(100) NULL,"
                + "UnitPrice decimal NULL,"
                + "CategoryID INTEGER NULL"
                + ");");
            InitializeProductList().ForEach(x => InsertProduct(x));
        }

        private static void InitializeCartItemTableIfNotExists()
        {
            if (DbHelper.SQLite.TableExists("CartItem")) return;
            int i = DbHelper.SQLite.ExecuteNonQuery(
                "CREATE TABLE CartItem ("
                + "ItemId varchar(100) NOT NULL,"
                + "CartId varchar(100) NULL,"
                + "Quantity INTEGER NOT NULL DEFAULT 0,"
                + "DateCreated datetime NOT NULL,"
                + "ProductId INTEGER NOT NULL"
                + ");");
        }

        private static void InitializeOrderHeaderTableIfNotExists()
        {
            if (DbHelper.SQLite.TableExists("OrderHeader")) return;
            int i = DbHelper.SQLite.ExecuteNonQuery(
                "CREATE TABLE OrderHeader ("
                + "OrderHeaderId INTEGER PRIMARY KEY NOT NULL,"
                + "OrderDate datetime NOT NULL,"
                + "Username varchar(100) NULL,"
                + "FirstName varchar(160) NULL,"
                + "LastName varchar(160) NULL,"
                + "Address varchar(70) NULL,"
                + "City varchar(40) NULL,"
                + "State varchar(40) NULL,"
                + "PostalCode varchar(10) NULL,"
                + "Country varchar(40) NULL,"
                + "Phone varchar(24) NULL,"
                + "Email varchar(200) NULL,"
                + "Total decimal NOT NULL DEFAULT 0,"
                + "PaymentTransactionId varchar(200) NULL,"
                + "HasBeenShipped bit NOT NULL DEFAULT 0"
                + ");");
        }

        private static void InitializeOrderDetailTableIfNotExists()
        {
            if (DbHelper.SQLite.TableExists("OrderDetail")) return;
            int i = DbHelper.SQLite.ExecuteNonQuery(
                "CREATE TABLE OrderDetail ("
                + "OrderDetailId INTEGER PRIMARY KEY NOT NULL,"
                + "OrderHeaderId INTEGER NOT NULL,"
                + "Username varchar(100) NULL,"
                + "ProductId INTEGER NOT NULL,"
                + "Quantity INTEGER NOT NULL DEFAULT 0,"
                + "UnitPrice decimal NULL"
                + ");");
        }

        private static List<Models.Category> InitializeCategoryList()
        {
            return new List<Models.Category>
            {
                new Models.Category
                {
                    CategoryID = 1,
                    CategoryName = "Cars"
                },
                new Models.Category
                {
                    CategoryID = 2,
                    CategoryName = "Planes"
                },
                new Models.Category
                {
                    CategoryID = 3,
                    CategoryName = "Trucks"
                },
                new Models.Category
                {
                    CategoryID = 4,
                    CategoryName = "Boats"
                },
                new Models.Category
                {
                    CategoryID = 5,
                    CategoryName = "Rockets"
                }
            };
        }

        private static List<Models.Product> InitializeProductList()
        {
            return new List<Models.Product>
            {
                new Models.Product
                {
                    ProductID=1,
                    ProductName="Convertible Car",
                    Description="This convertible car is fast! The engine is powered by a neutrino based battery (not included). Power it up and let it go!",
                    ImagePath="carconvert.png",
                    UnitPrice=22.5,
                    CategoryID=1
                },
                new Models.Product
                {
                    ProductID = 2,
                    ProductName = "Old-time Car",
                    Description = "There's nothing old about this toy car, except it's looks. Compatible with other old toy cars.",
                    ImagePath="carearly.png",
                    UnitPrice = 15.95,
                     CategoryID = 1
               },
                new Models.Product
                {
                    ProductID = 3,
                    ProductName = "Fast Car",
                    Description = "Yes this car is fast, but it also floats in water.",
                    ImagePath="carfast.png",
                    UnitPrice = 32.99,
                    CategoryID = 1
                },
                new Models.Product
                {
                    ProductID = 4,
                    ProductName = "Super Fast Car",
                    Description = "Use this super fast car to entertain guests. Lights and doors work!",
                    ImagePath="carfaster.png",
                    UnitPrice = 8.95,
                    CategoryID = 1
                },
                new Models.Product
                {
                    ProductID = 5,
                    ProductName = "Old Style Racer",
                    Description = "This old style racer can fly (with user assistance). Gravity controls flight duration." +
                                  "No batteries required.",
                    ImagePath="carracer.png",
                    UnitPrice = 34.95,
                    CategoryID = 1
                },
                new Models.Product
                {
                    ProductID = 6,
                    ProductName = "Ace Plane",
                    Description = "Authentic airplane toy. Features realistic color and details.",
                    ImagePath="planeace.png",
                    UnitPrice = 95.00,
                    CategoryID = 2
                },
                new Models.Product
                {
                    ProductID = 7,
                    ProductName = "Glider",
                    Description = "This fun glider is made from real balsa wood. Some assembly required.",
                    ImagePath="planeglider.png",
                    UnitPrice = 4.95,
                    CategoryID = 2
                },
                new Models.Product
                {
                    ProductID = 8,
                    ProductName = "Paper Plane",
                    Description = "This paper plane is like no other paper plane. Some folding required.",
                    ImagePath="planepaper.png",
                    UnitPrice = 2.95,
                    CategoryID = 2
                },
                new Models.Product
                {
                    ProductID = 9,
                    ProductName = "Propeller Plane",
                    Description = "Rubber band powered plane features two wheels.",
                    ImagePath="planeprop.png",
                    UnitPrice = 32.95,
                    CategoryID = 2
                },
                new Models.Product
                {
                    ProductID = 10,
                    ProductName = "Early Truck",
                    Description = "This toy truck has a real gas powered engine. Requires regular tune ups.",
                    ImagePath="truckearly.png",
                    UnitPrice = 15.00,
                    CategoryID = 3
                },
                new Models.Product
                {
                    ProductID = 11,
                    ProductName = "Fire Truck",
                    Description = "You will have endless fun with this one quarter sized fire truck.",
                    ImagePath="truckfire.png",
                    UnitPrice = 26.00,
                    CategoryID = 3
                },
                new Models.Product
                {
                    ProductID = 12,
                    ProductName = "Big Truck",
                    Description = "This fun toy truck can be used to tow other trucks that are not as big.",
                    ImagePath="truckbig.png",
                    UnitPrice = 29.00,
                    CategoryID = 3
                },
                new Models.Product
                {
                    ProductID = 13,
                    ProductName = "Big Ship",
                    Description = "Is it a boat or a ship. Let this floating vehicle decide by using its " +
                                  "artifically intelligent computer brain!",
                    ImagePath="boatbig.png",
                    UnitPrice = 95.00,
                    CategoryID = 4
                },
                new Models.Product
                {
                    ProductID = 14,
                    ProductName = "Paper Boat",
                    Description = "Floating fun for all! This toy boat can be assembled in seconds. Floats for minutes!" +
                                  "Some folding required.",
                    ImagePath="boatpaper.png",
                    UnitPrice = 4.95,
                    CategoryID = 4
                },
                new Models.Product
                {
                    ProductID = 15,
                    ProductName = "Sail Boat",
                    Description = "Put this fun toy sail boat in the water and let it go!",
                    ImagePath="boatsail.png",
                    UnitPrice = 42.95,
                    CategoryID = 4
                },
                new Models.Product
                {
                    ProductID = 16,
                    ProductName = "Rocket",
                    Description = "This fun rocket will travel up to a height of 200 feet.",
                    ImagePath="rocket.png",
                    UnitPrice = 122.95,
                    CategoryID = 5
                }
            };
        }

        public static void InitializeDb()
        {
            //if (!DbHelper.SQLite.TableExists("Product")) InitializeProductList().ForEach(x => InsertProduct(x));
            //if (!DbHelper.SQLite.TableExists("Category")) InitializeCategoryList().ForEach(x => InsertCategory(x));
            InitializeCategoryTableIfNotExists();
            InitializeProductTableIfNotExists();
            InitializeCartItemTableIfNotExists();
            InitializeOrderHeaderTableIfNotExists();
            InitializeOrderDetailTableIfNotExists();
        }
    }
}