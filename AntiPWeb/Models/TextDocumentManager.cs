using Spire.Doc;

namespace AntiPShared
{
    public class TextDocumentManager
    {
        public static string TextFromFile(string path)
        {
            var doc = new Document();
            doc.LoadFromFile(path);

            return doc.GetText();
        }
    }
}