using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Clark.Common.Models
{
    public class WebPageResponse
    {
        public WebHeaderCollection Headers = new WebHeaderCollection();
        public CookieCollection CookieJar = new CookieCollection();
        public string Body = "";
        public Dictionary<string, string> Scripts = new Dictionary<string, string>();
        public bool TimeOut = false;
        public string Message = "";
        public bool NotFound = false;
        public string Code = "";
    }
}