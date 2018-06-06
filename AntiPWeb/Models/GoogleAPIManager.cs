using GoogleCSE;
using System.Collections.Generic;
using System.Linq;

namespace AntiPShared
{
    public class GoogleAPIManager
    {
        public static List<string> GetGoogleSearchResultsUrls(string query, int count = 10, int maxPages = 1)
        {
            string ApiKey = "AIzaSyDAKlpT9R-SmDjfbPZBJtNSv1SE0O_6UtY";
            string SystemID = "011958540907442234339:3amnjvx0hhq";

            var googleSearch = new GoogleSearch(SystemID, ApiKey, hl: "uk", pageSize: count, maxPages: maxPages);
            var GSResults = googleSearch.Search(query);

            //System.Diagnostics.Debug.WriteLine($"Query: {query}");
            //foreach (var res in GSResults)
            //{
            //    System.Diagnostics.Debug.WriteLine(res.Title);
            //    System.Diagnostics.Debug.WriteLine(res.Description);
            //    System.Diagnostics.Debug.WriteLine("");
            //}

            return GSResults.Select(result => result.Url).ToList();
        }
    }
}