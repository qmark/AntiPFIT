using AntiPShared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        //private readonly string wordFilePath = "D:\\indexes.docx";
        //private readonly string wordFilePath = "D:\\tt.docx";

        //private readonly string wordFilePath = @"C:\Users\alex1\Desktop\TEST.docx";
        //private readonly string wordFilePath = @"D:\TEST.docx";
        private readonly string wordFilePath = @"D:\TEST1.docx";

        /// 

        private readonly string oldTextFile = "D:\\old633.docx";
        private readonly string newTextFile = "D:\\t633.docx";
        //private readonly string newTextFile = "D:\\new.docx";

        //private readonly string oldTextFile = "D:\\oldKurs.docx";
        //private readonly string newTextFile = "D:\\kurs.docx";

        private List<CheckResult> _webResults;

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

            var result = _webResults.Find(r => r.Url == e.LinkText);

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
            //await SearchForPlagiarismInWeb(wordFilePath);
            await SearchForPlagiarismLocally(wordFilePath);
        }

        private async Task SearchForPlagiarismInWeb(string fileName)
        {
            richTextBox1.Text = string.Empty;
            textBox2.Text = string.Empty;

            var simplifiedText = TextDocumentManager.SimplifiedTextFromFile(fileName);

            var plagiarismInWeb = await PlagiarismInWebFinder.Find(simplifiedText);

            richTextBox1.Text += simplifiedText + Environment.NewLine;
            richTextBox1.Text += $"Words count: {plagiarismInWeb.WordCount}" + Environment.NewLine;
            ShowUrlToWordIndexes(plagiarismInWeb.OrderedUrls, plagiarismInWeb.OrderedUrlToWordsIndexes);

            _webResults = plagiarismInWeb.OrderedWebResults;
            for (int i = 0; i < _webResults.Count; i++)
            {
                textBox2.Text += _webResults[i].Url;
                textBox2.Text += Environment.NewLine;

                textBox2.Text += String.Format("{0:P2} text found", _webResults[i].CharactersPercentage);
                textBox2.Text += Environment.NewLine;

                foreach (var commonTextPart in _webResults[i].CommonTextParts)
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

            var simplifiedText = TextDocumentManager.SimplifiedTextFromFile(fileName);

            var plagiarismInLocalDB = await PlagiarismInLocalDBFinder.Find(simplifiedText);

            richTextBox1.Text += simplifiedText + Environment.NewLine;
            richTextBox1.Text += $"Words count: {plagiarismInLocalDB.WordCount}" + Environment.NewLine;

            textBox2.Text += "Водность текста: " + plagiarismInLocalDB.Vodnost + " % " + Environment.NewLine;
            textBox2.Text += "Тошнотность текста: " + plagiarismInLocalDB.Toshnotnost + " % " + Environment.NewLine;

            foreach (var kvp in plagiarismInLocalDB.PlagiarismResult)
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