namespace AntiPShared
{
    public class Plagiarism
    {
        public string[] InitialWords { get; set; }
        public string[] SimplifiedWords { get; set; }
        public int WordCount { get; set; }
        public double Vodnost { get; set; }
        public double Toshnotnost { get; set; }
        public string AllPlagiarismHtmlText { get; set; }
        public PlagiarismWeb PlagiarismWeb { get; set; }
        public PlagiarismDB PlagiarismDB { get; set; }
    }
}