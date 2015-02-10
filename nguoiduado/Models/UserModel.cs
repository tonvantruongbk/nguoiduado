using nguoiduado.Controllers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;

namespace nguoiduado.Models
{
    public class UserModel : ModelView
    {
        public IEnumerable GetTBLUser()
        {
            return (from c in nguoiduadodb.TBL_User select c).ToList();
        }

        //public List<NguoiDungChiTiet> GetAllUser()
        //{
        //    return (from c in BKTDB.TBL_User
        //            select new NguoiDungChiTiet
        //            {

        //                UserID = c.UserID,
        //                MaVaiTro = c.MaVaiTro,
        //                TenVaiTro = c.TBL_AD_VaiTro.TenVaiTro,
        //                MaNhanVien = c.MaNhanVien,
        //                TenDangNhap = c.TenDangNhap,
        //                MatKhau = c.MatKhau,
        //                HoVaTen = c.HoVaTen,
        //                GioiTinh = c.GioiTinh,
        //                NgaySinh = c.NgaySinh,
        //                NoiSinh = c.NoiSinh,
        //                NguyenQuan = c.NguyenQuan,
        //                ImageAvatar = c.ImageAvatar,
        //                DienThoaiDD = c.DienThoaiDD,
        //                DienThoaiCD = c.DienThoaiCD,
        //                Email = c.Email,
        //                NgayTao = c.NgayTao,
        //                NguoiTao = c.NguoiTao,
        //                NgayCapNhat = c.NgayCapNhat,
        //                NguoiCapNhat = c.NguoiCapNhat,
        //                SuDung = c.SuDung

        //            }).ToList();


        //}

        public TBL_User GetUserByTenDangNhap(string tenDangNhap)
        {
            return (from c in nguoiduadodb.TBL_User
                    where c.TenDangNhap == tenDangNhap
                    select c).FirstOrDefault();
        }

        public TBL_User GetUserByID(decimal id)
        {
            return (from c in nguoiduadodb.TBL_User
                    where c.UserID == id
                    select c).FirstOrDefault();
        }

        public string GetUserByUserName(string userName)
        {
            return (from c in nguoiduadodb.TBL_User
                    where c.TenDangNhap == userName
                    select c.HoVaTen).FirstOrDefault();
        }

        // Add a User 
        public bool AddNewUser(TBL_User user)
        {
            try
            {
                nguoiduadodb.TBL_User.Add(user);
                nguoiduadodb.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        //Update a User
        public bool UpdateUser(TBL_User user)
        {
            try
            {
                TBL_User us = (from c in nguoiduadodb.TBL_User
                               where c.UserID == user.UserID
                               select c).First();
                us.TenDangNhap = user.TenDangNhap;
                us.MaVaiTro = user.MaVaiTro;
                us.MaNhanVien = user.MaNhanVien;
                us.MatKhau = user.MatKhau;
                us.Email = user.Email;
                us.GioiTinh = user.GioiTinh;
                us.HoVaTen = user.HoVaTen;
                us.NgaySinh = user.NgaySinh;
                us.NoiSinh = user.NoiSinh;

                us.DienThoaiCD = user.DienThoaiCD;
                us.DienThoaiDD = user.DienThoaiDD;
                us.NguyenQuan = user.NguyenQuan;
                us.SuDung = user.SuDung;
                us.ImageAvatar = user.ImageAvatar;
                us.NgayCapNhat = DateTime.Now;
                //nguoiduadodb.Entry(us).State = EntityState.Modified;
                nguoiduadodb.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        //Update a User
        public bool UpdateNguoiDung(TBL_User user)
        {
            try
            {
                TBL_User us = (from c in nguoiduadodb.TBL_User
                               where c.UserID == user.UserID
                               select c).First();
                us.TenDangNhap = user.TenDangNhap;
                us.MaVaiTro = user.MaVaiTro;
                us.MaNhanVien = user.MaNhanVien;
                us.MatKhau = user.MatKhau;
                us.Email = user.Email;
                us.GioiTinh = user.GioiTinh;
                us.HoVaTen = user.HoVaTen;
                us.NgaySinh = user.NgaySinh;
                us.NoiSinh = user.NoiSinh;

                us.NoiO = user.NoiO;
                us.DienThoaiCD = user.DienThoaiCD;
                us.DienThoaiDD = user.DienThoaiDD;
                us.NguyenQuan = user.NguyenQuan;
                us.SuDung = user.SuDung;
                us.ImageAvatar = user.ImageAvatar;
                us.NgayCapNhat = DateTime.Now;
               // nguoiduadodb.Entry(us).State = EntityState.Modified;
                nguoiduadodb.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        //Delete a User

        public bool DeleteNguoiDung(int userId)
        {
            try
            {
                TBL_User tblUser = (from u in nguoiduadodb.TBL_User
                                    where u.UserID == userId
                                    select u).FirstOrDefault();

                nguoiduadodb.TBL_User.Remove(tblUser);
                nguoiduadodb.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    [MetadataType(typeof(UserMetadata))]
    public partial class TBL_User
    {
        public class UserMetadata
        {
            public decimal UserID { get; set; }
            public decimal? MaVaiTro { get; set; }
            [Display(Name = "Mã Người dùng:")]
            [Required(ErrorMessage = "Mã người dùng không được để trống")]
            [StringLength(256, ErrorMessage = "Mã người dùng tối đa 256 ký tự")]
            public string MaNhanVien { get; set; }
            [Display(Name = "Tên người dùng:")]
            [Required(ErrorMessage = "Tên người dùng không được để trống")]
            [StringLength(256, ErrorMessage = "Tên người dùng tối đa 256 ký tự")]
            public string TenDangNhap { get; set; }
            [Display(Name = "Mật khẩu:")]
            [Required(ErrorMessage = "Mật khẩu không được để trống")]
            [StringLength(256, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự trở lên", MinimumLength = 6)]
            public string MatKhau { get; set; }
            public string HoVaTen { get; set; }

            public string GioiTinh { get; set; }
            // [RegularExpression("^([0]?[1-9]|[1|2][0-9]|[3][0|1])[./-]([0]?[1-9]|[1][0-2])[./-]([0-9]{4}|[0-9]{2})$", ErrorMessage = "sai kiểu ngày tháng")]
            public DateTime? NgaySinh { get; set; }
            public string NoiSinh { get; set; }
            public string NguyenQuan { get; set; }
            public string ImageAvatar { get; set; }
            public string DienThoaiDD { get; set; }
            public string DienThoaiCD { get; set; }
            [Display(Name = "Email:")]
            [Required(ErrorMessage = "Email không được để trống")]
            [RegularExpression("\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*", ErrorMessage = "Email không đúng")]
            public string Email { get; set; }
            public DateTime? NgayTao { get; set; }
            public string NguoiTao { get; set; }
            public DateTime? NgayCapNhat { get; set; }
            public string NguoiCapNhat { get; set; }
            public bool SuDung { get; set; }
        }
    }
    public class NewUserViewModel
    {
        public TBL_User User { get; set; }

        [Required]

        [DisplayName("Re-enter Password")]
        [Compare("Password", ErrorMessage = "Passwords must match")]
        public string ComparePassword { get; set; }
    }
    public class Tacgia
    {
        public decimal MaUser { get; set; }
        public string TenTacGia { get; set; }
    }
}