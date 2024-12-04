using FritzBoxClient.Enums;
using FritzBoxClient.Models;
using FritzBoxClient.Models.EnergyModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FritzBoxClient.Interfaces
{
    public interface IFritzLogic
    {
        Task<List<T>> GetAllConnectedDevicesInNetworkAsync<T>() where T : IDevice;
        Task<string> GetWiFiPasswordAsync();
        Task<T> GetSingleDeviceAsync<T>(string deviceName) where T : IDevice;
        Task<T> GetSingleDeviceAsync<T>(IPAddress ip) where T : IDevice;
        Task ChangeInternetAccessStateForDeviceAsync(string devName, InternetState internetDetailState, IPAddress ipAdress, string uid);
        Task ChangeInternetAccessStateForDeviceAsync(IDevice device, InternetState internetDetailState);
        Task ReconnectAsync();
        Task<List<Port>> GetOpenPortsAsync();
        Task<List<PowerConsumer>> GetPowerConsumersAsync();
    }
}
