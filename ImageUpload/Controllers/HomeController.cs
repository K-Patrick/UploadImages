using ImageUpload.Models;
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
            string imageLoc = "~/Images";
            Photos p = new Photos(imageLoc, Server.MapPath(imageLoc));

            ViewBag.photos = p.lst;
            return View();
        }

        [HttpPost]
        public ActionResult DeleteImage(FormCollection form)
        {
            var filepath = form["filepath"];
            Photos p = new Photos();
            p.DeletePhoto(Server.MapPath("~/" + filepath));
            
            return Json(true, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult UploadFiles(IEnumerable<HttpPostedFileBase> files)
        {
            Photos p = new Photos();

            foreach (var file in files)
            {
                p.SavePhoto(file, Server.MapPath("~/Images"));
            }

            return Json(p.lst);
        }

    }
}