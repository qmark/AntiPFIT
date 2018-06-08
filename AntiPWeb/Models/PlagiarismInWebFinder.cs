using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AntiPShared
{
    public class PlagiarismInWebFinder
    {
        public static async Task<PlagiarismInWebResult> FindAsync(string serverMapPath, string[] initialWords, Dictionary<int, string> initialDocIndexToSimplifiedWord, int[] initialDocIndexes, string[] simplifiedWords, int wordCount)
        {
            //return new PlagiarismInWebResult { PlagiarismWeb = new PlagiarismWeb() };

            var plagiarismWeb = new PlagiarismWeb();

            Stopwatch stopwatch = new Stopwatch();
            //stopwatch.Start();
            //Dictionary<string, SortedSet<int>> urlToInitialDocWordsIndexes = GetUrlToInitialDocWordsIndexesFromGoogleAPIParallel(initialDocIndexes, simplifiedWords);
            //stopwatch.Stop();
            //Debug.WriteLine("GSEARCH TIME " + stopwatch.ElapsedMilliseconds);

            var fileName = "serializedGS.txt";
            var path = Path.Combine(serverMapPath, fileName);
            //var serializedGS = JsonConvert.SerializeObject(urlToInitialDocWordsIndexes);
            //File.WriteAllText(path, serializedGS);

            var urlToInitialDocWordsIndexes = JsonConvert.DeserializeObject<Dictionary<string, SortedSet<int>>>(File.ReadAllText(path));

            var orderedUrlToInitialDocWordsIndexes = urlToInitialDocWordsIndexes.OrderByDescending(kvp => kvp.Value.Count).ToDictionary(pair => pair.Key, pair => pair.Value.ToList());
            var orderedUrls = orderedUrlToInitialDocWordsIndexes.Keys.ToList();

            var urlsCountCap = (int)Math.Ceiling(wordCount * 0.1);
            stopwatch.Restart();
            var orderedUrlsSimplifiedTexts = await WebManager.TextsAsync(urlsCountCap, orderedUrls);
            stopwatch.Stop();
            Debug.WriteLine("WebManager.TextsAsync TIME " + stopwatch.ElapsedMilliseconds);

            stopwatch.Restart();
            for (int i = 0; i < urlsCountCap; i++)
            {
                TextManager.PrepareText(orderedUrlsSimplifiedTexts[i], out _, out _, out int[] urlInitialDocIndexes, out string[] urlSimplifiedWords, out _);
                var indexedUrlText = Logic.Indexing(urlInitialDocIndexes, urlSimplifiedWords);
                var initialDocIndexesFoundOnUrl = orderedUrlToInitialDocWordsIndexes[orderedUrls[i]];

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
                        { orderedUrls[i], urlTextWordsPositionsForShingle }
                    };

                    var plagiarismWebForShingle = Logic.FindPlagiarism(urlToWebPageWordsPositionsForShingle, initialDocIndexesForShingle);
                    foreach (var kvp in plagiarismWebForShingle.UrlToWebPageWordsIndexes)
                    {
                        if (plagiarismWeb.UrlToWebPageWordsIndexes.TryGetValue(kvp.Key, out HashSet<int> plagiarizedWebPageWordsIndexes))
                        {
                            plagiarizedWebPageWordsIndexes.UnionWith(kvp.Value);
                            plagiarismWeb.UrlToInitialWordsIndexes[kvp.Key].UnionWith(plagiarismWebForShingle.UrlToInitialWordsIndexes[kvp.Key]);
                        }
                        else
                        {
                            plagiarismWeb.UrlToWebPageWordsIndexes.Add(kvp.Key, new HashSet<int>(kvp.Value));
                            plagiarismWeb.UrlToInitialWordsIndexes.Add(kvp.Key, new HashSet<int>(plagiarismWebForShingle.UrlToInitialWordsIndexes[kvp.Key]));
                        }
                    }
                    foreach (var kvp in plagiarismWebForShingle.InitialWordIndexToUrls)
                    {
                        if (plagiarismWeb.InitialWordIndexToUrls.TryGetValue(kvp.Key, out HashSet<string> sourceUrls))
                        {
                            sourceUrls.UnionWith(kvp.Value);
                        }
                        else
                        {
                            plagiarismWeb.InitialWordIndexToUrls.Add(kvp.Key, new HashSet<string>(kvp.Value));
                        }
                    }
                }
            }
            stopwatch.Stop();
            Debug.WriteLine("FIND WEB PLAGIARISM ON TOP URLS TIME " + stopwatch.ElapsedMilliseconds);

            foreach (var kvp in plagiarismWeb.UrlToInitialWordsIndexes)
            {
                plagiarismWeb.UrlToInitialDocumentHtml.Add(kvp.Key, TextManager.ComposeHtmlText(initialWords, kvp.Value));
            }

            return new PlagiarismInWebResult
            {
                PlagiarismWeb = plagiarismWeb
            };
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
    }
}