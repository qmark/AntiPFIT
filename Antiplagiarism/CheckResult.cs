using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Antiplagiarism
{
    public class CheckResult
    {
        public string Url { get; set; }
        public List<string> CommonTextParts { get; set; }
        public double CharactersPercentage { get; set; }
    }
}
