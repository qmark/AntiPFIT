using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AntiPShared
{
    public class PlagiarismInLocalDBFinder
    {
        public static async Task<PlagiarismInLocalDB> Find(string initialText)
        {
            var initialWords = TextManager.WordsFromText(initialText).ToArray();
            var initialDocIndexToSimplifiedWord = new Dictionary<int, string>();
            for (int initialDocIndex = 0; initialDocIndex < initialWords.Length; initialDocIndex++)
            {
                var currentInitialWord = initialWords[initialDocIndex];
                if (!(currentInitialWord.Length == 1 && char.IsPunctuation(Convert.ToChar(currentInitialWord))))
                    initialDocIndexToSimplifiedWord.Add(initialDocIndex, TextManager.SimplifyText(currentInitialWord));
            }

            var initialDocIndexes = initialDocIndexToSimplifiedWord.Keys.ToArray();
            var simplifiedWords = initialDocIndexToSimplifiedWord.Values.ToArray();
            var wordCount = simplifiedWords.Length;

            var plagiarismDB = new PlagiarismDB();
            double vodnost = 0;
            for (int i = 0; i <= initialDocIndexes.Length - Shingle.Lenght; i++)
            {
                if (TextManager.StopWords.Contains(simplifiedWords[i]))
                    vodnost++;

                var documentIdToDBDocWordsPositionsForShingle = SQLLoader.GetDocuments(Shingle.ListFromWords(simplifiedWords, i));

                var initialDocIndexesForShingle = new List<int>();
                for (int j = 0; j < Shingle.Lenght; j++)
                {
                    initialDocIndexesForShingle.Add(initialDocIndexes[i + j]);
                }

                var plagiarismDBForShingle = Logic.FindPlagiarism(documentIdToDBDocWordsPositionsForShingle, initialDocIndexesForShingle);
                foreach (var kvp in plagiarismDBForShingle.DocumentIdToDBWordsIndexes)
                {
                    if (plagiarismDB.DocumentIdToDBWordsIndexes.TryGetValue(kvp.Key, out HashSet<int> plagiarizedDBWordsIndexes))
                    {
                        plagiarizedDBWordsIndexes.UnionWith(kvp.Value);
                        plagiarismDB.DocumentIdToInitialWordsIndexes[kvp.Key].UnionWith(plagiarismDBForShingle.DocumentIdToInitialWordsIndexes[kvp.Key]);
                    }
                    else
                    {
                        plagiarismDB.DocumentIdToDBWordsIndexes.Add(kvp.Key, kvp.Value);
                        plagiarismDB.DocumentIdToInitialWordsIndexes.Add(kvp.Key, plagiarismDBForShingle.DocumentIdToInitialWordsIndexes[kvp.Key]);
                    }
                }
                foreach (var kvp in plagiarismDBForShingle.InitialWordIndexToDocumentIds)
                {
                    if (plagiarismDB.InitialWordIndexToDocumentIds.TryGetValue(kvp.Key, out HashSet<int> sourceDocumentIds))
                    {
                        sourceDocumentIds.UnionWith(kvp.Value);
                    }
                    else
                    {
                        plagiarismDB.InitialWordIndexToDocumentIds.Add(kvp.Key, kvp.Value);
                    }
                }
            }

            for (int i = simplifiedWords.Length - Shingle.Lenght + 1; i < simplifiedWords.Length; i++)
            {
                if (TextManager.StopWords.Contains(simplifiedWords[i]))
                    vodnost++;
            }

            vodnost /= Convert.ToDouble(simplifiedWords.Length);
            double toshnotnost = (simplifiedWords.Length - simplifiedWords.Distinct().Count()) / Convert.ToDouble(simplifiedWords.Length);

            string htmlText = ComposeHtmlText(initialWords, plagiarismDB.InitialWordIndexToDocumentIds.Keys);

            foreach (var kvp in plagiarismDB.DocumentIdToInitialWordsIndexes)
            {
                plagiarismDB.DocumentIdToInitialDocumentHtml.Add(kvp.Key, ComposeHtmlText(initialWords, kvp.Value));
            }

            return new PlagiarismInLocalDB
            {
                InitialWords = initialWords,
                SimplifiedWords = simplifiedWords,
                WordCount = wordCount,
                PlagiarismDB = plagiarismDB,
                Vodnost = vodnost,
                Toshnotnost = toshnotnost,
                HtmlText = htmlText
            };
        }

        private static string ComposeHtmlText(string[] initialWords, IEnumerable<int> plagiarismIndexes)
        {
            bool plagiarizedTagOpened = plagiarismIndexes.Contains(0) ? true : false;
            var openTag = "<span style=\"color: #ff0000\">";
            var closeTag = "</span>";
            var htmlText = plagiarizedTagOpened ? openTag : "";
            for (int initialDocIndex = 0; initialDocIndex < initialWords.Length; initialDocIndex++)
            {
                if (plagiarismIndexes.Contains(initialDocIndex))
                {
                    if (plagiarizedTagOpened)
                        htmlText += initialWords[initialDocIndex] + " ";
                    else
                    {
                        htmlText += $"{openTag}{initialWords[initialDocIndex]} ";
                        plagiarizedTagOpened = true;
                    }
                }
                else
                {
                    if (plagiarizedTagOpened)
                    {
                        htmlText += closeTag + initialWords[initialDocIndex] + " ";
                        plagiarizedTagOpened = false;
                    }
                    else
                        htmlText += initialWords[initialDocIndex] + " ";
                }
            }
            if (plagiarizedTagOpened)
                htmlText += closeTag;
            return htmlText;
        }
    }
}