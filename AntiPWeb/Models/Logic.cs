using System;
using System.Collections.Generic;
using System.Linq;

namespace AntiPShared
{
    public class Logic
    {

        //Создает словарь слова - позиции для одного дока
        public static Dictionary<string, List<int>> Indexing(string[] words)
        {
            var wordToPositions = new Dictionary<string, List<int>>();

            for (int position = 0; position < words.Length; position++)
            {
                if (wordToPositions.TryGetValue(words[position], out List<int> positions))
                {
                    positions.Add(position);
                }
                else
                {
                    wordToPositions.Add(words[position], new List<int> { position });
                }
            }

            return wordToPositions;
        }

        public static Dictionary<string, string> IndexingForDB(string[] words)
        {
            var wordToPositions = new Dictionary<string, string>();

            for (int position = 0; position < words.Length; position++)
            {
                if (wordToPositions.ContainsKey(words[position]))
                {
                    wordToPositions[words[position]] += $",{position}";
                }
                else
                {
                    wordToPositions.Add(words[position], position.ToString());
                }
            }

            return wordToPositions;
        }

        public static List<CheckResult> GetCheckResults(List<string> orderedUrls, int urlCountCap, Dictionary<string, List<string>> urlToCommonTextParts, double originalTextCharactersCount)
        {
            List<CheckResult> results = new List<CheckResult>();
            for (int i = 0; i < urlCountCap; i++)
            {
                CheckResult result = new CheckResult();
                result.Url = orderedUrls[i];

                result.CommonTextParts = urlToCommonTextParts[orderedUrls[i]];

                string commonText = string.Empty;
                foreach (var commonTextPart in result.CommonTextParts)
                {
                    commonText += commonTextPart;
                }

                result.CharactersPercentage = commonText.RemoveWhiteSpaces().Length / originalTextCharactersCount;

                results.Add(result);
            }
            return results;
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

        public static Dictionary<int, List<(int, int)>> FindPlagiarism(Dictionary<int, List<List<int>>> documentIdsToWordPositions, int firstWordInitialDocIndex)
        {
            const int MagicDistanceBetweenIndexes = 3;

            // documentID to words' indexes
            var plagiarism = new Dictionary<int, List<(int, int)>>();

            foreach (var kvp in documentIdsToWordPositions)
            {
                for (int firstWordPosition = 0; firstWordPosition < kvp.Value[0].Count; firstWordPosition++)
                {
                    var wordsPositons = new List<(int DBDocIndex, int initialDocIndex)>
                    {
                        (kvp.Value[0][firstWordPosition], firstWordInitialDocIndex)
                    };

                    bool hasEverything = true;
                    //                                   kvp.Value.Count == Shingle.Lenght
                    for (int i = 1; hasEverything && i < kvp.Value.Count; i++)
                    {
                        int wordIndexOnDistance = kvp.Value[i].Find(item => Math.Abs(item - wordsPositons[i - 1].DBDocIndex) < MagicDistanceBetweenIndexes);

                        if (wordIndexOnDistance != 0)
                        {
                            wordsPositons.Add((wordIndexOnDistance, firstWordInitialDocIndex + i));
                        }
                        else
                        {
                            hasEverything = false;
                            break;
                        }
                    }

                    if (hasEverything)
                    {
                        if (plagiarism.TryGetValue(kvp.Key, out List<(int, int)> plagiarizedPositions))
                        {
                            plagiarizedPositions.AddRange(wordsPositons);
                        }
                        else
                        {
                            plagiarism.Add(kvp.Key, wordsPositons);
                        }
                    }
                }
            }

            return plagiarism;
        }

    }
}
