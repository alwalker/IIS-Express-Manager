using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using System.Xml.Linq;
using IISExpressManager;
using System.IO;

namespace IntegrationTests
{
    public class FileIOTests
    {
        public FileIOTests()
        {
            File.Copy("applicationhost.config", "applicationhost_saved.config", true);
            File.Copy("applicationhost.config", "applicationhost_deleted.config", true);
        }

        [Fact]
        public void ConstructorThrowsOnNonExistingConfigFile()
        {
            Assert.Throws(typeof(ApplicationException), () => new FileIO("ThisDoesNotExist.foo"));
        }

        [Fact]
        public void TestGetSitesSection()
        {
            var expected = XDocument.Load("LongValidSites.xml").Descendants("sites");
            var fileIO = new FileIO("applicationhost.config");

            var actual = fileIO.GetSitesSection();

            Assert.Equal(expected.Count(), actual.Count());
            foreach (var x in actual)
            {
                foreach (var y in expected)
                {
                    Assert.True(XElement.DeepEquals(x, y));
                }
            }
        }

        [Fact]
        public void TestExists_DoesExist()
        {
            var fileIO = new FileIO("applicationhost.config");

            Assert.True(fileIO.Exists("C:\\"));
        }

        [Fact]
        public void TestExists_DoesNotExist()
        {
            var fileIO = new FileIO("applicationhost.config");

            Assert.False(fileIO.Exists("C:\\this_dir_no_exist"));
        }

        [Fact]
        public void TestSave()
        {
            var fileIO = new FileIO("applicationhost_saved.config");
            var newSite = XDocument.Load("New_Site.xml").Element("site");

            fileIO.Save(newSite, 1);

            var savedSite =
                (from s in XDocument.Load("applicationhost_saved.config").Descendants("site")
                 where s.Attribute("id").Value == "1"
                 select s).SingleOrDefault();
            Assert.Equal("false", savedSite.Attribute("serverAutoStart").Value);
        }

        [Fact]
        public void TestSaveNewSite()
        {
            var fileIO = new FileIO("applicationhost_saved.config");
            var newSite = XDocument.Load("New_Site_Doesnt_Exist.xml").Element("site");

            fileIO.Save(newSite, 1);

            var savedSite =
                (from s in XDocument.Load("applicationhost_saved.config").Descendants("site")
                 where s.Attribute("id").Value == "122"
                 select s).SingleOrDefault();
            Assert.NotNull(savedSite);
        }

        [Fact]
        public void TestDelete()
        {
            var fileIO = new FileIO("applicationhost_deleted.config");
            var siteToDelete = XDocument.Load("New_Site.xml").Element("site");
            var expected = WebSite.GetAllWebsites(fileIO).Count - 1;

            fileIO.Delete(siteToDelete, 1);
            var actual = WebSite.GetAllWebsites(fileIO).Count;

            Assert.Equal(expected, actual);
        }
    }
}
