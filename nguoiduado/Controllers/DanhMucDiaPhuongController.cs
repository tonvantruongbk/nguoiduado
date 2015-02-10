using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using nguoiduado.Models;
using DevExpress.Web.Mvc;

namespace nguoiduado.Controllers
{
    public class DanhMucDiaPhuongController : Controller
    {
        public ActionResult Index()
        {
            // DXCOMMENT: Pass a data model for GridView
            return View();
        }

        nguoiduado.Models.nguoiduado_dbEntities db = new nguoiduado.Models.nguoiduado_dbEntities();

        [ValidateInput(false)]
        public ActionResult GridView2Partial()
        {
            var model = db.TBL_DiaPhuong;
            return PartialView("_GridView2Partial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult GridView2PartialAddNew(nguoiduado.Models.TBL_DiaPhuong item)
        {
            var model = db.TBL_DiaPhuong;
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
            return PartialView("_GridView2Partial", model.ToList());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult GridView2PartialUpdate(nguoiduado.Models.TBL_DiaPhuong item)
        {
            var model = db.TBL_DiaPhuong;
            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = model.FirstOrDefault(it => it.MaDiaPhuong == item.MaDiaPhuong);
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
            return PartialView("_GridView2Partial", model.ToList());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult GridView2PartialDelete(System.Decimal MaDiaPhuong)
        {
            var model = db.TBL_DiaPhuong;
            if (MaDiaPhuong != null)
            {
                try
                {
                    var item = model.FirstOrDefault(it => it.MaDiaPhuong == MaDiaPhuong);
                    if (item != null)
                        model.Remove(item);
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            return PartialView("_GridView2Partial", model.ToList());
        }
    }
}