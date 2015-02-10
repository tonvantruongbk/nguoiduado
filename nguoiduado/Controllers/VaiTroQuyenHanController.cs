using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Web.Mvc;
using DevExpress.Web.Mvc;
using nguoiduado.Models;

namespace nguoiduado.Controllers
{
    public class VaiTroQuyenHanController : Controller
    {
        //
        // GET: /VaiTroQuyenHan/
        #region "Vai trò quyền hạn"
        public ActionResult Index()
        {
            var vaiTro = new VaiTroQuyenHanModel();
            return View(vaiTro);
        }

        public ActionResult ListVaiTro()
        {
            var vaiTro = new VaiTroQuyenHanModel();
            return PartialView("ListVaiTro", vaiTro.GetTBLAdVaiTros);
        }

        public JsonResult kiemTraTenVaiTro(decimal? maVaiTro, string tenVaiTro)
        {
            var model = new VaiTroQuyenHanModel();
            var count = model.CheckTenVaiTro(maVaiTro, tenVaiTro);
            _checkV = count == 0;
            return Json(count == 0, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddNewVaiTro()
        {
            _checkV = true;
            var vaiTro = new TBL_AD_VaiTro();
            if (Request["thongbao"] != null)
            {
                ViewBag.success = Request["thongbao"].ToString(CultureInfo.InvariantCulture);
            }
            return View(vaiTro);
        }

        [HttpPost]
        public ActionResult AddNewVaiTro([ModelBinder(typeof(DevExpressEditorsBinder))] TBL_AD_VaiTro vaiTro)
        {
            if (!_checkV)
            {
                ModelState.AddModelError("", "");
            }
            if (ModelState.IsValid)
            {
                var model = new VaiTroQuyenHanModel();
                var result = model.AddNewVaiTro(vaiTro);
                // Nếu tồn tại mã thì đưa ra câu thông báo
                string mess = !result ? "Mã tồn tại" : "Cập nhật thành công";
                return RedirectToAction("AddNewVaiTro", new { thongbao = mess });
            }
            return View(vaiTro);
        }

        public ActionResult EditVaiTro(string maVaiTro)
        {
            _checkV = true;
            var modelVaiTro = new VaiTroQuyenHanModel();
            decimal dmaDT;
            decimal.TryParse(maVaiTro, out dmaDT);
            return View(modelVaiTro.GetVaiTroByID(dmaDT));
        }

        [HttpPost]
        public ActionResult EditVaiTro([ModelBinder(typeof(DevExpressEditorsBinder))] TBL_AD_VaiTro vaiTro)
        {
            ViewBag.thongbao = "";
            ViewBag.success = "";
            // Kiểm tra validation
            if (!_checkV)
            {
                ModelState.AddModelError("", "");
            }
            if (ModelState.IsValid)
            {
                var vaiTroModel = new VaiTroQuyenHanModel();

                ////Check trùng Tên
                //var chekTen = vaiTroModel.UpdateCheckTen(vaiTro);
                //if (!chekTen)
                //{
                //    ViewBag.TenTonTai = Resources.Resource.CommonTenTonTai;
                //    return View(vaiTro);
                //}

                var result = vaiTroModel.UpdateVaiTro(vaiTro);
                // Nếu tồn tại mã thì đưa ra câu thông báo
                if (!result)
                {
                    @ViewBag.thongbao = "Tên vai trò " + vaiTro.TenVaiTro + " đã tồn tại";
                }
                else
                {
                    ViewBag.success = "Cập nhật thành công";
                }
                return View(vaiTro);
            }
            return View(vaiTro);
        }

        public String DeleteVaiTro(string maVaiTro)
        {
            var vaiTroModel = new VaiTroQuyenHanModel();
            decimal dmaTH;
            decimal.TryParse(maVaiTro, out dmaTH);
            string strReturn;
            //var checkdelete = vaiTroModel.DeleteCheck(dmaTH);

            //if (checkdelete)
            //{
            //    strReturn = "Không xóa được";
            //}
            //else
            //{
            //vaiTroModel.DeleteVaitro(dmaTH);
            //strReturn = "Xóa thành công";
            //  }
            vaiTroModel.DeleteVaitro(dmaTH);
            return "Xóa thành công";
            //return strReturn;
        }

        public String DeleteVaiTroNguoiDung(FormCollection collection, string maVaiTro, string maNguoiDung)
        {
            if (maVaiTro == null) throw new ArgumentNullException("maVaiTro");
            if (maNguoiDung == null) throw new ArgumentNullException("maNguoiDung");
            var vaiTroModel = new VaiTroQuyenHanModel();
            var strReturn = "";
            //string ListNhanVienSelected = collection["UserIDs"];

            maVaiTro = Request.Params["MaVaiTro"];
            maNguoiDung = Request.Params["MaNguoiDungs"];

            string[] listUserID = maNguoiDung.Split(',');

            foreach (var item in listUserID)
            {
                maNguoiDung = item;
                decimal dmaVT;
                decimal dMaND;
                decimal.TryParse(maVaiTro, out dmaVT);
                decimal.TryParse(maNguoiDung, out dMaND);

                if (dmaVT == 1 && dMaND == 54)
                {
                    strReturn = "Bạn không được loại bỏ người dùng này";
                    return strReturn;
                }

                var checkdelete = vaiTroModel.DeleteVaiTroQuyenHan(dMaND, dmaVT);

                strReturn = !checkdelete ? "Không xóa được" : "Xóa thành công";
            }
            return strReturn;
        }

        #endregion

        #region 'Detail Vai trò quyền hạn'
        // Người viết : HuongND
        // Ngày Viết : 18/07/2013

        private static decimal _idNvFirstVtqh;
        private static bool _firstPostBackDetailVtqh;
        public ActionResult DetailPanelVaiTroQuyenHan(VaiTroQuyenHanModel model)
        {
            _firstPostBackDetailVtqh = true;
            //var model = new VaiTroQuyenHanModel();
            return PartialView("DetailPanelVaiTroQuyenHan", model);
        }

        public ActionResult DetailPageControlVaiTroQuyenHan(VaiTroQuyenHanModel model)
        {
            if (!_firstPostBackDetailVtqh)
            {
                model = new VaiTroQuyenHanModel();
                //model.NhanVienDetail = model.GetNguoiDungByVaiTro(MaVaiTro);

            }
            return PartialView("DetailPageControlVaiTroQuyenHan", model);
        }

        public ActionResult DetailDanhSachNguoidung()
        {

            decimal userID = !string.IsNullOrEmpty(Request.Params["MaVaiTro"])
                                    ? decimal.Parse(Request.Params["MaVaiTro"])
                                    : 0;
            if (userID == 0)
            {
                userID = _idNvFirstVtqh;
            }
            else _idNvFirstVtqh = userID;
            var bCapReporsitory = new VaiTroQuyenHanModel();
            return PartialView("DetailDanhSachNguoidung", bCapReporsitory.GetNguoiDungByVaiTro(userID));
        }



        public ActionResult DetailNguoiDungList()
        {
            var bCapReporsitory = new VaiTroQuyenHanModel();
            return PartialView("DetailNguoiDungList", bCapReporsitory.GetNguoiDungByVaiTro(_idNvFirstVtqh));
        }

        #endregion


        #region "Danh sách người dùng

        private static readonly List<Decimal> ListVT = new List<Decimal>();
        public ActionResult PopUpVaiTro()
        {
            var vaitroModel = new VaiTroQuyenHanModel();
            var listVaitros = Request["ListVaiTros"];
            if (listVaitros != "[]")
            {
                var a = listVaitros.Replace("'", "").Replace("[", "").Replace("]", "");
                string[] b = a.Split(',');
                foreach (var item in b)
                {
                    ListVT.Add(Convert.ToDecimal(item));
                }
                ViewBag.ListVaiTros = a;
            }

            return View(vaitroModel.GetTBLAdVaiTroChuaSetNguoiDungs(ListVT));
        }

        [HttpPost]
        public ActionResult PopUpVaiTro(FormCollection collection, TBL_AD_VaiTro_NguoiDung itemVaiTroNguoiDung)
        {
            string listVaitroSelected = collection["idvaitros"];
            string[] listUserID = listVaitroSelected.Split(',');
            foreach (var item in listUserID)
            {
                itemVaiTroNguoiDung.MaVaiTro = Convert.ToDecimal(item);
                itemVaiTroNguoiDung.MaNguoiDung = Convert.ToDecimal(Request.Params["manhanvien"]);
                var vtqhModel = new VaiTroQuyenHanModel();
                vtqhModel.AddNewVaiTroNguoiDung(itemVaiTroNguoiDung);
            }

            ViewBag.Success = "true";

        

            return View();
        }


        public ActionResult ListVaiTroNV()
        {
            var vaiTro = new VaiTroQuyenHanModel();
            return PartialView("ListVaiTroNV", vaiTro.GetTBLAdVaiTroChuaSetNguoiDungs(ListVT));
        }

        public ActionResult DanhSachNguoiDung()
        {
            var nguoiDungModel = new QuanLyNguoiDungViewModel();
            return View(nguoiDungModel);
        }

        public ActionResult ListAD_NguoiDung()
        {

            var nguoiDungModel = new QuanLyNguoiDungViewModel();
            return PartialView("ListAD_NguoiDung", nguoiDungModel);
        }

        public ActionResult ComboBoxVaiTro()
        {
            var vaitroModel = new VaiTroQuyenHanModel();
            return PartialView("ComboBoxVaiTro", vaitroModel.GetTBLAdVaiTros);
        }

        public ActionResult AddNewAdNguoiDung()
        {
            var vtqh = new VaiTroQuyenHanModel();
            ViewBag.MaToChucNguoiDung = new SelectList(vtqh.GetTblAdToChucNguoiDung(0), "MaToChucNguoiDung", "MaDonVi");
            _lToChuc = new List<decimal?>();
            var adNguoiDungDtl = new QuanLyNguoiDungViewModel { MatKhau = "" };

            // var nhanvienRepository = new NhanVienRepository();
            //  ViewData["TreeCoCau"] = nhanvienRepository.CoCauDataTable();
            if (Request["thongbao"] != null)
            {
                ViewBag.success = Request["thongbao"].ToString(CultureInfo.InvariantCulture);
            }
            return View(adNguoiDungDtl);
        }

        public ActionResult ThongTinChung()
        {
            return PartialView();
        }

        //************Tab Page : Cơ cấu của tổ chức được phân quyền
        public ActionResult CoCauToChucPhanQuyen()
        {
            return PartialView();
        }

        static List<decimal?> _lToChuc = new List<decimal?>();
        public ActionResult TreeViewCoCauToChuc()
        {
            // var nhanvienRepository = new NhanVienRepository();
            string nodeName = Request.Params["NodeName"] ?? string.Empty;

            decimal tem;
            decimal.TryParse(nodeName, out tem);
            //List<decimal?> maDonVis = dequy.GetChildMaDonVi(tem);

            ////Thêm vào list khi check=true:
            //if (key != "0")
            //{
            //    foreach (var maDonVi in maDonVis)
            //    {
            //        if (!lToChuc.Contains(maDonVi) && nhanvienRepository.CheckToChucLeaf((decimal)maDonVi))
            //        {
            //            lToChuc.Add(maDonVi);
            //        }
            //    }
            //}
            ////Xóa khỏi list khi check=false:
            //else
            //{
            //    //nếu là lá (không có đơn vị con)
            //    if (nhanvienRepository.CheckToChucLeaf(tem))
            //    {
            //        lToChuc.Remove(tem);
            //    }
            //    //nếu có đơn vị con
            //    else
            //    {
            //        List<decimal?> maDonViCons = dequy.GetChildMaDonVi(tem);
            //        foreach (var maDonViCon in maDonViCons)
            //        {
            //            lToChuc.Remove(maDonViCon);
            //        }
            //    }
            //}

            //ViewData["TreeCoCau"] = nhanvienRepository.CoCauDataTable();
            //var model = nhanvienRepository.CoCauDataTable();
            return PartialView();
        }

        private static bool _checkV;

       
        public ActionResult EditAdNguoiDung(string maNguoiDung)
        {
            var nguoiDungModel = new VaiTroQuyenHanModel();
            // var nhanvienRepository = new NhanVienRepository();
            //  ViewData["TreeCoCau"] = nhanvienRepository.CoCauDataTable();
            decimal dAdNguoiDung = 0;
            decimal.TryParse(maNguoiDung, out dAdNguoiDung);

            var vtqh = new VaiTroQuyenHanModel();
            ViewBag.MaToChucNguoiDung = new SelectList(vtqh.GetTblAdToChucNguoiDung(dAdNguoiDung), "MaToChucNguoiDung", "MaDonVi");
            ViewBag.UserID = maNguoiDung;
            //Bind dữ liệu vào list Tổ chức trong tree tổ chức phân quyền
            _lToChuc = new List<decimal?>();
            _lToChuc = nguoiDungModel.GetToChucNguoiDungByNguoiDung(dAdNguoiDung);
            //var nguoiDungDetail = nguoiDungModel.GetNguoiDungByID(dAdNguoiDung);
            //nguoiDungDetail.MatKhau = "";

            return View("AddNewAdNguoiDung");
        }

        private static decimal _userIDForCoCauToChuc;

        [HttpPost]
       
        public ActionResult EditAdNguoiDung(FormCollection collection, string btnLoad)
        {
            var userID = collection["UserID"].ToString(CultureInfo.InvariantCulture);

            decimal dAdNguoiDung;
            decimal.TryParse(userID, out dAdNguoiDung);
            if (dAdNguoiDung > 0)
            {
                _userIDForCoCauToChuc = dAdNguoiDung;
            }
            // var nhanvienRepository = new NhanVienRepository();
            var vtqh = new VaiTroQuyenHanModel();
            if (btnLoad == "")
            {
                var nguoiDungModel = new VaiTroQuyenHanModel();

                //ViewData["TreeCoCau"] = nhanvienRepository.CoCauDataTable();

                ViewBag.MaToChucNguoiDung = new SelectList(vtqh.GetTblAdToChucNguoiDung(_userIDForCoCauToChuc), "MaToChucNguoiDung", "MaDonVi");

                //Bind dữ liệu vào list Tổ chức trong tree tổ chức phân quyền
                _lToChuc = new List<decimal?>();
                _lToChuc = nguoiDungModel.GetToChucNguoiDungByNguoiDung(_userIDForCoCauToChuc);
                var nguoiDungDetail = nguoiDungModel.GetNguoiDungByID(_userIDForCoCauToChuc);
                nguoiDungDetail.MatKhau = "";
                return View("AddNewAdNguoiDung", nguoiDungDetail);
            }

            //ViewData["TreeCoCau"] = nhanvienRepository.CoCauDataTable();
            ViewBag.MaToChucNguoiDung = new SelectList(vtqh.GetTblAdToChucNguoiDung(_userIDForCoCauToChuc), "MaToChucNguoiDung", "MaDonVi");
            ViewBag.thongbao = "";
            ViewBag.UpdateSuccess = "";

            if (_lToChuc.Count == 0)
            {
                ModelState.AddModelError("", "");
                ViewBag.success = "Bạn chưa chọn cơ cấu tổ chức nào.Vui lòng chọn cơ cấu tổ chức mà người dùng quản lý.";
            }
            ////Kiểm tra validation
            //if (ModelState.IsValid)
            //{

            //    var result = vaitroModel.UpdateCoCauToChucPhanQuyen(UserIDForCoCauToChuc, lToChuc);

            //    // Nếu tồn tại mã thì đưa ra câu thông báo
            //    if (!result)
            //    { @ViewBag.thongbao = "Mã đã tồn tại"; }
            //    else
            //    {
            //        ViewBag.success = "Cập nhật thành công";

            //        ViewBag.MaToChucNguoiDung = new SelectList(vtqh.GetTblAdToChucNguoiDung(UserIDForCoCauToChuc), "MaToChucNguoiDung", "MaDonVi");
            //    }
            //    return View("AddNewAdNguoiDung");
            //    //return RedirectToAction("EditAdNguoiDung", "VaiTroQuyenHan", new {maNguoiDung = UserID});
            //}
            return View("AddNewAdNguoiDung");
        }

        #endregion


        #region DetailQuanLyNguoiDung

        private static bool _firstPostBackDetail;
        public ActionResult DetailPanel(QuanLyNguoiDungViewModel model)
        {
            _firstPostBackDetail = true;
            decimal maNguoiDung = !string.IsNullOrEmpty(Request.Params["MaNguoiDung"]) ? decimal.Parse(Request.Params["MaNguoiDung"]) : 0;
            if (maNguoiDung > 0)
            {
                _idNvFirst = maNguoiDung;
            }
            var repository = new VaiTroQuyenHanModel();
            model.NguoiDungDetail = repository.GetNguoiDungDetailByID(maNguoiDung);

            return PartialView("DetailPanel", model);
        }

        private static decimal _idNvFirst;
        public ActionResult DetailPageControl(QuanLyNguoiDungViewModel model)
        {
            if (!_firstPostBackDetail)
            {
                decimal maNguoiDung = !string.IsNullOrEmpty(Request.Params["MaNguoiDung"])
                                         ? decimal.Parse(Request.Params["MaNguoiDung"])
                                         : 0;
                //  var nhanvienRepo = new NhanVienRepository();
                if (maNguoiDung == 0)
                {
                    //   maNguoiDung = nhanvienRepo.GetUserIDFirst();
                    _idNvFirst = maNguoiDung;
                }
                model = new QuanLyNguoiDungViewModel();
                //  model.NguoiDungDetail = repository.GetNguoiDungDetailByID(maNguoiDung);

                return PartialView("DetailPageControl", model);
            }
            return PartialView("DetailPageControl", model);
        }

        public ActionResult DetailThongTinChung(QuanLyNguoiDungViewModel model)
        {
            if (!_firstPostBackDetail || model.NguoiDungDetail == null)
            {
                //  var nhanvienRepo = new NhanVienRepository();
                var repository = new VaiTroQuyenHanModel();
                if (_idNvFirst == 0)
                {
                    //  idNVFirst = nhanvienRepo.GetUserIDFirst();
                }
                model = new QuanLyNguoiDungViewModel { NguoiDungDetail = repository.GetNguoiDungDetailByID(_idNvFirst) };

                return PartialView("DetailThongTinChung", model);
            }
            return PartialView("DetailThongTinChung", model);
        }


        public ActionResult DetailVaiTro(QuanLyNguoiDungViewModel model)
        {
            var repository = new VaiTroQuyenHanModel();
            //var nhanvienRepo = new NhanVienRepository();
            if (_idNvFirst == 0)
            {
                //idNVFirst = nhanvienRepo.GetUserIDFirst();
            }
            var result = repository.GetTBLAdVaiTroNguoiDungByMaNguoiDung(_idNvFirst);

            return PartialView("DetailVaiTro", result);
        }

        public ActionResult DetailVaiTroList()
        {
            var repository = new VaiTroQuyenHanModel();
            return PartialView("DetailVaiTroList", repository.GetTBLAdVaiTroNguoiDungByMaNguoiDung(_idNvFirst));
        }

        public string DeleteVaiTroDetail(string maVaiTroNguoiDung)
        {
            var kdtModel = new VaiTroQuyenHanModel();
            decimal maVTND;
            decimal.TryParse(maVaiTroNguoiDung, out maVTND);
            string strReturn;

            var return_ = kdtModel.DeleteVaiTroDetail(maVTND);
            strReturn = return_ ? "Xóa thành công" : "Không xóa được";
            return strReturn;
        }
        #endregion


        #region Popup Nhanvien

        public ActionResult TreeViewCoCau()
        {
            return PartialView();
        }

        private static int _d;

        public static void CreateTreeViewNodesRecursive(DataTable model, MVCxTreeViewNodeCollection nodesCollection, Int32 parentID)
        {
            var rows = model.Select("[MaMenuCha] = " + parentID);
            if (_d == 0 && parentID != 0)
            {
                _d++;
            }
            //d++;
            foreach (DataRow row in rows)
            {
                MVCxTreeViewNode node = nodesCollection.Add(row["TenMenu"].ToString(), row["MaMenu"].ToString());
                CreateTreeViewNodesRecursive(model, node.Nodes, Convert.ToInt32(row["MaMenu"]));
            }
        }

        private static int _d2;

        public static void CreateTreeViewMenuNodesRecursive(DataTable model, MVCxTreeViewNodeCollection nodesCollection, Int32 parentID)
        {
            var rows = model.Select("[MenuCha] = " + parentID);
            if (_d2 == 0 && parentID != 0)
            {
                _d2++;
            }
            //d++;
            foreach (DataRow row in rows)
            {
                MVCxTreeViewNode node = nodesCollection.Add(row["TenMenu"].ToString(), row["MenuID"].ToString());
                CreateTreeViewMenuNodesRecursive(model, node.Nodes, Convert.ToInt32(row["MenuID"]));
            }
        }

        public static void CreateTreeViewNodesRecursiveHienThi(DataTable model, MVCxTreeViewNodeCollection nodesCollection, Int32 parentID)
        {
            var rows = model.Select("[MenuCha] = " + parentID);
            if (_d2 == 0 && parentID != 0)
            {
                _d2++;
            }
            //d++;
            foreach (DataRow row in rows)
            {
                MVCxTreeViewNode node = nodesCollection.Add(row["TenMenu"].ToString(), row["MenuID"].ToString());
                CreateTreeViewMenuNodesRecursive(model, node.Nodes, Convert.ToInt32(row["MenuID"]));
            }
        }

        #endregion

        private static decimal _sMaVaiTro;
        public ActionResult PhanQuyen_(string mavaitro)
        {
            if (mavaitro != null) _sMaVaiTro = Convert.ToDecimal(mavaitro);
            var vtqhRepository = new VaiTroQuyenHanModel();
            var model = vtqhRepository.GetPhanQuyenViewModel(_sMaVaiTro);
            return View("PhanQuyen_", model);
        }

        [HttpPost]
        public ActionResult PhanQuyen_(FormCollection collection, List<PhanQuyenViewModel> model)
        {
            var vtqhRepository = new VaiTroQuyenHanModel();
            model = vtqhRepository.GetPqMd(_sMaVaiTro);
            foreach (var item in model)
            {
                if (string.IsNullOrEmpty(item.ControllerName)) continue;
                string chksd = "chksd" + item.MaMenu;
                string chkt = "chkt" + item.MaMenu;
                string chks = "chks" + item.MaMenu;
                string chkx = "chkx" + item.MaMenu;
                item.SuDung = collection[chksd] == "C";
                item.Them = collection[chkt] == "C";
                item.Sua = collection[chks] == "C";
                item.Xoa = collection[chkx] == "C";
            }

            var result = vtqhRepository.UpdatePhanQuyenVaiTro(model, _sMaVaiTro);
            model = vtqhRepository.GetPhanQuyenViewModel(_sMaVaiTro);
            if (result)
            {
                ViewBag.success = "Cập nhật thành công";
                return View("PhanQuyen_", model);
            }
            return View("PhanQuyen_", model);
        }


        public ActionResult EditPhanQuyenTheoVaiTro(decimal? mavaitro)
        {
            try
            {
                _sMaVaiTro = mavaitro.HasValue ? mavaitro.Value : 0;
            }
            catch
            {
                _sMaVaiTro = 1;
            }
            var vtqhRepository = new VaiTroQuyenHanModel();
            ViewData["ChucNang"] = vtqhRepository.ChucNangDataTable();
            var model = vtqhRepository.GetPhanQuyenViewModel(_sMaVaiTro);
            return View("PhanQuyen_", model);
        }

        public ActionResult EditPhanQuyen()
        {

            var vtqhRepository = new VaiTroQuyenHanModel();
            //ViewData["ChucNang"] = vtqhRepository.ChucNangDataTable();
            List<PhanQuyenViewModel> model = vtqhRepository.GetPhanQuyenViewModel(_sMaVaiTro);
            return PartialView(model);
        }

        private static int _tr;
        public static void CreateMenuNodesRecursive(DataTable model, MVCxTreeViewNodeCollection nodesCollection, Int32 parentID)
        {
            var rows = model.Select("[MaMenuCha] = " + parentID);
            if (_tr == 0 && parentID != 0)
            {
                _tr++;
            }
            foreach (DataRow row in rows)
            {
                MVCxTreeViewNode node = nodesCollection.Add(row["TenMenu"].ToString(), row["MaMenu"].ToString());
                CreateMenuNodesRecursive(model, node.Nodes, Convert.ToInt32(row["MaMenu"]));
            }
        }

        public ActionResult TreeViewChucNang()
        {
            return PartialView();
        }

        private static int _d3;
        public static void CreateTreeViewChucNangNodesRecursive(DataTable model, MVCxTreeViewNodeCollection nodesCollection, Int32 parentID)
        {
            var rows = model.Select("[MaMenuCha] = " + parentID);
            if (_d3 == 0 && parentID != 0)
            {
                _d3++;
            }
            foreach (DataRow row in rows)
            {
                MVCxTreeViewNode node = nodesCollection.Add(row["TenMenu"].ToString(), row["MaMenu"].ToString());
                CreateTreeViewChucNangNodesRecursive(model, node.Nodes, Convert.ToInt32(row["MaMenu"]));
            }
        }

        public ActionResult TreeViewHoatDong()
        {
            return PartialView();
        }

        #region "Phân quyền"
        public ActionResult PhanQuyen()
        {
            return View(VaiTroQuyenHanModel.GetDepartments());
        }

        public ActionResult ListMenu()
        {
            return PartialView("ListMenu", VaiTroQuyenHanModel.GetDepartments());
        }
        #endregion
    }
}
