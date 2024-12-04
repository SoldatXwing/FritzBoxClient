using FritzBoxClient.Converter;
using FritzBoxClient.Interfaces;
using Newtonsoft.Json;
using System.Net;
namespace FritzBoxClient.Models.FritzOsVersion8
{
    public class Device : IDevice
    {
        [JsonProperty("myfritz_enabled")]
        public string MyFritzEnabled { get; set; }

        [JsonProperty("device_class_user")]
        public string DeviceClassUser { get; set; }

        [JsonProperty("maclist")]
        public string MacList { get; set; } 

        [JsonProperty("UID")]
        public string Uid { get; set; }

        [JsonProperty("plc_UIDs")]
        public string PlcUIDs { get; set; }

        [JsonProperty("modelname")]
        public string ModelName { get; set; }

        [JsonProperty("speed")]
        public int Speed { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("auto_wakeup")]
        [JsonConverter(typeof(BoolConverter))]

        public bool AutoWakeup { get; set; }

        [JsonProperty("igd_fw_cnt_pcp")]
        public int IgdFwCntPcp { get; set; }

        [JsonProperty("flags")]
        [JsonConverter(typeof(StringToListSeperatedByCommaConverter))]
        public List<string> Flags { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("blocked")]
        [JsonConverter(typeof(BoolConverter))]

        public bool Blocked { get; set; }

        [JsonProperty("mesh_UIDs")]
        public string MeshUIDs { get; set; }

        [JsonProperty("friendly_name")]
        public string FriendlyName { get; set; }

        [JsonProperty("parental_control_abuse")]
        [JsonConverter(typeof(BoolConverter))]

        public bool ParentalControlAbuse { get; set; }

        [JsonProperty("deleteable")]
        public int Deleteable { get; set; }

        [JsonProperty("active")]
        [JsonConverter(typeof(BoolConverter))]

        public bool Active { get; set; }

        [JsonProperty("user_UIDs")]
        public string UserUIDs { get; set; }

        [JsonProperty("modification_flags")]
        [JsonConverter(typeof(StringToListSeperatedByCommaConverter))]
        public List<string> ModificationFlags { get; set; }

        [JsonProperty("static_dhcp")]
        [JsonConverter(typeof(BoolConverter))]

        public bool StaticDhcp { get; set; }

        [JsonProperty("ip")]
        [JsonConverter(typeof(IPAddressConverter))]
        public IPAddress Ip { get; set; }

        [JsonProperty("firstused")]
        public long FirstUsed { get; set; }

        [JsonProperty("wakeup")]
        [JsonConverter(typeof(BoolConverter))]

        public bool Wakeup { get; set; }

        [JsonProperty("vendorname")]
        public string VendorName { get; set; }

        [JsonProperty("wlan_UIDs")]
        [JsonConverter(typeof(StringToListSeperatedByCommaConverter))]
        public List<string> WlanUIDs { get; set; }

        [JsonProperty("iplist")]
        public List<IpList> IpList { get; set; }

        [JsonProperty("ipv6firewallrules_UIDs")]
        public string Ipv6FirewallRulesUIDs { get; set; }

        [JsonProperty("rrd")]
        [JsonConverter(typeof(BoolConverter))]

        public bool Rrd { get; set; }

        [JsonProperty("lastused")]
        public long LastUsed { get; set; }

        [JsonProperty("mac")]
        public string Mac { get; set; }

        [JsonProperty("link_list")]
        public List<LinkList>? LinkList { get; set; }

        [JsonProperty("manu_name")]
        public string ManuName { get; set; }

        [JsonProperty("parentuid")]
        public string ParentUid { get; set; }

        [JsonProperty("ethernetport")]
        public string EthernetPort { get; set; }

        [JsonProperty("parental_control_prolong_possible")]
        [JsonConverter(typeof(BoolConverter))]
        public bool ParentalControlProlongPossible { get; set; }

        [JsonProperty("wlan_station_type")]
        public string WlanStationType { get; set; }

        [JsonProperty("updateavailable")]
        public int UpdateAvailable { get; set; }

        [JsonProperty("allow_pcp_and_upnp")]
        [JsonConverter(typeof(BoolConverter))]

        public bool AllowPcpAndUpnp { get; set; }

        [JsonProperty("online")]
        [JsonConverter(typeof(BoolConverter))]

        public bool Online { get; set; }

        [JsonProperty("neighbour_name")]
        public string NeighbourName { get; set; }

        [JsonProperty("igd_fw_cnt_upnp")]
        public int IgdFwCntUpnp { get; set; }

        [JsonProperty("dhcp")]
        [JsonConverter(typeof(BoolConverter))]

        public bool Dhcp { get; set; }

        [JsonProperty("forwardrules_UIDs")]
        public string ForwardRulesUIDs { get; set; }

        [JsonProperty("nexuspeer_UID")]
        public string NexusPeerUID { get; set; }

        [JsonProperty("ipv6_ifid")]
        public string Ipv6IfId { get; set; }
    }

    public class IpListEntry
    {
        [JsonProperty("addrtype")]
        public string AddrType { get; set; }

        [JsonProperty("dhcp")]
        [JsonConverter(typeof(BoolConverter))]

        public bool Dhcp { get; set; }

        [JsonProperty("lastused")]
        public long LastUsed { get; set; }

        [JsonProperty("ip")]
        public string Ip { get; set; }

        [JsonProperty(nameof(UID))]
        public string UID { get; set; }
    }

    public class IpList
    {
        [JsonProperty("entry")]
        public List<IpListEntry> Entry { get; set; }
    }
    public class LinkList
    {
        [JsonProperty("entry")]
        public List<LinkListEntry>? Entry { get; set; }
    }
    public class LinkListEntry
    {
        [JsonProperty("is_uplink")]
        public string IsUplink { get; set; }

        [JsonProperty("remote_interface_name")]
        public string RemoteInterfaceName { get; set; }

        [JsonProperty("media_type")]
        public string MediaType { get; set; }

        [JsonProperty("is_connected")]
        [JsonConverter(typeof(BoolConverter))]
        public bool IsConnected { get; set; }

        [JsonProperty("speed")]
        public int Speed { get; set; }

        [JsonProperty("local_interface_name")]
        public string LocalInterfaceName { get; set; }

        [JsonProperty("remote_dev_mesh_uid")]
        public string RemoteDevMeshUid { get; set; }

        [JsonProperty("UID")]
        public string Uid { get; set; }


    }


}
