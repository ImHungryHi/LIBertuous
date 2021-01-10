using Excel = Microsoft.Office.Interop.Excel;
using SharepointWorkflow.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharepointWorkflow.Data.Unused
{
    // May contain bugs.
    class ExcelWriter
    {
        private Excel.Application excelApp;     // Container which will open the Excel file.
        private Excel.Workbook workbook;        // Contains all Excel worksheets.
        private ErrorDelegate errorDelegate;    // Enables the class to handle errors in another class.
        private string path;                    // Contains the path to the Excel file.

        public ExcelWriter(string path, ErrorDelegate errorDelegate)
        {
            // First of all, check to see if the given file is an Excel file. If it is not, throw a custom error.
            if (!path.Contains(".xls") && !path.Contains(".xlsx"))
            {
                errorDelegate("Invalid file type found in the ExcelReader constructor.");
            }

            // Set the delegated update method so we can send error handling to another class. Also set the path field to indicate where the Excel file is located.
            this.errorDelegate = errorDelegate;
            this.path = path;

            // Initialize the connection to the Excel file using the given path and the needed data (different provider and extended properties for .xls and .xlsx files).
            excelApp = new Excel.Application();
        }

        public void Write(List<Item> items)
        {
            foreach (Item item in items)
            {
                // Can't smart-write to different ranges in the same worksheet yet.
                if (item.ConfigItem.Ranges.Count != item.ConfigItem.Sheets.Count)
                {
                    throw new Exception("The amount of cell ranges differs from the amount of worksheets.");
                }

                for (int x = 0; x < item.ConfigItem.Ranges.Count; x++)
                {
                    // Check if the range contains the delimiter which denotes a range larger than 1 cell.
                    if (!item.ConfigItem.Ranges[x].Contains(":"))
                    {
                        // The range is only 1 cell large, we can only have 1 value.
                        WriteToCell(item.ConfigItem.Sheets[x], item.ConfigItem.Ranges[x], item.Values.First().First());
                    }
                    else
                    {
                        // Convert the multidimensional list to a multidimensional string array first.
                        string[,] values = new string[item.Values.Count, item.Values[0].Count];

                        for (int col = 1; col <= item.Values.Count; col++)
                        {
                            for (int row = 1; row <= item.Values[col].Count; row++)
                            {
                                values[col, row] = item.Values[col][row];
                            }
                        }

                        WriteToRange(item.ConfigItem.Sheets[x], item.ConfigItem.Ranges[x], values);
                    }
                }
            }
        }

        /// <summary>
        /// Writes a value to a certain cell in a chosen worksheet of the Excel file.
        /// Single value writing method.
        /// </summary>
        /// <param name="sheet">Preferred worksheet.</param>
        /// <param name="cell">Index of altering.</param>
        /// <param name="content">New content.</param>
        public void WriteToCell(string sheet, string cell, string content)
        {
            // Loop through the worksheets and start editing when a match for the parameter "sheet" is found.
            foreach (Excel.Worksheet worksheet in workbook.Worksheets)
            {
                if (worksheet.Name.Equals(sheet))
                {
                    Excel.Range range = worksheet.Range[cell];  // Get the range in which the program will be altering values (most likely a single cell, depending on the syntax).
                    range.Value = content;                      // Overwrite the old value of the range, in case old input was found.
                    workbook.Save();                            // Save changes to the Excel file.
                }
            }
        }

        /// <summary>
        /// Writes an array of values to certain cells in a chosen worksheet of the Excel file.
        /// Multiple value writing method.
        /// </summary>
        /// <param name="sheet">Preferred worksheet.</param>
        /// <param name="cells">Indexes of altering.</param>
        /// <param name="contentArray">New content of multiple cells.</param>
        public void WriteToRange(string sheet, string cells, string[,] contentArray)
        {
            // Loop through the worksheets and start editing when a match for the parameter "sheet" is found.
            foreach (Excel.Worksheet worksheet in workbook.Worksheets)
            {
                if (worksheet.Name.Equals(sheet))
                {
                    Excel.Range range = worksheet.Range[cells];
                    range.Value2 = contentArray;
                    workbook.Save();
                }
            }
        }

        /// <summary>
        /// Setter method to initialize the error handling delegate.
        /// </summary>
        /// <param name="errorDelegate">Delegated update method</param>
        public void SetDelegate(ErrorDelegate errorDelegate)
        {
            this.errorDelegate = errorDelegate;
        }

        /// <summary>
        /// Opens the Excel.Workbook field instance, using the class' path field.
        /// </summary>
        public void Open()
        {
            workbook = excelApp.Workbooks.Open(path);
        }

        /// <summary>
        /// Closes the Excel.Workbook field instance, usefull in case an error occurs in the middle of processing.
        /// Must always be called after writing has completed.
        /// </summary>
        public void Close()
        {
            workbook.Close();
        }
    }
}
