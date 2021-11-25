using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCoreClientTest.Web2.Code.WSconsumer
{
    public class Common
    {
        /// <summary>
        /// chceck test name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool IsValidTestByName(string name)
        {
            string[] nameParts = name.Split('_');

            if (nameParts.Length < 2)
            {
                return false;
            }

            if (nameParts[0].ToLower() != "test")
            {
                return false;
            }

            return true;
        }
    }
}
