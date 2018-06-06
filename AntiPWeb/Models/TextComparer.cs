using DiffPlex;
using DiffPlex.Model;
using System.Collections.Generic;

namespace AntiPShared
{
    public class TextComparer
    {
        public static DiffResult CompareByWords(string oldText, string newText)
        {
            char[] _wordSeparators = { ' ' };

            var differ = new Differ();
            return differ.CreateWordDiffs(newText, oldText, true, _wordSeparators);
            //return diffResuls = differ.CreateWordDiffs(oldText, newText, true, _wordSeparators);
        }

        //private void CompareByLines(string oldText, string newText)
        //{
        //    var diffBuilder = new InlineDiffBuilder(new Differ());
        //    var diff = diffBuilder.BuildDiffModel(oldText, newText);

        //    foreach (var line in diff.Lines)
        //    {
        //        Color textColor;
        //        string text;

        //        switch (line.Type)
        //        {
        //            case ChangeType.Inserted:
        //                textColor = Color.Red;
        //                text = "+ ";
        //                break;

        //            case ChangeType.Deleted:
        //                textColor = Color.Green;
        //                text = "- ";
        //                break;

        //            default:
        //                textColor = Color.Black;
        //                text = "  ";
        //                break;
        //        }

        //        richTextBox1.AppendText(text + line.Text + Environment.NewLine, textColor);
        //    }
        //}

        public static Dictionary<string, List<string>> CommonTextParts(List<string> orderedUrls, Dictionary<string, List<int>> orderedUrlToWordsIndexes, string[] words, string[] webPagesTexts)
        {
            Dictionary<string, List<string>> urlToCommonTextParts = new Dictionary<string, List<string>>();
            for (int i = 0; i < webPagesTexts.Length; i++)
            {
                var shingleTexts = Logic.WordsIndexesToShingleTexts(words, orderedUrlToWordsIndexes[orderedUrls[i]]);

                List<string> commonTextParts = new List<string>();
                for (int j = 0; j < shingleTexts.Count; j++)
                {
                    var diffResuls = TextComparer.CompareByWords(shingleTexts[j], webPagesTexts[i]);
                    commonTextParts.Add(UnidiffFormater.CommonPart(diffResuls));
                }
                urlToCommonTextParts.Add(orderedUrls[i], commonTextParts);
            }
            return urlToCommonTextParts;
        }

        public static List<List<int>> FindWordsInIndexedText(List<string> input, Dictionary<string, List<int>> indexedText)
        {
            List<List<int>> result = new List<List<int>>();

            bool allFound = true;
            for (int i = 0; i < input.Count; i++)
            {
                if (indexedText.TryGetValue(input[i], out List<int> wordIndexes))
                {
                    result.Add(wordIndexes);
                }
                else
                {
                    allFound = false;
                }
            }

            return allFound ? result : new List<List<int>>();
        }

    }
}
