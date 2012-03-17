using System.Security.Cryptography;
using System.Text;

namespace RazorPad.Web.Services
{
    public class UniqueKeyGenerator
    {
        private static readonly char[] Chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();

        public string Generate(int length = 7)
        {
            var crypto = new RNGCryptoServiceProvider();

            var data = new byte[1];
            crypto.GetNonZeroBytes(data);
            data = new byte[length];
            crypto.GetNonZeroBytes(data);

            var result = new StringBuilder(length);
            foreach (var bit in data)
            {
                result.Append(Chars[bit % (Chars.Length - 1)]);
            }

            return result.ToString();
        }
    }
}
