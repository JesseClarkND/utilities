using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clark.Socialite.Data
{
    public static class Constants
    {
        public static readonly Dictionary<CommonDomains, DomainData> CommonDomainData = new Dictionary<CommonDomains, DomainData>() 
        {
            {CommonDomains.Facebook, new DomainData("facebook.com", new List<string>(){ "share", "pages", "search"})},
            {CommonDomains.Twitter, new DomainData("twitter.com", new List<string>(){ "intent", "statuses", "status"})},
            {CommonDomains.Pintrist, new DomainData("pintrist.com", new List<string>(){ "pin/create"})},
            {CommonDomains.Youtube, new DomainData("youtube.com", new List<string>(){ "watch"})},
            {CommonDomains.Instagram, new DomainData("instagram.com")},
            {CommonDomains.LinkedIn, new DomainData("linkedin.com")},
        };
    }
}