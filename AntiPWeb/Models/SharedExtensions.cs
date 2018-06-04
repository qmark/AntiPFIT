using System.Linq;

namespace AntiPShared
{
    public static class SharedExtensions
    {
        public static string RemoveWhiteSpaces(this string input)
        {
            return new string(input.ToCharArray()
                .Where(c => !char.IsWhiteSpace(c))
                .ToArray());
        }
    }
}
