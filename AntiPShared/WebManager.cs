using HtmlAgilityPack;
using ReadSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using System.Web;

namespace AntiPShared
{
    public class WebManager
    {
        public static Dictionary<string, List<int>> WebSitesFromWordsParallel(string[] words)
        {
            var shingles = new List<Shingle>();
            for (int i = 0; i <= words.Length - Shingle.Lenght; i++)
            {
                shingles.Add(new Shingle(words, i));
            }

            var staScheduler = new StaTaskScheduler(numberOfThreads: 1); // МЕДЛЕННО, НО ДОЛЬШЕ БЕЗ КАПЧИ
            //var staScheduler = new StaTaskScheduler(numberOfThreads: shingles.Count); // (В Ж)БАН ПРИЛЕТАЕТ СРАЗУ ЖЕ
            var tasks = new Task<(int, List<string>)>[shingles.Count];
            for (int i = 0; i < shingles.Count; i++)
            {
                int j = i;
                tasks[j] = Task<(int, List<string>)>.Factory.StartNew(() => FirstWordIndexAndUrls(shingles[j])
                    , CancellationToken.None
                    , TaskCreationOptions.None
                    //, TaskScheduler.FromCurrentSynchronizationContext()
                    , staScheduler
                );
            }

            Task.WaitAll(tasks);

            var urlToWordsIndexes = new Dictionary<string, List<int>>();
            for (int i = 0; i < tasks.Length; i++)
            {
                var (FirstWordIndex, Urls) = tasks[i].Result;

                UrlsToDictionaries(Urls, FirstWordIndex, urlToWordsIndexes);
            }
            return urlToWordsIndexes;
        }

        public static void UrlsToDictionaries(List<string> urls, int firstWordIndex, Dictionary<string, List<int>> urlToWordsIndexes)
        {
            for (int j = 0; j < urls.Count; j++)
            {
                var currentUrl = urls[j];

                //if (urlToWordsIndexes.TryGetValue(currentUrl, out int doesntmatter))
                if (urlToWordsIndexes.ContainsKey(currentUrl))
                {
                    urlToWordsIndexes[currentUrl].Add(firstWordIndex);
                }
                else
                {
                    urlToWordsIndexes.Add(currentUrl, new List<int>() { firstWordIndex });
                }
            }
        }

        public static (int firstWordIndex, List<string> urls) FirstWordIndexAndUrls(Shingle shingle)
        {
            Console.WriteLine(shingle.Query);

            var links = UrlsFromGoogle(shingle.Query);
            //var links = UrlsFromShukalka(shingle.Query);
            //var tuple = FirstWordIndexAndUrlsFromQueryLooksmart(shingle);

            return (shingle.FirstWordIndex, links);
        }

        public static List<string> UrlsFromGoogle(string query)
        {
            List<string> links = new List<string>();

            string url = "https://www.google.com.ua/search?q=" + query;
            var web1 = new HtmlWeb();
            var doc1 = web1.LoadFromBrowser(url);
            var nodes = doc1.DocumentNode.SelectNodes("//h3").Where(x => x.Attributes.Contains("class") && x.Attributes["class"].Value == "r" && x.FirstChild.Attributes.Contains("href"));
            foreach (var node in nodes)
            {
                links.Add(node.FirstChild.Attributes["href"].Value);
            }

            return links;
        }

        public static List<string> UrlsFromLooksmart(string query)
        {
            List<string> links = new List<string>();

            string url = "http://www.looksmart.com/search.php?st=web&q=" + query;
            var web1 = new HtmlWeb();
            var doc1 = web1.LoadFromBrowser(url, o => { return !o.Contains("<div class=\"csr-holding\" id=\"loading\">"); });
            var nodes = doc1.DocumentNode.SelectNodes("//a").Where(x => x.Attributes.Contains("class") && x.Attributes["class"].Value == "resultTitle");
            foreach (var node in nodes)
            {
                string resultLink = HttpUtility.UrlDecode(HttpUtility.UrlDecode(HttpUtility.UrlDecode(node.Attributes["href"].Value)));
                String result = resultLink.Substring(resultLink.IndexOf("&ru=") + "&ru=".Length, resultLink.LastIndexOf("&du") - resultLink.IndexOf("&ru=") - "&ru=".Length);
                links.Add(result);
            }

            return links;
        }

        public static List<string> UrlsFromShukalka(string query)
        {
            List<string> links = new List<string>();

            string url = "http://shukalka.com.ua/srch2.tm?q=" + query;
            var web1 = new HtmlWeb();
            var doc1 = web1.LoadFromBrowser(url);
            var nodes0 = doc1.DocumentNode.SelectNodes("");
            foreach (var node in nodes0)
            {
                links.Add(node.Attributes["href"].Value);
            }
            //var nodes = doc1.DocumentNode.SelectNodes("//a").Where(x => x.Attributes.Contains("class") && x.Attributes["class"].Value == "gs-title");
            //foreach (var node in nodes)
            //{
            //    links.Add(node.Attributes["href"].Value);
            //    //links.Add(node.FirstChild.Attributes["href"].Value);
            //}

            return links;
        }

        public static async Task<string[]> UrlsToSimplifiedTextsAsync(int urlsCountCap, List<string> urls)
        {
            int urlsToTextsCount = urlsCountCap;
            string[] webPagesTexts = new string[urlsToTextsCount];
            for (int i = 0; i < urlsToTextsCount; i++)
            {
                webPagesTexts[i] = TextManager.SimplifyText(await HtmlToText(urls[i]));
            }
            return webPagesTexts;
        }

        public static async Task<string> HtmlToText(string url)
        {
            Reader reader = new Reader();
            ReadOptions options = new ReadOptions
            {
                HasHeadline = true
            };

            try
            {
                var article = await reader.Read(new Uri(url), options);
                var r = new Regex(@"<(.+?)>");
                var r2 = new Regex(@"\s+");
                string result = r2.Replace(r.Replace(article.Content, " "), " ");
                return result;
            }
            catch (ReadException exc)
            {
                return string.Empty;
            }
        }

    }
}
