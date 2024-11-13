<h1>FritzBoxClient</h1>
<span>This simple client library logs in to your local FRITZ!Box using the provided password and URL (default: https://fritz.box) and provides access to a range of network management features, including retrieving all connected devices and more.</span>

<h2>What makes this wrapper special</h2>
<span>
    This wrapper does not use either the TR-064 or the AHA-HTTP interface. It uses the "proprietary" API of the Fritzbox. Due to the lack of official API documentation (as of 2024), I had to figure out the endpoints myself, test them, and implement them in the client.
</span>
<h2>FritzBoxAcesser usage</h2>
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
<h2>FritzBoxNasAcesser usage</h2>
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
<h2>Disclaimer</h2>
 <span>This tool is only for testing and academic purposes and can only be used where strict consent has been given. Do not use it for illegal purposes! It is the end user’s responsibility to obey all applicable local, state, and federal laws. Developers assume no liability and are not responsible for any misuse or damage caused by this tool and software in general.</span>
 
