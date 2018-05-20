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

        //для гугл поиска(общие части, проценты)
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

    }
}
