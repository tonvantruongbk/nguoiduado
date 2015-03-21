using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;
using System.Linq;
using System.Data;
namespace nguoiduado.Models
{

    public class DanhMucNghiaTrangModel:ModelView
    {
    
      
     
        public bool AddNewNghiaTrang(TBL_DanhMucNghiaTrang NghiaTrang)
        {
            try
            {
                nguoiduadodb.TBL_DanhMucNghiaTrang.Add(NghiaTrang);
                nguoiduadodb.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public TBL_DanhMucNghiaTrang GetNghiaTrangByID(Decimal dID)
        {

            var query = from c in nguoiduadodb.TBL_DanhMucNghiaTrang
                        where c.ID == dID
                        select c;
            return query.SingleOrDefault();
        }
        

    }
    // Class dùng dùng để Validation
    
}