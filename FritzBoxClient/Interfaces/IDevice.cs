using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FritzBoxClient.Interfaces
{
    public interface IDevice
    {
        string Name { get; set; }
        string Uid { get; set; }
        IPAddress Ip { get; set; }
        string Url { get; set; }
    }
}
