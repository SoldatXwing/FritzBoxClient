<h1>FritzBoxClient</h1>
<span>This simple client library logs in to your local FRITZ!Box using the provided password and URL (default: https://fritz.box) and provides access to a range of network management features, including retrieving all connected devices and more.</span>

<h2>What makes this wrapper special</h2>
<span>
    This wrapper does not use either the TR-064 or the AHA-HTTP interface. It uses the "proprietary" API of the Fritzbox. Due to the lack of official API documentation (as of 2024), I had to figure out the endpoints myself, test them, and implement them in the client.
</span>
<h2>FritzBoxAccessor usage</h2>
<span>This simple approach shows how to initialize the FritzBoxAccessor and get the devices from the FritzBox.</span>
<br/><br/>

```csharp
using FritzBoxClient;

using FritzBoxAccessor accessor = await FritzBoxAccessor.CreateAsync("password");
var devices = await accessor.GetAllConnectedDevciesInNetworkAsync();

foreach (var device in devices)
    Console.WriteLine($"Device: {device.Name}, Ip: {device.Ip}");

```

<span>Specify more details for the access:</span>
```csharp
FritzBoxAccessor fritzBoxAccessor = await FritzBoxAccessor.CreateAsync(fritzBoxPassword: "password", fritzBoxUrl: "https://192.168.178.1", userName: "fritz3000");
```
<br/>
<span>
    To change a device's internet access state, do the following:
</span>

```csharp
using FritzBoxClient;
using FritzBoxClient.Models;

using FritzBoxAccessor accessor = await FritzBoxAccessor.CreateAsync(fritzBoxPassword: "password");
var device = await accessor.GetSingleDeviceAsync(deviceName: "DESKTOP123");

await accessor.ChangeInternetAccessStateForDeviceAsync(device, InternetState.Blocked);

```
<h2>FritzBoxNasAccessor usage</h2>
<span>This simple approach shows how to initialize the FritzBoxNasAccessor and get the NAS storage disk information.</span>
<br/><br/>


```csharp
using FritzBoxClient;

using FritzBoxNasAccessor nasAccessor = await FritzBoxNasAccessor.CreateAsync(fritzBoxPassword: "password", fritzBoxUrl: "https://192.168.178.1");
var diskInformation = await nasAccessor.GetNasStorageDiskInfoAsync(path: "/Files/");

Console.WriteLine($"Total storage: {diskInformation.TotalInMb}Mb, free storage: {diskInformation.FreeInMb}Mb, used storage: {diskInformation.UsedInMb}Mb");****

```
<span>To get the bytes of a file, do the following:</span>
```csharp
byte[] fileBytes = await nasAccessor.GetNasFileBytes("/path/to/file.png");
```

<br/>
<h2>Disclaimer</h2>
 <span>This tool is only for testing and academic purposes and can only be used where strict consent has been given. Do not use it for illegal purposes! It is the end user’s responsibility to obey all applicable local, state, and federal laws. Developers assume no liability and are not responsible for any misuse or damage caused by this tool and software in general.</span>
 
