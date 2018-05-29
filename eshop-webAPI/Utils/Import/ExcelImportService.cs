using eshopAPI.DataAccess;
using eshopAPI.Models;
using eshopAPI.Models.ViewModels;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace eshopAPI.Utils.Import
{
    public class ExcelImportService : IImportService
    {
        private string FileName { get; set; }
        public ImportErrorLogger ImportErrorLogger { get; set; }

        public void SetFileName(string fileName)
        {
            this.FileName = @"C:\Users\greta\Desktop\" + fileName + ".xlsx";
        }

        public Task<IEnumerable<ItemVM>> ImportItems()
        {
            return Task.FromResult(ImportRecords());
        }

        public IEnumerable<ItemVM> ImportRecords()
        {
            var ret = new List<ItemVM>();
            var fInfo = new FileInfo(FileName);
            try
            {
                using (var excel = new ExcelPackage(fInfo))
                {
                    var wks = excel.Workbook.Worksheets["items"];
                    var lastRow = wks.Dimension.End.Row;

                    for (int i = 2; i <= lastRow; i++)
                    {
                        string picturesString = wks.Cells[i, 5].Value.ToString();
                        string[] pictures = picturesString.Replace(" ", "").Split(",");

                        string categoriesString = wks.Cells[i, 6].Value.ToString();
                        string[] categories = categoriesString.Split("/");


                        var importedRecord = new ItemVM
                        {
                            SKU = wks.Cells[i, 1].Value.ToString(),
                            Name = wks.Cells[i, 2].Value.ToString(),
                            Price = Convert.ToDecimal(wks.Cells[i, 3].Value),
                            Description = wks.Cells[i, 4].Value.ToString(),
                            Pictures = pictures.Select(p => new ItemPictureVM
                            {
                                URL = p
                            }),
                            Attributes = null,
                            Category = new ItemCategoryVM
                            {
                                Name = categories[0],
                            },
                            SubCategory = new ItemSubCategoryVM { Name = categories[1] }
                        };
                        ret.Add(importedRecord);
                    }
                }
            }
            catch (IOException e)
            {
                ImportErrorLogger.LogError(e.Message);
            }
            return ret;
        }

        private List<ItemPictureVM> ImportPictures(int columnIndex)
        {
            return null;
        }

    }
}
