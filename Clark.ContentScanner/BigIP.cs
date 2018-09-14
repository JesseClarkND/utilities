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
            WebRequest request = new WebRequest(domain.Trim('/')+"/my.policy");
            WebRequestUtility.GetWebText(request);
            if (request.Response.Code.Equals("200"))
            {
                WebRequest compareRequest = new WebRequest(domain.Trim('/') + "/asfasdfasdf");
                WebRequestUtility.GetWebText(compareRequest);
                if (request.Response.Body != compareRequest.Response.Body)
                    return true;
            }
            return false;
        }
    }
}