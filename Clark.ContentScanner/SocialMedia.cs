﻿using Clark.ContentScanner.Utility;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clark.ContentScanner
{
    public static class SocialMedia
    {
        private static List<DomainData> _socialDomains = new List<DomainData>();

        public static List<string> Check(string body)
        {
            if (_socialDomains.Count == 0)
                Initilize();

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(body);

            List<string> linksFound = new List<string>();

            List<HtmlNode> anchorLinks = htmlDoc.DocumentNode.Descendants("a").ToList();
            foreach (HtmlNode node in anchorLinks)
            {
                if (node.Attributes["href"] != null)
                {
                    string value = node.Attributes["href"].Value;
                    if (!value.StartsWith("mailto"))
                    {
                        if (CheckURL(value) && !linksFound.Contains(value))
                            linksFound.Add(value);
                    }
                }
            }

            return linksFound;
        }

        private static bool CheckURL(string url) {
            string foundURL = DomainUtility.StripProtocol(url.Split('?')[0]);

            if (CheckIfSocialMediaSite(foundURL))
            {

                //if (userNames.Count == 0)
                //{
                //    if (!foundUrls.ContainsKey(foundURL))
                //    {

                WebRequest request = new WebRequest(DomainUtility.EnsureHTTPS(foundURL));
                WebRequestUtility.GetWebText(request);
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

        private static bool CheckIfSocialMediaSite(string url)
        {
            string foundDomain = DomainUtility.GetDomainFromUrl(url);
            string foundURL = url.Split('?')[0];

            foreach (DomainData social in _socialDomains)
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

        private static void Initilize()
        {
            _socialDomains = new List<DomainData>() 
            {
                new DomainData("facebook.com", new List<string>(){ "iframe", "share", "pages", "search"}),
                new DomainData("twitter.com", new List<string>(){ "iframe", "intent", "statuses", "status", "/search"}),
             //   new DomainData("pintrist.com", new List<string>(){ "iframe", "pin/create"}),
                new DomainData("youtube.com", new List<string>(){ "iframe", "watch", "embed"}),
                new DomainData("instagram.com", new List<string>(){ "iframe", "instagram.com/p/"}),
              //  new DomainData("linkedin.com", new List<string>(){ "iframe"}),
            };
        }
    }

    internal class DomainData
    {
        public string DomainName;
        public List<string> IgnorePhrases;
        public List<string> UserNames;

        public DomainData(string domainName)
        {
            DomainName = domainName;
            IgnorePhrases = new List<string>();
        }

        public DomainData(string domainName, List<string> ignorePhrases)
        {
            DomainName = domainName;
            IgnorePhrases = ignorePhrases;
        }
    }
}