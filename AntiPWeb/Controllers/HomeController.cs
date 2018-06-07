﻿using AntiPShared;
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
            
            var lol = await WebManager.HtmlToText(@HttpUtility.UrlDecode(id));
            HashSet<int> docIndexes = Session[@HttpUtility.UrlDecode(id)] as HashSet<int>;
            TextManager.PrepareText(lol, out string[] urlInitialWords, out int[] urlInitialDocIndexes, out string[] urlSimplifiedWords, out int urlWordCount);
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
                TextManager.PrepareText(initialText, out string[] initialWords, out int[] initialDocIndexes, out string[] simplifiedWords, out int wordCount);

                var plagiarismInWebResult = await PlagiarismInWebFinder.Find(Server.MapPath("~/App_Data/uploads"), initialWords, initialDocIndexes, simplifiedWords, wordCount, initialText.RemoveWhiteSpaces().Length);
                var plagiarismInLocalDBResult = await PlagiarismInLocalDBFinder.Find(initialWords, initialDocIndexes, simplifiedWords);

                var DBPlagiarizedIndexes = plagiarismInLocalDBResult.PlagiarismDB.InitialWordIndexToDocumentIds.Keys.ToList();
                DBPlagiarizedIndexes.AddRange(plagiarismInWebResult.PlagiarismWeb.InitialWordIndexToUrls.Keys.ToList());
                var allPlagiarismHtmlText = TextManager.ComposeHtmlText(initialWords, DBPlagiarizedIndexes);

                Plagiarism plagiarism = new Plagiarism
                {
                    InitialWords = initialWords,
                    SimplifiedWords = simplifiedWords,
                    WordCount = wordCount,
                    Vodnost = plagiarismInLocalDBResult.Vodnost,
                    Toshnotnost = plagiarismInLocalDBResult.Toshnotnost,
                    PlagiarismWeb = plagiarismInWebResult.PlagiarismWeb,
                    PlagiarismDB = plagiarismInLocalDBResult.PlagiarismDB,
                    AllPlagiarismHtmlText = allPlagiarismHtmlText
                };

               
               
                foreach (KeyValuePair<int, HashSet<int>> lists in plagiarismInLocalDBResult.PlagiarismDB.DocumentIdToDBWordsIndexes)
                {
                    Session["Doc" + lists.Key] = lists.Value;
                }
                foreach (KeyValuePair<string, HashSet<int>> lists in plagiarismInWebResult.PlagiarismWeb.UrlToWebPageWordsIndexes)
                {
                    Session[lists.Key] = lists.Value;
                }
                return View("Main", plagiarism);
            }

            return RedirectToAction("Main");
        }
    }
}