using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ImageUpload.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UploadFiles(IEnumerable<HttpPostedFileBase> files)
        {
            List<string> filenames = new List<string>();
            foreach (var file in files)
            {
                Image bmp = new Bitmap(file.InputStream);
                Bitmap newImg = this.ResizeImage(bmp, 200, 200);
                string filePath = Guid.NewGuid() + Path.GetExtension(file.FileName);
                string saveFileName = Path.Combine(Server.MapPath("~/Images"), filePath);
                newImg.Save(saveFileName);
                filenames.Add("Images/" + filePath);
                //file.SaveAs(Path.Combine(Server.MapPath("~/Images"), filePath));
                //here you can write code for save this information in your database if you want
            }

            return Json(filenames);
        }

        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public Bitmap ResizeImage(Image image, int width, int height)
        {
            //http://www.itjungles.com/image-resize-maintain-aspect-ratio-formula.html
            
            int newWidth = Convert.ToInt32((Double.Parse(image.Width.ToString()) / Double.Parse(image.Height.ToString())) * height);
            int newHeight = Convert.ToInt32((Double.Parse(image.Height.ToString()) / Double.Parse(image.Width.ToString())) * width);
           
            if (newWidth <= width )
            {
                width = newWidth;
            } else if (newHeight <= height )
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