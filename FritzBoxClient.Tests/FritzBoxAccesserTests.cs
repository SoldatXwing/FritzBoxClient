namespace FritzBoxApi.Tests
{
    internal class FritzBoxAccesserTests
    {
        [Test]
        public async Task GetAllDevicesInNetworkAsync_Success()
        {
            List<Device> devices = await SetUp.FritzBoxAccesser.GetAllDevciesInNetworkAsync();
            Assert.Multiple(() =>
            {
                Assert.That(devices, Is.Not.Null);
                Assert.That(devices.Count, Is.GreaterThan(0));
            });
        }
        [Test]
        public async Task GetAllDevicesInNetworkAsyncWithIpAndUid_Success()
        {
            List<Device> devices = await SetUp.FritzBoxAccesser.GetAllDevciesInNetworkAsync(true);
            Assert.Multiple(() =>
            {
                Assert.That(devices, Is.Not.Null);
                Assert.That(devices, Is.Not.Empty);
            });
        }
        [Test]
        public void ChangeInternetAccessState_EmptyParameters()
        {
            var exception = Assert.ThrowsAsync<NotImplementedException>(async () =>
                await SetUp.FritzBoxAccesser.ChangeInternetAccessStateForDeviceAsync(
                    "",
                    FritzBoxClient.Models.InternetDetail.Unlimited,
                    new System.Net.IPAddress(new byte[] { 2, 2, 2, 2 }),
                    ""
                )
            );
            Assert.Multiple(() =>
            {
                Assert.That(exception, Is.Not.Null);
                Assert.That(exception.Message, Does.Contain("Parameters cant be empty or null!"));
            });

        }
    }
}
