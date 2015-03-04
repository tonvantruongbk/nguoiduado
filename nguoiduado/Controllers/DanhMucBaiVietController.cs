using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using nguoiduado.Models;
using DevExpress.Web.Mvc;
using System.Data;
using WebTinTuc.Controllers;
using nguoiduado.Code;
using DevExpress.Web.ASPxHtmlEditor;
using DevExpress.Utils;
using DevExpress.Web;
using System.ComponentModel;
using System.Web.UI.WebControls;
using nguoiduado.Models;

namespace nguoiduado.Controllers
{
    public class DanhMucBaiVietController : Controller
    {
        public ActionResult Index()
        {
            // DXCOMMENT: Pass a data model for GridView
            return View();
        }

        nguoiduado.Models.nguoiduado_dbEntities db = new nguoiduado.Models.nguoiduado_dbEntities();

        [ValidateInput(false)]
        public ActionResult GridView1Partial()
        {

            DanhMucBaiVietModel DMModel = new DanhMucBaiVietModel();
            return PartialView("_GridView1Partial", DMModel.GetAllBaiViet());
        }
        public class ListMenuKT
        {
            DanhMucMenuModel menu = new DanhMucMenuModel();
            public DataTable Menu { get { return menu.MenuDataTable(); } }
        }
        public ActionResult AddNewBaiViet()
        {
            TBL_NoiDung noidung = new TBL_NoiDung();
            var menuModel = new ListMenuKT();
            ViewData["MenuList"] = menuModel.Menu;
            //ViewData["ToChucCapDo"] = tccdModel.GetTBL_ToChuc_CapDos();
            if (Request["thongbao"] != null)
            {
                ViewBag.success = Request["thongbao"];
            }
            if (Request["datontai"] != null)
            {
                ViewBag.success = Request["datontai"];
            }

            Session["Storage"] = new UploadControlFilesStorage();

            var tendangnhap = "";
            var httpCookie1 = CookiesHelper.GetHttpCookie(Utils.Constants.TenDangNhap);
            if (httpCookie1 != null)
                tendangnhap = httpCookie1.Value;
            noidung.NguoiNhap = tendangnhap;

            decimal noidungcheck = 0;

            //noidungcheck = noidung.MaNoiDung;
            noidung.MaSo = DateTime.Now.Millisecond.ToString() + DateTime.Now.Second.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + noidungcheck + noidung.MaNoiDung + 1;
            noidung.NgayNhap = DateTime.Now;

            return View(noidung);

        }
        [HttpPost]
        [ValidateInput(false)]

        public ActionResult AddNewBaiViet(FormCollection collection, TBL_NoiDung noiDung)
        {
            #region Lưu Nội dung
            var mess = "";
            var menuModel = new ListMenuKT();

            //nguoiduado.Models.DanhMucBaoVietModel
            nguoiduado.Models.DanhMucBaiVietModel BVModel = new nguoiduado.Models.DanhMucBaiVietModel();

            ViewData["MenuList"] = menuModel.Menu;
            var MaSo = collection["maso"];
            if (!string.IsNullOrEmpty(MaSo))
            {
                noiDung.MaSo = MaSo;
            }
            var TieuDe = collection["TieuDe"];
            if (!string.IsNullOrEmpty(TieuDe))
            {
                noiDung.TieuDe = TieuDe;
            }
            var TuKhoaTag = collection["TuKhoaTag"];
            if (!string.IsNullOrEmpty(TuKhoaTag))
            {
                noiDung.TuKhoaTag = TuKhoaTag;
            }
            //Người nhập
            var tendangnhap = "";
            var httpCookie1 = CookiesHelper.GetHttpCookie(Utils.Constants.TenDangNhap);
            if (httpCookie1 != null)
                tendangnhap = httpCookie1.Value;
            noiDung.NguoiNhap = tendangnhap;
            noiDung.NgayNhap = DateTime.Now;

            var NguonTin = collection["NguonTin"];
            if (!string.IsNullOrEmpty(NguonTin))
            {
                noiDung.NguonTin = NguonTin;
            }
            noiDung.NoiDung = HttpUtility.HtmlDecode(collection["FeaturesHtml_Html"]);

            string KhaiThac = collection["ComboLoaiBaiViet"];
            #endregion

            if (ModelState.IsValid)
            {
                if (httpCookie1 != null)
                    tendangnhap = httpCookie1.Value;
                noiDung.NguoiNhap = tendangnhap;

                bool result = false;

                noiDung.MenuID = Convert.ToInt32(collection["ComboBoxMenu_VI"].ToString());
                result = BVModel.AddNewBaiViet(noiDung);

                // Nếu tồn tại mã thì đưa ra câu thông báo
                if (!result)
                {
                    ViewBag.thongbao = "Thêm mới thất bại";
                    return View(noiDung);
                }
                else
                {
                    mess = "Thêm mới thành công";
                    CacheHelper _cacheHelper = new CacheHelper();
                    _cacheHelper.ClearCaches();
                    return RedirectToAction("AddNewBaiViet", new { thongbao = mess });
                }

            }
            return View();
        }

