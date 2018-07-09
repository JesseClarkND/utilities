using Clark.Crawler.Utilities;
using Clark.Socialite.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clark.Socialite.Utility
{
    public static class SocialDomainUtility
    {
        public static bool CheckIfSocialMediaSite(string url, List<DomainData> socialDomains)
        {
            string foundDomain = DomainUtility.GetDomainFromUrl(url);
            string foundURL = url.Split('?')[0];

            foreach (DomainData social in socialDomains)
            {
                if (foundDomain.ToLower().Equals(social.DomainName.ToLower()))
                {
                    if (!CheckForSharing(url, social))
                        return false;

                    return true;
                }
            }

            return false;
        }

        private static bool CheckForSharing(string url, DomainData domainData)
        {
            //todo:
            if (url.Contains("iframe"))
                return false;

            foreach (string ignoreCase in domainData.IgnorePhrases)
            {
                if (url.Contains(ignoreCase))
                    return false;
            }

            return true;
        }
    }
}