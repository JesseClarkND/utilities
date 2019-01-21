using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Clark.Common.Models
{
    public class WebPageRequest
    {
        public WebPageRequest() { }
        public WebPageRequest(string address)
        {
            Address = address;
        }

        public WebPageRequest(WebPageRequest previousRequest)
        {
            Address = previousRequest.Address;
            CookieJar = previousRequest.CookieJar;
            Headers = previousRequest.Headers;
            Method = previousRequest.Method;
            RequestBody = previousRequest.RequestBody;
        }

        public string Address = "";
        public CookieCollection CookieJar = new CookieCollection();
        public WebHeaderCollection Headers = new WebHeaderCollection();
        public string Method = "GET";
        public string RequestBody = "";
        public string ContentType = "";
        public bool Log = false;
        public string LogDir = "";

        public WebPageResponse Response = new WebPageResponse();
    }
}