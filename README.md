# FritzBoxClient

This simple client library logs in to your local FRITZ!Box using the provided password and URL (default: `https://fritz.box`) and provides access to a range of network management features, including retrieving all connected devices and more.

## What makes this wrapper special

This wrapper does not use either the TR-064 or the AHA-HTTP interface. It uses the "proprietary" API of the Fritzbox. Due to the lack of official API documentation (as of 2024), I had to figure out the endpoints myself, test them, and implement them in the client.

## Download sources:
- [![NuGet](https://img.shields.io/badge/NuGet-Package-blue)](https://www.nuget.org/packages/SoldatXwing.FritzBoxClient)
- [![GitHub](https://img.shields.io/badge/GitHub-Releases-black)](https://github.com/SoldatXwing/FritzBoxClient/releases)

## Compatibality
- [v.1.0.0](https://github.com/SoldatXwing/FritzBoxClient/releases/tag/v1.0.0) 
    - Fully compatible with FritzOs versions < 8.0 and > 7.0
    - Partially compatible with FritzOs version 8.x
- v1.1.0
    - Currently working on 
## FritzBoxAccessor Usage

This simple approach shows how to initialize the `FritzBoxAccessor` and get the devices from the FritzBox:

```csharp
using FritzBoxClient;

using FritzBoxAccessor accessor = await FritzBoxAccessor.CreateAsync("password");
var devices = await accessor.GetAllConnectedDevciesInNetworkAsync();

foreach (var device in devices)
    Console.WriteLine($"Device: {device.Name}, Ip: {device.Ip}");
```
Specify more details for the access:
```csharp
FritzBoxAccessor fritzBoxAccessor = await FritzBoxAccessor.CreateAsync(fritzBoxPassword: "password", fritzBoxUrl: "https://192.168.178.1", userName: "fritz3000");
```
To change a device's internet access state, do the following: 
```csharp
using FritzBoxClient;
using FritzBoxClient.Models;

using FritzBoxAccessor accessor = await FritzBoxAccessor.CreateAsync(fritzBoxPassword: "password");
var device = await accessor.GetSingleDeviceAsync(deviceName: "DESKTOP123");

await accessor.ChangeInternetAccessStateForDeviceAsync(device, InternetState.Blocked);
```
## FritzBoxNasAccessor usage
This simple approach shows how to initialize the FritzBoxNasAccessor and get the NAS storage disk information. 
```csharp
using FritzBoxClient;

using FritzBoxNasAccessor nasAccessor = await FritzBoxNasAccessor.CreateAsync(fritzBoxPassword: "password", fritzBoxUrl: "https://192.168.178.1");
var diskInformation = await nasAccessor.GetNasStorageDiskInfoAsync(path: "/Files/");

Console.WriteLine($"Total storage: {diskInformation.TotalInMb}Mb, free storage: {diskInformation.FreeInMb}Mb, used storage: {diskInformation.UsedInMb}Mb");
```
To get the bytes of a file, do the following:
```csharp
byte[] fileBytes = await nasAccessor.GetNasFileBytes("/path/to/file.png");
```
## Disclaimer
This tool is only for testing and academic purposes and can only be used where strict consent has been given. Do not use it for illegal purposes! It is the end user’s responsibility to obey all applicable local, state, and federal laws. Developers assume no liability and are not responsible for any misuse or damage caused by this tool and software in general.

This project is not affiliated with, endorsed by, or sponsored by AVM GmbH. "AVM," "FRITZ!," and "FRITZ!Box" are trademarks of AVM GmbH, used here solely to indicate compatibility. No AVM logos, designs, or graphical elements are included. Use this project responsibly and in compliance with AVM's guidelines. AVM assumes no liability for this project.
