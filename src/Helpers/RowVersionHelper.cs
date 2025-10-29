using System.Text;

namespace ContosoUniversity.Helpers
{
    public static class RowVersionHelper
    {
        /// <summary>
        /// Convert byte array to hexadecimal string
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string ToHexString(byte[] bytes)
        {
            if (bytes == null)
                return string.Empty;

            StringBuilder hex = new StringBuilder(bytes.Length * 2);
            foreach (byte b in bytes)
                hex.AppendFormat("{0:x2}", b);

            return hex.ToString();
        }

        /// <summary>
        /// Convert hexadecimal string to byte array
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static byte[] FromHexString(string hexString)
        {
            if (string.IsNullOrWhiteSpace(hexString))
                return new byte[0];

            //ArgumentNullException.ThrowIfNullOrWhiteSpace(hexString);
            ArgumentOutOfRangeException.ThrowIfNotEqual<int>(0, hexString.Length % 2);

            int numberChars = hexString.Length;
            byte[] bytes = new byte[numberChars / 2];
            for (int i = 0; i < numberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hexString.Substring(i, 2), 16);

            return bytes;
        }
    }
}
