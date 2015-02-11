using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using nguoiduado.Models;
using DevExpress.Web.Mvc;

namespace nguoiduado.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            // DXCOMMENT: Pass a data model for GridView

            return View();
        }

        public ActionResult _rootHomeHeader()
        {
            // DXCOMMENT: Pass a data model for GridView in the PartialView method's second parameter
            return PartialView("_rootHomeHeader");
        }
        public ActionResult _rootHomeLeft()
        {
            nguoiduado_dbEntities db = new nguoiduado_dbEntities();
            ViewData["MenuDanhMuc"] = db.TBL_MenuDanhMuc.ToList();
            ViewData["LienKetWeb"] = db.TBL_LienKet.OrderBy(m => m.TenLienKet).ToList();
            // DXCOMMENT: Pass a data model for GridView in the PartialView method's second parameter
            return PartialView("_rootHomeLeft");
        }
        public ActionResult _rootHomeRight()
        {
            DanhMucBaiVietModel bvmodel = new DanhMucBaiVietModel();
            List<TBL_NoiDung> LstNDTop10 = new List<TBL_NoiDung>();
            LstNDTop10 = bvmodel.GetTop10BaiViet();
            ViewData["10BaiVietMoiNhat"] = LstNDTop10;

            TBL_NoiDung ItemNoiBat = new TBL_NoiDung();
            ItemNoiBat = LstNDTop10[0];
            ViewData["BaiVietNoiBat"] = ItemNoiBat;

            //Get 2 menu đầu tiê
            List<TBL_MenuDanhMuc> LstAllMenu = new List<TBL_MenuDanhMuc>();
            DanhMucMenuModel MenuModel = new DanhMucMenuModel();
            LstAllMenu = MenuModel.GetAllMenuTop();
            ViewData["LstAllDanhMuc"] = LstAllMenu;
            //Get 5 bai menu 1
            List<TBL_NoiDung> LstNDTop5Menu1 = new List<TBL_NoiDung>();
            List<TBL_NoiDung> LstNDTop5Menu2 = new List<TBL_NoiDung>();
            LstNDTop5Menu1 = bvmodel.GetTop5BaiViet(LstAllMenu[0].MenuID);
            LstNDTop5Menu2 = bvmodel.GetTop5BaiViet(LstAllMenu[1].MenuID);

            ViewData["LstNDTop5Menu1"] = LstNDTop5Menu1;
            ViewData["LstNDTop5Menu2"] = LstNDTop5Menu2;

            for (int i = 2; i < LstAllMenu.Count; i++)
            {
                ViewData["LstNDTop10Menu" + i] = bvmodel.GetTop10BaiViet(LstAllMenu[i].MenuID);
            }

            return PartialView("_rootHomeRight");
        }
        public ActionResult _rootHome3ColRight()
        {

            // DXCOMMENT: Pass a data model for GridView in the PartialView method's second parameter
            return PartialView("_rootHome3ColRight");
        }
        public ActionResult _rootHomeFooter()
        {
            // DXCOMMENT: Pass a data model for GridView in the PartialView method's second parameter
            return PartialView("_rootHomeFooter");
        }


        public ActionResult ListItemBaiViet(decimal MenuID)
        {
            nguoiduado_dbEntities db = new nguoiduado_dbEntities();
            DanhMucMenuModel DMModel = new DanhMucMenuModel();
            if (MenuID == -1)
            { ViewBag.TenMenu = "Tin Tức"; }
            else
                ViewBag.TenMenu = DMModel.GetTenMenuByID(MenuID);
            return View();
        }



        [ValidateInput(false)]
        public ActionResult GridView1Partial(int MenuID)
        {
            nguoiduado_dbEntities db = new nguoiduado_dbEntities();
            List<TBL_NoiDung> model = new List<TBL_NoiDung>();

            if (MenuID == -1)
            {
                model = db.TBL_NoiDung.ToList();
            }
            else
            { model = db.TBL_NoiDung.Where(c => c.MenuID == MenuID).ToList(); }

            return PartialView("_GridView1Partial", model);
        }

        public ActionResult Detail(decimal MaNoiDung)
        {
            nguoiduado_dbEntities db = new nguoiduado_dbEntities();
            DanhMucBaiVietModel DMModel = new DanhMucBaiVietModel();
            TBL_NoiDung ND = new TBL_NoiDung();
            ND = DMModel.GetBaiVietByID(MaNoiDung);
            ViewBag.TieuDe = ND.TieuDe;
            return View(ND);
        }
        public ActionResult Intro()
        {

            return View();
        }
        public ActionResult Guide()
        {

            return View();
        }
        public ActionResult Contact()
        {

            return View();
        }
        public ActionResult TimMoLietSi()
        {
            return View();
        }
        public ActionResult DangTinTimMo(string thongbao)
        {
            if(thongbao!=null)
            {
                ViewBag.ThongBao = thongbao;
            }
            return View();
        }
        [HttpPost]
        public ActionResult DangTinTimMo(TBL_DanhSachLietSi DSLietSi)
        {
            DanhMucBaiVietModel BVModel= new DanhMucBaiVietModel();
            BVModel.AddNewLietSi(DSLietSi);
            string thongbao = "Thêm mới thành công.";
            return RedirectToAction("DangTinTimMo", new { thongbao = thongbao });
        }
        [ValidateInput(false)]
        public ActionResult GridView2Partial()
        {
            nguoiduado_dbEntities db = new nguoiduado_dbEntities();
            List<TBL_DanhSachLietSi> Model = new List<TBL_DanhSachLietSi>();
            Model = db.TBL_DanhSachLietSi.ToList();
            return PartialView("_GridView2Partial", Model);
        }
    }
}