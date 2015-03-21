using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using nguoiduado.Models;
using DevExpress.Web.Mvc;

namespace nguoiduado.Controllers
{
    public class DanhMucNghiaTrangController : Controller
    {
        public ActionResult Index()
        {
            // DXCOMMENT: Pass a data model for GridView
            return View();
        }

        nguoiduado.Models.nguoiduado_dbEntities db = new nguoiduado.Models.nguoiduado_dbEntities();
        public class DanhMucNT
        {
            public Decimal ID { get; set; }
            public string TenDiaPhuong { get; set; }
            public string TenNghiaTrang { get; set; }
        }
        [ValidateInput(false)]
        public ActionResult GridViewPartial()
        {
            var model = (from D in db.TBL_DanhMucNghiaTrang
                         join x in db.TBL_DiaPhuong on D.MaDiaPhuong equals x.MaDiaPhuong
                         into t
                         from y in t.DefaultIfEmpty()
                         orderby D.TenNghiaTrang ascending
                         select new DanhMucNT
                         {
                             ID = D.ID,
                             TenDiaPhuong = y.TenDiaPhuong,
                             TenNghiaTrang = D.TenNghiaTrang
                         }).ToList();
            return PartialView("_GridViewPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult GridViewPartialAddNew(nguoiduado.Models.TBL_DanhMucNghiaTrang item)
        {
            var model = db.TBL_DanhMucNghiaTrang;
            if (ModelState.IsValid)
            {
                try
                {
                    model.Add(item);
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            return PartialView("_GridViewPartial", model.ToList());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult GridViewPartialUpdate(nguoiduado.Models.TBL_DanhMucNghiaTrang item)
        {
            var model = db.TBL_DanhMucNghiaTrang;
            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = model.FirstOrDefault(it => it.ID == item.ID);
                    if (modelItem != null)
                    {
                        this.UpdateModel(modelItem);
                        db.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            return PartialView("_GridViewPartial", model.ToList());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult GridViewPartialDelete(System.Int32 ID)
        {
            var model = db.TBL_DanhMucNghiaTrang;
            if (ID >= 0)
            {
                try
                {
                    var item = model.FirstOrDefault(it => it.ID == ID);
                    if (item != null)
                        model.Remove(item);
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }

            var model2 = (from D in db.TBL_DanhMucNghiaTrang
                         join x in db.TBL_DiaPhuong on D.MaDiaPhuong equals x.MaDiaPhuong
                         into t
                         from y in t.DefaultIfEmpty()
                         orderby D.TenNghiaTrang ascending
                         select new DanhMucNT
                         {
                             ID = D.ID,
                             TenDiaPhuong = y.TenDiaPhuong,
                             TenNghiaTrang = D.TenNghiaTrang
                         }).ToList();
            return PartialView("_GridViewPartial", model2);
        }

        public ActionResult AddNewNghiaTrang()
        {
            TBL_DanhMucNghiaTrang nghiatrang = new TBL_DanhMucNghiaTrang();

            if (Request["thongbao"] != null)
            {
                ViewBag.success = Request["thongbao"];
            }
            if (Request["datontai"] != null)
            {
                ViewBag.success = Request["datontai"];
            }

            return View(nghiatrang);

        }
        [HttpPost]
        public ActionResult AddNewNghiaTrang(FormCollection collection, TBL_DanhMucNghiaTrang nghiatrang)
        {
            nghiatrang.NoiDungMoTa = HttpUtility.HtmlDecode(collection["FeaturesHtml_Html"]);

            string MaDiaPhuong = collection["ComboBoxDiaPhuong_VI"];
            if (!string.IsNullOrEmpty(MaDiaPhuong))
            { 
            nghiatrang.MaDiaPhuong = Convert.ToDecimal( MaDiaPhuong);
            }
            DanhMucNghiaTrangModel NghiaTrangModel = new DanhMucNghiaTrangModel();
            bool result = NghiaTrangModel.AddNewNghiaTrang(nghiatrang);

            // Nếu tồn tại mã thì đưa ra câu thông báo
            if (!result)
            {
                ViewBag.thongbao = "Thêm mới thất bại";
                return View(nghiatrang);
            }
            else
            {

                return RedirectToAction("AddNewNghiaTrang");
            }


        }
        public ActionResult EditNghiaTrang(string ID)
        {
            decimal dID = 0;
            decimal.TryParse(ID, out dID);
            DanhMucNghiaTrangModel NghiaTrangModel = new DanhMucNghiaTrangModel();
            var nghiatrangdtl = NghiaTrangModel.GetNghiaTrangByID(dID);
            ViewBag.DiaPhuongSelect = nghiatrangdtl.MaDiaPhuong;
            ViewBag.NoiDung = nghiatrangdtl.NoiDungMoTa;
            return View(nghiatrangdtl);
        }
        public ActionResult ComboBox3Partial()
        {
            // return PartialView("_ComboBox3Partial");
            ModelView md = new ModelView();
            return PartialView("_ComboBox3Partial", md.nguoiduadodb.TBL_DiaPhuong.ToList());
        }
    }
}