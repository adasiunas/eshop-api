using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace eshopAPI.Helpers
{
    public class PasswordHash
    {
        public static string ComputeHash(string value)
        {
            SHA1 sha = SHA1.Create();
            sha.Initialize();
            byte[] data = System.Text.Encoding.UTF8.GetBytes(value);
            byte[] hashData = sha.ComputeHash(data);
            StringBuilder hash = new StringBuilder(2 * hashData.Length);
            foreach (byte b in hashData)
            {
                hash.AppendFormat("{0:x2}", b);
            }
            return hash.ToString();
            //return System.Text.Encoding.ASCII.GetString(hashData);
        }
    }
}
