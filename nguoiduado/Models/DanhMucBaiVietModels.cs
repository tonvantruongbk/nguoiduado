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

    public class DanhMucBaiVietModel:ModelView
    {
    
        public class LstBaiViet
        {
            public decimal MaNoiDung { get; set; }
            public string TenMenu { get; set; }
            public string MaSo { get; set; }
            public string TieuDe { get; set; }
            public string TrichDan { get; set; }
            public string TrangThaiPhatHanh { get; set; }

            public string NguoiNhap { get; set; }
            public DateTime? NgayNhap { get; set; }

            public string NguoiCapNhat { get; set; }
            public DateTime? NgayCapNhat { get; set; }

            public string NguonTin { get; set; }
            public string NguoiDuyet { get; set; }
            public DateTime? NgayDuyet { get; set; }
            public int? LuotXem { get; set; }
            public string FileWord { get; set; }
            public string FileImage { get; set; }
            public string KhaiThac { get; set; }
            public string TuKhoaTag { get; set; }
            public string Comment { get; set; }
            public int TrangThaiSua { get; set; }
        }
        public bool AddNewLietSi(TBL_DanhSachLietSi LS)
        {
            try
            {
                nguoiduadodb.TBL_DanhSachLietSi.Add(LS);
                nguoiduadodb.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool AddNewBaiViet(TBL_NoiDung NoiDung)
        {
            try
            {
                NoiDung.LuotXem = 0;
                NoiDung.NgayNhap = DateTime.Now;
                NoiDung.NgayCapNhat = DateTime.Now;
                nguoiduadodb.TBL_NoiDung.Add(NoiDung);
                nguoiduadodb.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
       
        public IEnumerable<LstBaiViet> GetAllBaiViet()
        {
            return (from c in nguoiduadodb.TBL_NoiDung
                    orderby c.MaNoiDung, c.NgayNhap ascending
                    select new LstBaiViet
                    {
                        MaNoiDung = c.MaNoiDung,
                        TenMenu = c.TBL_MenuDanhMuc.TenMenu,
                        MaSo = c.MaSo,
                        TieuDe = c.TieuDe,
                        TrichDan = c.TrichDan,
                        TrangThaiPhatHanh = (string)(c.TrangThaiPhatHanh == 0 ? "Chờ xử lý" : (c.TrangThaiPhatHanh == 1 ? "Phát hành" : "Hủy phát hành")),
                        NguoiNhap = c.NguoiNhap,
                        NgayNhap = c.NgayNhap,
                        NguoiCapNhat = c.NguoiCapNhat,
                        NgayCapNhat = c.NgayCapNhat,
                        NguonTin = c.NguonTin,
                        NguoiDuyet = c.NguoiDuyet,
                        NgayDuyet = c.NgayDuyet,
                        LuotXem = c.LuotXem,
                        FileWord = c.FileWord,
                        FileImage = c.FileImage,
                        KhaiThac = c.KhaiThac
                    }).ToList();
        }

        public TBL_NoiDung GetBaiVietByID(Decimal dMaBaiViet)
        {

            var query = from c in nguoiduadodb.TBL_NoiDung
                        where c.MaNoiDung == dMaBaiViet
                        select c;
            return query.SingleOrDefault();
        }

        public bool UpdateBaiViet(TBL_NoiDung nd)
        {
            try
            {
                // Bắt đầu cập nhật dữ liệu
                TBL_NoiDung NoiDung = (from c in nguoiduadodb.TBL_NoiDung
                                       where c.MaNoiDung == nd.MaNoiDung
                                       select c).First();

                NoiDung.TieuDe = nd.TieuDe;
                NoiDung.LinkAnh = nd.LinkAnh;
                NoiDung.NguoiCapNhat = nd.NguoiCapNhat;
                NoiDung.NgayCapNhat = nd.NgayCapNhat;
                NoiDung.NguonTin = nd.NguonTin;
                NoiDung.FileImage = nd.FileImage;
                NoiDung.FileWord = nd.FileWord;
                NoiDung.KhaiThac = nd.KhaiThac;
                NoiDung.NoiDung = nd.NoiDung;
                NoiDung.TrichDan = nd.TrichDan;
                NoiDung.Comment = nd.Comment;
                NoiDung.MucUuTienBaiViet = nd.MucUuTienBaiViet;
                NoiDung.MaLienQuanBaiViet = nd.MaLienQuanBaiViet;
                NoiDung.TrangThaiPhatHanh = nd.TrangThaiPhatHanh;
                NoiDung.TuKhoaTag = nd.TuKhoaTag;
                NoiDung.TrangThaiSua = 0;
                NoiDung.PictureId = nd.PictureId;
                NoiDung.ExistVideo = nd.ExistVideo;
                nguoiduadodb.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        //  [nguoiduadoTWAuthorization]
       
        public bool DeleteBaiViet(decimal dMaNoiDung)
        {
            var nd = (from c in nguoiduadodb.TBL_NoiDung
                      where c.MaNoiDung == dMaNoiDung
                      select c).SingleOrDefault();
            if (nd != null)
            {

                nguoiduadodb.TBL_NoiDung.Remove(nd);
                nguoiduadodb.SaveChanges();
                return true;
            }
            // Nếu có lỗi xảy ra
            return false;
        }

        public List<TBL_NoiDung> GetTop10BaiViet()
        {

            var query = (from c in nguoiduadodb.TBL_NoiDung
                         orderby c.NgayCapNhat descending, c.NgayNhap descending
                         select c).Take(10);
            return query.ToList();
        }

        public List<TBL_NoiDung> GetTop5BaiVietCoVideo()
        {

            var query = (from c in nguoiduadodb.TBL_NoiDung
                         where c.ExistVideo==true
                         orderby c.NgayCapNhat descending, c.NgayNhap descending
                         select c).Take(5);
            return query.ToList();
        }
        public List<TBL_NoiDung> GetBaiVietByMenu(decimal MaMenu)
        {

            var query = (from c in nguoiduadodb.TBL_NoiDung
                         where c.MenuID == MaMenu
                         orderby c.NgayCapNhat descending, c.NgayNhap descending
                         select c);
            return query.ToList();
        }
        public List<TBL_NoiDung> GetTop5BaiViet(decimal MaMenu)
        {

            var query = (from c in nguoiduadodb.TBL_NoiDung
                         where c.MenuID == MaMenu
                         orderby c.NgayCapNhat descending, c.NgayNhap descending
                         select c).Take(5);
            return query.ToList();
        }

        public List<TBL_NoiDung> GetTop10BaiViet(decimal MaMenu)
        {

            var query = (from c in nguoiduadodb.TBL_NoiDung
                         where c.MenuID == MaMenu
                         orderby c.NgayCapNhat descending, c.NgayNhap descending
                         select c).Take(5);
            return query.ToList();
        }
        public List<TBL_NoiDung> GetBaiVietPhanTrang(decimal menuID, int pageNumber, int pageSize)
        {
                var ND = (from c in nguoiduadodb.TBL_NoiDung
                          where menuID==c.MenuID

                          orderby c.NgayCapNhat descending, c.NgayNhap descending
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
    [MetadataType(typeof(BaiVietMetadata))]
    public partial class TBL_NoiDung
    {
        public class BaiVietMetadata
        {
            [HiddenInput(DisplayValue = false)]
            public decimal MaNoiDung { get; set; }

            [UIHint("tinymce_jquery_full"), AllowHtml]
            [Display(Name = "Chi tiết bài viết")]
            public string NoiDung { get; set; }

            [UIHint("Picture")]
            public int PictureId { get; set; }
        }

    }

    // Class dùng dùng để Validation
    [MetadataType(typeof(NghiaTrangMetadata))]
    public partial class TBL_DanhMucNghiaTrang
    {
        public class NghiaTrangMetadata
        {
            [HiddenInput(DisplayValue = false)]
            public decimal ID { get; set; }

            [UIHint("Picture")]
            public int PictureId { get; set; }
        }

    }
}