        public ActionResult EditBaiViet(string MaNoiDung)
        {
            var menuModel = new ListMenuKT();
            DanhMucBaiVietModel BVModel = new DanhMucBaiVietModel();
            decimal dMaNoiDung = 0;
            decimal.TryParse(MaNoiDung, out dMaNoiDung);
            var Menudtl = BVModel.GetBaiVietByID(dMaNoiDung);
            if (Request["thongbao"] != null)
            {
                ViewBag.success = Request["thongbao"].ToString();
            }
            ViewBag.MenuSelect = Menudtl.MenuID;
            ViewData["DanhMucMenu"] = menuModel.Menu;
            ViewBag.TrichDan = Menudtl.TrichDan;
            ViewBag.NoiDung = Menudtl.NoiDung;
            ViewBag.TenMenu = Menudtl.TBL_MenuDanhMuc.TenMenu;
            //Get quyen phat hanh bai viet
            var tendangnhap = "";
            var httpCookie1 = CookiesHelper.GetHttpCookie(Utils.Constants.TenDangNhap);
            if (httpCookie1 != null)
                tendangnhap = httpCookie1.Value;
            ViewBag.TenMenu = Menudtl.TBL_MenuDanhMuc.TenMenu;
            return View(Menudtl);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditBaiViet(FormCollection collection, TBL_NoiDung noiDung)
        {
            #region Lưu Nội dung
            var mess = "";
            var menuModel = new ListMenuKT();
            nguoiduado.Models.DanhMucBaiVietModel BVModel = new nguoiduado.Models.DanhMucBaiVietModel();
            var TieuDe = collection["TieuDe"];
            if (!string.IsNullOrEmpty(TieuDe))
            {
                noiDung.TieuDe = TieuDe;
            }
            var TuKhoaTag = collection["TuKhoaTag"];
            if (!string.IsNullOrEmpty(TuKhoaTag))
            {
                noiDung.TuKhoaTag = TuKhoaTag;
            }
            //Người nhập
            var tendangnhap = "";
            var httpCookie1 = CookiesHelper.GetHttpCookie(Utils.Constants.TenDangNhap);
            if (httpCookie1 != null)
                tendangnhap = httpCookie1.Value;
            noiDung.NguoiNhap = tendangnhap;
            noiDung.NgayNhap = DateTime.Now;
            var NguonTin = collection["NguonTin"];
            if (!string.IsNullOrEmpty(NguonTin))
            {
                noiDung.NguonTin = NguonTin;
            }
            noiDung.NoiDung = HttpUtility.HtmlDecode(collection["FeaturesHtml_Html"]);
            string KhaiThac = collection["ComboLoaiBaiViet"];
            #endregion

            if (ModelState.IsValid)
            {
                if (httpCookie1 != null)
                    tendangnhap = httpCookie1.Value;
                noiDung.NguoiNhap = tendangnhap;
                bool result = false;
                noiDung.MenuID = Convert.ToInt32(collection["ComboBox_VI"].ToString());
                result = BVModel.UpdateBaiViet(noiDung);
                // Nếu tồn tại mã thì đưa ra câu thông báo
                if (!result)
                {
                    ViewBag.thongbao = "Cập nhật thất bại";
                    return View(noiDung);
                }
                else
                {
                    mess = "Cập nhật thành công";
                    CacheHelper _cacheHelper = new CacheHelper();
                    _cacheHelper.ClearCaches();
                    return RedirectToAction("EditBaiViet", new { MaNoiDung = noiDung.MaNoiDung, thongbao = mess });
                 
                }

            }
            return View();
        }
        public ActionResult DeleteBaiViet(decimal MaNoiDung)
        {
            DanhMucBaiVietModel bvModel = new Models.DanhMucBaiVietModel();
            Boolean checkdel = false;
            checkdel=bvModel.DeleteBaiViet(MaNoiDung);
            var strReturn = "Delete thành công";
            return RedirectToAction("Index");
        }
        public string CompareString(string sFirst, string sSecond)
        {
            var dmp = new DiffMatchPatch();
            var dMain = dmp.DiffMain(sFirst ?? string.Empty, sSecond ?? string.Empty);
            dmp.DiffCleanupSemantic(dMain);
            var result = dmp.ReDiffPrettyHtml(dMain);
            return result;
        }
        private static int _d2;
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
        public ActionResult FeaturesPartial()
        {
            return PartialView("FeaturesPartial");
        }
        public ActionResult FeaturesImageUpload()
        {
            HtmlEditorExtension.SaveUploadedImage("FeaturesHtml", HtmlEditorFeaturesDemosHelper.ImageUploadValidationSettings, HtmlEditorFeaturesDemosHelper.UploadDirectory);
            return null;
        }
        public ActionResult FeaturesImageSelectorUpload()
        {
            HtmlEditorExtension.SaveUploadedImage("FeaturesHtml", HtmlEditorFeaturesDemosHelper.ImageSelectorSettings);
            return null;
        }



        protected override void Execute(System.Web.Routing.RequestContext requestContext)
        {
            var binder = (DevExpressEditorsBinder)ModelBinders.Binders.DefaultBinder;
            var actionName = (string)requestContext.RouteData.Values["Action"];
            switch (actionName)
            {
                //case "Features":
                //    binder.HtmlEditorBinderSettings.HtmlEditingSettings = new ASPxHtmlEditorHtmlEditingSettings()
                //    {
                //        AllowFormElements = GetValueFromRequest<bool>("AllowFormElements"),
                //        AllowIFrames = GetValueFromRequest<bool>("AllowIFrames"),
                //        AllowScripts = GetValueFromRequest<bool>("AllowScripts"),
                //        EnterMode = GetValueFromRequest<HtmlEditorEnterMode>("EnterMode"),
                //        UpdateBoldItalic = GetValueFromRequest<bool>("UpdateBoldItalic"),
                //        UpdateDeprecatedElements = GetValueFromRequest<bool>("UpdateDeprecatedElements")
                //    };
                //    break;
                //case "Validation":
                //    binder.HtmlEditorBinderSettings.ValidationSettings = new HtmlEditorValidationSettings();
                //    binder.HtmlEditorBinderSettings.ValidationSettings.RequiredField.IsRequired = true;
                //    binder.HtmlEditorBinderSettings.ValidationHandler = OnValidation;
                //    break;
            }
            base.Execute(requestContext);
        }
        static T GetValueFromRequest<T>(string key)
        {
            var request = System.Web.HttpContext.Current.Request;
            if (!request.Params.AllKeys.Contains(key))
                return default(T);

            string rawValue = request.Form.GetValues(key).FirstOrDefault();
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
            return converter != null && converter.IsValid(rawValue) ? (T)converter.ConvertFrom(rawValue) : default(T);
        }

        static void OnValidation(object sender, HtmlEditorValidationEventArgs e)
        {
            const int MaxLength = 50;
            string CustomErrorText = string.Format("Custom validation fails because the HTML content&prime;s length exceeds {0} characters.", MaxLength);

            if (e.Html.Length > MaxLength)
            {
                e.IsValid = false;
                e.ErrorText = CustomErrorText;
            }
        }

        public ActionResult ComboBoxPartial()
        {
            ModelView md = new ModelView();
            return PartialView("_ComboBoxPartial", md.nguoiduadodb.TBL_MenuDanhMuc.ToList());
        }
    }

