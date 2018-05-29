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

        private int skuColumn = 4;
        private int nameColumn = 1;
        private int picturesColumn = 3;
        private int attributeKeyColumn = 7;
        private int attributeValueColumn = 8;
        private int descriptionColumn = 5;
        private int priceColumn = 2;
        private int categoriesColumn = 6;

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
            var importedItems = new List<ItemVM>();
            var fInfo = new FileInfo(FileName);
            try
            {
                using (var excel = new ExcelPackage(fInfo))
                {
                    var wks = excel.Workbook.Worksheets["items"];
                    var lastRow = wks.Dimension.End.Row;
                    List<ItemAttributesVM> attributes = new List<ItemAttributesVM>();

                    for (int i = 2; i <= lastRow; i++)
                    {
                        var sku = GetCellValueFromPossiblyMergedCell(wks, i, skuColumn);

                        string attributeKey = GetCellValueFromPossiblyMergedCell(wks, i, attributeKeyColumn);
                        string attributeValue = GetCellValueFromPossiblyMergedCell(wks, i, attributeValueColumn);

                        if(!string.IsNullOrEmpty(attributeKey) && !string.IsNullOrEmpty(attributeValue))
                        {

                            attributes.Add(new ItemAttributesVM { Name = attributeKey, Value = attributeValue });
                        }

                        if (GetCellValueFromPossiblyMergedCell(wks, i + 1, skuColumn) == sku)
                        {
                            continue;
                        }

                        var importedRecord = new ItemVM
                        {
                            SKU = sku,
                            Name = GetCellValueFromPossiblyMergedCell(wks, i, nameColumn),
                            Price = Convert.ToDecimal(GetCellValueFromPossiblyMergedCell(wks, i, priceColumn)),
                            Description = GetCellValueFromPossiblyMergedCell(wks, i, descriptionColumn),
                            Pictures = PreparePictures(GetCellValueFromPossiblyMergedCell(wks, i, picturesColumn)),
                            Attributes = attributes,
                            ItemCategory = PrepareCategory(GetCellValueFromPossiblyMergedCell(wks, i, categoriesColumn))
                        };

                        importedItems.Add(importedRecord);
                        attributes = new List<ItemAttributesVM>();
                    }
                }
            }
            catch (IOException e)
            {
                ImportErrorLogger.LogError(e.Message);
            }

            return importedItems;
        }

        private IEnumerable<ItemPictureVM> PreparePictures(string picturesCell)
        {
            if(string.IsNullOrEmpty(picturesCell))
            {
                return null;
            }
            string[] pictures = picturesCell.Replace(" ", "").Split(",");
            return pictures.Select(p => new ItemPictureVM { URL = p }).ToList();
        }

        private ItemCategoryVM PrepareCategory(string categoriesCell)
        {
            string[] categories = categoriesCell.Split("/");

            return new ItemCategoryVM
                {
                    Name = categories[0],
                    SubCategory = new ItemSubCategoryVM { Name = categories[1] }
                };
        }

        private string GetCellValueFromPossiblyMergedCell(ExcelWorksheet wks, int row, int col)
        {
            var cell = wks.Cells[row, col];
            if (cell.Merge)
            {
                var mergedId = wks.MergedCells[row, col];
                return wks.Cells[mergedId].First().Value != null ? wks.Cells[mergedId].First().Value.ToString() : string.Empty;
            }
            else
            {
                return cell.Value != null ? cell.Value.ToString() : string.Empty;
            }
        }

    }
}
