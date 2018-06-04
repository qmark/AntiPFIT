using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AntiPShared
{
    public class PlagiarismInWebFinder
    {
        public static async Task<PlagiarismInWeb> Find(string simplifiedText)
        {
            var words = TextManager.WordsFromText(simplifiedText).ToArray();

            var wordCount = words.Length;

            string path = @"D:\urlToWordsIndexes.txt";
            //var shingles = Shingle.ShinglesFromWords(words);
            //var urlToWordsIndexes = WebManager.WebSitesFromWordsParallel(shingles);
            //File.WriteAllText(path, JsonConvert.SerializeObject(urlToWordsIndexes));
            var urlToWordsIndexes = JsonConvert.DeserializeObject<Dictionary<string, List<int>>>(File.ReadAllText(path));

            var orderedUrlToWordsIndexes = urlToWordsIndexes.OrderByDescending(kvp => kvp.Value.Count).ToDictionary(pair => pair.Key, pair => pair.Value);
            var orderedUrls = orderedUrlToWordsIndexes.Keys.ToList();

            var urlsCountCap = (int)Math.Ceiling(wordCount * 0.1);
            var webPagesSimplifiedTexts = await WebManager.UrlsToSimplifiedTextsAsync(urlsCountCap, orderedUrls);

            var urlToCommonTextParts = TextComparer.CommonTextParts(orderedUrls, orderedUrlToWordsIndexes, words, webPagesSimplifiedTexts);

            double originalTextCharactersCount = simplifiedText.RemoveWhiteSpaces().Length;
            var webResults = Logic.GetCheckResults(orderedUrls, urlsCountCap, urlToCommonTextParts, originalTextCharactersCount);

            var orderedWebResults = webResults.OrderByDescending(res => res.CharactersPercentage).ToList();

            return new PlagiarismInWeb
            {
                Words = words,
                WordCount = wordCount,
                OrderedUrlToWordsIndexes = orderedUrlToWordsIndexes,
                OrderedUrls = orderedUrls,
                OrderedWebResults = orderedWebResults
            };
        }
    }
}
