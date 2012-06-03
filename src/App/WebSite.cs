using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace IISExpressManager
{
    public class WebSite : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string name)
        {
            if (name != "IsDirty")
            {
                IsDirty = true;
            }
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }


        public enum BindingProtocol { Unknown = 0, http, ftp };

        private readonly int _id;
        private bool _isDirty;
        private string _bindingInformation;
        private BindingProtocol _protocol;
        private string _physicalPath;
        private string _virtualPath;
        private string _applicationPath;
        private string _applicationPool;
        private bool _serverAutoStart;
        private string _name;        

        public int Id { get { return _id; } }
        public bool IsDirty
        {
            get
            {
                return _isDirty;
            }
            set
            {
                if (value != _isDirty)
                {
                    _isDirty = value;
                    OnPropertyChanged("IsDirty");
                }
            }
        }
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (value != _name)
                {
                    _name = value;
                    OnPropertyChanged("Name");
                }
            }
        }
        public bool ServerAutoStart
        {
            get
            {
                return _serverAutoStart;
            }
            set
            {
                if (value != _serverAutoStart)
                {
                    _serverAutoStart = value;
                    OnPropertyChanged("ServerAutoStart");
                }
            }
        }
        public string ApplicationPool  //TODO:Research app pools
        {
            get
            {
                return _applicationPool;
            }
            set
            {
                if (value != _applicationPool)
                {
                    _applicationPool = value;
                    OnPropertyChanged("ApplicationPool");
                }
            }
        }
        public string ApplicationPath
        {
            get
            {
                return _applicationPath;
            }
            set
            {
                if (value != _applicationPath)
                {
                    _applicationPath = value;
                    OnPropertyChanged("ApplicationPath");
                }
            }
        }
        public string VirtualPath
        {
            get
            {
                return _virtualPath;
            }
            set
            {
                if (value != _virtualPath)
                {
                    _virtualPath = value;
                    OnPropertyChanged("VirtualPath");
                }
            }
        }
        public string PhysicalPath //TODO:Check if exists
        {
            get
            {
                return _physicalPath;
            }
            set
            {
                if (value != _physicalPath)
                {
                    _physicalPath = value;
                    OnPropertyChanged("PhysicalPath");
                }
            }
        }
        public BindingProtocol Protocol
        {
            get
            {
                return _protocol;
            }
            set
            {
                if (value != _protocol)
                {
                    _protocol = value;
                    OnPropertyChanged("Protocol");
                }
            }
        }
        public string BindingInformation
        {
            get
            {
                return _bindingInformation;
            }
            set
            {
                if (value != _bindingInformation)
                {
                    _bindingInformation = value;
                    OnPropertyChanged("BindingInformation");
                }
            }
        }

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
            IsDirty = false;
        }

        public override string ToString()
        {
            return String.Format("{0}({1})", Name, Id);
        }

        public static ObservableCollection<WebSite> GetAllWebsites(IFileIO fileIO)
        {
            if (fileIO == null)
            {
                throw new ArgumentNullException("fileIO");
            }

            IEnumerable<XElement> xdoc = null;
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
                return new ObservableCollection<WebSite>(
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
                        site.Element("bindings").Element("binding").Attribute("bindingInformation").Value));               
            }
            catch (Exception ex)
            {
                throw new ApplicationException(
                    String.Format("Error parsing applicationhost config file: {0}", ex.Message));
            }
        }
    }
}
