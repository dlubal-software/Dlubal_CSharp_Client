using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCoreClientTest.Web2.Models
{
    public class MethodWSModel
    {
        public int ID { get; set; }
        public int TreePosition { get; set; }
        public string TreePath { get; set; }
        public string Name { get; set; }
        public List<string> TreePathList { get; set; }

    }
}
