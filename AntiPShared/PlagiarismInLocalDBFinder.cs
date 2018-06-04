using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AntiPShared
{
    public class PlagiarismInLocalDBFinder
    {
        public static async Task<PlagiarismInLocalDB> Find(string simplifiedText)
        {
            var words = TextManager.WordsFromText(simplifiedText).ToArray();
            var wordCount = words.Length;

            var plagiarismResult = new Dictionary<int, HashSet<(int DBDocIndex, int initialDocIndex)>>();
            double vodnost = 0;
            for (int i = 0; i <= words.Length - Shingle.Lenght; i++)
            {
                if (TextManager.StopWords.Contains(words[i]))
                    vodnost++;

                var documentIdToWordsPositions = SQLLoader.GetDocuments(Shingle.ListFromWords(words, i));

                var plagiarismForShingle = Logic.FindPlagiarism(documentIdToWordsPositions, i);
                foreach (var kvp in plagiarismForShingle)
                {
                    if (plagiarismResult.TryGetValue(kvp.Key, out HashSet<(int DBDocIndex, int initialDocIndex)> plagiarizedPositions))
                    {
                        plagiarizedPositions.UnionWith(kvp.Value);
                    }
                    else
                    {
                        plagiarismResult.Add(kvp.Key, new HashSet<(int, int)>(kvp.Value));
                    }
                }

                Console.WriteLine(i);
            }

            for (int i = words.Length - Shingle.Lenght + 1; i < words.Length; i++)
            {
                if (TextManager.StopWords.Contains(words[i]))
                    vodnost++;
            }

            vodnost /= Convert.ToDouble(words.Length);
            double toshnotnost = (words.Length - words.Distinct().Count()) / Convert.ToDouble(words.Length);

            return new PlagiarismInLocalDB
            {
                Words = words,
                WordCount = wordCount,
                PlagiarismResult = plagiarismResult,
                Vodnost = vodnost,
                Toshnotnost = toshnotnost
            };
        }
    }
}
