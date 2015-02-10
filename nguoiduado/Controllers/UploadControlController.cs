using DevExpress.Web;
using nguoiduado.Code;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace WebTinTuc.Controllers
{
    public class UploadControlController : Controller
    {
        //
        // GET: /UploadControl/

        //  public override string Name { get { return "UploadControl"; } }

        //public ActionResult Index()
        //{
        //    return MultiFileUpload();
        //}

        //protected override void Execute(System.Web.Routing.RequestContext requestContext)
        //{
        //    var binder = (DevExpressEditorsBinder)ModelBinders.Binders.DefaultBinder;
        //    binder.UploadControlBinderSettings.ValidationSettings = UploadControlDemosHelper.ValidationSettings;
        //    var actionName = (string)requestContext.RouteData.Values["Action"];
        //    switch (actionName)
        //    {
        //        case "CallbacksImageUpload":
        //            binder.UploadControlBinderSettings.FileUploadCompleteHandler = UploadControlDemosHelper.ucCallbacks_FileUploadComplete;
        //            break;
        //        case "MultiSelectionImageUpload":
        //            binder.UploadControlBinderSettings.FileUploadCompleteHandler = UploadControlDemosHelper.ucMultiSelection_FileUploadComplete;
        //            break;
        //    }
        //    base.Execute(requestContext);
        //}
    }

    public class UploadControlDemosHelper
    {
        public const string UploadDirectory = "~/Content/UploadControl/UploadFolder/";
        public const string ThumbnailFormat = "Thumbnail{0}{1}";

        public static readonly DevExpress.Web.UploadControlValidationSettings UploadControlValidationSettings = new DevExpress.Web.UploadControlValidationSettings
        {
            AllowedFileExtensions = new[] { ".jpg", ".jpeg", ".jpe", ".gif" },
            MaxFileSize = 20971520
        };

        public static List<string> Files
        {
            get
            {
                var storage = HttpContext.Current.Session["Storage"] as UploadControlFilesStorage;
                return storage != null ? storage.Files : new List<string>();
            }
        }
        public static int FileInputCount
        {
            get
            {
                var storage = HttpContext.Current.Session["Storage"] as UploadControlFilesStorage;
                return storage != null ? storage.FileInputCount : 2;
            }
        }

        public static void AddImagesToCollection(UploadedFile[] files)
        {
            var storage = HttpContext.Current.Session["Storage"] as UploadControlFilesStorage;
            if (storage == null) return;
            foreach (UploadedFile item in files)
            {
                if (item.FileBytes.Length <= 0 || !item.IsValid) continue;
                if (storage.Files.Contains(item.FileName)) continue;
                string filePath = UploadDirectory +
                                  string.Format(ThumbnailFormat, storage.Files.Count, Path.GetExtension(item.FileName));
                item.SaveAs(HttpContext.Current.Request.MapPath(filePath));
                storage.Files.Add(item.FileName);
            }
            storage.FileInputCount = files.Length;
        }

        public static void ClearImageCollection()
        {
            var storage = HttpContext.Current.Session["Storage"] as UploadControlFilesStorage;
            if (storage != null)
                storage.Files.Clear();
        }

        public static void ucCallbacks_FileUploadComplete(object sender, FileUploadCompleteEventArgs e)
        {
            if (!e.UploadedFile.IsValid) return;
            string resultFilePath = UploadDirectory + string.Format(ThumbnailFormat, "", Path.GetExtension(e.UploadedFile.FileName));
            using (Image original = Image.FromStream(e.UploadedFile.FileContent))
            using (Image thumbnail = PhotoUtils.Inscribe(original, 100))
            {
                PhotoUtils.SaveToJpeg(thumbnail, HttpContext.Current.Request.MapPath(resultFilePath));
            }
            var urlResolver = sender as IUrlResolutionService;
            if (urlResolver != null)
                e.CallbackData = urlResolver.ResolveClientUrl(resultFilePath) + "?refresh=" + Guid.NewGuid().ToString();
        }

        public static void ucMultiSelection_FileUploadComplete(object sender, FileUploadCompleteEventArgs e)
        {
            string resultFilePath = UploadDirectory + e.UploadedFile.FileName;
            e.UploadedFile.SaveAs(HttpContext.Current.Request.MapPath(resultFilePath));

            UploadingUtils.RemoveFileWithDelay(e.UploadedFile.FileName, HttpContext.Current.Request.MapPath(resultFilePath), 5);

            var urlResolver = sender as IUrlResolutionService;
            if (urlResolver == null) return;
            string file = string.Format("{0} ({1}) {2}KB", e.UploadedFile.FileName, e.UploadedFile.ContentType,
                                        e.UploadedFile.ContentLength / 1024);
            string url = urlResolver.ResolveClientUrl(resultFilePath);
            e.CallbackData = file + "|" + url;
        }
    }

    public class UploadControlFilesStorage
    {
        readonly List<string> _files;

        public UploadControlFilesStorage()
        {
            _files = new List<string>();
            FileInputCount = 2;
        }

        public int FileInputCount { get; set; }
        public List<string> Files { get { return _files; } }

    }
}
