using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using AntiPShared;
using System.Linq;

namespace FilesIndexer
{
    class Program
    {
        //private void dbconnection()
        //{

        //    for (int i = 0; i < 100; i++)
        //    {
        //        SqlDbConnection SQL = new SqlDbConnection();
        //        SQL.Procedurename = "readFromTbl";
        //        SQL.AddParameter("command", "getText");
        //        SQL.ExecuteObject();
        //        for (int j = 0; j < SQL.ResultRowAmount; j++)
        //        {
        //            textBox2.Text += i + " ";           
        //                textBox1.Text += SQL.GetFieldByName(j, "text");

        //        }

        //        string ss = "Inverted indexes are the most fundamental and widely used data structures in information retrieval. For each unique word occurring in a document collection, the inverted index indexes which work naturally for strings as";
        //    }

        //}
        private static Dictionary<int, List<List<int>>> GetDocuments(List<String> input)
        {
            Dictionary<int, List<List<int>>> result = new Dictionary<int, List<List<int>>>();
           
            SqlDbConnection SQL = new SqlDbConnection();
            SQL.Procedurename = "readFromTbl";
            SQL.AddParameter("command", "getDocuments");
            SQL.AddParameter("firstWord", input[0]);
            SQL.AddParameter("secondWord", input[1]);
            SQL.AddParameter("thirdWord", input[2]);
            SQL.AddParameter("fourthWord", input[3]);
            SQL.AddParameter("fifthWord", input[4]);
            SQL.ExecuteObject();
            for (int j = 0; j < SQL.ResultRowAmount; j++)
            {
                
                List<List<int>> PositionsLists = new List<List<int>>();
                PositionsLists.Add(ParsePositions(SQL.GetFieldByName(j, "FirstList")));
                PositionsLists.Add(ParsePositions(SQL.GetFieldByName(j, "SecondList")));
                PositionsLists.Add(ParsePositions(SQL.GetFieldByName(j, "ThirdList")));
                PositionsLists.Add(ParsePositions(SQL.GetFieldByName(j, "FourthList")));
                PositionsLists.Add(ParsePositions(SQL.GetFieldByName(j, "FifthList")));
                result.Add(Convert.ToInt32(SQL.GetFieldByName(j, "DocumentId")), PositionsLists);
            }
            return result;
            
        }
        private static void AddDoc(Dictionary<String, String> words)
        {
            Dictionary<int, List<List<int>>> result = new Dictionary<int, List<List<int>>>();

            SqlDbConnection SQL = new SqlDbConnection();

            var dt = ConvertToDataTable(words);
            SQL.Procedurename = "readFromTbl";
            SQL.AddParameter("command", "getDocuments");
           
            SQL.AddDict("go", dt);
            for (int j = 0; j < SQL.ResultRowAmount; j++)
            {

                
            }
            

        }




        private static DataTable ConvertToDataTable(Dictionary<string, string> dict)
        {
            var dt = new DataTable();
            dt.Columns.Add("Word", typeof(string));
            dt.Columns.Add("Positions", typeof(string));
            foreach (var pair in dict)
            {
                var row = dt.NewRow();
                row["Word"] = pair.Key;
                row["Positions"] = pair.Value;
                dt.Rows.Add(row);
            }
            return dt;
        }


        public static List<int> ParsePositions(String input)
        {
            int[] myInts = Array.ConvertAll(input.Split(','), s => int.Parse(s));
            return myInts.OfType<int>().ToList();
        }



        static void Main(string[] args)
        {
            string str = "";
            List<String> example = new List<string> { "oleg", "vinnik", "gadkiy", "utenok", "kal" };
            foreach (KeyValuePair<int, List<List<int>>> pair in GetDocuments(example))
            {
              
                Console.WriteLine("Документ - " + pair.Key);
                foreach (List<int> positions in pair.Value)
                {
                    foreach (int pos in positions)
                    {
                        str += Convert.ToString(pos) + " ";
                    }
                    Console.WriteLine("  Позиции - " + str);
                    str = "";
                }
            }
            Console.ReadKey();

        }



    }
}