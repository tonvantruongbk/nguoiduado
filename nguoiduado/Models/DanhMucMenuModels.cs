using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;




namespace nguoiduado.Models
{
    public class DanhMucMenuModel:ModelView
    {
        public class LstMenu
        {
            public decimal MenuID { get; set; }
            public string TenMenu { get; set; }
            public string TenMenuCha { get; set; }
            public string Link { get; set; }
            public int? ThuTu { get; set; }
        }


        public IEnumerable<TBL_MenuDanhMuc> GetMenu()
        {
            return (from c in nguoiduadodb.TBL_MenuDanhMuc
                    orderby c.TenMenu ascending
                    select c).ToList();
        }
        public DataTable MenuDataTable()
        {
            return ToDataTable(GetMenu());
        }
        #region 'common method'
        public static DataTable ToDataTable<T>(IEnumerable<T> items)
        {
            // Create the result table, and gather all properties of a T        
            var table = new DataTable(typeof(T).Name);
            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // Add the properties as columns to the datatable
            foreach (var prop in props)
            {
                Type propType = prop.PropertyType;

                // Is it a nullable type? Get the underlying type 
                if (propType.IsGenericType && propType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    propType = new NullableConverter(propType).UnderlyingType;

                table.Columns.Add(prop.Name, propType);
            }

            // Add the property values per T as rows to the datatable
            foreach (var item in items)
            {
                var values = new object[props.Length];
                for (var i = 0; i < props.Length; i++)
                    values[i] = props[i].GetValue(item, null);

                table.Rows.Add(values);
            }

            return table;
        }
        #endregion
        public List<TBL_MenuDanhMuc> GetAllMenuTop()
        {

            var query = (from c in nguoiduadodb.TBL_MenuDanhMuc
                         orderby c.ThuTu descending
                         select c);
            return query.ToList();
        }
        public string GetTenMenuByID(decimal menuid)
        {

            var query = (from c in nguoiduadodb.TBL_MenuDanhMuc
                         where c.MenuID == menuid
                         
                         select c).Single();
            return query.TenMenu.ToString();
        }
    }

    // Class dùng dùng để Validation
    [MetadataType(typeof(MenuMetadata))]
    public partial class TBL_Menu
    {
        public class MenuMetadata
        {
            [HiddenInput(DisplayValue = false)]
            public decimal MenuID { get; set; }
            public int? CapMenu { get; set; }
            public int? MenuCha { get; set; }
            public string TenMenu { get; set; }
            public string Link { get; set; }
            public int? ThuTu { get; set; }
        }
    }
}