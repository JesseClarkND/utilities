using Clark.Attack.Common.Interfaces;
using Clark.Attack.Common.Models;
using Clark.Common.Models;
using Clark.Common.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Clark.Attack.Redirect
{
    public class Processor : IAttack
    {
        public string Name { get { return "Open Redirect"; } set { } }

        #region Private

        private static List<string> _redirectAppends = new List<string>()
        {
            "//google.com",
            "//google.com/%2F",
            "/%5Cgoogle.com",
            "//google.com/..;/css"
        };

        #endregion

        public AttackResult Check(AttackRequest request)
        {
            var result = new AttackResult();

            foreach (var appendedRedirect in _redirectAppends)
            {
                string testURL = request.URL.Trim('/') + appendedRedirect;
                var webRequest = new WebPageRequest(testURL);
                webRequest.FollowRedirects = true;
                WebPageLoader.Load(webRequest);

                if (webRequest.Response.Body.Contains("<title>Google</title>"))
                {
                    result.Success = true;
                    result.Results.Enqueue("Open Redirect: " + testURL);
                }
            }

            return result;
        }
    }
}