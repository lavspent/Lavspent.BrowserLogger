using System.IO;

namespace Lavspent.BrowserLogger.Extensions
{
    public static class StreamExtensions
    {
        public static string ReadString(this Stream stream)
        {
            string result;
            using (var reader = new StreamReader(stream))
            {
                result = reader.ReadToEnd();
            }

            return result;
        }
    }
}