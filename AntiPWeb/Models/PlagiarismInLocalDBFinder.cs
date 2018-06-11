using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AntiPShared
{
    public class PlagiarismInLocalDBFinder
    {
        public static async Task<Plagiarism<int>> FindAsync(string[] initialWords, Dictionary<int, string> initialDocIndexToSimplifiedWord, int[] initialDocIndexes, string[] simplifiedWords)
        {
            string debugStr = "";
            var plagiarismDB = new Plagiarism<int>();

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            var shinglesCount = initialDocIndexes.Length - Shingle.Lenght;
            var tasks = new Task<Plagiarism<int>>[shinglesCount + 1];

            for (int i = 0; i <= shinglesCount; i++)
            {
                int savei = i;

                tasks[i] = Task<Plagiarism<int>>.Factory.StartNew(() =>
                {
                    List<string> wordsList = new List<string>();
                    for (int j = 0; j < Shingle.Lenght; j++)
                    {
                        wordsList.Add(initialDocIndexToSimplifiedWord[initialDocIndexes[savei + j]]);
                    }

                    var documentIdToDBDocWordsPositionsForShingle = SQLLoader.GetDocuments(wordsList);

                    var initialDocIndexesForShingle = new List<int>();
                    for (int j = 0; j < Shingle.Lenght; j++)
                    {
                        initialDocIndexesForShingle.Add(initialDocIndexes[savei + j]);
                    }

                    var plagiarismDBForShingle = Logic.FindPlagiarism(documentIdToDBDocWordsPositionsForShingle, initialDocIndexesForShingle);
                    return plagiarismDBForShingle;
                });
            }

            Task.WaitAll(tasks);

            for (int i = 0; i <= shinglesCount; i++)
            {
                var plagiarismDBForShingle = tasks[i].Result;
                plagiarismDB.Add(plagiarismDBForShingle);
            }
            stopwatch.Stop();
            debugStr += "DB PLAG TIME " + stopwatch.ElapsedMilliseconds +" ";

            foreach (var kvp in plagiarismDB.SourceIdToInitialWordsIndexes)
            {
                plagiarismDB.SourceIdToInitialDocumentHtml.Add(kvp.Key, TextManager.ComposeHtmlText(initialWords, kvp.Value));
            }
            plagiarismDB.DebugLogs = debugStr;
            return plagiarismDB;
        }
    }
}