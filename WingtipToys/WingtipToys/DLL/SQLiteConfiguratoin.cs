using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
using System.Data.Entity;
using System.Data.Entity.Core.Common;
using System.Data.SQLite;
using System.Data.SQLite.EF6;

namespace WingtipToys.DLL
{
    public class SQLiteConfiguratoin : DbConfiguration
    {
        /// <summary>
        /// SQLite with C#.Net and Entity Framework
        /// https://www.codeproject.com/Articles/1158937/SQLite-with-Csharp-Net-and-Entity-Framework
        /// </summary>
        public SQLiteConfiguratoin()
        {
            SetProviderFactory("System.Data.SQLite", SQLiteFactory.Instance);
            SetProviderFactory("System.Data.SQLite.EF6", SQLiteProviderFactory.Instance);
            SetProviderServices("System.Data.SQLite", (DbProviderServices)SQLiteProviderFactory.Instance.GetService(typeof(DbProviderServices)));
        }
    }
}