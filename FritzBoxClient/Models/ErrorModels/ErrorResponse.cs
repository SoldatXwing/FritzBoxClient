using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FritzBoxClient.Models.ErrorModels
{
    public class ErrorResponse
    {
        [JsonProperty("error")]
        public ErrorContent Error { get; set; }
    }

}
