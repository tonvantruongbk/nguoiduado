using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Caching;

using DevExpress.Web.Internal;

public static class UploadingUtils {
    public static void RemoveFileWithDelay(string key, string fullPath, int delay) {
        if(HttpUtils.GetCache()[key] == null) {
            DateTime absoluteExpiration = DateTime.Now.Add(new TimeSpan(0, delay, 0));
            HttpUtils.GetCache().Insert(key, fullPath, null, absoluteExpiration,
                Cache.NoSlidingExpiration, CacheItemPriority.NotRemovable, new CacheItemRemovedCallback(RemovedCallback));
        }
    }
    public static void RemovedCallback(string key, object value, CacheItemRemovedReason reason) {
        if(File.Exists(value.ToString()))
            File.Delete(value.ToString());
    }
}
