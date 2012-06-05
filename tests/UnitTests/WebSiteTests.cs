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
        private IEnumerable<XElement> _validSites = XDocument.Load("Valid_Sites_Section.xml").Elements();

        [Fact]
        public void TestGetAllWebSite()
        {
            _fileIOMock.Setup(x => x.GetSitesSection()).Returns(_validSites);
            _fileIOMock.Setup(x => x.Exists(It.IsAny<string>())).Returns(true);

            var sites = WebSite.GetAllWebsites(_fileIOMock.Object);

            Assert.Equal(4, sites.Count);
            Assert.Equal("WebSite1", sites[0].Name);
            Assert.Equal(1, sites[0].Id);
            Assert.True(sites[0].ServerAutoStart);
            Assert.Equal("/", sites[0].ApplicationPath);
            Assert.Equal(string.Empty, sites[0].ApplicationPool);
            Assert.Equal("Clr4IntegratedAppPool", sites[1].ApplicationPool);
            Assert.Equal("/", sites[0].VirtualPath);
            Assert.Equal(@"C:\Users\alwalker\Desktop\MyFacebookSite3434\MyFacebookSite3434", sites[1].PhysicalPath);
            Assert.Equal(WebSite.BindingProtocol.http, sites[0].Protocol);
            Assert.Equal(":8080:localhost", sites[0].BindingInformation);
            Assert.True(sites[0].PhysicalDirectoryIsValid);
        }

        [Fact]
        public void TestGetAllWebsites_NullFileIO()
        {
            Assert.Throws(typeof(ArgumentNullException), () => WebSite.GetAllWebsites(null));
        }

        [Fact]
        public void TestGetAllWebsites_FileIOThrows()
        {
            _fileIOMock.Setup(x => x.GetSitesSection()).Throws(new Exception());

            Assert.Throws(typeof(ApplicationException), () => WebSite.GetAllWebsites(_fileIOMock.Object));
        }

        [Fact]
        public void TestGetAllWebsites_NullReturnFromFileIO()
        {
            _fileIOMock.Setup(x => x.GetSitesSection()).Returns(null as IEnumerable<XElement>);

            var sites = WebSite.GetAllWebsites(_fileIOMock.Object);

            Assert.Null(sites);
        }

        [Fact]
        public void TestGetAllWebsites_EmptyReturnFromFileIO()
        {
            _fileIOMock.Setup(x => x.GetSitesSection()).Returns(new List<XElement>());

            var sites = WebSite.GetAllWebsites(_fileIOMock.Object);

            Assert.Empty(sites);
        }
    }
}
