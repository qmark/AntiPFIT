using System;
using System.Collections.Generic;

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

        public static PlagiarismDB FindPlagiarism(Dictionary<int, List<List<int>>> documentIdToDBDocWordsPositionsForShingle, List<int> initialDocIndexesForShingle)
        {
            const int MagicDistanceBetweenIndexes = 3;

            PlagiarismDB plagiarismDBForShingle = new PlagiarismDB();

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

    }
}
