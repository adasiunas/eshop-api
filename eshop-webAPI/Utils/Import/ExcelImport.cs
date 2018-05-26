using eshopAPI.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace eshopAPI.Utils.Import
{
    public class ExcelImport : FileImport
    {
        public ExcelImport(string fileName)
        {
            this.FileName = @"C:\Users\greta\Desktop\" + fileName + ".xlsx";
        }

        public List<ItemVM> ImportRecords()
        {
            var ret = new List<ItemVM>();
            var fInfo = new FileInfo(FileName);
            using (var excel = new ExcelPackage(fInfo))
            {
                var wks = excel.Workbook.Worksheets["items"];
                var lastRow = wks.Dimension.End.Row;

                for (int i = 2; i <= lastRow; i++)
                {
                    var importedRecord = new ItemVM
                    {
                        Name = wks.Cells[i, 4].Value.ToString(),
                        //SKU = //GetCellValueFromPossiblyMergedCell(wks, i, 3),
                        //Description = //GetCellValueFromPossiblyMergedCell(wks, i, 2)
                    };
                    ret.Add(importedRecord);
                }
            }

            return ret;
        }
    }
}
