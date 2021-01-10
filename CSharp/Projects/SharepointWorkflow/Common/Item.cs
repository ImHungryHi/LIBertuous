using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharepointWorkflow.Common
{
    class Item
    {
        // This is only a container class, so we only need containers for the value lists and corresponding config item.
        public ConfigItem ConfigItem { get; set; }
        public List<List<string>> Values { get; set; }

        /// <summary>
        /// Default constructor which initializes the ConfigItem and Values fields.
        /// </summary>
        public Item()
        {
            ConfigItem = new ConfigItem();
            Values = new List<List<string>>();
        }

        /// <summary>
        /// Custom constructor which fills in the ConfigItem and Values fields using the parameters.
        /// </summary>
        /// <param name="configItem">ConfigItem corresponding to the current item.</param>
        /// <param name="values">A 2-dimensional list of values.</param>
        public Item(ConfigItem configItem, List<List<string>> values)
        {
            ConfigItem = configItem;
            Values = new List<List<string>>();

            // Shallow-copy the list from the parameters into the class container.
            foreach (List<string> list in values)
            {
                List<string> newList = new List<string>();

                foreach (string value in list)
                {
                    newList.Add(value);
                }

                Values.Add(newList);
            }
        }
    }
}
