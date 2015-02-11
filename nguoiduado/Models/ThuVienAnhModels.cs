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

    public class ThuVienAnhModel : ModelView
    {

        public class LstThuVienAnh
        {
            public decimal HinhAnhID { get; set; }       
            public string TieuDe { get; set; }
            public string ListAnh { get; set; }
            public string ListNoiDung { get; set; }
            public string NguoiDang { get; set; }
            public Nullable<System.DateTime> NgayDang { get; set; }
            public Nullable<int> LuotXem { get; set; }
            public string TrangThaiPhatHanh { get; set; }
            public string NguoiCapNhat { get; set; }
            public Nullable<System.DateTime> NgayCapNhat { get; set; }
            public int TrangThaiSua { get; set; }
        }
        public bool AddNewThuVienAnh(TBL_ThuVienAnh thuVienAnh)
        {
            try
            {
                thuVienAnh.LuotXem = 0;
                thuVienAnh.NgayDang = DateTime.Now;
                //ThuVienAnh.NgayCapNhat = DateTime.Now;
                nguoiduadodb.TBL_ThuVienAnh.Add(thuVienAnh);
                nguoiduadodb.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public IEnumerable<LstThuVienAnh> GetAllThuVienAnh()
        {
            return (from c in nguoiduadodb.TBL_ThuVienAnh
                    orderby c.HinhAnhID, c.NgayDang ascending
                    select new LstThuVienAnh
                    {
                        HinhAnhID = c.HinhAnhID,
                        TieuDe = c.TieuDe,
                        TrangThaiPhatHanh = (string)(c.TrangThaiPhatHanh == 0 ? "Chờ xử lý" : (c.TrangThaiPhatHanh == 1 ? "Phát hành" : "Hủy phát hành")),
                        NguoiDang = c.NguoiDang,
                        NgayDang = c.NgayDang,
                        NguoiCapNhat = c.NguoiCapNhat,
                        NgayCapNhat = c.NgayCapNhat,
                        LuotXem = c.LuotXem,
                    }).ToList();
        }

        public TBL_ThuVienAnh GetThuVienAnhByID(Decimal dHinhAnhID)
        {

            var query = from c in nguoiduadodb.TBL_ThuVienAnh
                        where c.HinhAnhID == dHinhAnhID
                        select c;
            return query.SingleOrDefault();
        }

        public bool UpdateThuVienAnh(TBL_ThuVienAnh nd)
        {
            try
            {
                // Bắt đầu cập nhật dữ liệu
                TBL_ThuVienAnh ThuVienAnh = (from c in nguoiduadodb.TBL_ThuVienAnh
                                             where c.HinhAnhID == nd.HinhAnhID
                                             select c).First();
                ThuVienAnh.TieuDe = nd.TieuDe;
                ThuVienAnh.NguoiCapNhat = nd.NguoiCapNhat;
                ThuVienAnh.NgayCapNhat = nd.NgayCapNhat;
                ThuVienAnh.TrangThaiPhatHanh = nd.TrangThaiPhatHanh;
                nguoiduadodb.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        //  [nguoiduadoTWAuthorization]

        public bool DeleteThuVienAnh(decimal dHinhAnhID)
        {
            var nd = (from c in nguoiduadodb.TBL_ThuVienAnh
                      where c.HinhAnhID == dHinhAnhID
                      select c).SingleOrDefault();
            if (nd != null)
            {

                nguoiduadodb.TBL_ThuVienAnh.Remove(nd);
                nguoiduadodb.SaveChanges();
                return true;
            }
            // Nếu có lỗi xảy ra
            return false;
        }

        public List<TBL_ThuVienAnh> GetTop10ThuVienAnh()
        {

            var query = (from c in nguoiduadodb.TBL_ThuVienAnh.Take(10)
                         orderby c.NgayCapNhat, c.NgayDang, c.TieuDe descending
                         select c);
            return query.ToList();
        }
        public List<TBL_ThuVienAnh> GetThuVienAnhByMenu(decimal MaMenu)
        {

            var query = (from c in nguoiduadodb.TBL_ThuVienAnh
                         //where c.MenuID == MaMenu
                         orderby c.NgayCapNhat, c.NgayDang, c.TieuDe descending
                         select c);
            return query.ToList();
        }
        public List<TBL_ThuVienAnh> GetTop5ThuVienAnh(decimal MaMenu)
        {

            var query = (from c in nguoiduadodb.TBL_ThuVienAnh.Take(5)
                         //where c.MenuID == MaMenu
                         orderby c.NgayCapNhat, c.NgayDang, c.TieuDe descending
                         select c);
            return query.ToList();
        }

        public List<TBL_ThuVienAnh> GetTop10ThuVienAnh(decimal MaMenu)
        {

            var query = (from c in nguoiduadodb.TBL_ThuVienAnh.Take(10)
                         //where c.MenuID == MaMenu
                         orderby c.NgayCapNhat, c.NgayDang, c.TieuDe descending
                         select c);
            return query.ToList();
        }
        public List<TBL_ThuVienAnh> GetThuVienAnhPhanTrang(decimal menuID, int pageNumber, int pageSize)
        {
            var ND = (from c in nguoiduadodb.TBL_ThuVienAnh
                      //where menuID==c.MenuID
                      orderby c.NgayCapNhat descending, c.NgayDang descending
                      select c)

                     .Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            return ND;

        }
        public class ListMenuKT
        {

            DanhMucMenuModel menu = new DanhMucMenuModel();
            public DataTable Menu { get { return menu.MenuDataTable(); } }
        }

    }
    // Class dùng dùng để Validation
    [MetadataType(typeof(ThuVienAnhMetadata))]
    public partial class TBL_ThuVienAnh
    {
        public class ThuVienAnhMetadata
        {
            [HiddenInput(DisplayValue = false)]
            public decimal HinhAnhID { get; set; }

            [AllowHtml]
            [Display(Name = "Chi tiết bài viết")]
            public string ThuVienAnh { get; set; }

            [UIHint("Picture")]
            public int PictureId { get; set; }
        }

    }
}