using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AntiPShared
{
    public class PlagiarismInWebFinder
    {
        public static async Task<Plagiarism<string>> FindAsync(string[] initialWords, Dictionary<int, string> initialDocIndexToSimplifiedWord, int[] initialDocIndexes, string[] simplifiedWords, string serverMapPath)
        {
            string debugLog = "";

            var plagiarismWeb = new Plagiarism<string>();

            Stopwatch stopwatch = new Stopwatch();
            //stopwatch.Start();
            //Dictionary<string, SortedSet<int>> urlToInitialDocWordsIndexesSet = GetUrlToInitialDocWordsIndexesFromGoogleAPIParallel(initialDocIndexes, simplifiedWords);
            //stopwatch.Stop();
            //Debug.WriteLine("GSEARCH TIME " + stopwatch.ElapsedMilliseconds);

            var fileName = "serializedGS.txt";
            var path = Path.Combine(serverMapPath, fileName);
            //var serializedGS = JsonConvert.SerializeObject(urlToInitialDocWordsIndexesSet);
            //File.WriteAllText(path, serializedGS);

            var urlToInitialDocWordsIndexesSet = JsonConvert.DeserializeObject<Dictionary<string, SortedSet<int>>>(File.ReadAllText(path));
            var urlToInitialDocWordsIndexesList = urlToInitialDocWordsIndexesSet.ToDictionary(pair => pair.Key, pair => pair.Value.ToList());

            var mostPopularUrls = GetMostPopularUrls(urlToInitialDocWordsIndexesSet);

            stopwatch.Restart();
            var urlsSimplifiedTexts = await WebManager.TextsAsync(mostPopularUrls);
            stopwatch.Stop();
            debugLog += "WebManager.TextsAsync TIME " + stopwatch.ElapsedMilliseconds + " ";

            stopwatch.Restart();
            var tasks = new Task<List<Plagiarism<string>>>[mostPopularUrls.Count];

            for (int i = 0; i < mostPopularUrls.Count; i++)
            {
                int savei = i;

                tasks[i] = Task<List<Plagiarism<string>>>.Factory.StartNew(() =>
                {
                    TextManager.PrepareText(urlsSimplifiedTexts[savei], out _, out _, out int[] urlInitialDocIndexes, out string[] urlSimplifiedWords, out _);
                    var indexedUrlText = Indexer.Indexing(urlInitialDocIndexes, urlSimplifiedWords);
                    var initialDocIndexesFoundOnUrl = urlToInitialDocWordsIndexesList[mostPopularUrls[savei]];

                    var plagiarismWebs = new List<Plagiarism<string>>();
                    for (int j = 0; j <= initialDocIndexesFoundOnUrl.Count - Shingle.Lenght; j++)
                    {
                        List<string> wordsList = new List<string>();
                        for (int k = 0; k < Shingle.Lenght; k++)
                        {
                            wordsList.Add(initialDocIndexToSimplifiedWord[initialDocIndexesFoundOnUrl[j + k]]);
                        }

                        var urlTextWordsPositionsForShingle = TextComparer.FindWordsInIndexedText(wordsList, indexedUrlText);
                        if (urlTextWordsPositionsForShingle.Count == 0) continue;

                        var initialDocIndexesForShingle = new List<int>();
                        for (int k = 0; k < Shingle.Lenght; k++)
                        {
                            initialDocIndexesForShingle.Add(initialDocIndexesFoundOnUrl[j + k]);
                        }

                        var urlToWebPageWordsPositionsForShingle = new Dictionary<string, List<List<int>>>
                        {
                            { mostPopularUrls[savei], urlTextWordsPositionsForShingle }
                        };

                        var plagiarismWebForShingle = Plagiarism<string>.FindPlagiarism(urlToWebPageWordsPositionsForShingle, initialDocIndexesForShingle);
                        plagiarismWebs.Add(plagiarismWebForShingle);
                    }

                    return plagiarismWebs;
                });
            }

            Task.WaitAll(tasks);

            for (int i = 0; i < mostPopularUrls.Count; i++)
            {
                var plagiarismWebs = tasks[i].Result;

                for (int j = 0; j < plagiarismWebs.Count; j++)
                {
                    var plagiarismWebForShingle = plagiarismWebs[j];
                    plagiarismWeb.Add(plagiarismWebForShingle);
                }
            }
            stopwatch.Stop();

            foreach (var kvp in plagiarismWeb.SourceIdToInitialWordsIndexes)
            {
                plagiarismWeb.SourceIdToInitialDocumentHtml.Add(kvp.Key, TextManager.ComposeHtmlText(initialWords, kvp.Value));
            }
            plagiarismWeb.DebugLogs = debugLog;
            return plagiarismWeb;
        }

        private static Dictionary<string, SortedSet<int>> GetUrlToInitialDocWordsIndexesFromGoogleAPIParallel(int[] initialDocIndexes, string[] simplifiedWords)
        {
            var shinglesCount = initialDocIndexes.Length - Shingle.Lenght;
            var tasks = new Task<List<string>>[shinglesCount + 1];

            for (int i = 0; i <= shinglesCount; i++)
            {
                var query = Shingle.QueryFromWords(simplifiedWords, i);
                tasks[i] = Task<List<string>>.Factory.StartNew(() => GoogleAPIManager.GetGoogleSearchResultsUrls(query));
            }

            Task.WaitAll(tasks);

            Dictionary<string, SortedSet<int>> urlToInitialDocWordsIndexes = new Dictionary<string, SortedSet<int>>();
            for (int i = 0; i <= shinglesCount; i++)
            {
                var urlsForShingle = tasks[i].Result;

                var initialDocIndexesForShingle = new SortedSet<int>();
                for (int j = 0; j < Shingle.Lenght; j++)
                {
                    initialDocIndexesForShingle.Add(initialDocIndexes[i + j]);
                }

                for (int j = 0; j < urlsForShingle.Count; j++)
                {
                    if (urlToInitialDocWordsIndexes.TryGetValue(urlsForShingle[j], out SortedSet<int> initialDocWordsIndexes))
                    {
                        initialDocWordsIndexes.UnionWith(initialDocIndexesForShingle);
                    }
                    else
                    {
                        urlToInitialDocWordsIndexes.Add(urlsForShingle[j], new SortedSet<int>(initialDocIndexesForShingle));
                    }
                }
            }

            return urlToInitialDocWordsIndexes;
        }

        private static List<string> GetMostPopularUrls(Dictionary<string, SortedSet<int>> urlToInitialDocWordsIndexes)
        {
            urlToInitialDocWordsIndexes = urlToInitialDocWordsIndexes.Where(kvp => kvp.Value.Count >= Plagiarism<string>.WordCap).ToDictionary(pair => pair.Key, pair => pair.Value);

            Dictionary<int, (string url, int indexesCount)> initialDocWordIndexToMostPopularUrlAndIndexesCount = new Dictionary<int, (string url, int indexesCount)>();

            foreach (var urlAndIndexes in urlToInitialDocWordsIndexes)
            {
                foreach (var index in urlAndIndexes.Value)
                {
                    if (initialDocWordIndexToMostPopularUrlAndIndexesCount.TryGetValue(index, out (string url, int indexesCount) urlAndIndexesCount))
                    {
                        if (urlAndIndexes.Value.Count > urlAndIndexesCount.indexesCount)
                        {
                            initialDocWordIndexToMostPopularUrlAndIndexesCount[index] = (urlAndIndexes.Key, urlAndIndexes.Value.Count);
                        }
                    }
                    else
                    {
                        initialDocWordIndexToMostPopularUrlAndIndexesCount.Add(index, (urlAndIndexes.Key, urlAndIndexes.Value.Count));
                    }
                }
            }

            return initialDocWordIndexToMostPopularUrlAndIndexesCount.Values.Select(x => x.url).Distinct().ToList();
        }

    }
}