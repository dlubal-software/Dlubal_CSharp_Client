using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCoreClientTest.Web2.Models
{
    public class MainPageWSModel: IMethodWS
    {
        public List<MethodWSModel> WSlist { get; set; }

        public string resultWS { get; set; }
        public int activeNode { get; set; }
    }
}
