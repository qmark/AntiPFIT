using System.Collections.Generic;

namespace AntiPShared
{
    public class Indexer
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
    }
}