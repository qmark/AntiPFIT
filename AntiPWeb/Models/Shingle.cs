using System;
using System.Collections.Generic;

namespace AntiPShared
{
    public class Shingle
    {
        public const byte Lenght = 5;

        public string Query { get; private set; }
        public int FirstWordIndex { get; private set; }

        public Shingle(string[] words, int firstWordIndex)
        {
            FirstWordIndex = firstWordIndex;
            Query = QueryFromWords(words, FirstWordIndex);
        }

        public static string QueryFromWords(string[] words, int firstWordIndex)
        {
            string query = string.Empty;
            for (int i = 0; i < Shingle.Lenght - 1; i++)
            {
                query += words[firstWordIndex + i] + " ";
            }
            query += words[firstWordIndex + Shingle.Lenght - 1];
            return query;
        }

        public static List<string> ListFromWords(string[] words, int firstWordIndex)
        {
            List<string> wordsList = new List<string>();
            for (int i = 0; i < Shingle.Lenght; i++)
            {
                wordsList.Add(words[firstWordIndex + i]);
            }
            return wordsList;
        }

        public static List<Shingle> ShinglesFromWords(string[] words)
        {
            var shingles = new List<Shingle>();
            for (int i = 0; i <= words.Length - Shingle.Lenght; i++)
            {
                shingles.Add(new Shingle(words, i));
            }
            return shingles;
        }
    }
}
