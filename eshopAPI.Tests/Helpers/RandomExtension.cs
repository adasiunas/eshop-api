using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eshopAPI.Tests.Helpers
{
    public static class RandomExtension
    {
        public static String RandomString(this Random rnd, int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[rnd.Next(s.Length)]).ToArray());
        }
    }
}
