using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;

namespace IISExpressManager
{
    public class WebSite
    {
        public enum BindingProtocol { Unknown = 0, http, ftp };

        private int _id;

        public int Id { get { return _id; } }
        public string Name { get; set; }
        public bool ServerAutoStart { get; set; }
        public string ApplicationPool { get; set; } //TODO:Research app pools
        public string ApplicationPath { get; set; }
        public string VirtualPath { get; set; }
        public string PhysicalPath { get; set; }  //TODO:Check if exists
        public BindingProtocol Protocol { get; set; }
        public string BindingInformation { get; set; }

        protected WebSite(int id, string name, bool serverAutoStart, string applicationPath, string applicationPool,
            string virtualPath, string physicalPath, BindingProtocol protocol, string bindingInfo)
        {
            _id = id;
            Name = name;
            ServerAutoStart = serverAutoStart;
            ApplicationPath = applicationPath;
            ApplicationPool = applicationPool;
            VirtualPath = virtualPath;
            PhysicalPath = physicalPath;
            Protocol = protocol;
            BindingInformation = bindingInfo;
        }

        public static IList<WebSite> GetAllWebsites(IFileIO fileIO)
        {
            if (fileIO == null)
            {
                throw new ArgumentNullException("fileIO");
            }

            XDocument xdoc = null;
            try
            {
                xdoc = fileIO.GetSitesSection();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(
                    String.Format("Error access IIS Express applicationhost config file: {0}", ex.Message));
            }
            if (xdoc == null)
            {
                return null;
            }

            try
            {
                var sites =
                    from site in xdoc.Descendants("site")
                    select new WebSite(
                        Convert.ToInt32(site.Attribute("id").Value),
                        site.Attribute("name").Value,
                        site.Attribute("serverAutoStart") == null ? true : Convert.ToBoolean(site.Attribute("serverAutoStart").Value),
                        site.Element("application").Attribute("path").Value,
                        site.Element("application").Attribute("applicationPool") == null ? string.Empty : site.Element("application").Attribute("applicationPool").Value,
                        site.Element("application").Element("virtualDirectory").Attribute("path").Value,
                        site.Element("application").Element("virtualDirectory").Attribute("physicalPath").Value,
                        (BindingProtocol)Enum.Parse(typeof(BindingProtocol), site.Element("bindings").Element("binding").Attribute("protocol").Value),
                        site.Element("bindings").Element("binding").Attribute("bindingInformation").Value);
                return sites.ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(
                    String.Format("Error parsing applicationhost config file: {0}", ex.Message));
            }
        }
    }
}
