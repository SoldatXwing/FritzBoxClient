using FritzBoxClient.Exceptions.NasExceptions;
namespace FritzBoxClient.Tests
{
    internal class FritzBoxNasAccessorTests
    {
        //No specific tests are possible because the NAS systems are constructed differently
        [Test]
        public async Task GetNasBaseStorageDiskInfo_Success()
        {
            var result = await SetUp.NasAccessor!.GetNasStorageDiskInfoAsync();
            Assert.That(result, Is.Not.Null);
        }
        [Test]
        public async Task GetNasBaseFoldersAsync_Success()
        {
            var result = await SetUp.NasAccessor!.GetNasFoldersAsync();
            Assert.That(result, Is.Not.Null);
        }
        [Test]
        public void GetNasFileBytes_WrongPath()
        {
            string invalidPath = "/huhuh/se.jpg";
            var exception = Assert.ThrowsAsync<FritzBoxFileSystemException>(async () => await SetUp.NasAccessor!.GetNasFileBytes(invalidPath));

            Assert.Multiple(() =>
            {
                Assert.That(exception, Is.Not.Null);
                Assert.That(exception!.ErrorCode, Is.EqualTo(400));
                Assert.That(exception!.Message[..20], Is.EqualTo("[picture_controller]"));
                Assert.That(exception.ErrorDetails.First().Path, Is.EqualTo(invalidPath));
            });
        }
        [Test]
        public void DeleteNasFile_InvalidPath()
        {
            var exception = Assert.ThrowsAsync<FritzBoxFileSystemException>(async () =>
                await SetUp.NasAccessor!.DeleteFiles(new List<FritzBoxClient.Models.NasModels.NasFile> { new() { Path = "/this/is/invalid/path/to/file.png" } }));
            Assert.Multiple(() =>
            {
                Assert.That(exception, Is.Not.Null);
                Assert.That(exception!.ErrorCode, Is.EqualTo(400));
                Assert.That(exception.Message[..24], Is.EqualTo("[file_system_controller]"));
            });
        }

    }
}
