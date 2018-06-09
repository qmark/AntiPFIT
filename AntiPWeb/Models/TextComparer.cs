using System.Collections.Generic;

namespace AntiPShared
{
    public class TextComparer
    {
        public static List<List<int>> FindWordsInIndexedText(List<string> input, Dictionary<string, List<int>> indexedText)
        {
            List<List<int>> result = new List<List<int>>();

            for (int i = 0; i < input.Count; i++)
            {
                if (indexedText.TryGetValue(input[i], out List<int> wordIndexes))
                {
                    result.Add(wordIndexes);
                }
                else
                {
                    return new List<List<int>>();
                }
            }

            return result;
        }
    }
}
