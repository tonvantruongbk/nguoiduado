using System;
using System.Drawing;
using System.Linq;
using ImageResizer;
using System.IO;
using System.Web.Hosting;
using nguoiduado.Helper;

namespace nguoiduado.Models
{
    public class PictureModel : ModelView
    {
        private static readonly object SLock = new object();
        public virtual TBL_Picture GetPictureById(int pictureId)
        {
            if (pictureId == 0)
                return null;

            return (from c in nguoiduadodb.TBL_Picture
                    where c.Id == pictureId
                    select c).FirstOrDefault();
        }

        public virtual string GetPictureUrl(int pictureId,
            int targetSize = 0,
            bool showDefaultPicture = true,
            string storeLocation = null,
            PictureType defaultPictureType = PictureType.Entity)
        {
            var picture = GetPictureById(pictureId);
            return GetPictureUrl(picture, targetSize, showDefaultPicture, storeLocation, defaultPictureType);
        }

        public virtual string GetPictureUrl(TBL_Picture picture,
            int targetSize = 0,
            bool showDefaultPicture = true,
            string storeLocation = null,
            PictureType defaultPictureType = PictureType.Entity)
        {
            string url = string.Empty;
            byte[] pictureBinary = null;
            if (picture != null)
                pictureBinary = LoadPictureBinary(picture);
            if (picture == null || pictureBinary == null || pictureBinary.Length == 0)
            {
                if (showDefaultPicture)
                {
                    url = GetDefaultPictureUrl(targetSize, defaultPictureType, storeLocation);
                }
                return url;
            }

            string lastPart = GetFileExtensionFromMimeType(picture.MimeType);
            string thumbFileName;
            if (picture.IsNew)
            {
                DeletePictureThumbs(picture);

                //we do not validate picture binary here to ensure that no exception ("Parameter is not valid") will be thrown
                picture = UpdatePicture(picture.Id,
                    pictureBinary,
                    picture.MimeType,
                    false,
                    false);
            }
            lock (SLock)
            {
                string seoFileName = picture.SeoFilename; // = GetPictureSeName(picture.SeoFilename); //just for sure
                if (targetSize == 0)
                {
                    thumbFileName = !String.IsNullOrEmpty(seoFileName) ?
                        string.Format("{0}_{1}.{2}", picture.Id.ToString("0000000"), seoFileName, lastPart) :
                        string.Format("{0}.{1}", picture.Id.ToString("0000000"), lastPart);
                    var thumbFilePath = GetThumbLocalPath(thumbFileName);
                    if (!File.Exists(thumbFilePath))
                    {
                        File.WriteAllBytes(thumbFilePath, pictureBinary);
                    }
                }
                else
                {
                    thumbFileName = !String.IsNullOrEmpty(seoFileName) ?
                        string.Format("{0}_{1}_{2}.{3}", picture.Id.ToString("0000000"), seoFileName, targetSize, lastPart) :
                        string.Format("{0}_{1}.{2}", picture.Id.ToString("0000000"), targetSize, lastPart);
                    var thumbFilePath = GetThumbLocalPath(thumbFileName);
                    if (!File.Exists(thumbFilePath))
                    {
                        using (var stream = new MemoryStream(pictureBinary))
                        {
                            Bitmap b = null;
                            try
                            {
                                //try-catch to ensure that picture binary is really OK. Otherwise, we can get "Parameter is not valid" exception if binary is corrupted for some reasons
                                b = new Bitmap(stream);
                            }
                            catch (ArgumentException)
                            {
                                //_logger.Error(string.Format("Error generating picture thumb. ID={0}", picture.Id), exc);
                            }
                            if (b == null)
                            {
                                //bitmap could not be loaded for some reasons
                                return url;
                            }

                            var newSize = CalculateDimensions(b.Size, targetSize);

                            var destStream = new MemoryStream();
                            ImageBuilder.Current.Build(b, destStream, new ResizeSettings
                            {
                                Width = newSize.Width,
                                Height = newSize.Height,
                                Scale = ScaleMode.Both,
                                Quality = 80 //_mediaSettings.DefaultImageQuality
                            });
                            var destBinary = destStream.ToArray();
                            File.WriteAllBytes(thumbFilePath, destBinary);

                            b.Dispose();
                        }
                    }
                }
            }
            url = GetThumbUrl(thumbFileName, storeLocation);
            return url;
        }

