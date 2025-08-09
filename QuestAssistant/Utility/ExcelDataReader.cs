using OfficeOpenXml; // Aus dem EPPlus-Paket
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class ExcelDataReader
{
    public static List<TaskItem> ReadTasks(string filePath)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Oder Commercial
        List<TaskItem> tasks = new List<TaskItem>();
        using (var package = new ExcelPackage(new FileInfo(filePath)))
        {
            var worksheet = package.Workbook.Worksheets[0]; // Erstes Worksheet
            int rowCount = worksheet.Dimension.Rows;

            for (int row = 2; row <= rowCount; row++) // Annahme: Erste Zeile sind Header
            {
                try
                {
                    tasks.Add(new TaskItem
                    {
                        Name = worksheet.Cells[row, 1].Text,
                        Description = worksheet.Cells[row, 2].Text,
                        Frequency = worksheet.Cells[row, 3].Text,
                        Duration = TimeSpan.Parse(worksheet.Cells[row, 4].Text), // Format beachten (z.B. "01:00:00")
                        Points = int.Parse(worksheet.Cells[row, 5].Text),
                        EarliestStartTime = DateTime.Parse(worksheet.Cells[row, 6].Text),
                        LatestEndTime = DateTime.Parse(worksheet.Cells[row, 7].Text),
                        Status = worksheet.Cells[row, 8].Text,
                        AssignedUser = worksheet.Cells[row, 9].Text
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Fehler beim Lesen der Zeile {row}: {ex.Message}");
                }
            }
        }
        return tasks;
    }
}
