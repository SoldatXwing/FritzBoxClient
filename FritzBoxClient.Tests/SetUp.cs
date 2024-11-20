using FritzBoxClient;
using Microsoft.Extensions.Configuration;

namespace FritzBoxApi.Tests
{
    [SetUpFixture]
    internal class SetUp
    {
        public static FritzBoxAccessor? FritzBoxAccessor { get; private set; }
        public static FritzBoxNasAccessor? NasAccessor { get; private set; }
        public static IConfiguration Config { get; }
        static SetUp()
        {
            Config = new ConfigurationBuilder()
                        .SetBasePath(AppContext.BaseDirectory)
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)  // Ensure appsettings.json is present
                        .Build();
        }
        [OneTimeSetUp]
        public void SetUpAsync()
        {
            IConfigurationSection fritzBoxConfig = Config.GetSection("fritzBox");
            string password = fritzBoxConfig["password"]!;
            string fritzUserName = fritzBoxConfig["fritzUserName"]!;
            string fritzBoxUrl = fritzBoxConfig["fritzUrl"]!;

            if (string.IsNullOrEmpty(password))
                throw new InvalidOperationException("Password is not present in AppSettings!");

            FritzBoxAccessor = new FritzBoxAccessor(password, fritzBoxUrl, fritzUserName);
            NasAccessor = new FritzBoxNasAccessor(password, fritzBoxUrl, fritzUserName);
        }
        [OneTimeTearDown]
        public void TearDown()
        {
            FritzBoxAccessor?.Dispose();
            NasAccessor?.Dispose();
        }
    }
}