        public virtual TBL_Picture UpdatePicture(int pictureId, byte[] pictureBinary, string mimeType,
             bool isNew, bool validateBinary = true)
        {
            mimeType = CommonHelper.EnsureNotNull(mimeType);
            mimeType = CommonHelper.EnsureMaximumLength(mimeType, 20);


            if (validateBinary)
                pictureBinary = ValidatePicture(pictureBinary, mimeType);

            var picture = GetPictureById(pictureId);
            if (picture == null)
                return null;

            //delete old thumbs if a picture has been changed
            //if (seoFilename != picture.SeoFilename)
            //    DeletePictureThumbs(picture);

            picture.PictureBinary = (StoreInDb ? pictureBinary : new byte[0]);
            picture.MimeType = mimeType;
            //picture.SeoFilename = seoFilename;
            picture.IsNew = isNew;

            //_pictureRepository.Update(picture);

            //if (!this.StoreInDb)
            //    SavePictureInFile(picture.Id, pictureBinary, mimeType);

            ////event notification
            //_eventPublisher.EntityUpdated(picture);

            return picture;
        }

        public virtual string GetDefaultPictureUrl(int targetSize = 0,
            PictureType defaultPictureType = PictureType.Entity,
            string storeLocation = null)
        {
            string defaultImageFileName;
            switch (defaultPictureType)
            {
                case PictureType.Entity:
                    defaultImageFileName = "default-image.gif";//GetSettingByKey("Media.DefaultImageName", "default-image.gif");
                    break;
                case PictureType.Avatar:
                    defaultImageFileName = "default-avatar.jpg";//;GetSettingByKey("Media.Customer.DefaultAvatarImageName", "default-avatar.jpg");
                    break;
                default:
                    defaultImageFileName = "default-image.gif"; GetSettingByKey("Media.DefaultImageName", "default-image.gif");
                    break;
            }

            string filePath = GetPictureLocalPath(defaultImageFileName);
            if (!File.Exists(filePath))
            {
                return "";
            }
            if (targetSize == 0)
            {
                const string url = "/content/uploads/images/Thumbs/default-image_100.gif"; // +defaultImageFileName;
                return url;
            }
            else
            {
                string fileExtension = Path.GetExtension(filePath);
                string thumbFileName = string.Format("{0}_{1}{2}",
                    Path.GetFileNameWithoutExtension(filePath),
                    targetSize,
                    fileExtension);
                var thumbFilePath = GetThumbLocalPath(thumbFileName);
                if (!File.Exists(thumbFilePath))
                {
                    using (var b = new Bitmap(filePath))
                    {
                        var newSize = CalculateDimensions(b.Size, targetSize);

                        var destStream = new MemoryStream();
                        ImageBuilder.Current.Build(b, destStream, new ResizeSettings
                        {
                            Width = newSize.Width,
                            Height = newSize.Height,
                            Scale = ScaleMode.Both,
                            Quality = 80 //_mediaSettings.DefaultImageQuality
                        });
                        var destBinary = destStream.ToArray();
                        File.WriteAllBytes(thumbFilePath, destBinary);
                    }
                }
                var url = GetThumbUrl(thumbFileName, storeLocation);
                return url;
            }
        }

        public virtual TBL_Picture InsertPicture(byte[] pictureBinary, string mimeType, string seoFilename,
            bool isNew, bool validateBinary = true)
        {
            mimeType = CommonHelper.EnsureNotNull(mimeType);
            mimeType = CommonHelper.EnsureMaximumLength(mimeType, 20);

            seoFilename = CommonHelper.EnsureMaximumLength(seoFilename, 100);

            if (validateBinary)
                pictureBinary = ValidatePicture(pictureBinary, mimeType);

            var picture = new TBL_Picture
            {
                PictureBinary = StoreInDb ? pictureBinary : new byte[0],
                MimeType = mimeType,
                SeoFilename = seoFilename,
                IsNew = isNew,
            };
            nguoiduadodb.TBL_Picture.Add(picture);
            nguoiduadodb.SaveChanges();

            if (!StoreInDb)
                SavePictureInFile(picture.Id, pictureBinary, mimeType);

            //event notification
            //_eventPublisher.EntityInserted(picture);

            return picture;
        }
        #region "Private method"
        protected virtual void SavePictureInFile(int pictureId, byte[] pictureBinary, string mimeType)
        {
            string lastPart = GetFileExtensionFromMimeType(mimeType);
            string fileName = string.Format("{0}_0.{1}", pictureId.ToString("0000000"), lastPart);
            File.WriteAllBytes(GetPictureLocalPath(fileName), pictureBinary);
        }
        protected virtual byte[] ValidatePicture(byte[] pictureBinary, string mimeType)
        {
            var destStream = new MemoryStream(); ImageBuilder.Current.Build(pictureBinary, destStream, new ResizeSettings
            {
                MaxWidth = 1280,
                MaxHeight = 1280,
                Quality = 80
            });
            return destStream.ToArray();
        }

