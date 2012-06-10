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
        private readonly FileSystemWatcher _watcher = new FileSystemWatcher();
        private readonly string _pathToConfig;

        public event EventHandler FileChanged;

        public FileIO(string pathToConfig)
        {
            _pathToConfig = pathToConfig;

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

        public IEnumerable<XElement> GetSitesSection()
        {
            XDocument config = null;
            using (var stream = new FileStream(_pathToConfig, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                config = XDocument.Load(stream);
            }

            return config.Descendants("sites");
        }

        public bool Exists(string path)
        {
            try
            {
                return Directory.Exists(path);
            }
            catch
            {
                return false;
            }
        }

        public void Save(XElement site, int id)
        {
            _watcher.EnableRaisingEvents = false;

            try
            {
                XDocument config = null;
                using (var stream = new FileStream(_pathToConfig, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    config = XDocument.Load(stream);
                }

                (from s in config.Descendants("site")
                 where s.Attribute("id").Value == id.ToString()
                 select s).SingleOrDefault().Remove();

                config.Descendants("sites").SingleOrDefault().Add(site);

                config.Save(_pathToConfig);

            }
            catch
            {
                throw;
            }
            finally
            {
                _watcher.EnableRaisingEvents = true;
            }
        }

        public void Delete(XElement site, int id)
        {
            _watcher.EnableRaisingEvents = false;

            try
            {
                XDocument config = null;
                using (var stream = new FileStream(_pathToConfig, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    config = XDocument.Load(stream);
                }

                (from s in config.Descendants("site")
                 where s.Attribute("id").Value == id.ToString()
                 select s).SingleOrDefault().Remove();

                config.Save(_pathToConfig);

            }
            catch
            {
                throw;
            }
            finally
            {
                _watcher.EnableRaisingEvents = true;
            }
        }
    }
}
