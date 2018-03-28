using System;
using System.Text;

namespace eshopAPI.Utils
{
    public static class EncodeHelper
    {
        public static string Base64Encode(string str)
        {
            var encbuff = Encoding.UTF8.GetBytes(str);
            return Convert.ToBase64String(encbuff);
        }

        public static string Base64Decode(string str)
        {
            byte[] decodedBytes = Convert.FromBase64String(str);
            return Encoding.UTF8.GetString(decodedBytes);
        }
    }
}