﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FritzBoxClient.Models
{
    public class Data
    {
        [JsonProperty("net")]
        public Net Net { get; set; }
    }
}
