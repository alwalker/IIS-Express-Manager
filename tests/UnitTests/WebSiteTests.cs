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

        [Fact]
        public void TestSave()
        {
            var newSite = XDocument.Load("New_Site.xml").Element("site");
            _fileIOMock.Setup(x => x.GetSitesSection()).Returns(_validSites);
            _fileIOMock.Setup(x => x.Exists(It.IsAny<string>())).Returns(true);
            _fileIOMock.Setup(x => x.Save(It.Is<XElement>(y => CompareElements(y, newSite)), It.IsAny<int>())).Verifiable();
            var site = WebSite.GetAllWebsites(_fileIOMock.Object)[0];

            site.PhysicalPath = "test test";
            site.Save(_fileIOMock.Object);

            Assert.False(site.IsDirty);
            _fileIOMock.VerifyAll();
        }

        [Fact]
        public void TestDelete()
        {
            var newSite = XDocument.Load("Site.xml").Element("site");
            _fileIOMock.Setup(x => x.GetSitesSection()).Returns(_validSites);
            _fileIOMock.Setup(x => x.Exists(It.IsAny<string>())).Returns(true);
            _fileIOMock.Setup(x => x.Delete(It.Is<XElement>(y => CompareElements(y, newSite)), It.IsAny<int>())).Verifiable();
            var sites = WebSite.GetAllWebsites(_fileIOMock.Object);
            var site = sites.First();

            site.Delete(_fileIOMock.Object);

            _fileIOMock.VerifyAll();
        }

        [Fact]
        public void TestCreateNewWebsite()
        {
            var id = 27;
            var name = "Test Add";
            var serverAutoStart = false;
            var applicationPath = "/";
            var applicationPool = "test";
            var virtualPath = "/";
            var physicalPath = @"c:\Users";
            var protocol = WebSite.BindingProtocol.ftp;
            var bindingInformation = ":8080";
            var newSiteXML = XDocument.Load("CreatedSite.xml").Element("site");
            _fileIOMock.Setup(x => x.Exists(It.IsAny<string>())).Returns(true);
            _fileIOMock.Setup(x => x.Save(It.Is<XElement>(y => CompareElements(y, newSiteXML)), It.IsAny<int>())).Verifiable();

            var newSite = WebSite.Create(_fileIOMock.Object, id, name, serverAutoStart, applicationPath,applicationPool,
                virtualPath, physicalPath,protocol, bindingInformation);

            _fileIOMock.VerifyAll();
            Assert.Equal(id, newSite.Id);
            Assert.Equal(name, newSite.Name);
            Assert.Equal(serverAutoStart, newSite.ServerAutoStart);
            Assert.Equal(applicationPool, newSite.ApplicationPool);
            Assert.Equal(applicationPath, newSite.ApplicationPath);
            Assert.Equal(virtualPath, newSite.VirtualPath);
            Assert.Equal(physicalPath, newSite.PhysicalPath);
            Assert.Equal(protocol, newSite.Protocol);
            Assert.Equal(bindingInformation, newSite.BindingInformation);
            Assert.True(newSite.PhysicalDirectoryIsValid);
        }

        private bool CompareElements(XElement left, XElement right)
        {
            var lnodes = left.Nodes().ToList();
            var rnodes = right.Nodes().ToList();

            if (lnodes.Count != rnodes.Count)
            {
                return false;
            }

            for (int i = 0; i < lnodes.Count; i++)
            {
                if (!XElement.DeepEquals(lnodes[i], rnodes[i]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
