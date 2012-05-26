using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using Moq;
using IISExpressManager;
using System.Xml.Linq;

namespace UnitTests
{
    public class WebSiteTests
    {
        private Mock<IFileIO> _fileIOMock = new Mock<IFileIO>();
        private XDocument _validSites = XDocument.Load("Valid_Sites_Section.xml");

        [Fact]
        public void TestGetAllWebSite()
        {
            _fileIOMock.Setup(x => x.GetSitesSection()).Returns(_validSites);

            var sites = WebSite.GetAllWebsites(_fileIOMock.Object);

            Assert.Equal(4, sites.Count);
            Assert.Equal("WebSite1", sites[0].Name);
            Assert.Equal(1, sites[0].Id);
            Assert.True(sites[0].ServerAutoStart);
            Assert.Equal("/", sites[0].ApplicationPath);
            Assert.Equal(string.Empty, sites[0].ApplicationPool);
            Assert.Equal("Clr4IntegratedAppPool", sites[1].ApplicationPool);
        }
    }
}
