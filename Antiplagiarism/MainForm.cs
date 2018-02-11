using GoogleCSE;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Spire.Doc;
using System.Json;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tesseract;
using System.Linq;
using HtmlAgilityPack;
using System.Threading;
using System.Threading.Tasks.Schedulers;
using System.Web;
using DiffPlex.DiffBuilder;
using DiffPlex;
using DiffPlex.DiffBuilder.Model;
using System.Drawing;
using System.Text.RegularExpressions;
using ReadSharp;
using DiffPlex.Model;
using System.Text;
using Newtonsoft.Json;

namespace Antiplagiarism
{
    public partial class MainForm : Form
    {
        private List<string> _wordsFromPic;

        //private readonly string wordFilePath = "D:\\kurs.docx";
        //private readonly string wordFilePath = "D:\\t633.docx";
        //private readonly string wordFilePath = "D:\\t170.docx";
        //private readonly string wordFilePath = "D:\\t21.docx";
        //private readonly string wordFilePath = "D:\\t8.docx";
        private readonly string wordFilePath = "D:\\tt.docx";

        /// 

        private readonly string oldTextFile = "D:\\old633.docx";
        private readonly string newTextFile = "D:\\t633.docx";
        //private readonly string newTextFile = "D:\\new.docx";

        //private readonly string oldTextFile = "D:\\oldKurs.docx";
        //private readonly string newTextFile = "D:\\kurs.docx";

        private int _progress;

        private char[] _wordSeparators = { ' ' };

        public MainForm()
        {
            InitializeComponent();
        }

        #region Tesseract OCR
        private async void button1_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;

            string text = "Text1";

            await Task.Run(async () => text = await GetTextFromPic());

            richTextBox1.Text = text;

            button2.Enabled = true;
        }

