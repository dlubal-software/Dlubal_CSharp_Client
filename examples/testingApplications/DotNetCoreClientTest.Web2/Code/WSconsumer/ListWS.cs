using Dlubal.WS.Clients.DotNetClientTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DotNetCoreClientTest.Web2.Code.WSconsumer
{
    public class ListWS
    {
        /// <summary>
        /// Show list of test method
        /// </summary>
        public void ListOfWsTest(Models.IMethodWS model)
        {
            Console.WriteLine("List web service tests...");

            MethodInfo[] methodInfos = typeof(TestingMethods).GetMethods(BindingFlags.Public | BindingFlags.Static);
            Console.WriteLine($"{methodInfos.Length} methods found.");

            int countOfMethods = DislayWSTest(methodInfos, $"Web Service Tests", model);
            
        }

        /// <summary>
        /// Show list of test method
        /// </summary>
        /// <param name="methodInfos"></param>
        /// <param name="rootName"></param>
        /// <returns></returns>
        public int DislayWSTest(MethodInfo[] methodInfos, string rootName, Models.IMethodWS model)
        {
            int countOfMethods = 0;
            int indexRow = 0;

            for (int methodIndex = 0; methodIndex < methodInfos.Length; methodIndex++)
            {
                MethodInfo methodInfo = methodInfos[methodIndex];

                string[] nameParts = methodInfo.Name.Replace('\u2040', ' ').Split('_');

                if (!(Common.IsValidTestByName(methodInfo.Name.Replace('\u2040', ' '))))
                {
                    continue;
                }

                indexRow++;

                Models.MethodWSModel wSmethodModel = new Models.MethodWSModel()
                {
                    ID = indexRow,
//                    TreePath = methodInfo.Name.Replace('\u2040', ' ').Replace('_', '-'),
//                    TreePosition = partIndex,
//                    Name = nameParts[partIndex]
                };

                wSmethodModel.TreePathList = new List<string>();
                for (int partIndex = 1; partIndex < nameParts.Length; partIndex++)
                {
                    wSmethodModel.TreePathList.Add(nameParts[partIndex]);
                    if (partIndex == 1)
                    {
                        wSmethodModel.TreePath = nameParts[partIndex];
                    }
                    else if (partIndex < nameParts.Length - 1)
                    {
                        wSmethodModel.TreePath = $"{wSmethodModel.TreePath}-{nameParts[partIndex]}";
                    }
                    else
                    {
                        wSmethodModel.Name = nameParts[partIndex];
                    }

                }

                model.WSlist.Add(wSmethodModel);
            }
            countOfMethods = indexRow;
            return countOfMethods;
        }
    }
}