        protected virtual void DeletePictureThumbs(TBL_Picture picture)
        {
            string filter = string.Format("{0}*.*", picture.Id.ToString("0000000"));
            var thumbDirectoryPath = MapPath("~/Content/uploads/Images/Thumbs");
            string[] currentFiles = Directory.GetFiles(thumbDirectoryPath, filter, SearchOption.AllDirectories);
            foreach (string currentFileName in currentFiles)
            {
                var thumbFilePath = GetThumbLocalPath(currentFileName);
                File.Delete(thumbFilePath);
            }
        }

        protected virtual string GetThumbLocalPath(string thumbFileName)
        {
            var thumbsDirectoryPath = MapPath("~/content/uploads/images/thumbs");
            //
            //if (_mediaSettings.MultipleThumbDirectories)
            //{
            //    //get the first two letters of the file name
            //    var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(thumbFileName);
            //    if (fileNameWithoutExtension != null && fileNameWithoutExtension.Length > MULTIPLE_THUMB_DIRECTORIES_LENGTH)
            //    {
            //        var subDirectoryName = fileNameWithoutExtension.Substring(0, MULTIPLE_THUMB_DIRECTORIES_LENGTH);
            //        thumbsDirectoryPath = Path.Combine(thumbsDirectoryPath, subDirectoryName);
            //        if (!System.IO.Directory.Exists(thumbsDirectoryPath))
            //        {
            //            System.IO.Directory.CreateDirectory(thumbsDirectoryPath);
            //        }
            //    }
            //}
            var thumbFilePath = Path.Combine(thumbsDirectoryPath, thumbFileName);
            return thumbFilePath;
        }

        protected virtual string GetThumbUrl(string thumbFileName, string storeLocation = null)
        {
            //storeLocation = !String.IsNullOrEmpty(storeLocation)
            //                        ? storeLocation
            //                        : _webHelper.GetStoreLocation();
            //var url = storeLocation + "content/images/thumbs/";

            var url = "/content/uploads/images/thumbs/";
            //
            //if (_mediaSettings.MultipleThumbDirectories)
            //{
            //    //get the first two letters of the file name
            //    var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(thumbFileName);
            //    if (fileNameWithoutExtension != null && fileNameWithoutExtension.Length > MULTIPLE_THUMB_DIRECTORIES_LENGTH)
            //    {
            //        var subDirectoryName = fileNameWithoutExtension.Substring(0, MULTIPLE_THUMB_DIRECTORIES_LENGTH);
            //        url = url + subDirectoryName + "/";
            //    }
            //}

            url = url + thumbFileName;
            return url;
        }

        protected virtual Size CalculateDimensions(Size originalSize, int targetSize,
            ResizeType resizeType = ResizeType.LongestSide, bool ensureSizePositive = true)
        {
            var newSize = new Size();
            switch (resizeType)
            {
                case ResizeType.LongestSide:
                    if (originalSize.Height > originalSize.Width)
                    {
                        // portrait 
                        newSize.Width = (int)(originalSize.Width * (targetSize / (float)originalSize.Height));
                        newSize.Height = targetSize;
                    }
                    else
                    {
                        // landscape or square
                        newSize.Height = (int)(originalSize.Height * (targetSize / (float)originalSize.Width));
                        newSize.Width = targetSize;
                    }
                    break;
                case ResizeType.Width:
                    newSize.Height = (int)(originalSize.Height * (targetSize / (float)originalSize.Width));
                    newSize.Width = targetSize;
                    break;
                case ResizeType.Height:
                    newSize.Width = (int)(originalSize.Width * (targetSize / (float)originalSize.Height));
                    newSize.Height = targetSize;
                    break;
                default:
                    throw new Exception("Not supported ResizeType");
            }

            if (ensureSizePositive)
            {
                if (newSize.Width < 1)
                    newSize.Width = 1;
                if (newSize.Height < 1)
                    newSize.Height = 1;
            }

            return newSize;
        }

