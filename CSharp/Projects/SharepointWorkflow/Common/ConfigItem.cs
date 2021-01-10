using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharepointWorkflow.Common
{
    class ConfigItem
    {
        // This is only a container class, so we only need containers for the variable name, the sheet name and the sheet coordinates.
        public string Variable { get; set; }
        public List<string> Sheets { get; set; }
        public List<string> Ranges { get; set; }

        /// <summary>
        /// An custom constructor to set the values in the fields.
        /// </summary>
        /// <param name="variable">Variable name container.</param>
        /// <param name="sheets">Sheet name container.</param>
        /// <param name="ranges">Cell range container.</param>
        public ConfigItem(string variable, List<string> sheets, List<string> ranges)
        {
            Variable = variable;
            Sheets = new List<string>();
            Ranges = new List<string>();

            // Shallow-copy the lists from the parameters into the class containers.
            foreach (string s in sheets)
            {
                Sheets.Add(s);
            }

            foreach (string s in ranges)
            {
                Ranges.Add(s);
            }
        }

        /// <summary>
        /// A standard constructor which initializes all class containers.
        /// </summary>
        public ConfigItem()
        {
            Variable = "";
            Sheets = new List<string>();
            Ranges = new List<string>();
        }
    }
}
