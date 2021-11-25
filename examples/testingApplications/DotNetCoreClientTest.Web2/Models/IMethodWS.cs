using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCoreClientTest.Web2.Models
{
    public interface IMethodWS
    {
        public List<MethodWSModel> WSlist { get; set; }
    }
}
