using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.IO;
using System.Diagnostics;

namespace IISExpressManager
{
    public class FileIO : IFileIO
    {
        private FileSystemWatcher _watcher = new FileSystemWatcher();

        public event EventHandler FileChanged;

        public FileIO()
        {
            _watcher.Path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"IISExpress\config");
            _watcher.Changed += Watcher_FileChanged;
            _watcher.Filter = "applicationhost.config";
            _watcher.EnableRaisingEvents = true;
        }

        private void Watcher_FileChanged(object sender, FileSystemEventArgs e)
        {
            if (FileChanged != null)
            {
                FileChanged(null, null);
            }
        }

        public IEnumerable<XElement> GetSitesSection(string path = "") //TODO: Find a better way to do this
        {
            XDocument config = null;
            try
            {
                if (string.IsNullOrEmpty(path))
                {
                    var myDocumentsLocation = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    using (var stream = new FileStream(Path.Combine(myDocumentsLocation, @"IISExpress\config\applicationhost.config"),
                        FileMode.Open,
                        FileAccess.Read,
                        FileShare.ReadWrite))
                    {
                        config = XDocument.Load(stream);
                    }
                }
                else
                {
                    using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        config = XDocument.Load(stream);
                    }
                }
            }
            catch (FileLoadException ex)
            {
                var x = 1;
            }

            return config.Descendants("sites");
        }

        public bool Exists(string path)
        {
            try
            {
                return File.Exists(path);
            }
            catch
            {
                return false;
            }
        }


    }
}
