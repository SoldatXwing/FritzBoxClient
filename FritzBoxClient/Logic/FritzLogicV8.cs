using FritzBoxClient.Enums;
using FritzBoxClient.Interfaces;
using FritzBoxClient.Models;
using FritzBoxClient.Models.EnergyModels;
using FritzBoxClient.Models.FritzOsVersion8;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FritzBoxClient.Logic
{
    public class FritzLogicV8 : BaseAccessor, IFritzLogic
    {
        public List<Device>? Devices { get; private set; }

        private FritzLogicV8(string fritzBoxPassword, string fritzBoxUrl, string userName) => (FritzBoxUrl, Password, FritzUserName) = (EnsureUrlHasScheme(fritzBoxUrl), fritzBoxPassword, userName);
        public static async Task<FritzLogicV8> CreateAsync(string fritzBoxPassword, string fritzBoxUrl = "https://fritz.box", string userName = "")
        {
            FritzLogicV8 logic = new(fritzBoxPassword, fritzBoxUrl, userName);
            await logic.InitializeAsync();
            return logic;
        }
        private async Task UpdateDeviceListAsync() => Devices = JsonConvert.DeserializeObject<List<Device>>(JObject.Parse(await HttpRequestFritzBox("api/v0/landevice", null, HttpRequestMethod.Get).Content.ReadAsStringAsync())["landevice"]!.ToString());
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
        /// Changes the internet access state for a specified device in the local network.
        /// </summary>
        /// <param name="devName">Device name.</param>
        /// <param name="internetDetailState">New internet access state.</param>
        /// <param name="ipAdress">IP address of the device.</param>
        /// <param name="uid">UID of the device.</param>
        /// <exception cref="NotImplementedException">Thrown if parameters are missing.</exception>
        /// <exception cref="ArgumentException">Thrown if IP address format is invalid.</exception>
        public async Task ChangeInternetAccessStateForDeviceAsync(string devName, InternetState internetDetailState, IPAddress ipAdress, string uid)
        {
            if (string.IsNullOrEmpty(devName) ||
                string.IsNullOrEmpty(uid) ||
                ipAdress is null)
                throw new NotImplementedException("Parameters cant be empty or null!");
            try
            {
                if (!IsSidValid)
                    await GenerateSessionIdAsync();
                var interFaceResponse = HttpRequestFritzBox("/data.lua", new StringContent($"xhr=1&sid={CurrentSid}&lang=de&page=edit_device&xhrId=all&dev={uid}&back_to_page=wSet", Encoding.UTF8, "application/x-www-form-urlencoded"), HttpRequestMethod.Post);
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
                    $"xhr=1&dev_name={devName}&internetdetail={internetDetailState.ToString().ToLower()}&allow_pcp_and_upnp=off&dev_ip0={ipOctets[0]}&dev_ip1={ipOctets[1]}&dev_ip2={ipOctets[2]}&dev_ip3={ipOctets[3]}&dev_ip={ipAdress}&static_dhcp=off&interface_id1={interFaceParts[2]}&interface_id2={interFaceParts[3]}&interface_id3={interFaceParts[4]}&interface_id4={interFaceParts[5]}&back_to_page=wSet&dev={uid}&apply=true&sid={CurrentSid}&lang=de&page=edit_device",
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
        public async Task ChangeInternetAccessStateForDeviceAsync(IDevice device, InternetState internetDetailState)
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

        public async Task<List<T>> GetAllConnectedDevicesInNetworkAsync<T>() where T : IDevice
        {
            var rawDeviceList = JArray.Parse(await HttpRequestFritzBox("api/v0/misc/updateStatus", null, HttpRequestMethod.Get).Content.ReadAsStringAsync());
            await UpdateDeviceListAsync();

            var filteredDevices = Devices!
                .OfType<T>()
                .Where(device => rawDeviceList.Any(raw => raw["deviceUID"]?.ToString() == device.Uid))
                .ToList();

            return filteredDevices;
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

        /// <summary>
        /// Retrieves a single device by IP address.
        /// </summary>
        /// <param name="ip">IP address of the device.</param>
        /// <returns>The device with the specified IP address.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the device is not found.</exception>
        public async Task<T> GetSingleDeviceAsync<T>(IPAddress ip) where T : IDevice
        {
            await UpdateDeviceListAsync();
            var device = Devices!.SingleOrDefault(c => c.Ip.Equals(ip));

            if (device == null)
                throw new InvalidOperationException($"No device with IP: {ip} found.");

            if (device is T typedDevice)
                return typedDevice;
            throw new InvalidOperationException($"Device found, but it's not of the expected type {typeof(T).Name}.");
        }
        /// <summary>
        /// Retrieves a single device by name.
        /// </summary>
        /// <param name="deviceName">Name of the device.</param>
        /// <returns>The device with the specified name.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the device is not found.</exception>
        public async Task<T> GetSingleDeviceAsync<T>(string deviceName) where T : IDevice
        {
            await UpdateDeviceListAsync();
            var device = Devices!.SingleOrDefault(c => c.Name.Equals(deviceName));

            if (device is null)
                throw new InvalidOperationException($"No Device with name: {deviceName} found!");
            if (device is T typedDevice)
            {
                return typedDevice;
            }
            throw new InvalidOperationException($"Device found, but it's not of the expected type {typeof(T).Name}.");
        }

        /// <summary>
        /// Retrieves the WiFi password from the router settings.
        /// </summary>
        /// <returns>The WiFi password as a string.</returns>
        public async Task<string> GetWiFiPasswordAsync() => JObject.Parse(await GetWifiRadioNetworkPageJsonAsync())!["data"]!["wlanSettings"]!["psk"]!.ToObject<string>()! ?? throw new InvalidOperationException("WiFi password could not be retrieved.");


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

    }
}
