using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace nguoiduado.Models
{
    public class VaiTroQuyenHanModel : ModelView
    {
        #region "Vai Trò quyền hạn"
      
        public IEnumerable GetTBLAdVaiTros
        {
            get
            {
                return (from c in nguoiduadodb.TBL_AD_VaiTro select c).ToList();
            }
        }

        public List<TBL_AD_VaiTro> GetLstVaiTroND
        {
            get
            {
                return (from c in nguoiduadodb.TBL_AD_VaiTro select c).ToList();
            }
        }

        public IEnumerable GetTBLAdVaiTroChuaSetNguoiDungs(List<Decimal> listVaitro)
        {
            var query = (from c in nguoiduadodb.TBL_AD_VaiTro
                         where !listVaitro.Contains(c.MaVaiTro)
                         select c).ToList();

            return query;
        }

        public int CheckTenVaiTro(decimal? maVaiTro, string tenVaiTro)
        {
            if (maVaiTro > 0)
            {
                var check = from c in nguoiduadodb.TBL_AD_VaiTro
                            where c.TenVaiTro == tenVaiTro && c.MaVaiTro != maVaiTro
                            select c;
                return check.Count();
            }
            else
            {
                var check = from c in nguoiduadodb.TBL_AD_VaiTro
                            where c.TenVaiTro == tenVaiTro
                            select c;
                return check.Count();
            }
        }

        public bool AddNewCheckTen(TBL_AD_VaiTro vaiTro)
        {
            try
            {
                var check = from c in nguoiduadodb.TBL_AD_VaiTro
                            where c.TenVaiTro == vaiTro.TenVaiTro
                            select c;
                return !check.Any();
            }
            catch
            {
                return true;
            }
        }

        public bool AddNewVaiTro(TBL_AD_VaiTro vaiTro)
        {
            try
            {
                vaiTro.NguoiTao = "admin";
                vaiTro.NguoiCapNhat = "admin";
                vaiTro.NgayTao = DateTime.Now;
                vaiTro.NgayCapNhat = DateTime.Now;

                nguoiduadodb.TBL_AD_VaiTro.Add(vaiTro);
                nguoiduadodb.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool AddNewVaiTroNguoiDung(TBL_AD_VaiTro_NguoiDung vaiTroNguoiDung)
        {
            try
            {
                //vaiTroNguoiDung.MaNguoiDung = vaiTroNguoiDung.MaNguoiDung;
                //vaiTroNguoiDung.MaVaiTro = vaiTroNguoiDung.MaVaiTro;
                vaiTroNguoiDung.NguoiTao = "admin";
                vaiTroNguoiDung.NguoiCapNhat = "admin";
                vaiTroNguoiDung.NgayTao = DateTime.Now;
                vaiTroNguoiDung.NgayCapNhat = DateTime.Now;

                nguoiduadodb.TBL_AD_VaiTro_NguoiDung.Add(vaiTroNguoiDung);
                nguoiduadodb.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public TBL_AD_VaiTro GetVaiTroByID(decimal maVaiTro)
        {
            var query = from c in nguoiduadodb.TBL_AD_VaiTro
                        where c.MaVaiTro == maVaiTro
                        select c;
            return query.SingleOrDefault();
        }

        public bool UpdateCheckTen(TBL_AD_VaiTro vaiTro)
        {
            try
            {
                // Kiểm tra xem có trùng ký hiệu với các vai trò khác không?
                var check = from c in nguoiduadodb.TBL_AD_VaiTro
                            where c.MaVaiTro != vaiTro.MaVaiTro &&
                            c.TenVaiTro == vaiTro.TenVaiTro
                            select c;
                return !check.Any();
            }
            catch
            {
                return true;
            }
        }

        public bool UpdateVaiTro(TBL_AD_VaiTro vaiTro)
        {
            try
            {
                // Bắt đầu cập nhật dữ liệu
                TBL_AD_VaiTro vaiTroSingle = (from c in nguoiduadodb.TBL_AD_VaiTro
                                              where c.MaVaiTro == vaiTro.MaVaiTro
                                              select c).First();

                vaiTroSingle.TenVaiTro = vaiTro.TenVaiTro;
                vaiTroSingle.GhiChu = vaiTro.GhiChu;
                vaiTroSingle.NgayCapNhat = DateTime.Now;
                vaiTroSingle.NguoiCapNhat = "admin";
                vaiTroSingle.PhatHanhBaiViet = vaiTro.PhatHanhBaiViet;
                nguoiduadodb.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool DeleteCheck(decimal maVaiTro)
        {
            // Kiểm tra bảng vaitro_menu có sử dụng không
            // Nếu sử dụng thì không được xóa
            var checkMenu = from c in nguoiduadodb.TBL_AD_VaiTro_Menu
                            where c.MaVaiTro == maVaiTro
                            select c;
            var checkNguoidung = from vtnd in nguoiduadodb.TBL_AD_VaiTro_NguoiDung
                                 where vtnd.MaVaiTro == maVaiTro
                                 select vtnd;

            return checkMenu.Any() || checkNguoidung.Any();
            // Kiểm tra bảng vaitro_menu có sử dụng không
            // Nếu sử dụng thì không được xóa
        }

        public bool DeleteVaitro(decimal maVaiTro)
        {
            try
            {
                var vaiTroNguoidung = (from c in nguoiduadodb.TBL_AD_VaiTro_NguoiDung
                                       where c.MaVaiTro == maVaiTro
                                       select c).ToList();

                foreach (var tblAdVaiTroNguoiDung in vaiTroNguoidung)
                {
                    nguoiduadodb.TBL_AD_VaiTro_NguoiDung.Remove(tblAdVaiTroNguoiDung);
                    nguoiduadodb.SaveChanges();
                }

                var vaiTroMenu = (from c in nguoiduadodb.TBL_AD_VaiTro_Menu
                                  where c.MaVaiTro == maVaiTro
                                  select c).ToList();
                foreach (var tblAdVaiTroMenu in vaiTroMenu)
                {
                    nguoiduadodb.TBL_AD_VaiTro_Menu.Remove(tblAdVaiTroMenu);
                    nguoiduadodb.SaveChanges();
                }

                var vaiTro = (from c in nguoiduadodb.TBL_AD_VaiTro
                              where c.MaVaiTro == maVaiTro
                              select c).SingleOrDefault();
                if (vaiTro != null)
                {
                    nguoiduadodb.TBL_AD_VaiTro.Remove(vaiTro);
                    nguoiduadodb.SaveChanges();

                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool DeleteVaiTroQuyenHan(decimal dMaNguoiDung, decimal dMaVaiTro)
        {
            var vaitroNguoiDung = (from c in nguoiduadodb.TBL_AD_VaiTro_NguoiDung
                                   where c.MaVaiTro == dMaVaiTro && c.MaNguoiDung == dMaNguoiDung
                                   select c).SingleOrDefault();
            if (vaitroNguoiDung != null)
            {
                nguoiduadodb.TBL_AD_VaiTro_NguoiDung.Remove(vaitroNguoiDung);
                nguoiduadodb.SaveChanges();
                return true;
            }
            // Nếu có lỗi xảy ra
            return false;
        }

        #endregion
        #region "Danh sách người dùng"
        public QuanLyNguoiDungViewModel GetNguoiDungByID(decimal dMaNguoiDung)
        {
            var query = (from c in nguoiduadodb.TBL_User
                         where c.UserID == dMaNguoiDung
                         select new QuanLyNguoiDungViewModel
                         {
                             ////ChucDanh = c.ChucDanh,
                             //DaKichHoat = c.DaKichHoat,
                             //DienThoaiCoQuan = c.TTLienHe_DTCoQuan,
                             //DienThoaiDiDong = c.TTLienHe_DTDD,
                             ////DonViCongTac = c.DiaChiDonViCongTac,
                             //Email = c.TTLienHe_EmailCaNhan,
                             //Fax = c.Fax,
                             //GhiChu = c.GhiChu,
                             HoVaTen = c.HoVaTen,
                             MaNguoiDung = c.UserID,
                             //MaVaiTro = c.MaVaiTro,
                             MatKhau = c.MatKhau,
                             NgayCapNhat = c.NgayCapNhat,
                             NgayTao = c.NgayTao,
                             NguoiCapNhat = c.NguoiCapNhat,
                             NguoiTao = c.NguoiTao,
                             TenDangNhap = c.TenDangNhap
                         }).SingleOrDefault();
            return query;
        }




        public NguoiDungDetail GetNguoiDungDetailByID(decimal dMaNguoiDung)
        {


            var query = (from c in nguoiduadodb.TBL_User
                         where c.UserID == dMaNguoiDung
                         select new NguoiDungDetail
                         {
                             //ChucDanh = c.ChucDanh,
                             // DienThoaiCoQuan = c.TTLienHe_DTCoQuan,
                             DienThoaiDiDong = c.DienThoaiDD,
                             //DonViCongTac = c.DiaChiDonViCongTac,
                             Email = c.Email,
                             //Fax = c.t,
                             //GhiChu = c.GhiChu,
                             HoVaTen = c.HoVaTen,
                             MaNguoiDung = c.UserID,
                             MatKhau = c.MatKhau,
                             TenDangNhap = c.TenDangNhap
                         }).SingleOrDefault();
            return query;
        }

        public bool KiemTraMatKhau(decimal maND, string matkhau)
        {
            try
            {
                var query = (from c in nguoiduadodb.TBL_User
                             where c.UserID == maND && c.MatKhau == matkhau
                             select c);
                return query.Any();
            }
            catch
            {
                return false;
            }
        }


        public bool DoiMatKhau(decimal maND, string matkhau)
        {
            try
            {
                var query = (from c in nguoiduadodb.TBL_User
                             where c.UserID == maND
                             select c).SingleOrDefault();
                if (query != null) query.MatKhau = matkhau;
                nguoiduadodb.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

    

        public bool DeleteNguoiDung(decimal dMaNguoiDung)
        {
            try
            {
                // Xóa vai trò người dùng
                var vaitroNguoidung = (from c in nguoiduadodb.TBL_AD_VaiTro_NguoiDung
                                       where c.MaNguoiDung == dMaNguoiDung
                                       select c).SingleOrDefault();
                if (vaitroNguoidung != null)
                {
                    nguoiduadodb.TBL_AD_VaiTro_NguoiDung.Remove(vaitroNguoidung);
                }

                var nguoiDung = (from c in nguoiduadodb.TBL_User
                                 where c.UserID == dMaNguoiDung
                                 select c).SingleOrDefault();
                if (nguoiDung != null)
                {
                    nguoiduadodb.TBL_User.Remove(nguoiDung);
                }
                nguoiduadodb.SaveChanges();
                return true;

                // Nếu có lỗi xảy ra
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool DeleteVaiTroDetail(decimal maVTND)
        {
            try
            {
                // Xóa vai trò người dùng
                var vaitroNguoidung = (from c in nguoiduadodb.TBL_AD_VaiTro_NguoiDung
                                       where c.MaVaiTroNguoiDung == maVTND
                                       select c).FirstOrDefault();
                nguoiduadodb.TBL_AD_VaiTro_NguoiDung.Remove(vaitroNguoidung);
                nguoiduadodb.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public IEnumerable GetTblAdToChucNguoiDung(decimal manguoidung)
        {
            //var k = from h in nguoiduadodb.TBL_AD_ToChuc_NguoiDung
            //        where h.MaNguoiDung == manguoidung
            //        select h;
            return null;
        }

        public List<decimal?> GetToChucNguoiDungByNguoiDung(decimal manguoidung)
        {
            //var k = (from h in nguoiduadodb.TBL_AD_ToChuc_NguoiDung
            //         where h.MaNguoiDung == manguoidung
            //         select h.MaDonVi);
            return null;
        }

      

        public IEnumerable GetTBLAdVaiTroNguoiDungByMaNguoiDung(decimal manguoidung)
        {
            return (from c in nguoiduadodb.TBL_AD_VaiTro_NguoiDung
                    where c.MaNguoiDung == manguoidung
                    orderby c.TBL_AD_VaiTro.TenVaiTro ascending
                    select new
                    {
                        c.MaVaiTroNguoiDung,
                        c.MaNguoiDung,
                        c.MaVaiTro,
                        c.TBL_AD_VaiTro.GhiChu,
                        c.TBL_AD_VaiTro.TenVaiTro
                    }).ToList();
        }


        #endregion

        #region 'Phân quyền_'
        public List<PhanQuyenViewModel> GetPqMd(decimal mavaitro)
        {
            var k = from c in nguoiduadodb.TBL_AD_Menu
                    select new PhanQuyenViewModel
                    {
                        MaMenu = c.MaMenu,
                        MaMenuCha = c.MaMenuCha,
                        TenMenu = c.TenMenu,
                        MaVaiTro = mavaitro,
                        ControllerName = c.ControllerName
                    };
            return k.ToList();
        }

        private List<PhanQuyenViewModel> _listMenu;
        public List<PhanQuyenViewModel> GetPhanQuyenViewModel(decimal mavaitro)
        {
            _listMenu = new List<PhanQuyenViewModel>();
            var k = from m in nguoiduadodb.TBL_AD_Menu
                    join c in nguoiduadodb.TBL_AD_VaiTro_Menu.Where(d => d.MaVaiTro == mavaitro) on m.MaMenu equals c.MaMenu
                    into nvbc
                    from c in nvbc.DefaultIfEmpty()
                    orderby m.MaMenuCha, m.MaMenu
                    where m.MaMenuCha == 0
                    select new PhanQuyenViewModel
                    {
                        MaMenu = m.MaMenu,
                        MaMenuCha = m.MaMenuCha,
                        CapMenu = m.CapMenu,
                        TenMenu = m.TenMenu,
                        ControllerName = m.ControllerName,
                        //MaVaiTro = c.MaVaiTro,
                        SuDung = c != null && c.SuDung,
                        Them = c != null && c.Them,
                        Sua = c != null && c.Sua,
                        Xoa = c != null && c.Xoa
                    };

            foreach (var phanQuyenViewModel in k)
            {
                GetMenuChildDeQuy(phanQuyenViewModel, mavaitro);
            }
            return _listMenu;
        }

        private void GetMenuChildDeQuy(PhanQuyenViewModel menu, decimal mavaitro)
        {
           
            var query = from m in nguoiduadodb.TBL_AD_Menu.Where(d => d.MaMenuCha == menu.MaMenu)
                        join c in nguoiduadodb.TBL_AD_VaiTro_Menu.Where(d => d.MaVaiTro == mavaitro) on m.MaMenu equals c.MaMenu
                        into nvbc
                        from c in nvbc.DefaultIfEmpty()
                        orderby m.MaMenuCha, m.MaMenu
                        //where m.MaMenuCha == 0
                        select new PhanQuyenViewModel
                        {
                            MaMenu = m.MaMenu,
                            MaMenuCha = m.MaMenuCha,
                            CapMenu = m.CapMenu,
                            TenMenu = m.TenMenu,
                            ControllerName = m.ControllerName,
                            //MaVaiTro = c.MaVaiTro,
                            SuDung = c != null && c.SuDung,
                            Them = c != null && c.Them,
                            Sua = c != null && c.Sua,
                            Xoa = c != null && c.Xoa

                        };

            _listMenu.Add(menu);
            foreach (var child in query)
            {
                GetMenuChildDeQuy(child, mavaitro);
            }
        }

        public IEnumerable<TBL_AD_Menu> GetTBLAdMenus()
        {
            return (from c in nguoiduadodb.TBL_AD_Menu
                    orderby c.ControllerName, c.TenMenu ascending
                    select c).ToList();
        }

        public DataTable ChucNangDataTable()
        {
            return ToDataTable(GetTBLAdMenus());
        }

        public bool UpdatePhanQuyenVaiTro(List<PhanQuyenViewModel> models, decimal mavaitro)
        {
            try
            {
                //Xóa dữ liệu cũ trước khi cập nhật:
                var old = (from o in nguoiduadodb.TBL_AD_VaiTro_Menu
                           where o.MaVaiTro == mavaitro
                           select o);
                foreach (var item in old)
                {
                    nguoiduadodb.TBL_AD_VaiTro_Menu.Remove(item);
                }

                foreach (var model in models)
                {
                    var tbl = new TBL_AD_VaiTro_Menu
                    {
                        MaMenu = model.MaMenu,
                        MaVaiTro = model.MaVaiTro,
                        SuDung = model.SuDung,
                        Them = model.Them,
                        Sua = model.Sua,
                        Xoa = model.Xoa
                    };
                    nguoiduadodb.TBL_AD_VaiTro_Menu.Add(tbl);
                }
                nguoiduadodb.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }


        #endregion

        #region "Phân quyền"
        public class ListMenu
        {
            public int MaMenu { get; set; }
            public int MaMenuCha { get; set; }
            public string TenMenu { get; set; }
        }

        public class TreeGrid
        {
            public decimal MaMenu { get; set; }
            public string Name { get; set; }
            public TreeGrid Children { get; set; }
        }


        public List<ListMenu> GetTbladTBLAdVaiTroMenus()
        {
            var query = from c in nguoiduadodb.TBL_AD_Menu
                        select c;
            return query.Select(tblAdMenu => new ListMenu
            {
                MaMenu = Convert.ToInt32(tblAdMenu.MaMenu),
                MaMenuCha = Convert.ToInt32(tblAdMenu.MaMenuCha),
                TenMenu = tblAdMenu.TenMenu
            }).ToList();
        }

        public static List<Department> GetDepartments()
        {
            return new List<Department>
				       {
					       new Department(1, 0, "Corporate Headquarters", 1000000, "Monterey", "(408) 555-1234"),
					       new Department(2, 1, "Sales and Marketing", 22000, "San Francisco", "(415) 555-1234"),
					       new Department(3, 2, "Field Office: Canada", 500000, "Toronto", "(416) 677-1000", "(416) 555-1234"),
					       new Department(4, 2, "Field Office: East Coast", 500000, "Boston", "(617) 555-4234", "(415) 555-1234"),
					       new Department(5, 2, "Pacific Rim Headquarters", 600000, "Kuaui", "(808) 555-1234"),
					       new Department(6, 5, "Field Office: Singapore", 300000, "Singapore", "(606) 555-5486", "(606) 555-5786"),
					       new Department(7, 5, "Field Office: Japan", 500000, "Tokyo", "(707) 555-1526", "(707) 555-5432"),
					       new Department(8, 2, "Marketing", 1500000, "San Francisco", "(415) 555-1234"),
					       new Department(9, 1, "Finance", 40000, "Monterey", "(408) 555-1234"),
					       new Department(10, 1, "Engineering", 1100000, "Monterey", "(408) 555-1234"),
					       new Department(11, 10, "Consumer Electronics Div.", 1150000, "Burlington, VT", "(802) 555-1234"),
					       new Department(12, 11, "Software Development", 40000, "Monterey", "(408) 555-1234"),
					       new Department(13, 10, "Software Products Div.", 1200000, "Monterey", "(408) 555-1234"),
					       new Department(14, 13, "Quality Assurance", 48000, "Monterey", "(408) 555-1234", "(408) 555-1234"),
					       new Department(15, 13, "Customer Support", 38000, "Monterey", "(408) 555-1234"),
					       new Department(16, 13, "Research and Development", 460000, "Burlington, VT", "(802) 555-1234"),
					       new Department(17, 13, "Customer Services", 850000, "Burlington, VT", "(802) 555-1234")
				       };
        }
        public class Department
        {
            public Department(int id, int parentID, string name, int budget, string location, string phone1, string phone2 = null)
            {
                ID = id;
                ParentID = parentID;
                Name = name;
                Budget = budget;
                Location = location;
                Phone1 = phone1;
                Phone2 = string.IsNullOrEmpty(phone2) ? phone1 : phone2;
            }

            public int ID { get; set; }
            public int ParentID { get; set; }
            public string Name { get; set; }
            public int Budget { get; set; }
            public string Location { get; set; }
            public string Phone1 { get; set; }
            public string Phone2 { get; set; }
        }
        #endregion

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

        #region 'Phân quyền__'


        #endregion

        public IEnumerable GetNguoiDungByVaiTro(decimal maVaiTro)
        {
            var query = from c in nguoiduadodb.TBL_User
                        join vt in nguoiduadodb.TBL_AD_VaiTro_NguoiDung on c.UserID equals vt.MaNguoiDung
                        where vt.MaVaiTro == maVaiTro
                        select new NguoiDungDetail
                        {
                            HoVaTen = c.HoVaTen,
                            MaNguoiDung = c.UserID,
                            MatKhau = c.MatKhau,
                            TenDangNhap = c.MaNhanVien
                        };
            return query.ToList();
        }
    }

    public class QuanLyNguoiDungViewModel
    {
        #region insert

        [HiddenInput(DisplayValue = false)]
        public decimal MaNguoiDung { get; set; }


        //[Display(ResourceType = typeof(Resource), Name = "CommonHoVaTen")]
        public string HoVaTen { get; set; }

        //[Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "TBL_AD_NguoiDungMetadata_TenDangNhap_Tên_required")]
        //[Display(ResourceType = typeof(Resource), Name = "TBL_AD_NguoiDungMetadata_TenDangNhap")]
        //[Remote("kiemTraTenDangNhap", "VaiTroQuyenHan", AdditionalFields = "MaNguoiDung", ErrorMessage = "Tên đăng nhập đã tồn tại. Vui lòng nhập mã khác.")]
        public string TenDangNhap { get; set; }

        //[Required]
        //[Display(ResourceType = typeof(Resource), Name = "TBL_AD_NguoiDungMetadata_MatKhau")]
        //[StringLength(100, ErrorMessage = "{0} phải có ít nhất {2} ký tự", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string MatKhau { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Xác nhận mật khẩu mới")]
        [System.ComponentModel.DataAnnotations.Compare("MatKhau", ErrorMessage = "Mật khẩu mới phải trùng với Xác nhận lại mật khẩu")]
        public string MatKhauXacNhan { get; set; }

        public decimal? MaVaiTro { get; set; }

        public string Email { get; set; }

        public string DienThoaiCoQuan { get; set; }

        public string ChucDanh { get; set; }

        public string DienThoaiDiDong { get; set; }

        public string Fax { get; set; }

        public string DonViCongTac { get; set; }

        public DateTime? NgayTao { get; set; }

        public string NguoiTao { get; set; }

        public DateTime? NgayCapNhat { get; set; }

        public string GhiChu { get; set; }

        public string NguoiCapNhat { get; set; }

        public bool DaKichHoat { get; set; }

        #endregion
        public NguoiDungDetail NguoiDungDetail { get; set; }
    }

    public class PhanQuyenViewModel
    {
        public decimal MaMenu { get; set; }
        public decimal? MaMenuCha { get; set; }
        public decimal MaVaiTroManHinh { get; set; }
        public string ControllerName { get; set; }
        public byte? CapMenu { get; set; }
        public string TenMenu { get; set; }
        public decimal MaVaiTro { get; set; }
        public bool SuDung { get; set; }
        public bool Them { get; set; }
        public bool Sua { get; set; }
        public bool Xoa { get; set; }

    }

    public class NguoiDungDetail
    {
        public decimal MaNguoiDung { get; set; }
        public string HoVaTen { get; set; }
        public string TenDangNhap { get; set; }
        public string DonViCongTac { get; set; }
        public string MatKhau { get; set; }
        public string Email { get; set; }
        public string DienThoaiCoQuan { get; set; }
        public string ChucDanh { get; set; }
        public string DienThoaiDiDong { get; set; }
        public string Fax { get; set; }
        public string GhiChu { get; set; }


    }
   

    [MetadataType(typeof(VaiTroMetadata))]
    public partial class TBL_AD_VaiTro
    {
        public class VaiTroMetadata
        {
            [HiddenInput(DisplayValue = false)]
            public decimal MaVaiTro { get; set; }

            //[Display(ResourceType = typeof(Resource), Name = "TBL_AD_VaiTro_Ten")]
            //[Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "CommonRequired_Ten")]
            [Remote("kiemTraTenVaiTro", "VaiTroQuyenHan", AdditionalFields = "MaVaiTro", ErrorMessage = "Tên vai trò đã tồn tại. Vui lòng nhập tên khác.")]
            public string TenVaiTro { get; set; }

            //[Display(Name = "CommonGhiChu", ResourceType = typeof(Resource))]
            public string GhiChu { get; set; }
        }
    }
    public class VaiTroNguoiDung
    {
        public int Stt { get; set; }
        public string MaVaiTro { get; set; }
        public string TenVaiTro { get; set; }
    }
}
