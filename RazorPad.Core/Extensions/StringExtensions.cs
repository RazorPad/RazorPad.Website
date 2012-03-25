using System.IO;
using System.Text;

namespace RazorPad.Core
{
    public static class StringExtensions
    {
        public static TextReader ToTextReader(this string source)
        {
            var stream = new MemoryStream(Encoding.Default.GetBytes(source ?? string.Empty));
            return new StreamReader(stream);
        }

        public static string TruncateAtWord(this string input, int length)
        {
            if (input == null || input.Length < length)
                return input;
            int iNextSpace = input.LastIndexOf(" ", length);
            return string.Format("{0}...", input.Substring(0, (iNextSpace > 0) ? iNextSpace : length).Trim());
        }
    }
}