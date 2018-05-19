using GoogleCSE;
using System;
using System.Threading.Tasks;

namespace AntiPShared
{
    public class GoogleAPIManager
    {
        public static async Task<string> GetGoogleSearchResults(string query)
        {
            string ApiKey = "AIzaSyDAKlpT9R-SmDjfbPZBJtNSv1SE0O_6UtY";
            string SystemID = "011958540907442234339:3amnjvx0hhq";

            string result = $"QUERY: {query}" + Environment.NewLine;

            var gs = new GoogleSearch(SystemID, ApiKey, maxPages: 1, pageSize: 10);

            var results = gs.Search(query);

            for (int i = 0; i < results.Count; i++)
            {
                result += "RESULT: " + i + Environment.NewLine;
                result += "TITLE: " + results[i].Title + Environment.NewLine;
                result += "DESC: " + results[i].Description + Environment.NewLine;
                result += "URL: " + results[i].Url + Environment.NewLine;
                result += "MIME: " + results[i].Mime + Environment.NewLine;
                result += Environment.NewLine;
            }

            return result;
        }

    }
}
