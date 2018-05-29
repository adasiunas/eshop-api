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
                    item.Description = item.Description.Replace("\r", String.Empty).Replace("\n", String.Empty).Replace(",", String.Empty);
                    var line =
                        $"\"{ClearNewLines(item.Name.Replace(",", String.Empty))}\",\"{item.Price}\",\"{string.Join("|", item.Pictures.Select(p => p.URL))}\",\"{item.SKU}\",\"{ClearNewLines(item.Description.Replace(@",", String.Empty))}\",\"{string.Join("/", item.Category.Name, item.SubCategory?.Name)}\",\"[{string.Join(",", item.Attributes.Select(a => string.Join(":", a.Name, a.Value)))}]\"";
                    await writter.WriteLineAsync(line);
                }
            }
        }

        private string ClearNewLines(string value)
        {
            return value.Replace("\r", String.Empty).Replace("\n", String.Empty);
        }
    }
}