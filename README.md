<h1>FritzBoxClient</h1>
<span>This simple client library logs in to your local FRITZ!Box using the provided password and URL (default: https://fritz.box) and provides access to a range of network management features, including retrieving all connected devices and more.</span>

<h1>FritzBoxAcesser usage</h1>
<span>This simple approach shows how to initialize the FritzBoxAccesser and get the devices from the FritzBox.</span>
<br/><br/>

```csharp
using FritzBoxClient;
public class Program
{
    private static async Task Main(string[] args)
    {        
        FritzBoxAccesser fritzBoxAccesser = new FritzBoxAccesser(fritzBoxPassword: "password");
        var devices = await fritzBoxAccesser.GetAllDevciesInNetworkAsync();

        foreach(Device device in devices)
            Console.WriteLine($"Device: {device.Name}, is active: {device.StateInfo.Active}");
    }
}
```

<span>Specify more details for the access:</span>
```csharp
FritzBoxAccesser fritzBoxAccesser = new FritzBoxAccesser(fritzBoxPassword: "password", fritzBoxUrl: "https://192.168.178.1", userName: "fritz3000");
```
<br/>
<span>
    To change a device's internet access state, do the following:
</span>

```csharp
using FritzBoxClient;
public class Program
{
    private static async Task Main(string[] args)
    {
        FritzBoxAccesser fritzBoxAccesser = new FritzBoxAccesser(fritzBoxPassword: "password");
        var device = await fritzBoxAccesser.GetSingleDeviceAsync(deviceName: "DESKTOP123"); //Also works with ip: fritzBoxAccesser.GetSingleDeviceAsync(ip: IPAddress.Parse("192.168.178.2"));
        await fritzBoxAccesser.ChangeInternetAccessStateForDeviceAsync(device, InternetDetail.Blocked);
    }
}
```
<h1>FritzBoxNasAcesser usage</h1>
<span>This simple approach shows how to initialize the FritzBoxNasAccessor and get the NAS storage disk information.</span>
<br/><br/>


```csharp
using FritzBoxClient;
public class Program
{
    private static async Task Main(string[] args)
    {
        FritzBoxNasAccesser nasAccesser = new FritzBoxNasAccesser(fritzBoxPassword: "password", fritzBoxUrl: "https://192.168.178.1");
        var diskInformation = await nasAccesser.GetNasStorageDiskInfoAsync(path: "/");
        Console.WriteLine($"Total storage: {diskInformation.TotalInMb}Mb, free storage: {diskInformation.FreeInMb}Mb, used storage: {diskInformation.UsedInMb}Mb");
    }
}
```
<span>To get the bytes of a file, do the following:</span>
```csharp
FritzBoxNasAccesser nasAccesser = new FritzBoxNasAccesser(fritzBoxPassword: "password", fritzBoxUrl: "https://192.168.178.1");
byte[] fileBytes = await nasAccesser.GetNasFileBytes("/path/to/file.png");
```
<br/>
<h1>Info</h1>
<span>
  If you want to set a custom FritzBox URL, make sure to use <code>https://</code>. For example, <code>https://192.168.178.1</code>.
</span>
<br/><br/>
<span>The following benchmark shows the performance difference between the GetAllDevicesInNetworkAsync method with the getWithIp parameter set to true and false.</span>

```
| Method              | Mean    | Error    | StdDev   |
|-------------------- |--------:|---------:|---------:|
| GetDevicesWithoutIp | 4.451 s | 0.0881 s | 0.1633 s |
| GetDevicesWithIp    | 6.855 s | 0.1362 s | 0.2160 s |
```


<br/>
<h1>Disclaimer</h1>
 <span>This tool is only for testing and academic purposes and can only be used where strict consent has been given. Do not use it for illegal purposes! It is the end user’s responsibility to obey all applicable local, state, and federal laws. Developers assume no liability and are not responsible for any misuse or damage caused by this tool and software in general.</span>
 
