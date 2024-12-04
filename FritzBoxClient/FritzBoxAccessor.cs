using FritzBoxClient.Enums;
using FritzBoxClient.Interfaces;
using FritzBoxClient.Logic;
using FritzBoxClient.Models;
using FritzBoxClient.Models.EnergyModels;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text;
namespace FritzBoxClient;


public class FritzBoxAccessor : BaseAccessor
{
    private static IFritzLogic? _FritzLogic;
    private FritzBoxAccessor() { }
    public static async Task<FritzBoxAccessor> CreateAsync(string fritzBoxPassword, FritzOsVersion osVersion, string fritzBoxUrl = "https://fritz.box", string userName = "")
    {
        _FritzLogic = osVersion switch
        {
            Enums.FritzOsVersion.Version7 => _FritzLogic = await FritzLogicV7.CreateAsync(fritzBoxPassword, fritzBoxUrl, userName),
            Enums.FritzOsVersion.Version8 => _FritzLogic = await FritzLogicV8.CreateAsync(fritzBoxPassword, fritzBoxUrl, userName),
            _ => throw new NotImplementedException("Not supported FritzOS given!")
        };
        return new();

    }
    /// <summary>
    /// Retrieves all connected devices in the local network. 
    /// </summary>
    /// <returns>List of devices in the local network.</returns>
    public async Task<List<IDevice>> GetAllConnectedDevciesInNetworkAsync() => await _FritzLogic!.GetAllConnectedDevicesInNetworkAsync<IDevice>();
    /// <summary>
    /// Reconnects the FritzBox to obtain a new IP address from the provider.
    /// </summary>
    /// <returns>Task representing the asynchronous operation.</returns>
    /// <exception cref="Exception">Thrown if reconnection fails.</exception>
    public async Task ReconnectAsync() => await _FritzLogic!.ReconnectAsync();
    /// <summary>
    /// Retrieves the WiFi password from the router settings.
    /// </summary>
    /// <returns>The WiFi password as a string.</returns>
    public async Task<string> GetWiFiPasswordAsync() => await _FritzLogic!.GetWiFiPasswordAsync();
    /// <summary>
    /// Retrieves a single device by name.
    /// </summary>
    /// <param name="deviceName">Name of the device.</param>
    /// <returns>The device with the specified name.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the device is not found.</exception>
    public async Task<IDevice> GetSingleDeviceAsync(string deviceName) => await _FritzLogic!.GetSingleDeviceAsync<IDevice>(deviceName);
    /// <summary>
    /// Retrieves a single device by IP address.
    /// </summary>
    /// <param name="ip">IP address of the device.</param>
    /// <returns>The device with the specified IP address.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the device is not found.</exception>
    public async Task<IDevice> GetSingleDeviceAsync(IPAddress ip) => await _FritzLogic!.GetSingleDeviceAsync<IDevice>(ip);
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
    public async Task<List<PowerConsumer>> GetPowerConsumersAsync() => await _FritzLogic!.GetPowerConsumersAsync();
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
    public async Task<List<Port>> GetOpenPortsAsync() => await _FritzLogic!.GetOpenPortsAsync();
    /// <summary>
    /// Changes the internet access state for a specified device.
    /// </summary>
    /// <param name="device">Device object with properties.</param>
    /// <param name="internetDetailState">New internet access state.</param>
    /// <exception cref="NotImplementedException">Thrown if parameters are missing.</exception>
    /// <exception cref="ArgumentException">Thrown if IP address format is invalid.</exception>
    public async Task ChangeInternetAccessStateForDeviceAsync(IDevice device, InternetState internetDetailState) => await _FritzLogic!.ChangeInternetAccessStateForDeviceAsync(device, internetDetailState);
    /// <summary>
    /// Changes the internet access state for a specified device in the local network.
    /// </summary>
    /// <param name="devName">Device name.</param>
    /// <param name="internetDetailState">New internet access state.</param>
    /// <param name="ipAdress">IP address of the device.</param>
    /// <param name="uid">UID of the device.</param>
    /// <exception cref="NotImplementedException">Thrown if parameters are missing.</exception>
    /// <exception cref="ArgumentException">Thrown if IP address format is invalid.</exception>
    public async Task ChangeInternetAccessStateForDeviceAsync(string devName, InternetState internetDetailState, IPAddress ipAdress, string uid) => await _FritzLogic!.ChangeInternetAccessStateForDeviceAsync(devName, internetDetailState, ipAdress, uid);







}




