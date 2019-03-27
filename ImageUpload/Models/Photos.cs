using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

namespace ImageUpload.Models
{
    public class Photos
    {
        public List<string> lst = new List<string>();

        public string PhotoLocation { get; set; }
        private DirectoryInfo d;

        public Photos() { }

        //USED TO GET THE FILES FROM SPECIFIED DIRECTORY - lst WILL CONTAIN THE LIST
        public Photos(string PhotoDirectory, string serverPath)
        {
            this.PhotoLocation = PhotoDirectory;
            d = new DirectoryInfo(serverPath);

            FileInfo[] files = d.GetFiles();

            foreach (FileInfo f in files)
            {
                if (f.Name != "Thumbs.db")
                {
                    lst.Add(PhotoLocation.Replace("~", "") + "/" + f.Name);
                }

            }
        }

        public void DeletePhoto(string filePath)
        {
            FileInfo f = new FileInfo(filePath);
            f.Delete();

        }

        public void SavePhoto(HttpPostedFileBase file, string serverPath)
        {
            //CREATE THE BITMAP
            Image bmp = new Bitmap(file.InputStream);
            //RESIZE THE IMAGE
            Bitmap newImg = this.ResizeImage(bmp, 200, 200);
            //GENERATE A UNIQUE FILE NAME
            string filePath = Guid.NewGuid() + Path.GetExtension(file.FileName);
            //GENERATE FILE PATH
            string saveFileName = Path.Combine(serverPath, filePath);
            //SAVE FILE ON NETWORK
            newImg.Save(saveFileName);
            //ADD FILE TO LIST
            lst.Add("/Images/" + filePath);
        }

        private Bitmap ResizeImage(Image image, int width, int height)
        {
            int newWidth = Convert.ToInt32((Double.Parse(image.Width.ToString()) / Double.Parse(image.Height.ToString())) * height);
            int newHeight = Convert.ToInt32((Double.Parse(image.Height.ToString()) / Double.Parse(image.Width.ToString())) * width);

            if (newWidth <= width)
            {
                width = newWidth;
            }
            else if (newHeight <= height)
            {
                height = newHeight;

            }
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.Clamp);

                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

    }
}