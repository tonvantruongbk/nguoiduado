using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace nguoiduado.Models
{
    public class ModelView
    {

        readonly nguoiduado_dbEntities _nguoiduadodb = new nguoiduado_dbEntities();
        public nguoiduado_dbEntities nguoiduadodb
        {
            get { return _nguoiduadodb; }
        }


    }

    // Hàm dùng để validate so sánh ngày tháng
    // ví dụ:
    // +----------------------------------------------------------------------------------+
    // |          public DateTime? TuNgay { get; set; }                                   |
    // |          [MustBeGreaterThan("TuNgay", "Từ ngày phải nhỏ hơn đến ngày")]          |
    // |          public DateTime? DenNgay { get; set; }                                  |
    // +----------------------------------------------------------------------------------+
    public class MustBeGreaterThanAttribute : ValidationAttribute
    {
        private readonly string _otherProperty;

        public MustBeGreaterThanAttribute(string otherProperty, string errorMessage)
            : base(errorMessage)
        {
            _otherProperty = otherProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var otherProperty = validationContext.ObjectInstance.GetType().GetProperty(_otherProperty);
            var otherValue = otherProperty.GetValue(validationContext.ObjectInstance, null);
            var thisDateValue = Convert.ToDateTime(value);
            var otherDateValue = Convert.ToDateTime(otherValue);

            if (thisDateValue > otherDateValue)
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
        }
    }

    public class Common : ModelView
    {
        //List<decimal?> maDonVis;
        //public List<decimal?> GetChildMaDonVi(decimal maDonVi)
        //{
        //    maDonVis = new List<decimal?>();
        //    maDonVis.Add(maDonVi);
        //    var query = from c in BKTDB.TBL_ToChuc_CoCau
        //                where c.MaDonViCha == maDonVi
        //                select c;
        //    foreach (var child in query)
        //    {
        //        GetChildDeQuy(child.MaDonVi);
        //    }

        //    return maDonVis;
        //}

        //private void GetChildDeQuy(decimal maDonVi)
        //{
        //    var query = from c in BKTDB.TBL_ToChuc_CoCau
        //                where c.MaDonViCha == maDonVi
        //                select c;
        //    maDonVis.Add(maDonVi);
        //    foreach (var child in query)
        //    {
        //        GetChildDeQuy(child.MaDonVi);
        //    }
        //}


        //List<decimal?> maMenus;
        //public List<decimal?> GetChildMenu(decimal maMenu)
        //{
        //    maMenus = new List<decimal?>();
        //    maMenus.Add(maMenu);
        //    var query = from c in BKTDB.TBL_AD_Menu
        //                where c.MaMenuCha == maMenu
        //                select c;
        //    foreach (var child in query)
        //    {
        //        GetChildMenuDeQuy(child.MaMenu);
        //    }

        //    return maMenus;
        //}

        //private void GetChildMenuDeQuy(decimal maMenu)
        //{
        //    var query = from c in BKTDB.TBL_AD_Menu
        //                where c.MaMenuCha == maMenu
        //                select c;
        //    maMenus.Add(maMenu);
        //    foreach (var child in query)
        //    {
        //        GetChildDeQuy(child.MaMenu);
        //    }
        //}
        //GetChildMaDonVi là lá (Khong có đơn vị con) -- Mappv 09/07/2013

        //List<decimal?> _maDonVis;
        //public List<decimal?> GetChildMaDonViLeaf(decimal maDonVi)
        //{
        //    _maDonVis = new List<decimal?>();
        //    if (CheckToChucLeaf(maDonVi))
        //    {
        //        _maDonVis.Add(maDonVi);
        //    }
        //    var query = from c in BKTDB.TBL_ToChuc_CoCau
        //                where c.MaDonViCha == maDonVi
        //                select c;
        //    foreach (var child in query)
        //    {
        //        GetChildDeQuyLeaf(child.MaDonVi);
        //    }

        //    return _maDonVis;
        //}

        //private void GetChildDeQuyLeaf(decimal maDonVi)
        //{
        //    var query = from c in BKTDB.TBL_ToChuc_CoCau
        //                where c.MaDonViCha == maDonVi
        //                select c;
        //    if (CheckToChucLeaf(maDonVi))
        //    {
        //        _maDonVis.Add(maDonVi);
        //    }
        //    foreach (var child in query)
        //    {
        //        GetChildDeQuy(child.MaDonVi);
        //    }
        //}

        //public bool CheckToChucLeaf(decimal matochuc)
        //{
        //    var tc = (from c in BKTDB.TBL_ToChuc_CoCau
        //              where c.MaDonViCha == matochuc
        //              select c).Count();
        //    if (tc == 0) return true;
        //    return false;
        //}
    }

    public static class ConverToDataset
    {
        public static DataSet ToDataSet<T>(this IList<T> list)
        {
            Type elementType = typeof(T);
            var ds = new DataSet();
            var t = new DataTable();
            ds.Tables.Add(t);

            //add a column to table for each public property on T
            foreach (var propInfo in elementType.GetProperties())
            {
                var colType = Nullable.GetUnderlyingType(propInfo.PropertyType) ?? propInfo.PropertyType;

                t.Columns.Add(propInfo.Name, colType);
            }

            //go through each property on T and add each value to the table
            foreach (T item in list)
            {
                var row = t.NewRow();

                foreach (var propInfo in elementType.GetProperties())
                {
                    row[propInfo.Name] = propInfo.GetValue(item, null) ?? DBNull.Value;
                }

                t.Rows.Add(row);
            }

            return ds;
        }
    }
}