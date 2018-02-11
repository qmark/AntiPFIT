namespace Antiplagiarism
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
    }
}
