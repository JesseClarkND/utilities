using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clark.Socialite.Data
{

    public static class Common
    {
        public static List<DomainData> CommonDomainData = new List<DomainData>() 
        {
            new DomainData("facebook.com", new List<string>(){ "iframe", "share", "pages", "search"}),
            new DomainData("twitter.com", new List<string>(){ "iframe", "intent", "statuses", "status"}),
            new DomainData("pintrist.com", new List<string>(){ "iframe", "pin/create"}),
            new DomainData("youtube.com", new List<string>(){ "iframe", "watch"}),
            new DomainData("instagram.com", new List<string>(){ "iframe"}),
            new DomainData("linkedin.com", new List<string>(){ "iframe"}),
        };
    }
}