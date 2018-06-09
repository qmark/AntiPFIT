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

        public static Plagiarism<TSourceId> FindPlagiarism<TSourceId>(Dictionary<TSourceId, List<List<int>>> sourceIdToSourceWordsPositionsForShingle, List<int> initialDocIndexesForShingle)
        {
            var plagiarismForShingle = new Plagiarism<TSourceId>();

            foreach (var kvp in sourceIdToSourceWordsPositionsForShingle)
            {
                for (int firstWordPosition = 0; firstWordPosition < kvp.Value[0].Count; firstWordPosition++)
                {
                    var sourceWordsIndexes = new List<int>
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
                        int wordIndexOnDistance = kvp.Value[i].Find(item => Math.Abs(item - sourceWordsIndexes[i - 1]) < MagicDistanceBetweenIndexes);

                        if (wordIndexOnDistance != 0)
                        {
                            sourceWordsIndexes.Add(wordIndexOnDistance);
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
                        if (plagiarismForShingle.SourceIdToSourceWordsIndexes.TryGetValue(kvp.Key, out HashSet<int> DBDocPlagiarizedPositions))
                        {
                            DBDocPlagiarizedPositions.UnionWith(sourceWordsIndexes);
                            plagiarismForShingle.SourceIdToInitialWordsIndexes[kvp.Key].UnionWith(initialDocIndexes);
                        }
                        else
                        {
                            plagiarismForShingle.SourceIdToSourceWordsIndexes.Add(kvp.Key, new HashSet<int>(sourceWordsIndexes));
                            plagiarismForShingle.SourceIdToInitialWordsIndexes.Add(kvp.Key, new HashSet<int>(initialDocIndexes));
                        }

                        if (plagiarismForShingle.InitialWordIndexToSourceIds.ContainsKey(initialDocIndexesForShingle[0]))
                        {
                            for (int i = 0; i < Shingle.Lenght; i++)
                            {
                                plagiarismForShingle.InitialWordIndexToSourceIds[initialDocIndexesForShingle[i]].Add(kvp.Key);
                            }
                        }
                        else
                        {
                            for (int i = 0; i < Shingle.Lenght; i++)
                            {
                                plagiarismForShingle.InitialWordIndexToSourceIds.Add(initialDocIndexesForShingle[i], new HashSet<TSourceId>() { kvp.Key });
                            }
                        }
                    }
                }
            }

            return plagiarismForShingle;
        }

    }
}
