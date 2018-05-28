using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using eshopAPI.Models;

namespace eshopAPI.Utils.Export
{
    public class CsvExportService : IExportService
    {
        public async Task Export(IEnumerable<ItemVM> items, string pathToFile)
        {
            using (var writter = new StreamWriter(pathToFile))
            {
                var firstLine = "Title,Price,Image,SKU Code,Description,Category,Properties";
                await writter.WriteLineAsync(firstLine);
                foreach (var item in items)
                {
                    var line =
                        $"\"{item.Name.Replace(@"\n\r", "")}\",\"{item.Price}\",\"{string.Join("|", item.Pictures.Select(p => p.URL)).Replace(@"\n\r", "")}\",\"{item.SKU}\",\"{item.Description.Replace(@"\n\r", "")}\",\"{string.Join("/", item.ItemCategory.Name, item.ItemCategory.SubCategory.Name)}\",\"[{string.Join(",", item.Attributes.Select(a => string.Join(":", a.Name, a.Value))).Replace(@"\n\r", "")}]\"";
                    await writter.WriteLineAsync(line);
                }
            }
        }
    }
}