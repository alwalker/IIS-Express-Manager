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
            var fileIO = new FileIO();

            var actual = fileIO.GetSitesSection("applicationhost.config");

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
            var fileIO = new FileIO();

            Assert.True(fileIO.Exists("C:\\"));
        }

        [Fact]
        public void TestExists_DoesNotExist()
        {
            var fileIO = new FileIO();

            Assert.False(fileIO.Exists("C:\\this_dir_no_exist"));
        }
    }
}
