using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace nguoiduado.Code
{
    public class CookiesHelper
    {
        public CookiesHelper()
        {

        }
        //
        public static string GetCookieValue(string cookieName)
        {
            try
            {
                return HttpContext.Current.Request.Cookies[cookieName].Value;
            }
            catch (Exception ex)
            {
                return String.Empty;
            }

        }

        // get cookie
        public static HttpCookie GetHttpCookie(string cookieName)
        {
            return HttpContext.Current.Request.Cookies[cookieName];
        }

        //  tao 
        public static void CreateCookie(string cookieName, string value, int? expirationDays)
        {
            var cookie = new HttpCookie(cookieName, value);
            if (expirationDays.HasValue)
                cookie.Expires = DateTime.Now.AddDays(expirationDays.Value);
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        //

        public static void CreateCookieSecond(string cookieName, string value, int? expirationSecond)
        {
            var cookie = new HttpCookie(cookieName, value);
            if (expirationSecond.HasValue)
                cookie.Expires = DateTime.Now.AddSeconds(expirationSecond.Value);
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        //

        public static void DeleteCookie(string cookieName)
        {
            var cookie = HttpContext.Current.Request.Cookies[cookieName];
            if (cookie != null)
            {
                cookie.Expires = DateTime.Now.AddDays(-2);
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
        }

        //

        public static bool CookieExists(string cookieName)
        {
            bool exists = false;
            HttpCookie cookie = HttpContext.Current.Request.Cookies[cookieName];
            if (cookie != null)
                exists = true;
            return exists;
        }

        //
        public static bool CookieExpired(string cookieName)
        {
            bool expired = true;
            HttpCookie cookie = HttpContext.Current.Request.Cookies[cookieName];
            if (cookie != null && cookie.Value != null)
                expired = false;
            return expired;
        }
        //
        public static Dictionary<string, string> GetAllCookies()
        {
            var cookies = new Dictionary<string, string>();
            foreach (var key in HttpContext.Current.Request.Cookies.AllKeys)
            {
                cookies.Add(key, HttpContext.Current.Request.Cookies[key].Value);
            }
            return cookies;
        }

        //
        public static void DeleteAllCookies()
        {
            var x = HttpContext.Current.Request.Cookies;
            foreach (HttpCookie cook in x)
            {
                DeleteCookie(cook.Name);
            }
        }
    }
}