    public class HtmlEditorFeaturesDemoOptions
    {
        public HtmlEditorFeaturesDemoOptions()
        {
            UpdateDeprecatedElements = true;
            UpdateBoldItalic = true;
            EnterMode = HtmlEditorEnterMode.Default;
            AllowContextMenu = DefaultBoolean.True;
            AllowDesignView = true;
            AllowHtmlView = true;
            AllowPreview = true;
        }

        public bool AllowScripts { get; set; }
        public bool AllowIFrames { get; set; }
        public bool AllowFormElements { get; set; }
        public bool UpdateDeprecatedElements { get; set; }
        public bool UpdateBoldItalic { get; set; }
        public HtmlEditorEnterMode EnterMode { get; set; }
        public DefaultBoolean AllowContextMenu { get; set; }
        public bool AllowDesignView { get; set; }
        public bool AllowHtmlView { get; set; }
        public bool AllowPreview { get; set; }
        public HtmlEditorView ActiveView { get { return HtmlEditorExtension.GetActiveView("FeaturesHtml"); } }
    }

    public class HtmlEditorModel
    {
        public HtmlEditorModel(string html) : this(html, null) { }
        public HtmlEditorModel(string html, IEnumerable<string> cssFiles)
        {
            Html = html;
            CssFiles = cssFiles;
        }