        public virtual byte[] LoadPictureBinary(TBL_Picture picture)
        {
            return LoadPictureBinary(picture, StoreInDb);
        }

        protected virtual byte[] LoadPictureBinary(TBL_Picture picture, bool fromDb)
        {
            if (picture == null)
                throw new ArgumentNullException("picture");

            byte[] result = fromDb ? picture.PictureBinary : LoadPictureFromFile(picture.Id, picture.MimeType);
            return result;
        }

        protected virtual byte[] LoadPictureFromFile(int pictureId, string mimeType)
        {
            string lastPart = GetFileExtensionFromMimeType(mimeType);
            string fileName = string.Format("{0}_0.{1}", pictureId.ToString("0000000"), lastPart);
            var filePath = GetPictureLocalPath(fileName);
            if (!File.Exists(filePath))
                return new byte[0];
            return File.ReadAllBytes(filePath);
        }

        protected virtual string GetFileExtensionFromMimeType(string mimeType)
        {
            if (mimeType == null)
                return null;

            //also see System.Web.MimeMapping for more mime types

            string[] parts = mimeType.Split('/');
            string lastPart = parts[parts.Length - 1];
            switch (lastPart)
            {
                case "pjpeg":
                    lastPart = "jpg";
                    break;
                case "x-png":
                    lastPart = "png";
                    break;
                case "x-icon":
                    lastPart = "ico";
                    break;
            }
            return lastPart;
        }

        protected virtual string GetPictureLocalPath(string fileName)
        {
            var imagesDirectoryPath = MapPath("~/Content/Uploads/Images");// _webHelper.MapPath("~/content/images/");
            var filePath = Path.Combine(imagesDirectoryPath, fileName);
            return filePath;
        }

        protected virtual string MapPath(string path)
        {
            if (HostingEnvironment.IsHosted)
            {
                //hosted
                return HostingEnvironment.MapPath(path);
            }
            //not hosted. For example, run in unit tests
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            path = path.Replace("~/", "").TrimStart('/').Replace('/', '\\');
            return Path.Combine(baseDirectory, path);
        }

        public virtual bool StoreInDb
        {
            get
            {
                //return _settingService.GetSettingByKey<bool>("Media.Images.StoreInDB", true);
                return true;
            }
            set
            {
                //check whether it's a new value
                if (StoreInDb != value)
                {
                    //save the new setting value
                    //_settingService.SetSetting<bool>("Media.Images.StoreInDB", value); todo

                    StoreInDb = value;

                    //
                    ////update all picture objects
                    //var pictures = this.GetPictures(0, int.MaxValue);
                    //foreach (var picture in pictures)
                    //{
                    //    var pictureBinary = LoadPictureBinary(picture, !value);

                    //    //delete from file system
                    //    if (value)
                    //        DeletePictureOnFileSystem(picture);

                    //    //just update a picture (all required logic is in UpdatePicture method)
                    //    UpdatePicture(picture.Id,
                    //                  pictureBinary,
                    //                  picture.MimeType,
                    //                  picture.SeoFilename,
                    //                  true,
                    //                  false);
                    //    //we do not validate picture binary here to ensure that no exception ("Parameter is not valid") will be thrown when "moving" pictures
                    //}
                }
            }
        }

        public virtual T GetSettingByKey<T>(string key, T defaultValue = default(T),
            int storeId = 0, bool loadSharedValueIfNotFound = false)
        {
            return defaultValue;
            //
            //var settings = GetAllSettingsCached();
            //key = key.Trim().ToLowerInvariant();
            //if (settings.ContainsKey(key))
            //{
            //    var settingsByKey = settings[key];
            //    var setting = settingsByKey.FirstOrDefault(x => x.StoreId == storeId);

            //    //load shared value?
            //    if (setting == null && storeId > 0 && loadSharedValueIfNotFound)
            //        setting = settingsByKey.FirstOrDefault(x => x.StoreId == 0);

            //    if (setting != null)
            //        return CommonHelper.To<T>(setting.Value);
            //}
        }

        #endregion
    }

    public enum PictureType
    {
        /// <summary>
        /// Entities (products, categories, manufacturers)
        /// </summary>
        Entity = 1,
        /// <summary>
        /// Avatar
        /// </summary>
        Avatar = 10,
    }

    public enum ResizeType
    {
        LongestSide,
        Width,
        Height
    }
}