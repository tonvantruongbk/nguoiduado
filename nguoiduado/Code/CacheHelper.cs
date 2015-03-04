using nguoiduado.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web;

namespace nguoiduado.Code
{
    public class CacheHelper
    {

        public static MemoryCache _cache = MemoryCache.Default;
        //Left trang chủ
        // ViewData["MenuDanhMuc"] = db.TBL_MenuDanhMuc.ToList();
        // ViewData["LienKetWeb"] = db.TBL_LienKet.OrderBy(m => m.TenLienKet).ToList();
        //ViewData["LstNDTop5Menu1"] = LstNDTop5Menu1;
        //ViewData["LstNDTop5Menu2"] = LstNDTop5Menu2;
        //ViewData["LstNDTop10Menu" + i] = CacheHelper._cache["LstNDTop10Menu" + i];
        public CacheHelper()
        {
           

        }

        public void ClearCaches()
        {
           
            _cache.Remove("LienKetWeb");
            _cache.Remove("LstNDTop5Menu1");
            _cache.Remove("LstNDTop5Menu2");

            List<TBL_MenuDanhMuc> Menu = new List<TBL_MenuDanhMuc>();
            Menu = (List<TBL_MenuDanhMuc>)_cache["MenuDanhMuc"];
            
            for (int i = 0; i<Menu.Count;i++)
            {
                if (i != 0 && i != 1)
                {
                    _cache.Remove("LstNDTop10Menu" + i);
                }
             
            }
            _cache.Remove("MenuDanhMuc");

        }
        public void ClearCacheViDeo()
        {
            _cache.Remove("4VideoHienThi");

        }
        public void ClearCacheHinhAnh()
        {
            _cache.Remove("AllHinhAnhRight");
        }
        //

    }
}