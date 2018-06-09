namespace AntiPShared
{
    public class PlagiarismResult
    {
        public string[] InitialWords { get; set; }
        public string[] SimplifiedWords { get; set; }
        public int WordCount { get; set; }
        public double Vodnost { get; set; }
        public double Toshnotnost { get; set; }
        public string AllPlagiarismHtmlText { get; set; }
        public Plagiarism<string> PlagiarismWeb { get; set; }
        public Plagiarism<int> PlagiarismDB { get; set; }
    }
}