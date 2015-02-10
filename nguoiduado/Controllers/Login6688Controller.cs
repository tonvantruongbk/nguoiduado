using System.Globalization;
using nguoiduado.Code;
using nguoiduado.Models;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
namespace nguoiduado.Controllers
{
    public class Login6688Controller : Controller
    {
        // GET: /Login/
        readonly UserModel _userModel = new UserModel();

        public ActionResult Index()
        {
            ViewBag.Title = "Đăng nhập hệ thống Người đưa đò";
            var user = new LoginModel();
            ViewBag.ReturnUrl = !String.IsNullOrEmpty(Request.QueryString["ReturnUrl"])
                                    ? Request.QueryString["ReturnUrl"]
                                    : Request.Url != null ? Request.Url.PathAndQuery : "#";

            if (!ControllerContext.HttpContext.Request.Cookies.AllKeys.Contains(Utils.Constants.TenDangNhap) ||
                !ControllerContext.HttpContext.Request.Cookies.AllKeys.Contains(Utils.Constants.MatKhau))
            {
                return View(user);
            }

            var loginModel = new LoginModel();
            var httpCookie = CookiesHelper.GetHttpCookie(Utils.Constants.TenDangNhap);
            if (httpCookie != null)
                loginModel.UserName = httpCookie.Value;
            var cookie = CookiesHelper.GetHttpCookie(Utils.Constants.MatKhau);
            if (cookie != null)
                loginModel.Password = cookie.Value;
            loginModel.RememberMe = true;

            var userList = (from u in _userModel.nguoiduadodb.TBL_User
                            where ((u.TenDangNhap == loginModel.UserName) && (u.MatKhau == loginModel.Password))
                            select u).FirstOrDefault();
            if (userList != null)
            {
                var tenDangNhap = new HttpCookie(Utils.Constants.TenDangNhap)
                {
                    Value = loginModel.UserName,
                    Expires = DateTime.Now.AddDays(1)
                };
                ControllerContext.HttpContext.Response.Cookies.Add(tenDangNhap);

                var matKhau = new HttpCookie(Utils.Constants.MatKhau)
                {
                    Value = loginModel.Password,
                    Expires = DateTime.Now.AddDays(1)
                };
                ControllerContext.HttpContext.Response.Cookies.Add(matKhau);

                var userID = new HttpCookie(Utils.Constants.UserID)
                {
                    Value = userList.UserID.ToString(CultureInfo.InvariantCulture),
                    Expires = DateTime.Now.AddDays(1)
                };
                ControllerContext.HttpContext.Response.Cookies.Add(userID);

                return RedirectToAction("Index", "QuanTri");
            }
            return View(user);
        }

        [HttpPost]
        public ActionResult Index(LoginModel user, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var tendangnhap = user.UserName;
                var matkhau = Utils.EncodePassword(user.Password, true);

                var userlist = (from u in _userModel.nguoiduadodb.TBL_User
                                where ((u.TenDangNhap == tendangnhap) && (u.MatKhau == matkhau))
                                select u).FirstOrDefault();

                if (userlist != null)
                {

                    var tenDangNhap = new HttpCookie(Utils.Constants.TenDangNhap)
                    {
                        Value = user.UserName,
                        Expires = DateTime.Now.AddDays(1)
                    };
                    ControllerContext.HttpContext.Response.Cookies.Add(tenDangNhap);

                    var matKhau = new HttpCookie(Utils.Constants.MatKhau)
                    {
                        Value = matkhau,
                        Expires = DateTime.Now.AddDays(1)
                    };
                    ControllerContext.HttpContext.Response.Cookies.Add(matKhau);

                    var userid = new HttpCookie(Utils.Constants.UserID)
                    {
                        Value = userlist.UserID.ToString(CultureInfo.InvariantCulture),
                        Expires = DateTime.Now.AddDays(1)
                    };
                    ControllerContext.HttpContext.Response.Cookies.Add(userid);

                    if (Request.QueryString["returnurl"] == null || Request.QueryString["returnurl"].Contains("login"))
                        return RedirectToAction("index", "QuanTri");
                    return Redirect(Request.QueryString["returnurl"]);
                }

                ViewBag.Class = "warrning_false";
                @ViewBag.Status = "đăng nhập không thành công!";
                return View();
            }

            return View();
        }

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            // xóa cookie
            CookiesHelper.DeleteCookie("TenDangNhap");
            CookiesHelper.DeleteCookie("MatKhau");
            CookiesHelper.DeleteCookie("UserID");
            return RedirectToAction("Index", "LoginKTTU6688");
        }

        public ActionResult GetCookie()
        {
            var tendangnhap = "";
            if (ControllerContext.HttpContext.Request.Cookies.AllKeys.Contains(Utils.Constants.TenDangNhap))
            {
                var httpCookie = CookiesHelper.GetHttpCookie(Utils.Constants.TenDangNhap);
                if (httpCookie != null)
                    tendangnhap = httpCookie.Value;
            }

            var matkhau = "";
            if (ControllerContext.HttpContext.Request.Cookies.AllKeys.Contains(Utils.Constants.MatKhau))
            {
                var httpCookie = CookiesHelper.GetHttpCookie(Utils.Constants.MatKhau);
                if (httpCookie != null)
                    matkhau = httpCookie.Value;
            }

            var userID = "";
            if (ControllerContext.HttpContext.Request.Cookies.AllKeys.Contains(Utils.Constants.UserID))
            {
                var httpCookie = CookiesHelper.GetHttpCookie(Utils.Constants.UserID);
                if (httpCookie != null)
                    userID = httpCookie.Value;
            }

            ViewBag.TenDangNhap = tendangnhap;
            ViewBag.MatKhau = matkhau;
            ViewBag.userid = userID;
            return View();
        }

        public ActionResult KhongCoQuyen()
        {
            return View();
        }

    }
}