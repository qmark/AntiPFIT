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
        private readonly string wordFilePath = "D:\\tt.docx";
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
            string query = "The Flow";

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
                await CheckAllTheShit(openFileDialog1.FileName);
            }
        }

        private async void HardcodedFileButton_Click(object sender, EventArgs e)
        {
            await CheckAllTheShit(wordFilePath);
        }

        private async Task CheckAllTheShit(string fileName)
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

            //var wordToPositions = Logic.Indexing(words);
            //foreach (var kvp in wordToPositions)
            //{
            //    richTextBox1.Text += kvp.Key + " : ";
            //    foreach (var item in kvp.Value)
            //    {
            //        richTextBox1.Text += item + ", ";
            //    }
            //    richTextBox1.Text += Environment.NewLine;
            //}
            //richTextBox1.Text += Environment.NewLine;
            //richTextBox1.Text += Environment.NewLine;

            var wordCount = words.Length;
            richTextBox1.Text += $"Words count: {wordCount}" + Environment.NewLine;
            tasksCreatedTextBox.Text = (wordCount - Shingle.Lenght + 1).ToString();
            progressTextBox.Text = (wordCount - Shingle.Lenght + 1).ToString();

            string path = @"D:\urlToWordsIndexes.txt";
            //var urlToWordsIndexes = WebManager.WebSitesFromWordsParallel(words);
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

    }
}