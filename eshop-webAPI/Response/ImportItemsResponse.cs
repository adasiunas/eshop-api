using eshopAPI.Models;
using eshopAPI.Utils.Import;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eshopAPI.Response
{
    public class ImportItemsResponse
    {
        public IEnumerable<ItemVM> items { get; set; }
        public IEnumerable<ImportError> errors { get; set; }
    }
}
