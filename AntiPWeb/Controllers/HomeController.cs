using AntiPShared;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

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

        public ActionResult Source(int id)
        {

            //            plagiarismDB.DocumentIdToDBDocumentHtml.Add(kvp.Key, ComposeHtmlText(initialWords, plagiarismDB.DocumentIdToDBWordsIndexes[kvp.Key]));
            //            initialWords->documentInDBWords
            //PlagiarismInLocalDBFinder.ComposeHtmlText
            //        клик на соурс -> грузим текст из соурса(бд/ урл) -> PlagiarismInLocalDBFinder.ComposeHtmlText(словаСоурса, индексыСловСоурса)->показываем результат как хтмл
            HashSet<int> docIndexes = Session["Doc"+id] as HashSet<int>;


            ViewBag.Text = PlagiarismInLocalDBFinder.ComposeHtmlText(TextManager.WordsFromText(SQLLoader.GetDoc(id)).ToArray(), docIndexes);
            ViewBag.Message = "AntiP Main page.";

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

                var initialText = TextDocumentManager.TextFromFile(path);
                PlagiarismInLocalDB plagiarismInLocalDB = await PlagiarismInLocalDBFinder.Find(initialText);
                foreach (KeyValuePair<int, HashSet<int>> lists in plagiarismInLocalDB.PlagiarismDB.DocumentIdToDBWordsIndexes)
                {
                    Session["Doc" + lists.Key] = lists.Value;
                }
                return View("Main", plagiarismInLocalDB);
            }


            return RedirectToAction("Main");
        }
    }
}