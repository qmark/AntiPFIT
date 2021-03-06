using AntiPShared;
using System;
using System.Collections.Generic;
using System.Data;
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


        private static void AddWords(Dictionary<String, String> words, String docName)
        {
            SqlDbConnection SQL = new SqlDbConnection();
            var dt = ConvertToDataTable(words, docName);
            SQL.Procedurename = "readFromTbl";
            SQL.AddParameter("command", "addWordPos");
            SQL.AddParameter("name", docName);
            SQL.AddDict("go", dt);
        }


        private static void AddDocument(String name, String text)
        {
            SqlDbConnection SQL = new SqlDbConnection();
            SQL.Procedurename = "readFromTbl";
            SQL.AddParameter("command", "addDoc");
            SQL.AddParameter("name", name);
            SQL.AddParameter("text", text);
            SQL.ExecuteObject();
        }

        private static DataTable ConvertToDataTable(Dictionary<string, string> dict, String docName)
        {
            var dt = new DataTable();
            dt.Columns.Add("Word", typeof(string));
            dt.Columns.Add("Positions", typeof(string));
            dt.Columns.Add("DocName", typeof(string));
            foreach (var pair in dict)
            {
                var row = dt.NewRow();
                row["Word"] = pair.Key;
                row["Positions"] = pair.Value;
                row["DocName"] = docName;
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
            //string nameFile = "��� 3-01 �������� �������-��������";
            //string test = TextDocumentManager.TextFromFile(@"C:\Users\alex1\Desktop\" + nameFile + ".doc");
            //AddDocument(nameFile, test);
            //test = TextManager.SimplifyText(test).Replace("\r\n", " ");
            //AddWords(Logic.IndexingForDB(TextManager.WordsFromText(test).ToArray()), nameFile);

            string[] files = new string[5];
            files[0] = "��� 3-01 �������� �������-��������.doc";
            files[1] = "LOL.docx";
            files[2] = "GPK.docx";
            files[3] = "������ ���������� ���������� ���������� ������������.docx";
            files[4] = "�������� ��� �����.docx";
           // string fileName = "��� 3-01 �������� �������-��������";
            for (int i = 0; i < 5; i++)
            {
                string initialText = TextDocumentManager.TextFromFile(@"C:\Users\alex1\Desktop\" + files[i]);      
                AddDocument(files[i], initialText);
                TextManager.PrepareText(initialText, out string[] initialWords, out Dictionary<int, string> initialDocIndexToSimplifiedWord, out int[] initialDocIndexes, out string[] simplifiedWords, out int wordCount);
                AddWords(Logic.IndexingForDB(initialDocIndexes, simplifiedWords), files[i]);
            }





            //string str = "";
            //List<String> example = new List<string> { "index", "data", "are", "the", "of" };
            //foreach (KeyValuePair<int, List<List<int>>> pair in GetDocuments(example))
            //{

            //    Console.WriteLine("�������� - " + pair.Key);
            //    foreach (List<int> positions in pair.Value)
            //    {
            //        foreach (int pos in positions)
            //        {
            //            str += Convert.ToString(pos) + " ";
            //        }
            //        Console.WriteLine("  �����i� - " + str);
            //        str = "";
            //    }
            //}
            Console.WriteLine("Ready");
            Console.ReadKey();

        }



    }
}