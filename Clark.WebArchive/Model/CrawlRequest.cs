using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clark.WebArchiveCrawler.Model
{
    public class CrawlRequest
    {
        public string Address = "";
        public string FileType = "";
        public int Page = 0;
        public int PageSize = 0;
        public int Limit = 0;
        public bool FindAll = false;
    }
}