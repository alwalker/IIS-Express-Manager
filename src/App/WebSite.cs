using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace IISExpressManager
{
    public class WebSite
    {
        enum Binding {Unknown = 0, HTTP, FTP};

        private int _id;
        private string _name;
        private bool _serverAutoStart;
        private string _applicationPath;
        private string _applicationPool; //TODO:Research app pools
        private string _virtualPath;
        private string _physicalPath;
        private Binding _binding;     
        private string _bindingInformation;

        public static IList<WebSite> GetAllWebsites(IFileIO fileIO)
        {
            return null;
        }
    }
}
