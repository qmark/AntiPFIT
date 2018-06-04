using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AntiPShared;

namespace AntiPWeb.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Main()
        {
            ViewBag.Message = "AntiP Main page.";

            return View();
        }

        public ActionResult Test()
        {
            ViewBag.Message = "TESt TEST";

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Main(HttpPostedFileBase file)
        {

            if (file.ContentLength > 0)
            {
                var fileName = Path.GetFileName(file.FileName);
                var path = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileName);
                file.SaveAs(path);
                
                var simplifiedText = TextDocumentManager.TextFromFile(path);
                PlagiarismInLocalDB result = await PlagiarismInLocalDBFinder.Find(simplifiedText);

              
                
               
                return View("Main", result);
            }
            
           
            return RedirectToAction("Main");
        }
    }
}