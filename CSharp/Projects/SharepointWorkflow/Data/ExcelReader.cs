using SharepointWorkflow.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharepointWorkflow.Data
{
    class ExcelReader
    {
        private List<Item> items;
        private ErrorDelegate errorDelegate;
        private OleDbConnection connection;
        private string path;
        private string[] usedWorksheets;
        private string[] syntaxedUsedWorksheets;

        /// <summary>
        /// Custom constructor method used to initialize the fields and establish the connection to the Excel file.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="errorDelegate"></param>
        public ExcelReader(string path, ErrorDelegate errorDelegate)
        {
            // Set the delegated update method so we can send error handling to another class and fill the path field with the appropriate parameter.
            this.path = path;
            this.errorDelegate = errorDelegate;

            //excelApp = new Excel.Application();
            items = new List<Item>();

            connection = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source='" + path + "';Extended Properties='Excel 12.0 Xml;HDR=NO;IMEX=1'");
        }

        /// <summary>
        /// Sets the fields assigned to contain the worksheet names.
        /// </summary>
        /// <param name="configItems">The list of all config items containing sheet names.</param>
        private void SetUsedWorksheets(List<ConfigItem> configItems)
        {
            // Open the connection to the Excel file.
            Open();

            List<string> worksheets = new List<string>();

            DataTable table = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            string[] excelSheets = new string[table.Rows.Count];

            for (int x = 0; x < table.Rows.Count; x++)
            {
                excelSheets[x] = table.Rows[x]["TABLE_NAME"].ToString();
            }

            // Start filtering the double values out and only select the worksheets configured in the config file.
            foreach (ConfigItem item in configItems)
            {
                foreach (string worksheet in item.Sheets)
                {
                    if (!worksheets.Contains(worksheet))
                    {
                        worksheets.Add(worksheet);
                    }
                }
            }

            // Parse the used worksheets into an array and get the syntax as it's used in the oledb classes.
            usedWorksheets = worksheets.ToArray<string>();
            syntaxedUsedWorksheets = SharePointHelper.GetCorrectSyntax(usedWorksheets, excelSheets);    // Returns the syntaxed worksheets used in the config items, in the same order as the unsyntaxed worksheets.

            // Finally, close the connection.
            Close();
        }

        /// <summary>
        /// Reads the contents of every location in the ConfigItem list.
        /// </summary>
        /// <param name="configItems">ConfigItem list containing every variable location.</param>
        public void Read(List<ConfigItem> configItems)
        {
            if (items == null)
            {
                items = new List<Item>();
            }

            // Load all used worksheets names into the class' fields.
            SetUsedWorksheets(configItems);

            // Open the connection to the Excel file.
            Open();

            try
            {
                foreach (ConfigItem configItem in configItems)
                {
                    Item item = new Item();
                    item.ConfigItem = configItem;

                    // The values should be the same everywhere, so take the first range.
                    string range = configItem.Ranges.First<string>();
                    bool single = false;    // Check value, contains whether or not a configuration item contains only a single field in the range list.

                    // OleDbCommand only takes ranges, so in case of a single cell, the range consists of the cell going to the itself.
                    if (!range.Contains(":"))
                    {
                        single = true;
                        range = range + ":" + range;
                    }

                    for (int x = 0; x < syntaxedUsedWorksheets.Length; x++)
                    {
                        if (configItem.Sheets.First<string>().Equals(usedWorksheets[x]))
                        {
                            List<List<string>> values = new List<List<string>>();

                            // Load the values from the sheet and range we got out of the configuration item earlier.
                            OleDbCommand command = new OleDbCommand("SELECT * FROM [" + syntaxedUsedWorksheets[x] + range + "]", connection);
                            OleDbDataReader reader = command.ExecuteReader();
                            DataTable data = new DataTable();
                            data.Load(reader);

                            if (single)
                            {
                                List<string> temp = new List<string>();
                                object value = data.Rows[0].ItemArray.First();

                                if (value == null)
                                {
                                    temp.Add("");
                                }
                                else
                                {
                                    temp.Add(value.ToString());
                                }

                                values.Add(temp);
                            }
                            else
                            {
                                for (int col = 0; col < data.Columns.Count; col++)
                                {
                                    List<string> temp = new List<string>();

                                    for (int row = 0; row < data.Rows.Count; row++)
                                    {
                                        if (data.Rows[row].ItemArray[col] == null)
                                        {
                                            temp.Add("");
                                        }
                                        else
                                        {
                                            temp.Add(data.Rows[row].ItemArray[col].ToString());
                                        }
                                    }

                                    values.Add(temp);
                                }
                            }

                            item.Values = values;
                            items.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorDelegate(ex.Message + "\n" + ex.TargetSite);
            }
            finally
            {
                // This must happen! Otherwise errors will occur when running next instances of the program (concerning Excel writing permissions).
                Close();
            }
        }

        /// <summary>
        /// Opens the Excel.Workbook field instance, using the class' path field.
        /// </summary>
        public void Open()
        {
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
        }

        /// <summary>
        /// Closes the Excel.Workbook field instance, usefull in case an error occurs in the middle of processing.
        /// Must always be called after writing has completed.
        /// </summary>
        public void Close()
        {
            if (connection.State != ConnectionState.Closed)
            {
                connection.Close();
            }
        }

        /// <summary>
        /// Returns the list of items containing the content and location of a cell or range.
        /// </summary>
        /// <returns>Returns the list of items containing the content and location of a cell or range.</returns>
        public List<Item> GetItems()
        {
            return items;
        }
    }
}
