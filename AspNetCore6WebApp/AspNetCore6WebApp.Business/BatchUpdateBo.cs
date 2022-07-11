using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using Microsoft.Extensions.Logging;
using AspNetCore6WebApp.Entities;
using AspNetCore6WebApp.DLL;

namespace AspNetCore6WebApp.Business
{
    public interface IBatchUpdateBo
    {
        int UpdateFromExcel<T>(int createdBy, string path, string sheetName) where T : IExcelEntity;
    }

    public class BatchUpdateBo : IBatchUpdateBo
    {
        private readonly ILogger<BatchUpdateBo> logger;
        //private readonly TT.Logging tlogger;
        private readonly TT.IExcelDataReaderHelper excelDataReaderHelper;
        private readonly IProductDao productDao;
        private readonly IVendorDao vendorDao;

        public BatchUpdateBo(ILogger<BatchUpdateBo> logger,
            //TT.Logging tlogger,
            TT.IExcelDataReaderHelper excelDataReaderHelper,
            IProductDao productDao,
            IVendorDao vendorDao)
        {
            this.logger = logger;
            //this.tlogger = tlogger;
            this.excelDataReaderHelper = excelDataReaderHelper;
            this.productDao = productDao;
            this.vendorDao = vendorDao;
        }

        private int ImportRecordsByDataTable<T>(int createdBy, DateTime createdDt, DataTable? dt, IExcelEntityDao<T> excelEntityDao) where T : IExcelEntity
        {
            try
            {
                int updateVersion = excelEntityDao.GetMaxVersion() + 1;
                logger.LogWarning("Update version = {updateVersion}", updateVersion);
                if (updateVersion == int.MaxValue)
                {
                    logger.LogError("Update version reaches max value.");
                    return -11;
                }
                logger.LogWarning("Get from data table.");
                IEnumerable<T>? list = excelEntityDao.GetFromDataTable(updateVersion, createdBy, createdDt, dt, out string? outList);
                if (list == null || list.Any() == false)
                {
                    logger.LogError("Cannot extract records from DataSet.");
                    return -12;
                }
                int recordCount = list.Count();
                if (recordCount < 1)
                {
                    logger.LogError("DataSet Record Count is too few that seems not valid. Stop to import. DataSet Record Count = {recordCount}", recordCount);
                    return -13;
                }
                return excelEntityDao.BatchInsertAndUpdate(updateVersion, list);
            }
            catch (Exception ex)
            {
                logger.LogError("{ex.ToString()}", ex);
                return -11;
            }
        }

        public int UpdateFromExcel<T>(int createdBy, string path, string sheetName) where T : IExcelEntity
        {
            try
            {
                if (string.IsNullOrWhiteSpace(path))
                {
                    logger.LogWarning("UpdateFromCsv. Empty path.");
                    return -2;
                }
                DateTime createdTime = DateTime.Now;
                logger.LogWarning("------------------------------------------");
                logger.LogWarning("Start Date Time = {createdTime:yyyy-MM-dd HH:mm:ss}", createdTime);
                DataTable? dt = excelDataReaderHelper.GetDataTable(path, sheetName);
                int rowCount = dt?.Rows?.Count ?? 0;
                logger.LogWarning("Excel Row Count = {rowCount}", rowCount);
                if (rowCount < 1)
                {
                    logger.LogError("Excel Row Count is too few that seems not valid. Stop to import. Excel Row Count = {rowCount}", rowCount);
                    return -3;
                }
                DateTime createdDt = DateTime.Now;
                Type inputType = typeof(T);
                if (inputType == typeof(Product))
                    return ImportRecordsByDataTable<Product>(createdBy, createdDt, dt, productDao);
                else if (inputType == typeof(Vendor))
                    return ImportRecordsByDataTable<Vendor>(createdBy, createdDt, dt, vendorDao);
                return 0;
            }
            catch (Exception ex)
            {
                logger.LogError("{ex}", ex.ToString());
                return -1;
            }
        }
    }
}
