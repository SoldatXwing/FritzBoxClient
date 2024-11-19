using FritzBoxClient.Models.EnergyModels;

namespace FritzBoxApi.Tests
{
    internal class FritzBoxAccesserTests
    {
        [Test]
        public async Task GetAllConnectedDevicesInNetworkAsync_Success()
        {
            List<Device> devices = await SetUp.FritzBoxAccesser!.GetAllConnectedDevciesInNetworkAsync();
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
                await SetUp.FritzBoxAccesser!.ChangeInternetAccessStateForDeviceAsync(
                    "",
                    FritzBoxClient.Models.InternetState.Unlimited,
                    new System.Net.IPAddress(new byte[] { 2, 2, 2, 2 }),
                    ""
                )
            );
            Assert.Multiple(() =>
            {
                Assert.That(exception, Is.Not.Null);
                Assert.That(exception!.Message, Does.Contain("Parameters cant be empty or null!"));
            });

        }
        [Test]
        public async Task GetPowerConsumer_Success()
        {
            List<PowerConsumer> powerConsumers = await SetUp.FritzBoxAccesser!.GetPowerConsumersAsync();
            Assert.Multiple(() =>
            {
                Assert.That(powerConsumers, Is.Not.Null);
                Assert.That(powerConsumers, Is.Not.Empty); // Atleast the fritzbox itself is a power consumer
            });
        }
    }
}
