using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.IO;

namespace IISExpressManager
{
    public class FileIO : IFileIO
    {
        public IEnumerable<XElement> GetSitesSection(string path = "")
        {
            XDocument config = null;
            if (string.IsNullOrEmpty(path))
            {
                var myDocumentsLocation = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                config = XDocument.Load(Path.Combine(myDocumentsLocation, @"IISExpress\config\applicationhost.config"));
            }
            else
            {
                config = XDocument.Load(path);
            }

            return config.Descendants("sites");
        }
    }
}
