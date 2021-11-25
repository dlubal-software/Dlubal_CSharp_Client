using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCoreClientTest.Web2.Models
{
    public class ResultTestModel
    {
        public ResultTestModel()
        {
            Messages = new List<string>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }
        public List<string> Messages { get; set; }
        public string Result { get; set; }

        public string Error { get; set; }
    }
}
