using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore6WebApp.Entities
{
    public interface IExcelEntity
    {
        public int? Version { get; set; }
        //DateTime? CreatedDt { get; set; }
        //int? CreatedBy { get; set; }
        //DateTime? UpdatedDt { get; set; }
        //int? UpdatedBy { get; set; }
    }
}