        public string Html { get; set; }
        public IEnumerable<string> CssFiles { get; set; }
    }

    public class HtmlEditorFeaturesDemosHelper
    {
        public const string ImagesDirectory = "~/uploads";
        public const string ThumbnailsDirectory = "~/Content/HtmlEditor/Thumbnails/";
        public const string UploadDirectory = ImagesDirectory;

        public static readonly UploadControlValidationSettings ImageUploadValidationSettings = new UploadControlValidationSettings
        {
            AllowedFileExtensions = new string[] { ".jpg", ".jpeg", ".jpe", ".gif", ".png" },
            MaxFileSize = 4000000
        };

        static HtmlEditorValidationSettings validationSettings;
        public static HtmlEditorValidationSettings UploadControlValidationSettings
        {
            get
            {
                if (validationSettings == null)
                {
                    validationSettings = new HtmlEditorValidationSettings();
                    validationSettings.RequiredField.IsRequired = true;
                }
                return validationSettings;
            }
        }

        static MVCxHtmlEditorImageSelectorSettings imageSelectorSettings;
        public static HtmlEditorImageSelectorSettings ImageSelectorSettings
        {
            get
            {
                if (imageSelectorSettings == null)
                {
                    imageSelectorSettings = new MVCxHtmlEditorImageSelectorSettings();
                    SetHtmlEditorImageSelectorSettings(imageSelectorSettings);
                }
                return imageSelectorSettings;
            }
        }
        public static MVCxHtmlEditorImageSelectorSettings SetHtmlEditorImageSelectorSettings(MVCxHtmlEditorImageSelectorSettings settingsImageSelector)
        {
            settingsImageSelector.UploadCallbackRouteValues = new { Controller = "BaiViet", Action = "FeaturesImageSelectorUpload" };

            settingsImageSelector.Enabled = true;
            settingsImageSelector.CommonSettings.RootFolder = ImagesDirectory;
            settingsImageSelector.CommonSettings.ThumbnailFolder = ThumbnailsDirectory;
            settingsImageSelector.CommonSettings.AllowedFileExtensions = new string[] { ".jpg", ".jpeg", ".jpe", ".gif", ".png" };
            settingsImageSelector.EditingSettings.AllowCreate = true;
            settingsImageSelector.EditingSettings.AllowDelete = true;
            settingsImageSelector.EditingSettings.AllowMove = true;
            settingsImageSelector.EditingSettings.AllowRename = true;
            settingsImageSelector.UploadSettings.Enabled = true;
            settingsImageSelector.FoldersSettings.ShowLockedFolderIcons = true;

            settingsImageSelector.PermissionSettings.AccessRules.Add(
                new FileManagerFolderAccessRule
                {
                    Path = "",
                    Upload = Rights.Deny
                },
                new FileManagerFolderAccessRule
                {
                    Path = "Upload",
                    Upload = Rights.Allow
                });
            return settingsImageSelector;
        }

        public static HtmlEditorSettings SetHtmlEditorExportSettings(HtmlEditorSettings settings)
        {
            settings.Name = "heImportExport";
            settings.CallbackRouteValues = new { Controller = "BaiViet", Action = "ImportExportPartial" };
            settings.ExportRouteValues = new { Controller = "BaiViet", Action = "ExportTo" };
            settings.Width = Unit.Percentage(100);

            var toolbar = settings.Toolbars.Add();
            toolbar.Items.Add(new ToolbarUndoButton());
            toolbar.Items.Add(new ToolbarRedoButton());
            toolbar.Items.Add(new ToolbarBoldButton(true));
            toolbar.Items.Add(new ToolbarItalicButton());
            toolbar.Items.Add(new ToolbarUnderlineButton());
            toolbar.Items.Add(new ToolbarStrikethroughButton());
            toolbar.Items.Add(new ToolbarInsertImageDialogButton(true));
            ToolbarExportDropDownButton saveButton = new ToolbarExportDropDownButton(true);
            saveButton.CreateDefaultItems();
            toolbar.Items.Add(saveButton);

            settings.CssFiles.Add("~/Content/HtmlEditor/DemoCss/Export.css");

            return settings;
        }

    }
}