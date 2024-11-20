using FritzBoxClient.Models;
using FritzBoxClient.Models.EnergyModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text;
namespace FritzBoxClient;


public class FritzBoxAccesser : BaseAccesser
{
    /// <summary>
    /// Initializes a new instance of the FritzBoxAccesser with specified credentials.
    /// </summary>
    /// <param name="fritzBoxPassword">Password for FritzBox login.</param>
    /// <param name="fritzBoxUrl">URL of the FritzBox (default is "https://fritz.box").</param>
    /// <param name="userName">Username for FritzBox login.</param>
    public FritzBoxAccesser(string fritzBoxPassword, string fritzBoxUrl = "https://fritz.box", string userName = "") => (FritzBoxUrl, Password, FritzUserName) = (EnsureUrlHasScheme(fritzBoxUrl), fritzBoxPassword, userName);
    /// <summary>
    /// Checks if the given URL starts with "http://" or "https://".
    /// If not, "https://" is added by default.
    /// </summary>
    /// <param name="url">The URL to validate and adjust if necessary.</param>
    /// <returns>The corrected URL with a valid scheme.</returns>
    private static string EnsureUrlHasScheme(string url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            return "https://" + url.TrimStart('/');
        }
        return url;
    }
    /// <summary>
    /// Retrieves the JSON for the FritzBox overview page.
    /// </summary>
    /// <returns>JSON string of the overview page.</returns>
    /// <exception cref="Exception">Thrown if fetching fails.</exception>
    private async Task<string> GetOverViewPageJsonAsync()
    {
        if (!IsSidValid)
            await GenerateSessionIdAsync();
        var content = new StringContent($"xhr=1&sid={CurrentSid}&lang=de&page=overview&xhrId=all&useajax=1&no_sidrenew=", Encoding.UTF8, "application/x-www-form-urlencoded");
        var response = HttpRequestFritzBox("/data.lua", content, HttpRequestMethod.Post);
        if (response.IsSuccessStatusCode)
            return await response.Content.ReadAsStringAsync();
        throw new Exception("Failed to fetch fritzbox overview page json");
    }
    /// <summary>
    /// Retrieves the JSON for the FritzBox WiFi radio network page.
    /// </summary>
    /// <returns>JSON string of the WiFi radio network page.</returns>
    /// <exception cref="Exception">Thrown if fetching fails.</exception>
    private async Task<string> GetWifiRadioNetworkPageJsonAsync()
    {
        if (!IsSidValid)
            await GenerateSessionIdAsync();
        var content = new StringContent($"xhr=1&sid={CurrentSid}&lang=de&page=wSet&xhrId=all", Encoding.UTF8, "application/x-www-form-urlencoded");
        var response = HttpRequestFritzBox("/data.lua", content, HttpRequestMethod.Post);
        if (response.IsSuccessStatusCode)
            return await response.Content.ReadAsStringAsync();
        throw new Exception("Failed to fetch fritzbox Wifi radio network page json");
    }
    /// <summary>
    /// Resolves IP addresses and UIDs for a list of devices in the local network.
    /// </summary>
    /// <param name="devices">List of devices to resolve IP and UID for.</param>
    /// <returns>List of devices with updated IP and UID.</returns>
    private async Task<List<Device>> ResolveIpsAndUidForDevicesAsync(List<Device> devices)
    {
        var response = await GetWifiRadioNetworkPageJsonAsync();
        JToken knownWlanDevicesToken = JObject.Parse(response)["data"]!["wlanSettings"]!["knownWlanDevices"]!;
        List<KnownWlanDevice> knownWlanDevices = knownWlanDevicesToken.ToObject<List<KnownWlanDevice>>()!;
        devices.ForEach(c =>
        {
            try
            {
                var matchingDevice = knownWlanDevices.SingleOrDefault(d => d.Name == c.Name);
                if (matchingDevice is not null)
                {
                    c.Ip = IPAddress.Parse(matchingDevice.Ip);
                    c.Uid = matchingDevice.Uid;
                }
            }
            catch (InvalidOperationException) //catches if more than 1 "known" device is found, and now search in the active ones
            {
                var matchingDevice = knownWlanDevices.Where(c => c.Type == "active")
                    .SingleOrDefault(d => d.Name == c.Name);
                if (matchingDevice is not null)
                {
                    c.Ip = IPAddress.Parse(matchingDevice.Ip);
                    c.Uid = matchingDevice.Uid;
                }
            }

        });
        return devices;

    }
    /// <summary>
    /// Retrieves all connected devices in the local network. 
    /// </summary>
    /// <returns>List of devices in the local network.</returns>
    public async Task<List<Device>> GetAllConnectedDevciesInNetworkAsync() => await ResolveIpsAndUidForDevicesAsync(
        JsonConvert.DeserializeObject<FritzBoxResponse>(await GetOverViewPageJsonAsync())!.Data.Net.Devices!);

    /// <summary>
    /// Retrieves a single device by name.
    /// </summary>
    /// <param name="deviceName">Name of the device.</param>
    /// <returns>The device with the specified name.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the device is not found.</exception>

    public async Task<Device> GetSingleDeviceAsync(string deviceName)
    {
        var response = JObject.Parse(await GetWifiRadioNetworkPageJsonAsync());
        var deviceJson = response["data"]?["wlanSettings"]?["knownWlanDevices"]
                ?.FirstOrDefault(d => d["name"]?.ToString() == deviceName)
                ?.ToString();

        if (deviceJson is null)
            throw new InvalidOperationException($"No Device with name: {deviceName} found!");
        return JsonConvert.DeserializeObject<Device>(deviceJson)!;
    }
    /// <summary>
    /// Retrieves a single device by IP address.
    /// </summary>
    /// <param name="ip">IP address of the device.</param>
    /// <returns>The device with the specified IP address.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the device is not found.</exception>

    public async Task<Device> GetSingleDeviceAsync(IPAddress ip)
    {
        var response = await GetWifiRadioNetworkPageJsonAsync();
        var device = FindDeviceByIp(response, ip);

        if (device == null)
            throw new InvalidOperationException($"No device with IP: {ip} found.");

        return device;
    }

    private static Device? FindDeviceByIp(string jsonResponse, IPAddress ip)
    {
        var response = JObject.Parse(jsonResponse);
        var devices = response["data"]?["wlanSettings"]?["knownWlanDevices"];

        var deviceJson = devices?.FirstOrDefault(d => d["ip"]?.ToString() == ip.ToString())?.ToString();

        return deviceJson != null
            ? JsonConvert.DeserializeObject<Device>(deviceJson)
            : null;
    }
    /// <summary>
    /// Changes the internet access state for a specified device in the local network.
    /// </summary>
    /// <param name="devName">Device name.</param>
    /// <param name="internetDetailState">New internet access state.</param>
    /// <param name="ipAdress">IP address of the device.</param>
    /// <param name="dev">UID of the device.</param>
    /// <exception cref="NotImplementedException">Thrown if parameters are missing.</exception>
    /// <exception cref="ArgumentException">Thrown if IP address format is invalid.</exception>
    public async Task ChangeInternetAccessStateForDeviceAsync(string devName, InternetState internetDetailState, IPAddress ipAdress, string dev)
    {
        if (string.IsNullOrEmpty(devName) ||
            string.IsNullOrEmpty(dev) ||
            ipAdress is null)
            throw new NotImplementedException("Parameters cant be empty or null!");
        try
        {
            if (!IsSidValid)
                await GenerateSessionIdAsync();
            var interFaceResponse = HttpRequestFritzBox("/data.lua", new StringContent($"xhr=1&sid={CurrentSid}&lang=de&page=edit_device&xhrId=all&dev={dev}&back_to_page=wSet", Encoding.UTF8, "application/x-www-form-urlencoded"), HttpRequestMethod.Post);
            var iFaceIdJson = JObject.Parse(await interFaceResponse.Content.ReadAsStringAsync());
            string interFaceId = string.Empty;
            //IPv6
            if (bool.Parse(iFaceIdJson["data"]!["vars"]!["ipv6_enabled"]!.ToString()))
                interFaceId = iFaceIdJson["data"]!["vars"]!["dev"]!["ipv6"]!["iface"]!["ifaceid"]!.ToString();
            else //IPv4
                throw new Exception("Unable to get interfaceid from device!");

            string[] interFaceParts = interFaceId.Split(':');
            string[] ipOctets = ipAdress.ToString().Split('.');
            if (ipOctets.Length != 4)
                throw new ArgumentException("Invalid IP address format");

            var bodyParamters = new StringContent(
                $"xhr=1&dev_name={devName}&internetdetail={internetDetailState.ToString().ToLower()}&allow_pcp_and_upnp=off&dev_ip0={ipOctets[0]}&dev_ip1={ipOctets[1]}&dev_ip2={ipOctets[2]}&dev_ip3={ipOctets[3]}&dev_ip={ipAdress}&static_dhcp=off&interface_id1={interFaceParts[2]}&interface_id2={interFaceParts[3]}&interface_id3={interFaceParts[4]}&interface_id4={interFaceParts[5]}&back_to_page=wSet&dev={dev}&apply=true&sid={CurrentSid}&lang=de&page=edit_device",
                Encoding.UTF8,
                "application/x-www-form-urlencoded"
                );

            var response = HttpRequestFritzBox("/data.lua", bodyParamters, HttpRequestMethod.Post);
            if (!response.IsSuccessStatusCode)
                throw new Exception($"Error blocking internet access for device {devName}. Ensure all parameters are correct");
        }
        catch
        {
            throw new ArgumentException("Invalid IP address format");
        }


    }
    /// <summary>
    /// Changes the internet access state for a specified device.
    /// </summary>
    /// <param name="device">Device object with properties.</param>
    /// <param name="internetDetailState">New internet access state.</param>
    /// <exception cref="NotImplementedException">Thrown if parameters are missing.</exception>
    /// <exception cref="ArgumentException">Thrown if IP address format is invalid.</exception>
    public async Task ChangeInternetAccessStateForDeviceAsync(Device device, InternetState internetDetailState)
    {
        if (string.IsNullOrEmpty(device.Name) ||
            string.IsNullOrEmpty(device.Uid) ||
            device.Ip is null)
            throw new NotImplementedException("Paramters cant be empty or null!");
        try
        {
            if (!IsSidValid)
                await GenerateSessionIdAsync();
            var interFaceResponse = HttpRequestFritzBox("/data.lua", new StringContent($"xhr=1&sid={CurrentSid}&lang=de&page=edit_device&xhrId=all&dev={device.Uid}&back_to_page=wSet", Encoding.UTF8, "application/x-www-form-urlencoded"), HttpRequestMethod.Post);
            var iFaceIdJson = JObject.Parse(await interFaceResponse.Content.ReadAsStringAsync());
            string interFaceId = string.Empty;
            //IPv6
            if (bool.Parse(iFaceIdJson["data"]!["vars"]!["ipv6_enabled"]!.ToString()))
                interFaceId = iFaceIdJson["data"]!["vars"]!["dev"]!["ipv6"]!["iface"]!["ifaceid"]!.ToString();
            else //IPv4
                throw new Exception("Unable to get interfaceid from device!");

            string[] interFaceParts = interFaceId.Split(':');
            string[] ipOctets = device.Ip.ToString().Split('.');
            if (ipOctets.Length != 4)
                throw new ArgumentException("Invalid IP address format");

            var bodyParamters = new StringContent(
                $"xhr=1&dev_name={device.Name}&internetdetail={internetDetailState.ToString().ToLower()}&allow_pcp_and_upnp=off&dev_ip0={ipOctets[0]}&dev_ip1={ipOctets[1]}&dev_ip2={ipOctets[2]}&dev_ip3={ipOctets[3]}&dev_ip={device.Ip.ToString()}&static_dhcp=off&interface_id1={interFaceParts[2]}&interface_id2={interFaceParts[3]}&interface_id3={interFaceParts[4]}&interface_id4={interFaceParts[5]}&back_to_page=wSet&dev={device.Uid}&apply=true&sid={CurrentSid}&lang=de&page=edit_device",
                Encoding.UTF8,
                "application/x-www-form-urlencoded"
                );

            var response = HttpRequestFritzBox("/data.lua", bodyParamters, HttpRequestMethod.Post);
            if (!response.IsSuccessStatusCode)
                throw new Exception($"Error blocking internet access for device {device.Name}. Ensure all parameters are correct");
        }
        catch
        {
            throw new ArgumentException("Invalid IP address format");
        }


    }
    /// <summary>
    /// Reconnects the FritzBox to obtain a new IP address from the provider.
    /// </summary>
    /// <returns>Task representing the asynchronous operation.</returns>
    /// <exception cref="Exception">Thrown if reconnection fails.</exception>
    public async Task ReconnectAsync()
    {
        if (!IsSidValid)
            await GenerateSessionIdAsync();
        var content = new StringContent($"xhr=1&sid={CurrentSid}&lang=de&page=netMoni&xhrId=reconnect&disconnect=true&useajax=1&no_sidrenew=", Encoding.UTF8, "application/x-www-form-urlencoded");
        var response = HttpRequestFritzBox("/data.lua", content, HttpRequestMethod.Post);
        Thread.Sleep(1000);
        content = new StringContent($"xhr=1&sid={CurrentSid}&lang=de&page=netMoni&xhrId=reconnect&connect=true&useajax=1&no_sidrenew=");
        var secondResponse = HttpRequestFritzBox("/data.lua", content, HttpRequestMethod.Post);
        if (!response.IsSuccessStatusCode && secondResponse.IsSuccessStatusCode)
            throw new Exception("Error reconnecting frit box!");
    }
    /// <summary>
    /// Asynchronously retrieves all open ports with their service names and used protocols from the FritzBox.
    /// </summary>
    /// <returns>
    /// A list of <see cref="Port"/> objects representing the open ports. 
    /// Each object includes the service name, port number, used protocols, and an index.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the response from the FritzBox indicates failure to retrieve the open ports.
    /// </exception>
    public async Task<List<Port>> GetOpenPortsAsync()
    {
        if (!IsSidValid)
            await GenerateSessionIdAsync();
        var content = new StringContent($"xhr=1&sid={CurrentSid}&lang=de&page=secCheck&xhrId=all", Encoding.UTF8, "application/x-www-form-urlencoded");
        var response = HttpRequestFritzBox("/data.lua", content, HttpRequestMethod.Post);
        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException("Failed to recieved open ports");
        return JsonConvert.DeserializeObject<List<Port>>(JObject.Parse(await response.Content.ReadAsStringAsync())["data"]!["homenet"]!["services"]!
                                                                .ToString())!;
    }
    /// <summary>
    /// Returns all power consumers of the FritzBox, such as Wlan, DSL, and connected USB devices.
    /// The first element in the list is most likely the main system of the FritzBox.
    /// </summary>
    /// <returns>   
    /// A task representing the asynchronous operation. The task result is a list of <see cref="PowerConsumer"/> objects 
    /// representing the power consumers (e.g., Wlan, DSL, USB devices).
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the request to retrieve power consumers fails (e.g., network error or invalid response).
    /// </exception>
    public async Task<List<PowerConsumer>> GetPowerConsumersAsync()
    {
        if (!IsSidValid)
            await GenerateSessionIdAsync();
        var content = new StringContent($"sid={CurrentSid}&page=energy", Encoding.UTF8, "application/x-www-form-urlencoded");
        var response = HttpRequestFritzBox("/data.lua", content, HttpRequestMethod.Post);
        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException("Failed to recieve power consumers");
        return JsonConvert.DeserializeObject<List<PowerConsumer>>(JObject.Parse(await response.Content.ReadAsStringAsync())["data"]!["drain"]!
                                                                .ToString())!;
    }
   

}




