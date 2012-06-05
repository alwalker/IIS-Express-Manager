using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using IISExpressManager;
using System.Xml.Linq;

namespace IntegrationTests
{
    public class WebSiteTests
    {
        [Fact]
        public void TestGetAllWebSite()
        {

            var sites = WebSite.GetAllWebsites(new FileIO("applicationhost.config"));

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
            Assert.False(sites[1].PhysicalDirectoryIsValid);
            Assert.True(sites[0].PhysicalDirectoryIsValid);
        }
    }
}
