using AntiPShared;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public async Task<ActionResult> SourceUrl(string id)
        {

            //            plagiarismDB.DocumentIdToDBDocumentHtml.Add(kvp.Key, ComposeHtmlText(initialWords, plagiarismDB.DocumentIdToDBWordsIndexes[kvp.Key]));
            //            initialWords->documentInDBWords
            //PlagiarismInLocalDBFinder.ComposeHtmlText
            //        клик на соурс -> грузим текст из соурса(бд/ урл) -> PlagiarismInLocalDBFinder.ComposeHtmlText(словаСоурса, индексыСловСоурса)->показываем результат как хтмл

            var webPageText = await WebManager.HtmlToTextAsync(@HttpUtility.UrlDecode(id));
            HashSet<int> docIndexes = Session[@HttpUtility.UrlDecode(id)] as HashSet<int>;
            TextManager.PrepareText(webPageText, out string[] urlInitialWords, out _, out int[] urlInitialDocIndexes, out string[] urlSimplifiedWords, out int urlWordCount);
            ViewBag.Text = TextManager.ComposeHtmlText(urlInitialWords, docIndexes);
            ViewBag.Message = "AntiP Main page.";

            return View();
        }
        public ActionResult SourceDB(int id)
        {

            //            plagiarismDB.DocumentIdToDBDocumentHtml.Add(kvp.Key, ComposeHtmlText(initialWords, plagiarismDB.DocumentIdToDBWordsIndexes[kvp.Key]));
            //            initialWords->documentInDBWords
            //PlagiarismInLocalDBFinder.ComposeHtmlText
            //        клик на соурс -> грузим текст из соурса(бд/ урл) -> PlagiarismInLocalDBFinder.ComposeHtmlText(словаСоурса, индексыСловСоурса)->показываем результат как хтмл

            HashSet<int> docIndexes = Session["Doc" + id] as HashSet<int>;


            ViewBag.Text = TextManager.ComposeHtmlText(TextManager.WordsFromText(SQLLoader.GetDoc(id)).ToArray(), docIndexes);
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
                TextManager.PrepareText(initialText, out string[] initialWords, out Dictionary<int, string> initialDocIndexToSimplifiedWord, out int[] initialDocIndexes, out string[] simplifiedWords, out int wordCount);

                var plagiarismInWebSearch = PlagiarismInWebFinder.FindAsync(Server.MapPath("~/App_Data/uploads"), initialWords, initialDocIndexToSimplifiedWord, initialDocIndexes, simplifiedWords, wordCount);
                var plagiarismInLocalDBSearch = PlagiarismInLocalDBFinder.FindAsync(initialWords, initialDocIndexToSimplifiedWord, initialDocIndexes, simplifiedWords);

                await Task.WhenAll(plagiarismInWebSearch, plagiarismInLocalDBSearch);

                var plagiarismInWeb = plagiarismInWebSearch.Result;
                var plagiarismInLocalDB = plagiarismInLocalDBSearch.Result;

                var DBPlagiarizedIndexes = plagiarismInLocalDB.InitialWordIndexToSourceIds.Keys.ToList();
                DBPlagiarizedIndexes.AddRange(plagiarismInWeb.InitialWordIndexToSourceIds.Keys.ToList());
                var allPlagiarismHtmlText = TextManager.ComposeHtmlText(initialWords, DBPlagiarizedIndexes);

                var (vodnost, toshnotnost) = TextAnalyzer.Analyze(simplifiedWords);
                PlagiarismResult plagiarism = new PlagiarismResult
                {
                    InitialWords = initialWords,
                    SimplifiedWords = simplifiedWords,
                    WordCount = wordCount,
                    Vodnost = vodnost,
                    Toshnotnost = toshnotnost,
                    PlagiarismWeb = plagiarismInWeb,
                    PlagiarismDB = plagiarismInLocalDB,
                    AllPlagiarismHtmlText = allPlagiarismHtmlText
                };

                foreach (KeyValuePair<int, HashSet<int>> lists in plagiarismInLocalDB.SourceIdToSourceWordsIndexes)
                {
                    Session["Doc" + lists.Key] = lists.Value;
                }
                foreach (KeyValuePair<string, HashSet<int>> lists in plagiarismInWeb.SourceIdToSourceWordsIndexes)
                {
                    Session[lists.Key] = lists.Value;
                }

                return View("Main", plagiarism);
            }

            return RedirectToAction("Main");
        }
    }
}