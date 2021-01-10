using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharepointWorkflow.Common
{
    public delegate void CopyDelegate(string sourceUrl, string projectUrl, string documentLibrary, string fileName);
    public delegate MemoryStream OpenFileDelegate(string url);
    public delegate void SaveDelegate(MemoryStream memory, string projectUrl, string documentLibrary, string fileName);
}
