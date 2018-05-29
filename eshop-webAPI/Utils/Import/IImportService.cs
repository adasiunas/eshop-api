using eshopAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eshopAPI.Utils.Import
{
    public interface IImportService
    {
        ImportErrorLogger ImportErrorLogger { get; set; }
        void SetFileName(string name);
        Task<IEnumerable<ItemVM>> ImportItems();
    }
}
