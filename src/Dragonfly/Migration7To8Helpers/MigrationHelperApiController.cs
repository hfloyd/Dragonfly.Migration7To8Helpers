namespace Dragonfly.Migration7To8Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Web;
    using System.Web.Mvc;
    using Dragonfly.NetHelpers;
    using Dragonfly.NetModels;
    using Dragonfly.UmbracoHelpers;
    using Newtonsoft.Json;
    using Umbraco.Web.WebApi;

    // [IsBackOffice]
    // GET: /Umbraco/backoffice/Api/MigrationHelperApi <-- UmbracoAuthorizedApiController

    [IsBackOffice]
    class MigrationHelperApiController : UmbracoAuthorizedApiController
    {
        private Config _Config = Config.GetConfig();

        /// /Umbraco/backoffice/Api/MigrationHelperApi/Start
        [System.Web.Http.AcceptVerbs("GET")]
        public HttpResponseMessage Start()
        {
            var returnSB = new StringBuilder();
            var returnStatusMsg = new StatusMessage(true); //assume success
            var pvPath = $"{_Config.GetViewsPath()}Start.cshtml";

            //Setup
         

            //GET DATA TO DISPLAY

            //UPDATE STATUS MSG
            //returnStatusMsg.Success = true;


            //VIEW DATA 
            var viewData = new ViewDataDictionary();
            viewData.Model = returnStatusMsg;
            viewData.Add("SpecialMessage", "");
            viewData.Add("SpecialMessageClass", "bg-info");

            //RENDER
            var controllerContext = this.ControllerContext;
            var displayHtml =
                ApiControllerHtmlHelper.GetPartialViewHtml(controllerContext, pvPath, viewData, HttpContext.Current);
            returnSB.AppendLine(displayHtml);

            //RETURN AS HTML
            return new HttpResponseMessage()
            {
                Content = new StringContent(
                    returnSB.ToString(),
                    Encoding.UTF8,
                    "text/html"
                )
            };
        }

        /// /Umbraco/backoffice/Api/MigrationHelperApi/TestRaw
        [System.Web.Http.AcceptVerbs("GET")]
        public HttpResponseMessage TestRaw()
        {
            var returnSB = new StringBuilder();
            var returnStatus = new StatusMessage();

            //GET DATA TO DISPLAY

            //UPDATE STATUS MSG


            string json = JsonConvert.SerializeObject(returnStatus);

            returnSB.AppendLine(json);

            return new HttpResponseMessage()
            {
                Content = new StringContent(
                    returnSB.ToString(),
                    Encoding.UTF8,
                    "application/json"
                )
            };
        }



    }
}
