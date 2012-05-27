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
    }
}
