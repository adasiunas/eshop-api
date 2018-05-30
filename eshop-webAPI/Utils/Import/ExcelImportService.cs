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

        private readonly int nameColumn = 1;
        private readonly int priceColumn = 2;
        private readonly int PicturesColumn = 3;
        private readonly int skuColumn = 4;
        private readonly int descriptionColumn = 5;
        private readonly int categoriesColumn = 6;
        private readonly int attributeKeyColumn = 7;
        private readonly int attributeValueColumn = 8;      

        public Task<List<ItemVM>> ImportItems(Stream fileStream)
        {
            var importedItems = new List<ItemVM>();
            try
            {
                using (var excel = new ExcelPackage(fileStream))
                {

                    var wks = excel.Workbook.Worksheets["Sheet1"];
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
                            Pictures = PreparePictures(GetCellValueFromPossiblyMergedCell(wks, i, PicturesColumn)),
                            Attributes = attributes,
                            Category = PrepareCategory(GetCellValueFromPossiblyMergedCell(wks, i, categoriesColumn)),
                            SubCategory = PrepareSubCategory(GetCellValueFromPossiblyMergedCell(wks, i, categoriesColumn))
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

            return Task.FromResult(importedItems);
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
                    Name = categories[0]
                };
        }

        private ItemSubCategoryVM PrepareSubCategory(string categoriesCell)
        {
            string[] categories = categoriesCell.Split("/");

            return new ItemSubCategoryVM
            {
                Name = categories[1]
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
