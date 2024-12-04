﻿using FritzBoxClient.Converter;
using FritzBoxClient.Interfaces;
using Newtonsoft.Json;
using System.Net;

namespace FritzBoxClient.Models.FritzOsVersion7
{
    public class Device : IDevice
    {
        [JsonProperty("own_client_device")]
        public bool OwnClientDevice { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("stateinfo")]
        public StateInfo StateInfo { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("desc")]
        public string Description { get; set; }
        [JsonProperty("ip")]
        [JsonConverter(typeof(IPAddressConverter))]
        public IPAddress? Ip { get; set; } = null;
        [JsonProperty("uid")]
        public string Uid { get; set; } = string.Empty;

    }
}