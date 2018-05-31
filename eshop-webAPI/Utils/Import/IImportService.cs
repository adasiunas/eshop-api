using eshopAPI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace eshopAPI.Utils.Import
{
    public interface IImportService
    {
        ImportErrorLogger ImportErrorLogger { get; set; }
        Task<List<ItemVM>> ImportItems(Stream file);
    }
}
