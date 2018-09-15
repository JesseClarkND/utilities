using System;
using System.Collections.Generic;
using System.Linq;
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

        public string Address = "";
        public WebPageResponse Response = new WebPageResponse();
    }
}
