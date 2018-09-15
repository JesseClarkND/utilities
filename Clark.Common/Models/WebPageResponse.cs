using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clark.Common.Models
{
    public class WebPageResponse
    {
        public string Body = "";
        public Dictionary<string, string> Scripts = new Dictionary<string, string>();
        public bool TimeOut = false;
        public string Message = "";
        public bool NotFound = false;
        public string Code = "";
    }
}