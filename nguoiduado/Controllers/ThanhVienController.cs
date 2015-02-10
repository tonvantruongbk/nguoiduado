using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DevExpress.Web.Mvc;

namespace nguoiduado.Controllers
{
    public class ThanhVienController : Controller
    {
        // GET: ThanhVien
        public ActionResult Index()
        {
            return View();
        }

        nguoiduado.Models.nguoiduado_dbEntities db = new nguoiduado.Models.nguoiduado_dbEntities();

        [ValidateInput(false)]
        public ActionResult GridViewPartial()
        {
            var model = db.TBL_ThanhVien;
            return PartialView("_GridViewPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult GridViewPartialAddNew(nguoiduado.Models.TBL_ThanhVien item)
        {
            var model = db.TBL_ThanhVien;
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
        public ActionResult GridViewPartialUpdate(nguoiduado.Models.TBL_ThanhVien item)
        {
            var model = db.TBL_ThanhVien;
            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = model.FirstOrDefault(it => it.MaThanhVien == item.MaThanhVien);
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
        public ActionResult GridViewPartialDelete(System.Int32 MaThanhVien)
        {
            var model = db.TBL_ThanhVien;
            if (MaThanhVien >= 0)
            {
                try
                {
                    var item = model.FirstOrDefault(it => it.MaThanhVien == MaThanhVien);
                    if (item != null)
                        model.Remove(item);
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            return PartialView("_GridViewPartial", model.ToList());
        }
    }
}