using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eshopAPI.Utils.Import
{
    interface IFileImportViewModel
    {
        string Name { get; set; }
        string Code { get; set; }
    }
}
