using Clark.Attack.Common.Interfaces;
using Clark.Attack.Common.Models;
using Clark.Attack.SocialMedia.Models;
using Clark.Common.Models;
using Clark.Common.Utility;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clark.Attack.SocialMedia
{
    public class Processor : IAttack
    {
        public string Name { get { return "Dormant Social Media"; } set { } }

        #region Private
        private static List<DomainData> _socialDomains = new List<DomainData>()
            {
                new DomainData("facebook.com", new List<string>(){ "iframe", "share", "pages", "search", "photo.php"}),
                new DomainData("twitter.com", new List<string>(){ "iframe", "intent", "statuses", "status", "/search"}),
                new DomainData("pintrist.com", new List<string>(){ "iframe", "pin/create"}),
                new DomainData("youtube.com", new List<string>(){ "iframe", "watch", "embed", "channel"}),
                new DomainData("instagram.com", new List<string>(){ "iframe", "instagram.com/p/"}),
              //  new DomainData("linkedin.com", new List<string>(){ "iframe"}),
                new DomainData("tumblr.com", new List<string>(){ "iframe", "tumblr.com/share", "tumblr.com/widget"}),
                new DomainData("flickr.com", new List<string>(){ "iframe"}),
            };
        private static List<string> _masterIgnoreList = new List<string>()
        {
            "vhx.tv"
        };
        #endregion

        public AttackResult Check(AttackRequest request)
        {
            var result = new AttackResult();

            if (_masterIgnoreList.Contains(request.Domain))
                return result;

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(request.Body);

            List<string> linksFound = new List<string>();

            List<HtmlNode> anchorLinks = htmlDoc.DocumentNode.Descendants("a").ToList();
            foreach (HtmlNode node in anchorLinks)
            {
                if (node.Attributes["href"] != null)
                {
                    string value = node.Attributes["href"].Value;
                    if (!value.StartsWith("mailto"))
                    {
                        if (CheckURL(request.Domain, value) && !linksFound.Contains(value))
                        {
                            result.Success = true;
                            linksFound.Add(value + " - Dormant social media account");
                        }
                    }
                }
            }

            linksFound.ForEach(x => result.Results.Enqueue(x));
            //result.Results.AddRange(linksFound);

            return result;
        }

        private static bool CheckURL(string startingDomain, string url)
        {
            string foundURL = DomainUtility.StripProtocol(url.Split('?')[0]);

            if (CheckIfSocialMediaSite(startingDomain, foundURL))
            {

                //if (userNames.Count == 0)
                //{
                //    if (!foundUrls.ContainsKey(foundURL))
                //    {

                WebPageRequest request = new WebPageRequest(DomainUtility.EnsureHTTPS(foundURL).ToLower());
                WebPageLoader.Load(request);
                if (!request.Response.Code.Equals("200"))
                    return true;
                //       else if (!returnOnlyNone200)
                //            foundUrls.Add(foundURL, url);
                //    }
                //}
                //else
                //{
                //    foreach (string userName in userNames)
                //    {
                //        if (foundURL.ToLower().Contains(userName.ToLower()))
                //        {
                //            if (!foundUrls.ContainsKey(foundURL))
                //            {
                //                Request request = new Request(DomainUtility.EnsureHTTPS(foundURL));
                //                RequestUtility.GetWebText(request);
                //                if (!request.Response.Code.Equals("200"))
                //                    foundUrls.Add(foundURL, url);
                //                else if (!returnOnlyNone200)
                //                    foundUrls.Add(foundURL, url);
                //            }
                //        }
                //    }
                //}
            }
            return false;
        }

        private static bool CheckIfSocialMediaSite(string startingDomain, string url)
        {
            string foundDomain = DomainUtility.GetDomainFromUrl(url);
            string foundURL = url.Split('?')[0];

            foreach (DomainData social in _socialDomains)
            {
                if (foundDomain.ToLower().Equals(startingDomain))
                    continue;

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