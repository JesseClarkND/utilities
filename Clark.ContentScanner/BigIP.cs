using Clark.Common.Models;
using Clark.Common.Utility;
using Clark.ContentScanner.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clark.ContentScanner
{
    public class BigIP
    {
        public static bool Check(string domain)
        {
            WebPageRequest request = new WebPageRequest(domain.Trim('/') + "/my.policy");
            WebPageLoader.Load(request);
            if (request.Response.Code.Equals("200"))
            {
                WebPageRequest compareRequest = new WebPageRequest(domain.Trim('/') + "/asfasdfasdf");
                WebPageLoader.Load(compareRequest);
                if (request.Response.Body != compareRequest.Response.Body)
                    return true;
            }
            return false;
        }
    }
}