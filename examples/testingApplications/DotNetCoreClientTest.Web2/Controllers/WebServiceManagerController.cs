using DotNetCoreClientTest.Web2.Code.WSconsumer;
using DotNetCoreClientTest.Web2.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace DotNetCoreClientTest.Web2.Controllers
{
    public class WebServiceManagerController : Controller
    {
        /// <summary>
        /// user - guid from session and interim results 
        /// </summary>
        static Dictionary<Guid,string> runningTestInfo = new Dictionary<Guid, string>(); // dictionary works on thread - TODO: check thread safe
        /// <summary>
        /// interim results 
        /// </summary>
        const string SESSION_RUNNING_TEST_INFO = "SESSION_RUNNING_TEST_INFO";
        /// <summary>
        /// full result test per user
        /// </summary>
        const string SESSION_RUNNING_TEST_FULL_RESULT = "SESSION_RUNNING_TEST_FULL_RESULT";
        /// <summary>
        /// GUID user
        /// </summary>
        const string SESSION_USER_GUID = "SESSION_USER_GUID";

        public IActionResult Index()
        {
            MainPageWSModel model = new MainPageWSModel();

            model.WSlist = new List<MethodWSModel>();
            ListWS listWS = new ListWS();
            listWS.ListOfWsTest(model);

            if (string.IsNullOrEmpty(HttpContext.Session.GetString(SESSION_USER_GUID)))
            {
                var id = Guid.NewGuid();
                HttpContext.Session.SetString(SESSION_USER_GUID, id.ToString());
                runningTestInfo.Add(id,"");
            }

            return View(model);
        }

        public IActionResult List()
        {
            ListWSModel model = new ListWSModel();
            model.WSlist = new List<MethodWSModel>();
            ListWS listWS = new ListWS();
            listWS.ListOfWsTest(model);

            return View(model);
        }

        public IActionResult TestWS()
        {
            TestWSModel model = new TestWSModel();

            try
            {
                model = (new Code.WSconsumer.ConnectionTest()).TestConnectionWS();
            }
            catch (Exception ex)
            {
                model.Value1 = "---";
                model.Value2 = "---";
                model.ErrorMsg = ex.Message;
            }

            return View(model);
        }
        public IActionResult RunOneTest(int id)
        {
            ResultTestModel model = new ResultTestModel();

            ListWSModel listWSModel = new ListWSModel();
            listWSModel.WSlist = new List<MethodWSModel>();
            new ListWS().ListOfWsTest(listWSModel);
            RunTest runTest = new RunTest();

            model.Id = id;
            runTest.Run(model, id);

            return View(model);
        }

        public IActionResult RunTestAjax(int Id)
        {
            ResultTestModel model = new ResultTestModel();

            ListWSModel listWSModel = new ListWSModel();
            listWSModel.WSlist = new List<MethodWSModel>();
            new ListWS().ListOfWsTest(listWSModel);

            HttpContext.Session.SetString(SESSION_RUNNING_TEST_INFO, "");
            HttpContext.Session.SetString(SESSION_RUNNING_TEST_FULL_RESULT, "");

            string userId = HttpContext.Session.GetString(SESSION_USER_GUID);
            if (!string.IsNullOrEmpty(userId))
            {
                runningTestInfo[Guid.Parse(userId)] = "";
            }

            RunTest runTest = new RunTest();

            model.Id = Id;
            runTest.Run(model, Id);

            if (!string.IsNullOrEmpty(model.Error))
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }

            string json = JsonConvert.SerializeObject(model);

            return Content(json, "application/json");
        }

        public async Task<IActionResult> RunMultiTestAjax(string id)
        {

            ListWSModel listWSModel = new ListWSModel();
            listWSModel.WSlist = new List<MethodWSModel>();
            new ListWS().ListOfWsTest(listWSModel);
            HttpContext.Session.SetString(SESSION_RUNNING_TEST_INFO, "");
            HttpContext.Session.SetString(SESSION_RUNNING_TEST_FULL_RESULT, "");

            string userId = HttpContext.Session.GetString(SESSION_USER_GUID);
            if (!string.IsNullOrEmpty(userId))
            {
                runningTestInfo[Guid.Parse(userId)] = "";
            }

            if (string.IsNullOrEmpty(id))
            {
                if (!string.IsNullOrEmpty(userId))
                {
                    runningTestInfo[Guid.Parse(userId)] = "Id empty.";
                }
                string json2 = JsonConvert.SerializeObject("Id empty.");
                return Content(json2, "application/json");
            }

            if (id == "ALL")    //all tests
            {
                id = "";        // replace 'all' -> id of all test
                foreach (var item in listWSModel.WSlist)
                {
                    id = $"{id},{item.ID}";
                }
            }

            List<ResultTestModel> models = await RunMultiTestAsync(id);
            
            string fullResult = "";
            bool isError = false;

            foreach (var itemModel in models)
            {
                fullResult = $"{fullResult}<h3>{itemModel.Name}</h3>";
                foreach (var item in itemModel.Messages)
                {
                    fullResult = $"{fullResult}<br/>{item}";
                }

                if (!string.IsNullOrEmpty(itemModel.Error))
                {
                    isError = true;
                    fullResult = $"{fullResult}<br/>{itemModel.Error}";
                }
                fullResult = $"{fullResult}<hr/>";
            }

            HttpContext.Session.SetString(SESSION_RUNNING_TEST_FULL_RESULT, fullResult);
            string json = JsonConvert.SerializeObject("OK");

            if (isError)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                json = JsonConvert.SerializeObject("error");
            }

            return Content(json, "application/json");
        }

        public async Task<List<ResultTestModel>> RunMultiTestAsync(string p_id)
        {
            List<ResultTestModel> models = new List<ResultTestModel>();

            foreach (var item in p_id.Split(','))
            {
                if (string.IsNullOrEmpty(item))
                {
                    continue;
                }

                ResultTestModel model = new ResultTestModel();
                int id = int.Parse(item);
                RunTest runTest = new RunTest();
                model.Id = id;
                runTest.Run(model, id);

                string runningTestInfoSession = HttpContext.Session.GetString(SESSION_RUNNING_TEST_INFO);
                runningTestInfoSession = $"{runningTestInfoSession} <br/> {model.Name} - {model.Result} - {DateTime.Now.ToString()}";
                HttpContext.Session.SetString(SESSION_RUNNING_TEST_INFO, runningTestInfoSession);

                string userId = HttpContext.Session.GetString(SESSION_USER_GUID);
                if (!string.IsNullOrEmpty(userId))
                {
                    runningTestInfo[Guid.Parse(userId)] = runningTestInfoSession;
                }

                models.Add(model);
            }

            return models;
        }

        public IActionResult GetInfo()
        {
            List<string> data = new List<string>();

            string userId = HttpContext.Session.GetString(SESSION_USER_GUID);
            if (!string.IsNullOrEmpty(userId))
            {
                var runningTestInfoData = runningTestInfo[Guid.Parse(userId)];
                data.Add(runningTestInfoData + "<hr/>");  
            }

            data.Add("<br/>" +  DateTime.Now.ToString());
            data.Add("<br/>" + HttpContext.Session.GetString(SESSION_RUNNING_TEST_FULL_RESULT));

            string json = JsonConvert.SerializeObject(data);

            return Content(json, "application/json");
        }
        public IActionResult RunScenario()
        {
            ResultTestModel model = new ResultTestModel();
            //model.Desc = "Scenario 1";

            RunScenario runScenario = new RunScenario();
            //runScenario.RunScenario1(model);
            runScenario.RunScenario2(model);
            runScenario.RunScenario3(model);



            return View(model);
        }
    }
}
