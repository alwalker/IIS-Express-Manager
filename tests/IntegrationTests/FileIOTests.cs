using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using System.Xml.Linq;
using IISExpressManager;

namespace IntegrationTests
{
    public class FileIOTests
    {
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
            var fileIO = new FileIO(string.Empty);

            Assert.True(fileIO.Exists("C:\\"));
        }

        [Fact]
        public void TestExists_DoesNotExist()
        {
            var fileIO = new FileIO(string.Empty);

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
    }
}
