using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace AntiPShared
{
    public class PlagiarismInLocalDBFinder
    {
        public static async Task<PlagiarismInLocalDBResult> FindAsync(string[] initialWords, Dictionary<int, string> initialDocIndexToSimplifiedWord, int[] initialDocIndexes, string[] simplifiedWords)
        {
            var plagiarismDB = new PlagiarismDB();
            double vodnost = 0;

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 0; i <= initialDocIndexes.Length - Shingle.Lenght; i++)
            {
                if (TextManager.StopWords.Contains(simplifiedWords[i]))
                    vodnost++;

                List<string> wordsList = new List<string>();
                for (int j = 0; j < Shingle.Lenght; j++)
                {
                    wordsList.Add(initialDocIndexToSimplifiedWord[initialDocIndexes[i + j]]);
                }

                var documentIdToDBDocWordsPositionsForShingle = SQLLoader.GetDocuments(wordsList);
                if (documentIdToDBDocWordsPositionsForShingle.Count == 0) continue;

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
                        plagiarismDB.DocumentIdToDBWordsIndexes.Add(kvp.Key, new HashSet<int>(kvp.Value));
                        plagiarismDB.DocumentIdToInitialWordsIndexes.Add(kvp.Key, new HashSet<int>(plagiarismDBForShingle.DocumentIdToInitialWordsIndexes[kvp.Key]));
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
                        plagiarismDB.InitialWordIndexToDocumentIds.Add(kvp.Key, new HashSet<int>(kvp.Value));
                    }
                }
            }
            stopwatch.Stop();
            Debug.WriteLine("DB PLAG TIME " + stopwatch.ElapsedMilliseconds);

            foreach (var kvp in plagiarismDB.DocumentIdToInitialWordsIndexes)
            {
                plagiarismDB.DocumentIdToInitialDocumentHtml.Add(kvp.Key, TextManager.ComposeHtmlText(initialWords, kvp.Value));
            }

            //
            for (int i = simplifiedWords.Length - Shingle.Lenght + 1; i < simplifiedWords.Length; i++)
            {
                if (TextManager.StopWords.Contains(simplifiedWords[i]))
                    vodnost++;
            }

            vodnost /= Convert.ToDouble(simplifiedWords.Length);
            double toshnotnost = (simplifiedWords.Length - simplifiedWords.Distinct().Count()) / Convert.ToDouble(simplifiedWords.Length);

            return new PlagiarismInLocalDBResult
            {
                Vodnost = vodnost,
                Toshnotnost = toshnotnost,
                PlagiarismDB = plagiarismDB
            };
        }
    }
}