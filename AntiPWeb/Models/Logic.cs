using System;
using System.Collections.Generic;

namespace AntiPShared
{
    public class Logic
    {
        //Создает словарь слова - позиции для одного дока
        public static Dictionary<string, List<int>> Indexing(int[] initialDocIndexes, string[] simplifiedWords)
        {
            var wordToPositions = new Dictionary<string, List<int>>();

            for (int i = 0; i < simplifiedWords.Length; i++)
            {
                if (wordToPositions.TryGetValue(simplifiedWords[i], out List<int> positions))
                {
                    positions.Add(initialDocIndexes[i]);
                }
                else
                {
                    wordToPositions.Add(simplifiedWords[i], new List<int> { initialDocIndexes[i] });
                }
            }

            return wordToPositions;
        }

        public static Dictionary<string, string> IndexingForDB(int[] initialDocIndexes, string[] simplifiedWords)
        {
            var wordToPositions = new Dictionary<string, string>();

            for (int i = 0; i < simplifiedWords.Length; i++)
            {
                if (wordToPositions.ContainsKey(simplifiedWords[i]))
                {
                    wordToPositions[simplifiedWords[i]] += $",{initialDocIndexes[i]}";
                }
                else
                {
                    wordToPositions.Add(simplifiedWords[i], initialDocIndexes[i].ToString());
                }
            }

            return wordToPositions;
        }

        public static Dictionary<string, WebCheckResult> GetUrlToWebCheckResults(List<string> orderedUrls, int urlCountCap, Dictionary<string, List<string>> urlToCommonTextParts, double originalTextCharactersCount)
        {
            Dictionary<string, WebCheckResult> urlToWebCheckResults = new Dictionary<string, WebCheckResult>();

            for (int i = 0; i < urlCountCap; i++)
            {
                WebCheckResult result = new WebCheckResult
                {
                    CommonTextParts = urlToCommonTextParts[orderedUrls[i]]
                };

                string commonText = string.Empty;
                foreach (var commonTextPart in result.CommonTextParts)
                {
                    commonText += commonTextPart;
                }
                result.CharactersPercentage = commonText.RemoveWhiteSpaces().Length / originalTextCharactersCount;

                urlToWebCheckResults.Add(orderedUrls[i], result);
            }

            return urlToWebCheckResults;
        }

        //список шинглов
        public static List<string> WordsIndexesToShingleTexts(string[] words, List<int> wordsIndexes)
        {
            List<string> shingleTexts = new List<string>
            {
                Shingle.QueryFromWords(words, wordsIndexes[0])
            };

            for (int i = 1; i < wordsIndexes.Count; i++)
            {
                int overlap;
                if ((overlap = Shingle.Lenght - (wordsIndexes[i] - wordsIndexes[i - 1])) > 0)
                {
                    for (int j = overlap; j < Shingle.Lenght; j++)
                    {
                        shingleTexts[shingleTexts.Count - 1] += " " + words[wordsIndexes[i] + j];
                    }
                }
                else
                {
                    shingleTexts.Add(Shingle.QueryFromWords(words, wordsIndexes[i]));
                }
            }
            return shingleTexts;
        }

        private const int MagicDistanceBetweenIndexes = 3;

