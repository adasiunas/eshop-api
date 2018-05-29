using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using eshopAPI.Models;
using OfficeOpenXml;

namespace eshopAPI.Utils.Export
{
    public class ExportService : IExportService
    {
        public Task Export(IEnumerable<ItemVM> items, string pathToFile)
        {
            var fileInfo = new FileInfo(pathToFile);
            using (var excelPackage = new ExcelPackage(fileInfo))
            {
                var workSheet = excelPackage.Workbook.Worksheets.Add("Sheet1");
                // totalRows -> basically rows are counted for each item attribute, if it does have any it means item takes exactly onle line in the worksheet
                var totalRows = items.Sum(i => i.Attributes.Any() ? i.Attributes.Count() : 1) + 1;// plus is needed because of header row
                int colCount;
                workSheet = AddWorksheetHeader(workSheet, out colCount);
                int itemIndex = 0;                
                for (int rowIndex = 2; rowIndex <= totalRows; rowIndex++)
                {
                    var item = items.ElementAt(itemIndex);
                    var startingRowIndex = rowIndex;
                    workSheet.Cells[rowIndex, 1].Value = item.Name;
                    workSheet.Cells[rowIndex, 2].Value = item.Price;
                    workSheet.Cells[rowIndex, 3].Value = string.Join(",", item.Pictures.Select(p => p.URL));
                    workSheet.Cells[rowIndex, 4].Value = item.SKU;
                    workSheet.Cells[rowIndex, 5].Value = item.Description;
                    workSheet.Cells[rowIndex, 6].Value = string.Join("/", item.ItemCategory.Name, item.ItemCategory.SubCategory.Name);                       

                    for (int atributeIndex = 0; atributeIndex < item.Attributes.Count(); atributeIndex++)
                    {
                        var itemAtribute = item.Attributes.ElementAt(atributeIndex);
                        workSheet.Cells[rowIndex, 7].Value = itemAtribute.Name;
                        workSheet.Cells[rowIndex, 8].Value = itemAtribute.Value;
                        
                        if (atributeIndex != item.Attributes.Count() - 1)
                            rowIndex++;
                    }

                    for (int colIndex = 1; colIndex <= 6; colIndex++)
                    {
                        workSheet.Cells[startingRowIndex, colIndex, rowIndex, colIndex].Merge = true;
                    }
                    itemIndex++;
                }
                
                excelPackage.Save();
            }

            return Task.FromResult("ok");
        }

        private ExcelWorksheet AddWorksheetHeader(ExcelWorksheet worksheet, out int colCount)
        {
            var header = new [] { "Title", "Price", "Image", "SKU Code", "Description", "Category", "Properties"};
            colCount = header.Length;
            for (int i = 0; i < header.Length; i++)
            {
                worksheet.Cells[1, i + 1].Value = header[i];
            }
            worksheet.Cells[1, header.Length, 1, header.Length + 1].Merge = true;
            return worksheet;
        }
    }

    public interface IExportService
    {
        Task Export(IEnumerable<ItemVM> items, string pathToFile);
    }
}