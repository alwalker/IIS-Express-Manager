using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace IISExpressManager
{
    public interface IFileIO
    {
        IEnumerable<XElement> GetSitesSection(string path = "");
    }
}
