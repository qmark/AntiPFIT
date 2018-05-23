using AntiPShared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Antiplagiarism
{
    public partial class MainForm : Form
    {
        //private readonly string wordFilePath = "D:\\kurs.docx";
        //private readonly string wordFilePath = "D:\\t633.docx";
        //private readonly string wordFilePath = "D:\\t170.docx";
        //private readonly string wordFilePath = "D:\\t21.docx";
        //private readonly string wordFilePath = "D:\\t8.docx";
        private readonly string wordFilePath = @"C:\Users\alex1\Desktop\TEST.docx";
        //private readonly string wordFilePath = "D:\\indexes.docx";

        /// 

        private readonly string oldTextFile = "D:\\old633.docx";
        private readonly string newTextFile = "D:\\t633.docx";
        //private readonly string newTextFile = "D:\\new.docx";

        //private readonly string oldTextFile = "D:\\oldKurs.docx";
        //private readonly string newTextFile = "D:\\kurs.docx";

        private List<CheckResult> _results;

        public MainForm()
        {
            InitializeComponent();
        }

        private async void OCR_button_Click(object sender, EventArgs e)
        {
            await Task.Run(async () =>
            {
                string text = "";

                try
                {
                    text = await OCR.GetTextFromPic();
                }
                catch (Exception ex)
                {
                    ShowException(ex, "Tesseract Error");
                }

                richTextBox1.Text = text;
            });
        }

        private async void GoogleSearchButton_Click(object sender, EventArgs e)
        {
            string query = QueryTextBox.Text;

            await Task.Run(async () =>
            {
                string text = "";

                try
                {
                    text = await GoogleAPIManager.GetGoogleSearchResults(query);
                }
                catch (Exception ex)
                {
                    ShowException(ex, "GSearch Error");
                }

                textBox2.Text = text;
            });
        }

        private static void ShowException(Exception e, string caption)
        {
            Trace.TraceError(e.ToString());
            MessageBox.Show($"{e.Message}{Environment.NewLine}{e.ToString()}",
                caption,
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public void ShowUrlToWordIndexes(List<string> urls, Dictionary<string, List<int>> urlToWordsIndexes)
        {
            richTextBox1.Text += Environment.NewLine;
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

        private void CompareTextButton_Click(object sender, EventArgs e)
        {
            Stopwatch s = new Stopwatch();
            s.Start();

            var oldText = TextDocumentManager.SimplifiedTextFromFile(oldTextFile);
            var newText = TextDocumentManager.SimplifiedTextFromFile(newTextFile);

            s.Stop();
            textBox2.Text += $"\tReading: {s.Elapsed}" + Environment.NewLine;
            s.Restart();

            var diffResuls = TextComparer.CompareByWords(oldText, newText);
            //CompareLines(oldText, newText);

            richTextBox1.ShowDifferences(diffResuls);

            s.Stop();
            textBox2.Text += $"\tComparing: {s.Elapsed}" + Environment.NewLine;
        }

        private void textBox2_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            WebBrowserForm webBrowserForm = new WebBrowserForm();
            webBrowserForm.Navigate(e.LinkText);
            webBrowserForm.Show();

            var result = _results.Find(r => r.Url == e.LinkText);

            string commonText = string.Empty;
            foreach (var commonTextPart in result.CommonTextParts)
            {
                var textPartWords = TextManager.WordsFromText(commonTextPart);
                webBrowserForm.Highlight(textPartWords);
            }
        }

        private async void LoadFileButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Text files|*txt;*.docx;*.doc";
            openFileDialog1.Title = "Select a File to check";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                await SearchForPlagiarismInWeb(openFileDialog1.FileName);
            }
        }

        private async void HardcodedFileButton_Click(object sender, EventArgs e)
        {
            await SearchForPlagiarismLocally(wordFilePath);
        }

        private async Task SearchForPlagiarismInWeb(string fileName)
        {
            richTextBox1.Text = string.Empty;
            textBox2.Text = string.Empty;

            //Stopwatch s = new Stopwatch();
            //s.Start();

            var simplifiedText = TextDocumentManager.SimplifiedTextFromFile(fileName);

            var words = TextManager.WordsFromText(simplifiedText).ToArray();
            //s.Stop();
            //textBox2.Text += $"\tGet text and words: {s.Elapsed}" + Environment.NewLine;
            //s.Restart();

            var wordCount = words.Length;
            richTextBox1.Text += $"Words count: {wordCount}" + Environment.NewLine;

            string path = @"D:\urlToWordsIndexes.txt";
            //var shingles = Shingle.ShinglesFromWords(words);
            //var urlToWordsIndexes = WebManager.WebSitesFromWordsParallel(shingles);
            //File.WriteAllText(path, JsonConvert.SerializeObject(urlToWordsIndexes));
            var urlToWordsIndexes = JsonConvert.DeserializeObject<Dictionary<string, List<int>>>(File.ReadAllText(path));

            var orderedUrlToWordsIndexes = urlToWordsIndexes.OrderByDescending(kvp => kvp.Value.Count).ToDictionary(pair => pair.Key, pair => pair.Value);
            var orderedUrls = orderedUrlToWordsIndexes.Keys.ToList();

            //s.Stop();
            //textBox2.Text += $"\tGet web sites: {s.Elapsed}" + Environment.NewLine;

            ShowUrlToWordIndexes(orderedUrls, orderedUrlToWordsIndexes);

            //s.Restart();
            var urlsCountCap = (int)Math.Ceiling(wordCount * 0.1);
            var webPagesSimplifiedTexts = await WebManager.UrlsToSimplifiedTextsAsync(urlsCountCap, orderedUrls);
            //s.Stop();
            //textBox2.Text += $"\tGet texts from web sites: {s.Elapsed}" + Environment.NewLine;

            //s.Restart();
            var urlToCommonTextParts = TextComparer.CommonTextParts(orderedUrls, orderedUrlToWordsIndexes, words, webPagesSimplifiedTexts);
            //s.Stop();
            //textBox2.Text += $"\tComparing: {s.Elapsed}" + Environment.NewLine;

            double originalTextCharactersCount = simplifiedText.RemoveWhiteSpaces().Length;
            _results = Logic.GetCheckResults(orderedUrls, urlsCountCap, urlToCommonTextParts, originalTextCharactersCount);

            var orderedResults = _results.OrderByDescending(res => res.CharactersPercentage).ToList();

            for (int i = 0; i < orderedResults.Count; i++)
            {
                textBox2.Text += orderedResults[i].Url;
                textBox2.Text += Environment.NewLine;

                textBox2.Text += String.Format("{0:P2} text found", orderedResults[i].CharactersPercentage);
                textBox2.Text += Environment.NewLine;

                foreach (var commonTextPart in orderedResults[i].CommonTextParts)
                {
                    textBox2.Text += commonTextPart;
                    textBox2.Text += Environment.NewLine;
                }

                textBox2.Text += Environment.NewLine;
            }
        }

        private async Task SearchForPlagiarismLocally(string fileName)
        {
            richTextBox1.Text = string.Empty;
            textBox2.Text = string.Empty;
            //richTextBox1.Text += TextDocumentManager.TextFromFile(fileName);
            var simplifiedText = TextDocumentManager.SimplifiedTextFromFile(fileName);
           // textBox2.Text += simplifiedText;
            var words = TextManager.WordsFromText(simplifiedText).ToArray();
           // string str="";
            var wordCount = words.Length;
            richTextBox1.Text += $"Words count: {wordCount}" + Environment.NewLine;
            //Dictionary<int, List<List<int>>> documentIdsToWordPositions = SQLLoader.GetDocuments(Shingle.ListFromWords(words, i));
            //foreach (KeyValuePair<int, List<List<int>>> pair in documentIdsToWordPositions)
            //{

            //    richTextBox1.Text += "Документ - " + pair.Key;
            //    foreach (List<int> positions in pair.Value)
            //    {
            //        foreach (int pos in positions)
            //        {
            //            str += Convert.ToString(pos) + " ";
            //        }
            //        richTextBox1.Text +="  Позицiї - " + str;
            //        str = "";
            //    }
            //}


            Dictionary<int, HashSet<int>> plagiarismResult = new Dictionary<int, HashSet<int>>();
            double vodnost = 0;
            for (int i = 0; i <= words.Length - Shingle.Lenght; i++)
            {
                if (TextManager.StopWords.Contains(words[i]))
                    vodnost++;

                //

                Dictionary<int, List<List<int>>> documentIdsToWordPositions = SQLLoader.GetDocuments(Shingle.ListFromWords(words, i));
                //richTextBox1.Text += "Новий шингл" + "\n"+ "\n";
                //foreach (KeyValuePair<int, List<List<int>>> pair in documentIdsToWordPositions)
                //{

                //    richTextBox1.Text += "Документ - " + pair.Key + "\n";
                //    foreach (List<int> positions in pair.Value)
                //    {
                //        foreach (int pos in positions)
                //        {
                //            str += Convert.ToString(pos) + " ";
                //        }
                //        richTextBox1.Text += "  Позицiї - " + str +"\n";
                //        str = "";
                //    }
                //}

                //Dictionary<int, List<List<int>>> documentIdsToWordPositions = new Dictionary<int, List<List<int>>>
                //{
                //    {
                //        1, new List<List<int>>
                //        {
                //            new List<int> { 5 },
                //            new List<int> { 7 },
                //            new List<int> { 9 }
                //        }
                //    },
                //    {
                //        2, new List<List<int>>
                //        {
                //            new List<int> { 5 },
                //            new List<int> { 8 },
                //            new List<int> { 9 }
                //        }
                //    }
                //};

                var plagiarismForShingle = Logic.FindPlagiarism(documentIdsToWordPositions);
                foreach (var kvp in plagiarismForShingle)
                {
                    if (plagiarismResult.TryGetValue(kvp.Key, out HashSet<int> plagiarizedPositions))
                    {
                        plagiarizedPositions.UnionWith(kvp.Value);
                    }
                    else
                    {
                        plagiarismResult.Add(kvp.Key, new HashSet<int>(kvp.Value));
                    }
                }
            }

            for (int i = words.Length - Shingle.Lenght + 1; i < words.Length; i++)
            {
                if (TextManager.StopWords.Contains(words[i]))
                    vodnost++;
            }

            vodnost /= Convert.ToDouble(words.Length);
            double toshnotnost = (words.Length - words.Distinct().Count()) / Convert.ToDouble(words.Length);

            textBox2.Text += "Водность текста: " + vodnost + " % " + Environment.NewLine;
            textBox2.Text += "Тошнотность текста: " + toshnotnost + " % " + Environment.NewLine;

            foreach (var kvp in plagiarismResult)
            {
                textBox2.Text += "DocumentID: " + kvp.Key + "\tWord Indexes: ";
                foreach (var item in kvp.Value)
                {
                    textBox2.Text += item + ", ";
                }
                textBox2.Text += Environment.NewLine;
            }
        }

    }
}