        public static PlagiarismDB FindPlagiarism(Dictionary<int, List<List<int>>> documentIdToDBDocWordsPositionsForShingle, List<int> initialDocIndexesForShingle)
        {
            var plagiarismDBForShingle = new PlagiarismDB();

            foreach (var kvp in documentIdToDBDocWordsPositionsForShingle)
            {
                for (int firstWordPosition = 0; firstWordPosition < kvp.Value[0].Count; firstWordPosition++)
                {
                    var DBWordsIndexes = new List<int>
                    {
                        kvp.Value[0][firstWordPosition]
                    };

                    var initialDocIndexes = new List<int>
                    {
                        initialDocIndexesForShingle[0]
                    };

                    bool hasEverything = true;
                    for (int i = 1; hasEverything && i < Shingle.Lenght; i++)
                    {
                        int wordIndexOnDistance = kvp.Value[i].Find(item => Math.Abs(item - DBWordsIndexes[i - 1]) < MagicDistanceBetweenIndexes);

                        if (wordIndexOnDistance != 0)
                        {
                            DBWordsIndexes.Add(wordIndexOnDistance);
                            initialDocIndexes.Add(initialDocIndexesForShingle[i]);
                        }
                        else
                        {
                            hasEverything = false;
                            break;
                        }
                    }

                    if (hasEverything)
                    {
                        if (plagiarismDBForShingle.DocumentIdToDBWordsIndexes.TryGetValue(kvp.Key, out HashSet<int> DBDocPlagiarizedPositions))
                        {
                            DBDocPlagiarizedPositions.UnionWith(DBWordsIndexes);
                            plagiarismDBForShingle.DocumentIdToInitialWordsIndexes[kvp.Key].UnionWith(initialDocIndexes);
                        }
                        else
                        {
                            plagiarismDBForShingle.DocumentIdToDBWordsIndexes.Add(kvp.Key, new HashSet<int>(DBWordsIndexes));
                            plagiarismDBForShingle.DocumentIdToInitialWordsIndexes.Add(kvp.Key, new HashSet<int>(initialDocIndexes));
                        }

                        if (plagiarismDBForShingle.InitialWordIndexToDocumentIds.ContainsKey(initialDocIndexesForShingle[0]))
                        {
                            for (int i = 0; i < Shingle.Lenght; i++)
                            {
                                plagiarismDBForShingle.InitialWordIndexToDocumentIds[initialDocIndexesForShingle[i]].Add(kvp.Key);
                            }
                        }
                        else
                        {
                            for (int i = 0; i < Shingle.Lenght; i++)
                            {
                                plagiarismDBForShingle.InitialWordIndexToDocumentIds.Add(initialDocIndexesForShingle[i], new HashSet<int>() { kvp.Key });
                            }
                        }
                    }
                }
            }

            return plagiarismDBForShingle;
        }

        public static PlagiarismWeb FindPlagiarism(Dictionary<string, List<List<int>>> urlToWebPageWordsPositionsForShingle, List<int> initialDocIndexesForShingle)
        {
            var plagiarismWebForShingle = new PlagiarismWeb();

            foreach (var kvp in urlToWebPageWordsPositionsForShingle)
            {
                for (int firstWordPosition = 0; firstWordPosition < kvp.Value[0].Count; firstWordPosition++)
                {
                    var webPageWordsIndexes = new List<int>
                    {
                        kvp.Value[0][firstWordPosition]
                    };

                    var initialDocIndexes = new List<int>
                    {
                        initialDocIndexesForShingle[0]
                    };

                    bool hasEverything = true;
                    for (int i = 1; hasEverything && i < Shingle.Lenght; i++)
                    {
                        int wordIndexOnDistance = kvp.Value[i].Find(item => Math.Abs(item - webPageWordsIndexes[i - 1]) < MagicDistanceBetweenIndexes);

                        if (wordIndexOnDistance != 0)
                        {
                            webPageWordsIndexes.Add(wordIndexOnDistance);
                            initialDocIndexes.Add(initialDocIndexesForShingle[i]);
                        }
                        else
                        {
                            hasEverything = false;
                            break;
                        }
                    }

                    if (hasEverything)
                    {
                        if (plagiarismWebForShingle.UrlToWebPageWordsIndexes.TryGetValue(kvp.Key, out HashSet<int> webPagePlagiarizedPositions))
                        {
                            webPagePlagiarizedPositions.UnionWith(webPageWordsIndexes);
                            plagiarismWebForShingle.UrlToInitialWordsIndexes[kvp.Key].UnionWith(initialDocIndexes);
                        }
                        else
                        {
                            plagiarismWebForShingle.UrlToWebPageWordsIndexes.Add(kvp.Key, new HashSet<int>(webPageWordsIndexes));
                            plagiarismWebForShingle.UrlToInitialWordsIndexes.Add(kvp.Key, new HashSet<int>(initialDocIndexes));
                        }

                        if (plagiarismWebForShingle.InitialWordIndexToUrls.ContainsKey(initialDocIndexesForShingle[0]))
                        {
                            for (int i = 0; i < Shingle.Lenght; i++)
                            {
                                plagiarismWebForShingle.InitialWordIndexToUrls[initialDocIndexesForShingle[i]].Add(kvp.Key);
                            }
                        }
                        else
                        {
                            for (int i = 0; i < Shingle.Lenght; i++)
                            {
                                plagiarismWebForShingle.InitialWordIndexToUrls.Add(initialDocIndexesForShingle[i], new HashSet<string>() { kvp.Key });
                            }
                        }
                    }
                }
            }

            return plagiarismWebForShingle;
        }

    }
}
