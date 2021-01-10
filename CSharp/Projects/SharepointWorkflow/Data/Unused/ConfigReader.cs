using SharepointWorkflow.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace SharepointWorkflow.Data.Unused
{
    class ConfigReader
    {
        private List<ConfigItem> items;

        /// <summary>
        /// Standard constructor which automatically reads a config file from a static path.
        /// </summary>
        public ConfigReader()
        {
            items = new List<ConfigItem>();
            ReadXml("config.xml");
        }

        /// <summary>
        /// Custom constructor which automatically reads a config file from a dynamic path.
        /// </summary>
        /// <param name="path">Config file path.</param>
        public ConfigReader(string path)
        {
            items = new List<ConfigItem>();
            ReadXml(path);
        }

        /// <summary>
        /// Reads a .txt config file from a given path.
        /// </summary>
        /// <param name="path">Config file path.</param>
        public void ReadTxt(string path)
        {
            // Check if the path contains the correct extension, this method will read txt files only.
            if (!path.Contains(".txt"))
            {
                throw new Exception("Invalid file type, please load a .txt config file.");
            }

            // Use using to construct a new StreamReader and implicitly close it when the reading has finished.
            using (StreamReader reader = new StreamReader(File.OpenRead(path)))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] pairSplit;
                    List<string> valueList = new List<string>();
                    List<string> sheets = new List<string>();
                    List<string> ranges = new List<string>();

                    // Check if the config syntax is what we expect it to be.
                    if (!line.Contains('='))
                    {
                        throw new Exception("The config input is missing a variable-value pair.");
                    }
                    else
                    {
                        // Split the string into a variable-value pair.
                        pairSplit = line.Split('=');
                    }

                    // If the value variable contains a semicolon, there multiple worksheet-cellrange pairs
                    if (pairSplit[1].Contains(';'))
                    {
                        // Store each worksheet-cellrange pair into a temporary array.
                        foreach (string s in pairSplit[1].Split(';'))
                        {
                            valueList.Add(s);
                        }
                    }
                    else
                    {
                        // Store the only worksheet-cellrange pair into a temporary array, this will shrink our code a bit.
                        valueList.Add(pairSplit[1]);
                    }

                    // Loop through the temporary array
                    foreach (string s in valueList)
                    {
                        string[] valueSplit;

                        // Check the value strings and fill the lists with their corresponding data.
                        if (!s.Contains("]("))
                        {
                            throw new Exception("The config input is missing a sheet-range combination in it's value field.");
                        }
                        else
                        {
                            valueSplit = s.Split(']');
                        }

                        if (valueSplit.Length != 2 || valueSplit[0][0] != ('[') || valueSplit[1][0] != '(' || valueSplit[1][valueSplit[1].Length - 1] != ')')
                        {
                            // The right square bracket has already been trimmed away with the Split function.
                            throw new Exception("The config input has found a wrong syntax in it's value field (should be [worksheetname](cellrange).");
                        }
                        else
                        {
                            char[] brackets = { '(', ')', '[', ']' };

                            // Cast the values into strings without brackets, we won't need them anymore.
                            for (int x = 0; x < valueSplit.Length; x++)
                            {
                                valueSplit[x] = valueSplit[x].Trim(brackets);
                            }
                        }

                        // Store the sheet name and cell range inside their corresponding list.
                        sheets.Add(valueSplit[0]);
                        ranges.Add(valueSplit[1]);
                    }

                    // Initialize a ConfigItem container, store the needed values in it and add it to the list.
                    ConfigItem item = new ConfigItem(pairSplit[0], sheets, ranges);
                    items.Add(item);
                }
            }
        }

        /// <summary>
        /// Reads an xml config file from a given path.
        /// </summary>
        /// <param name="path">Config file path.</param>
        public void ReadXml(string path)
        {
            // Check whether or not the file is an xml file.
            if (!path.Contains(".xml"))
            {
                throw new Exception("The given path didn't contain an xml file.");
            }

            // Load an xml file into a new xml container.
            XDocument document = XDocument.Load(path);

            // Loop through all elements(/items) in the xml's root element.
            foreach (XElement item in document.Root.Elements())
            {
                // Create a new temporary ConfigItem container.
                ConfigItem configItem = new ConfigItem();

                // item level
                foreach (XElement child in item.Elements())
                {
                    // If the name of the child indicates a variable element, store the variable name into the ConfigItem container.
                    if (child.Name.ToString().Equals("variable"))
                    {
                        configItem.Variable = child.Value.ToString();
                    }
                    else if (child.Name.ToString().Equals("worksheets"))
                    {
                        // Loop through each worksheet in the worksheets child element and add it to the worksheet list.
                        foreach (XElement grandchild in child.Elements())
                        {
                            configItem.Sheets.Add(grandchild.Value.ToString());
                        }
                    }
                    else if (child.Name.ToString().Equals("ranges"))
                    {
                        // Loop through each range in the ranges child element and add it to the ranges list.
                        foreach (XElement grandchild in child.Elements())
                        {
                            configItem.Ranges.Add(grandchild.Value.ToString());
                        }
                    }
                }

                // Store the ConfigItem container into the list.
                items.Add(configItem);
            }
        }

        /// <summary>
        /// Returns the list containing all config information.
        /// </summary>
        /// <returns>Returns a list of ConfigItem containers.</returns>
        public List<ConfigItem> GetItems()
        {
            return items;
        }
    }
}
