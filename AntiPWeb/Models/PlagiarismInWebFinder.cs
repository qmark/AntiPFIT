using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AntiPShared
{
    public class PlagiarismInWebFinder
    {
        public static async Task<PlagiarismInWebResult> Find(string[] initialWords, int[] initialDocIndexes, string[] simplifiedWords, int wordCount, double originalTextCharactersCount)
        {
            var plagiarismWeb = new PlagiarismWeb();

            Dictionary<string, SortedSet<int>> urlToInitialDocWordsIndexes = new Dictionary<string, SortedSet<int>>();
            for (int i = 0; i <= initialDocIndexes.Length - Shingle.Lenght; i++)
            {
                var query = Shingle.QueryFromWords(simplifiedWords, i);
                var urlsForShingle = GoogleAPIManager.GetGoogleSearchResultsUrls(query);

                var initialDocIndexesForShingle = new SortedSet<int>();
                for (int j = 0; j < Shingle.Lenght; j++)
                {
                    initialDocIndexesForShingle.Add(initialDocIndexes[i + j]);
                }

                for (int k = 0; k < urlsForShingle.Count; k++)
                {
                    if (urlToInitialDocWordsIndexes.TryGetValue(urlsForShingle[k], out SortedSet<int> initialDocWordsIndexes))
                    {
                        initialDocWordsIndexes.UnionWith(initialDocIndexesForShingle);
                    }
                    else
                    {
                        urlToInitialDocWordsIndexes.Add(urlsForShingle[k], initialDocIndexesForShingle);
                    }
                }
            }

            var orderedUrlToInitialDocWordsIndexes = urlToInitialDocWordsIndexes.OrderByDescending(kvp => kvp.Value.Count).ToDictionary(pair => pair.Key, pair => pair.Value.ToList());
            var orderedUrls = orderedUrlToInitialDocWordsIndexes.Keys.ToList();

            var urlsCountCap = (int)Math.Ceiling(wordCount * 0.1);
            var orderedUrlsSimplifiedTexts = await WebManager.TextsAsync(urlsCountCap, orderedUrls);

            for (int i = 0; i < urlsCountCap; i++)
            {
                TextManager.PrepareText(orderedUrlsSimplifiedTexts[i], out string[] urlInitialWords, out int[] urlInitialDocIndexes, out string[] urlSimplifiedWords, out int urlWordCount);
                var indexedUrlText = Logic.Indexing(urlSimplifiedWords);
                var initialIndexesFoundOnUrl = orderedUrlToInitialDocWordsIndexes[orderedUrls[i]];

                for (int j = 0; j <= initialIndexesFoundOnUrl.Count - Shingle.Lenght; j++)
                {
                    var urlTextWordsPositionsForShingle = TextComparer.FindWordsInIndexedText(Shingle.ListFromWords(urlSimplifiedWords, j), indexedUrlText);

                    var initialDocIndexesForShingle = new List<int>();
                    for (int k = 0; k < Shingle.Lenght; k++)
                    {
                        initialDocIndexesForShingle.Add(initialIndexesFoundOnUrl[j + k]);
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
                            plagiarismWeb.UrlToWebPageWordsIndexes.Add(kvp.Key, kvp.Value);
                            plagiarismWeb.UrlToInitialWordsIndexes.Add(kvp.Key, plagiarismWebForShingle.UrlToInitialWordsIndexes[kvp.Key]);
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
                            plagiarismWeb.InitialWordIndexToUrls.Add(kvp.Key, kvp.Value);
                        }
                    }
                }
            }

            foreach (var kvp in plagiarismWeb.UrlToInitialWordsIndexes)
            {
                plagiarismWeb.UrlToInitialDocumentHtml.Add(kvp.Key, TextManager.ComposeHtmlText(initialWords, kvp.Value));
            }

            return new PlagiarismInWebResult
            {
                PlagiarismWeb = plagiarismWeb
            };
        }
    }
}