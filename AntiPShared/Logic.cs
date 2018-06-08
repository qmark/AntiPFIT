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
    }
}
