using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eshopAPI.Utils.Import
{
    public class FileImport
    {
        protected string FileName { get; set; }

        protected IEnumerable<string> Headers { get; set; }

        public FileImport()
        {
        }
    }
}
