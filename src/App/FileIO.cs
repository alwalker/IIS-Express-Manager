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
            try
            {
                if (!File.Exists(pathToConfig))
                {
                    throw new ApplicationException("Config file not found!");
                }

                _pathToConfig = pathToConfig;
                var splitPath = _pathToConfig.Split('\\');

                _watcher.Path =
                    splitPath.Length > 1 ?
                    string.Join("\\", splitPath.Take(_pathToConfig.Split('\\').Length - 1))
                    :
                    Directory.GetCurrentDirectory();
                _watcher.Changed += Watcher_FileChanged;
                _watcher.Filter = _pathToConfig.Split('\\').Last();
                _watcher.EnableRaisingEvents = true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error setting up file monitoring: " + ex.Message);
            }
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

        public void Save(XElement newSite, int id)
        {
            _watcher.EnableRaisingEvents = false;

            try
            {
                XDocument config = null;
                using (var stream = new FileStream(_pathToConfig, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    config = XDocument.Load(stream);
                }

                var site = (from s in config.Descendants("site")
                            where s.Attribute("id").Value == id.ToString()
                            select s).SingleOrDefault();

                if (site != null)
                {
                    site.Remove();
                }

                config.Descendants("sites").SingleOrDefault().Add(newSite);

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
