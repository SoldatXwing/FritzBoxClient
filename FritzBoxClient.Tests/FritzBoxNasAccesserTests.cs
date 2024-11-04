using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FritzBoxApi.Tests
{
    internal class FritzBoxNasAccesserTests
    {
        //No specific tests are possible because the NAS systems are constructed differently
        [Test]
        public async Task GetNasBaseStorageDiskInfo_Success()
        {
            var result = await SetUp.NasAccesser.GetNasStorageDiskInfoAsync();
            Assert.That(result, Is.Not.Null);
        }
        [Test]
        public async Task GetNasBaseFoldersAsync_Success()
        {
            var result = await SetUp.NasAccesser.GetNasFoldersAsync();
            Assert.That(result, Is.Not.Null);
        }
        [Test]
        public void GetNasFileBytes_WrongPath()
        {
            var exception = Assert.ThrowsAsync<InvalidOperationException>(async () => await SetUp.NasAccesser.GetNasFileBytes("/huhuh/se.jpg"));

            Assert.Multiple(() =>
            {
                Assert.That(exception, Is.Not.Null);
                Assert.That(exception.Message, Does.Contain("Failed to get file bytes"));
            });
        }
    }
}