        public async Task<string> GetTextFromPic(string testImagePath = "dou.PNG")
        {
            string result = string.Empty;

            _wordsFromPic = new List<string>();

            try
            {
                using (var engine = new TesseractEngine(@"tessdata", "ukr+eng", EngineMode.Default))
                {
                    using (var img = Pix.LoadFromFile(testImagePath))
                    {
                        using (var page = engine.Process(img))
                        {
                            //var text = page.GetText();
                            //result += $"Mean confidence: {page.GetMeanConfidence()}" + Environment.NewLine;
                            //result += $"Text (GetText):{Environment.NewLine}{text}" + Environment.NewLine;
                            //result += Environment.NewLine;
                            //result += "=================================" + Environment.NewLine;
                            //result += Environment.NewLine;
                            //result += $"Text (iterator):" + Environment.NewLine;

                            using (var iter = page.GetIterator())
                            {
                                iter.Begin();

                                do
                                {
                                    do
                                    {
                                        do
                                        {
                                            do
                                            {
                                                if (iter.IsAtBeginningOf(PageIteratorLevel.Block))
                                                {
                                                    //result += $"<BLOCK>" + Environment.NewLine;
                                                    //result += Environment.NewLine;
                                                }

                                                var word = iter.GetText(PageIteratorLevel.Word);

                                                result += $"{word} ";

                                                _wordsFromPic.Add(word);

                                                //if (iter.IsAtFinalOf(PageIteratorLevel.TextLine, PageIteratorLevel.Word))
                                                //{
                                                //result += Environment.NewLine;
                                                //}
                                            } while (iter.Next(PageIteratorLevel.TextLine, PageIteratorLevel.Word));

                                            if (iter.IsAtFinalOf(PageIteratorLevel.Para, PageIteratorLevel.TextLine))
                                            {
                                                result += Environment.NewLine;
                                            }
                                        } while (iter.Next(PageIteratorLevel.Para, PageIteratorLevel.TextLine));
                                    } while (iter.Next(PageIteratorLevel.Block, PageIteratorLevel.Para));
                                } while (iter.Next(PageIteratorLevel.Block));
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ShowException(e, "Tesseract Error");
            }

            return result;
        }
        #endregion

        #region Google Search API
        private async void button2_Click(object sender, EventArgs e)
        {
            //string url = "https://www.googleapis.com/customsearch/v1?key="
            //    + API_KEY
            //    + "&cx=" + SYSTEM_ID
            //    + "&q=" + "тест";

            //var json = await FetchWebRequestAsync(url);

            string text = "Text2";

            string q = string.Empty;

            for (int i = 0; i < 5; i++)
            {
                q += _wordsFromPic[i] + " ";
            }

            await Task.Run(async () => text = await GetSearchResults(q));

            textBox2.Text = text;
        }

        public async Task<string> GetSearchResults(string query)
        {
            string ApiKey = "AIzaSyDAKlpT9R-SmDjfbPZBJtNSv1SE0O_6UtY";
            string SystemID = "011958540907442234339:3amnjvx0hhq";


            string result = $"QUERY: {query}" + Environment.NewLine;

            try
            {
                var gs = new GoogleSearch(SystemID, ApiKey, maxPages: 1, pageSize: 10);

                try
                {
                    var results = gs.Search(query);

                    for (int i = 0; i < results.Count; i++)
                    {
                        result += "RESULT: " + i + Environment.NewLine;
                        result += "TITLE: " + results[i].Title + Environment.NewLine;
                        result += "DESC: " + results[i].Description + Environment.NewLine;
                        result += "URL: " + results[i].Url + Environment.NewLine;
                        result += "MIME: " + results[i].Mime + Environment.NewLine;
                        result += Environment.NewLine;
                    }
                }
                catch (Exception e)
                {
                    ShowException(e, "Google Search Error");
                }
            }
            catch (Exception e)
            {
                ShowException(e, "GoogleSearchInit Error");
            }

            return result;
        }

        public static async Task<JsonValue> FetchWebRequestAsync(string url)
        {
            HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
            webRequest.ContentType = "application/json";
            webRequest.Method = "GET";
            webRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            try
            {
                using (WebResponse response = await webRequest.GetResponseAsync())
                {
                    try
                    {
                        using (Stream stream = response.GetResponseStream())
                        {
                            try
                            {
                                JsonValue jsonDoc = await Task.Run(() => JsonObject.Load(stream));

                                return jsonDoc;
                            }
                            catch (Exception e)
                            {
                                //GAService.GetGASInstance().TrackAppException("Loading json from stream " + e.Message, false);

                                return string.Empty;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        //GAService.GetGASInstance().TrackAppException("Getting stream from webresponse " + e.Message, false);

                        return string.Empty;
                    }
                }
            }
            catch (Exception e)
            {
                //GAService.GetGASInstance().TrackAppException("Getting response from webrequest " + e.Message, false);

                return string.Empty;
            }
        }
        #endregion

        private static void ShowException(Exception e, string caption)
        {
            Trace.TraceError(e.ToString());
            MessageBox.Show($"{e.Message}{Environment.NewLine}{e.ToString()}",
                caption,
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        #region WebSites
        private async void button3_ClickAsync(object sender, EventArgs e)
        {
            richTextBox1.Text = string.Empty;
            textBox2.Text = string.Empty;
            _progress = 0;

            Stopwatch s = new Stopwatch();
            s.Start();

            var simplifiedText = SimplifiedTextFromFile(wordFilePath);

            var words = WordsFromText(simplifiedText).ToArray();
            s.Stop();
            textBox2.Text += $"\tGet text and words: {s.Elapsed}" + Environment.NewLine;
            s.Restart();

            textBox2.Text += $"Words count: {words.Length}" + Environment.NewLine;
            //ShowStrings(words);

            string path = @"D:\urlToWordsIndexes.txt";
            //var urlToWordsIndexes = WebSitesFromWordsParallel(words);
            //File.WriteAllText(path, JsonConvert.SerializeObject(urlToWordsIndexes));
            var urlToWordsIndexes = JsonConvert.DeserializeObject<Dictionary<string, List<int>>>(File.ReadAllText(path));

            var orderedUrlToWordsIndexes = urlToWordsIndexes.OrderByDescending(kvp => kvp.Value.Count).ToDictionary(pair => pair.Key, pair => pair.Value);
            var orderedUrls = orderedUrlToWordsIndexes.Keys.ToList();

            s.Stop();
            textBox2.Text += $"\tGet web sites: {s.Elapsed}" + Environment.NewLine;

            ShowUrlToWordIndexes(orderedUrls, orderedUrlToWordsIndexes);

            s.Restart();
            var webPagesSimplifiedTexts = await UrlsToSimplifiedTextsAsync(orderedUrls);
            s.Stop();
            textBox2.Text += $"\tGet texts from web sites: {s.Elapsed}" + Environment.NewLine;

            s.Restart();
            var urlToCommonTextParts = CommonTextParts(orderedUrls, orderedUrlToWordsIndexes, words, webPagesSimplifiedTexts);
            s.Stop();
            textBox2.Text += $"\tComparing: {s.Elapsed}" + Environment.NewLine;

            richTextBox1.Text += "\tCommon Text Parts:" + Environment.NewLine;
            double originalTextCharactersCount = simplifiedText.RemoveWhiteSpaces().Length;
            for (int i = 0; i < webPagesSimplifiedTexts.Length; i++)
            {
                richTextBox1.Text += orderedUrls[i];
                richTextBox1.Text += Environment.NewLine;

                WebBrowserForm webBrowserForm = new WebBrowserForm();
                webBrowserForm.Navigate(orderedUrls[i]);
                webBrowserForm.Show();

                string commonText = string.Empty;
                foreach (var commonTextPart in urlToCommonTextParts[orderedUrls[i]])
                {
                    var textPartWords = WordsFromText(commonTextPart);
                    webBrowserForm.Highlight(textPartWords);

                    commonText += commonTextPart;
                    richTextBox1.Text += commonTextPart;
                    richTextBox1.Text += Environment.NewLine;
                }

                var charactersPercentage = commonText.RemoveWhiteSpaces().Length / originalTextCharactersCount;
                richTextBox1.Text += String.Format("{0:P2}", charactersPercentage);
                richTextBox1.Text += Environment.NewLine;
                richTextBox1.Text += Environment.NewLine;

                break;
            }
        }

        private Dictionary<string, List<int>> WebSitesFromWordsParallel(string[] words)
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
                tasks[j] = Task<(int, List<string>)>.Factory.StartNew(() => FirstWordIndexAndUrlsFromQuery(shingles[j])
                    , CancellationToken.None
                    , TaskCreationOptions.None
                    //, TaskScheduler.FromCurrentSynchronizationContext()
                    , staScheduler
                );

                tasksCreatedTextBox.Text = j.ToString();
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

        private static void UrlsToDictionaries(List<string> urls, int firstWordIndex, Dictionary<string, List<int>> urlToWordsIndexes)
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

        private (int firstWordIndex, List<string> urls) FirstWordIndexAndUrlsFromQuery(Shingle shingle)
        {
            Console.WriteLine(shingle.Query);

            var tuple = FirstWordIndexAndUrlsFromQueryGoogle(shingle);
            //var tuple = FirstWordIndexAndUrlsFromQueryLooksmart(shingle);

            Console.WriteLine(++_progress);

            return tuple;
        }

        private (int firstWordIndex, List<string> urls) FirstWordIndexAndUrlsFromQueryGoogle(Shingle shingle)
        {
            List<string> links = new List<string>();

            string url = "https://www.google.com.ua/search?q=" + shingle.Query;
            var web1 = new HtmlWeb();
            var doc1 = web1.LoadFromBrowser(url);
            var nodes = doc1.DocumentNode.SelectNodes("//h3").Where(x => x.Attributes.Contains("class") && x.Attributes["class"].Value == "r" && x.FirstChild.Attributes.Contains("href"));
            foreach (var node in nodes)
            {
                links.Add(node.FirstChild.Attributes["href"].Value);
            }

            return (shingle.FirstWordIndex, links);
        }

        private (int firstWordIndex, List<string> urls) FirstWordIndexAndUrlsFromQueryLooksmart(Shingle shingle)
        {
            List<string> links = new List<string>();

            string url = "http://www.looksmart.com/search.php?st=web&q=" + shingle.Query;
            var web1 = new HtmlWeb();
            var doc1 = web1.LoadFromBrowser(url, o => { return !o.Contains("<div class=\"csr-holding\" id=\"loading\">"); });
            var nodes = doc1.DocumentNode.SelectNodes("//a").Where(x => x.Attributes.Contains("class") && x.Attributes["class"].Value == "resultTitle");
            foreach (var node in nodes)
            {
                string resultLink = HttpUtility.UrlDecode(HttpUtility.UrlDecode(HttpUtility.UrlDecode(node.Attributes["href"].Value)));
                String result = resultLink.Substring(resultLink.IndexOf("&ru=") + "&ru=".Length, resultLink.LastIndexOf("&du") - resultLink.IndexOf("&ru=") - "&ru=".Length);
                links.Add(result);
            }

            return (shingle.FirstWordIndex, links);
        }

        private void ShowUrlToWordIndexes(List<string> urls, Dictionary<string, List<int>> urlToWordsIndexes)
        {
            richTextBox1.Text += "\tUrls And Word Indexes:" + Environment.NewLine;
            for (int i = 0; i < urls.Count; i++)
            {
                var wordsIndexes = urlToWordsIndexes[urls[i]];

                if (wordsIndexes.Count <= 1) continue;

                richTextBox1.Text += $"{urls[i]}:" + Environment.NewLine;

                richTextBox1.Text += $"{wordsIndexes.Count} results:";

                for (int j = 0; j < wordsIndexes.Count; j++)
                {
                    richTextBox1.Text += " " + wordsIndexes[j];
                }

                richTextBox1.Text += Environment.NewLine;
                richTextBox1.Text += Environment.NewLine;
            }
            richTextBox1.Text += Environment.NewLine;
        }

        private async Task<string[]> UrlsToSimplifiedTextsAsync(List<string> urls)
        {
            // TODO urlsToTextsCount
            int urlsToTextsCount = 5;
            string[] webPagesTexts = new string[urlsToTextsCount];
            for (int i = 0; i < urlsToTextsCount; i++)
            {
                //webPagesTexts[i] = await HtmlToText(urls[i]);
                webPagesTexts[i] = SimplifyText(await HtmlToText(urls[i]));
            }
            return webPagesTexts;
        }

        private async Task<string> HtmlToText(string url)
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

        private Dictionary<string, List<string>> CommonTextParts(List<string> orderedUrls, Dictionary<string, List<int>> orderedUrlToWordsIndexes, string[] words, string[] webPagesTexts)
        {
            Dictionary<string, List<string>> urlToCommonTextParts = new Dictionary<string, List<string>>();
            for (int i = 0; i < webPagesTexts.Length; i++)
            {
                var shingleTexts = WordsIndexesToShingleTexts(words, orderedUrlToWordsIndexes[orderedUrls[i]]);

                List<string> commonTextParts = new List<string>();
                for (int j = 0; j < shingleTexts.Count; j++)
                {
                    var diffResuls = CompareByWords(shingleTexts[j], webPagesTexts[i]);
                    commonTextParts.Add(UnidiffFormater.CommonPart(diffResuls));
                }
                urlToCommonTextParts.Add(orderedUrls[i], commonTextParts);
            }
            return urlToCommonTextParts;
        }

        private List<string> WordsIndexesToShingleTexts(string[] words, List<int> wordsIndexes)
        {
            List<string> shingleTexts = new List<string>
            {
                Shingle.QueryFromWords(words, wordsIndexes[0])
            };

            for (int i = 1; i < wordsIndexes.Count; i++)
            {
                int overlap;
                if ((overlap = Shingle.Lenght - (wordsIndexes[i] - wordsIndexes[i - 1])) > 0)
                {
                    for (int j = overlap; j < Shingle.Lenght; j++)
                    {
                        shingleTexts[shingleTexts.Count - 1] += " " + words[wordsIndexes[i] + j];
                    }
                }
                else
                {
                    shingleTexts.Add(Shingle.QueryFromWords(words, wordsIndexes[i]));
                }
            }
            return shingleTexts;
        }
        #endregion

        private string SimplifiedTextFromFile(string fileName)
        {
            var doc = new Document();
            doc.LoadFromFile(fileName);

            return SimplifyText(doc.GetText()).Replace("\r\n", " ");
        }

        private List<string> WordsFromText(string text)
        {
            var words = text.Split().ToList();
            words.RemoveAll(x => string.IsNullOrWhiteSpace(x));
            return words;
        }

        private string SimplifyText(string text)
        {
            return new string(text.Replace('-', ' ').Where(c => !char.IsPunctuation(c)).ToArray()).ToLower();
        }

        private void ShowStrings(string[] strings)
        {
            for (int i = 0; i < strings.Length; i++)
            {
                textBox2.Text += strings[i] + Environment.NewLine;
            }
        }

        #region DiffPlex
        private void CompareTextButton_Click(object sender, EventArgs e)
        {
            Stopwatch s = new Stopwatch();
            s.Start();

            var oldText = SimplifiedTextFromFile(oldTextFile);
            var newText = SimplifiedTextFromFile(newTextFile);

            s.Stop();
            textBox2.Text += $"\tReading: {s.Elapsed}" + Environment.NewLine;
            s.Restart();

            var diffResuls = CompareByWords(oldText, newText);
            //CompareLines(oldText, newText);

            richTextBox1.ShowDifferences(diffResuls);

            s.Stop();
            textBox2.Text += $"\tComparing: {s.Elapsed}" + Environment.NewLine;
        }

        private DiffResult CompareByWords(string oldText, string newText)
        {
            var differ = new Differ();
            return differ.CreateWordDiffs(newText, oldText, true, _wordSeparators);
            //return diffResuls = differ.CreateWordDiffs(oldText, newText, true, _wordSeparators);
        }

        private void CompareByLines(string oldText, string newText)
        {
            var diffBuilder = new InlineDiffBuilder(new Differ());
            var diff = diffBuilder.BuildDiffModel(oldText, newText);

            foreach (var line in diff.Lines)
            {
                Color textColor;
                string text;

                switch (line.Type)
                {
                    case ChangeType.Inserted:
                        textColor = Color.Red;
                        text = "+ ";
                        break;

                    case ChangeType.Deleted:
                        textColor = Color.Green;
                        text = "- ";
                        break;

                    default:
                        textColor = Color.Black;
                        text = "  ";
                        break;
                }

                richTextBox1.AppendText(text + line.Text + Environment.NewLine, textColor);
            }
        }
        #endregion

    }
}