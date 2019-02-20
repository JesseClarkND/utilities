using Clark.Attack.Common.Interfaces;
using Clark.Attack.Common.Models;
using Clark.Common.Models;
using Clark.Common.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clark.Attack.HTTPResponse
{
    /// <summary>
    /// Check for anomalous HTTP response codes
    /// </summary>
    public class Processor : IAttack
    {
        public string Name { get { return "HTTP Response Attack"; } set { } }

        public AttackResult Check(AttackRequest request)
        {
            var result = new AttackResult();

            WebPageRequest webRequest = new WebPageRequest(request.URL);
            webRequest.FollowRedirects = false;
            WebPageLoader.Load(webRequest);

            if (webRequest.Response.Code == "401" && request.URL.StartsWith("http:"))
            {
                result.Success = true;
                result.Results.Enqueue("401 over HTTP found at " + request.URL);
            }

            if ((webRequest.Response.Code == "302" || webRequest.Response.Code == "301") && webRequest.Response.Body.Length!=0)
            {
                result.Success = true;
                result.Results.Enqueue("302/301 response with body " + request.URL);//possibly add a post check here as well
            }

            return result;
        }
    }
}
