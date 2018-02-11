using mshtml;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Antiplagiarism
{
    public partial class WebBrowserForm : Form
    {
        private List<string> _wordsToHighLight = new List<string>();
        private int _currentWordIndex = 0;

        public WebBrowserForm()
        {
            InitializeComponent();
            webBrowser1.DocumentCompleted += WebBrowser1_DocumentCompleted;
        }

        private void WebBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (_wordsToHighLight != null)
                Highlight(_wordsToHighLight);
        }

        public void Navigate(string url)
        {
            webBrowser1.Navigate(url);
        }

        public void Highlight(List<string> words)
        {
            if (webBrowser1.Document != null)
            {
                if (webBrowser1.Document.DomDocument is IHTMLDocument2 document)
                {
                    if (document.selection.createRange() is IHTMLTxtRange range)
                    {
                        for (int i = 0; i < words.Count; i++)
                        {
                            if (range.findText(words[i], words[i].Length, 0))
                            {
                                range.select();
                                range.execCommand("BackColor", false, "#FF0000");
                            }
                        }
                    }
                }
            }
            else
            {
                _wordsToHighLight.AddRange(words);
            }
        }

        private void Highlight(string word)
        {
            if (webBrowser1.Document != null)
            {
                if (webBrowser1.Document.DomDocument is IHTMLDocument2 document)
                {
                    if (document.selection.createRange() is IHTMLTxtRange range)
                    {
                        if (range.findText(word, word.Length, 0))
                        {
                            try
                            {
                                range.select();
                            }
                            catch { }
                        }
                    }
                }
            }
        }

        private void prevButton_Click(object sender, EventArgs e)
        {
            if (_currentWordIndex == 0) return;

            _currentWordIndex--;
            ScrollToCurrentWord();
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            if (_currentWordIndex == _wordsToHighLight.Count - 1) return;

            _currentWordIndex++;
            ScrollToCurrentWord();
        }

        private void ScrollToCurrentWord()
        {
            Highlight(_wordsToHighLight[_currentWordIndex]);
        }

    }
}
