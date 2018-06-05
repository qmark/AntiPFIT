namespace AntiPShared
{
    public class PlagiarismInLocalDB
    {
        public string[] InitialWords { get; set; }
        public string[] SimplifiedWords { get; set; }
        public int WordCount { get; set; }
        public PlagiarismDB PlagiarismDB { get; set; }
        public double Vodnost { get; set; }
        public double Toshnotnost { get; set; }
        public string HtmlText { get; set; }
    }
}