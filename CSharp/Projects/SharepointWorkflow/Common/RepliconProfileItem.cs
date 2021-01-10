using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharepointWorkflow.Common
{
    class RepliconProfileItem
    {
        public int StartIndex { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// Default constructor which initiates the fields, setting empty values.
        /// </summary>
        public RepliconProfileItem()
        {
            StartIndex = 0;
            Name = "";
        }

        /// <summary>
        /// Custom constructor which initiates the fields, setting the values to match the parameters.
        /// </summary>
        /// <param name="startIndex">Index of start</param>
        /// <param name="name"></param>
        public RepliconProfileItem(int startIndex, string name)
        {
            StartIndex = startIndex;
            Name = name;
        }
    }
}
