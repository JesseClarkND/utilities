using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clark.Crawler.Interfaces
{
    public interface IResponse
    {
        bool Error { get; set; }
        string ErrorMessage { get; set; }
        string Code { get; set; }//HTTP code 403, 404, 200, etc
        string Body { get; set; }
    }
}