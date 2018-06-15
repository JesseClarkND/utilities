using Clark.Crawler.Models;
using Clark.Crawler.Utilities;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Clark.Crawler
{
    public class LinkParser
    {
        private const string _LINK_REGEX = "href=\"[a-zA-Z./:&\\d_-]+\"";
        private List<Request> _goodUrls = new List<Request>();
        private List<string> _badUrls = new List<string>();
        private List<string> _otherUrls = new List<string>();
        private List<string> _externalUrls = new List<string>();
        private List<string> _exceptions = new List<string>();
        //private bool _RESTParse = true;

        public List<Request> GoodUrls
        {
            get { return _goodUrls; }
            set { _goodUrls = value; }
        }

        public void ParseLinksAgility(string body, string sourceUrl, bool allowExternal = false)
        {
            if (sourceUrl.Contains("#"))
            {
                sourceUrl = sourceUrl.Split('#')[0];
            }

            if (sourceUrl.Contains("?"))
            {
                sourceUrl = sourceUrl.Split('?')[0];
            }

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(body);
            List<string> srcList = new List<string>();

            //Find javascript redirect
            //document.location
            //location.href
            //window.location
            body = Regex.Replace(body, @"\s+", "");

            FindJavascriptLinks(body, srcList);
            FindAnchorLinks(htmlDoc, srcList);
            FindLinkedJavascriptFiles(htmlDoc, srcList);
            FindFormLinks(htmlDoc, srcList);
            FindFrameLinks(htmlDoc, srcList);
            if (CrawlerContext.ExhaustPageInput)
                BuildFullQueryString(htmlDoc, srcList, sourceUrl);

            List<string> fixedURLS = FixOrTossURLs(sourceUrl, srcList, allowExternal);

            foreach (string url in fixedURLS)
            {
                Request request = new Request(url);

                if (!_goodUrls.Contains(request))// && !ScanContext.Ignore.Directory.Contains(parentDirectory))
                {
                    // List<UrlEntity> attackUrls = Morph.MorpherizeQuery(urlEntity.Url);

                    _goodUrls.Add(request);
                    //GoodUrls.AddRange(attackUrls);
                }
            }
        }

        private static bool IgnoreDirectory(string url, string domain)
        {
            string directory = url.Split(new string[] { domain }, StringSplitOptions.None).Last().Trim('/');

            if (String.IsNullOrEmpty(directory))
                return false;

            foreach (string ignoreDir in CrawlerContext.IgnoreDirectory)
            {
                if (directory.StartsWith(ignoreDir))
                    return true;
            }

            return false;
        }

        #region Find Links
        private void FindJavascriptLinks(string body, List<string> srcList)
        {
            MatchCollection matches = Regex.Matches(body, "document\\.location=[\"\'](.*?)[\"\']");
            foreach (var match in matches)
            {
                string target = match.ToString().Split(new char[] { '=' }, 2)[1];
                if (target.StartsWith("'"))
                    target = target.Trim('\'');
                else
                    target = target.Trim('"');
                srcList.Add(target.Trim(' '));
            }

            matches = Regex.Matches(body, "location\\.href=[\"\'](.*?)[\"\']");
            foreach (var match in matches)
            {
                string target = match.ToString().Split(new char[] { '=' }, 2)[1];
                if (target.StartsWith("'"))
                    target = target.Trim('\'');
                else
                    target = target.Trim('"');
                srcList.Add(target.Trim(' '));
            }

            matches = Regex.Matches(body, "window\\.location=[\"\'](.*?)[\"\']");
            foreach (var match in matches)
            {
                string target = match.ToString().Split(new char[] { '=' }, 2)[1];
                if (target.StartsWith("'"))
                    target = target.Trim('\'');
                else
                    target = target.Trim('"');
                srcList.Add(target.Trim(' '));
            }
        }

        private void FindAnchorLinks(HtmlDocument htmlDoc, List<string> srcList)
        {
            List<HtmlNode> anchorLinks = htmlDoc.DocumentNode.Descendants("a").ToList();
            foreach (HtmlNode node in anchorLinks)
            {
                if (node.Attributes["href"] != null && !srcList.Contains(node.Attributes["href"].Value))
                {
                    string value = node.Attributes["href"].Value;
                    if (!value.StartsWith("mailto"))
                        srcList.Add(value.Trim(' '));
                }
            }
        }

        private void FindLinkedJavascriptFiles(HtmlDocument htmlDoc, List<string> srcList)
        {
            List<HtmlNode> jsLinks = htmlDoc.DocumentNode.Descendants("script").ToList();
            foreach (HtmlNode node in jsLinks)
            {
                if (node.Attributes["src"] != null && !srcList.Contains(node.Attributes["src"].Value))
                    srcList.Add(node.Attributes["src"].Value.Trim(' '));
            }
        }

        private void FindFormLinks(HtmlDocument htmlDoc, List<string> srcList)
        {
            List<HtmlNode> formLinks = htmlDoc.DocumentNode.Descendants("form").ToList();
            foreach (HtmlNode node in formLinks)
            {
                if (node.Attributes["action"] != null && !srcList.Contains(node.Attributes["action"].Value))
                    srcList.Add(node.Attributes["action"].Value.Trim(' '));
            }
        }

        private void FindFrameLinks(HtmlDocument htmlDoc, List<string> srcList)
        {
            List<HtmlNode> frameLinks = htmlDoc.DocumentNode.Descendants("frame").ToList();
            foreach (HtmlNode node in frameLinks)
            {
                if (node.Attributes["src"] != null && !srcList.Contains(node.Attributes["src"].Value))
                    srcList.Add(node.Attributes["src"].Value.Trim(' '));
            }
        }

        private void BuildFullQueryString(HtmlDocument htmlDoc, List<string> srcList, string sourceUrl)
        {
            if (htmlDoc.DocumentNode.SelectNodes("//input") != null)
            {
                string maximumUrl = sourceUrl.Trim('/');
                maximumUrl = maximumUrl.Split('#').First();
                maximumUrl = maximumUrl.Split('?').First();
                List<string> variablesToAdd = new List<string>();
                foreach (HtmlNode node in htmlDoc.DocumentNode.SelectNodes("//input"))
                {
                    if (node.Attributes["name"] == null)
                        continue;

                    string vartemp = node.Attributes["name"].Value;
                    if (!String.IsNullOrEmpty(vartemp))
                    {
                        variablesToAdd.Add(vartemp);
                    }
                }

                bool first = true;
                foreach (string variable in variablesToAdd)
                {
                    if (!maximumUrl.Contains(variable + '='))
                    {
                        if (first)
                        {
                            maximumUrl += "?";
                            first = false;
                        }
                        else
                            maximumUrl += "&";

                        maximumUrl += variable + "=filler";
                    }
                }
                srcList.Add(maximumUrl);
                // if (!first)
                // {
                // maximumUrl = maximumUrl.Substring(SettingsManager.Url.Trim('/').Length, (maximumUrl.Trim('/').Length - SettingsManager.Url.Trim('/').Length));
                // if (!maximumUrl.Trim('/').StartsWith("?"))
                //     srcList.Insert(0, maximumUrl);
                //  else
                //      srcList.Insert(0, sourceUrl.Split('?')[0].Trim('/') + '/' + maximumUrl.Trim('/'));
                //  }
            }
        }
        #endregion

        private List<string> FixOrTossURLs(string sourceUrl, List<string> srcList, bool allowExternal)
        {
            List<string> returnList = new List<string>();
            for (int x = 0; x < srcList.Count; x++)
            {
                if (String.IsNullOrEmpty(srcList[x]))
                {
                    srcList.RemoveAt(x);
                    x--;
                    continue;
                }

                if (srcList[x].StartsWith("javascript") ||
                    srcList[x].StartsWith("data:text") ||
                    srcList[x].StartsWith("#") ||
                    srcList[x].StartsWith("/#") ||
                    srcList[x].Equals("/") ||
                    srcList[x].Length < 1)
                {
                    srcList.RemoveAt(x);
                    x--;
                    continue;
                }

                if (srcList[x].StartsWith("//"))
                {
                    srcList[x] = "http:" + srcList[x];
                }

                if (CrawlerContext.ExhaustedURL.Contains(srcList[x]))
                {
                    srcList.RemoveAt(x);
                    x--;
                    continue;
                }

                if (IsExternalUrl(srcList[x]) && !allowExternal)
                {
                    srcList.RemoveAt(x);
                    x--;
                    continue;
                }

                string formattedLink = FixPath(sourceUrl, srcList[x]);
                returnList.Add(formattedLink);
            }

            return returnList;
        }

        private static string FixPath(string originatingUrl, string link)
        {
            string settingUrl = CrawlerContext.URI.ToString();
            string settingUrlType = "";
            if (settingUrl.StartsWith("https://"))
            {
                settingUrlType = "https://";
            }
            else if (settingUrl.StartsWith("http://"))
            {
                settingUrlType = "http://";
            }

            if (link.StartsWith("http:///"))
                link.Replace("http:///", "http://");

            if (link.StartsWith("https:///"))
                link.Replace("https:///", "https://");

            string formattedLink = String.Empty;


            if (String.IsNullOrEmpty(formattedLink) && link.IndexOf("../") > -1)
            {
                formattedLink = ResolveRelativePaths(link, originatingUrl);
            }
            else if (String.IsNullOrEmpty(formattedLink) && link.StartsWith("#"))
            {
                formattedLink = originatingUrl += link;
            }
            else if (String.IsNullOrEmpty(formattedLink) && originatingUrl.IndexOf(originatingUrl) > -1 && link.IndexOf(originatingUrl) == -1 && link.IndexOf("http") != 0)
            {
                if (link.StartsWith("/"))
                    formattedLink = settingUrlType + CrawlerContext.URI.Authority + '/' + link.Trim('/');
                else
                    formattedLink = originatingUrl.Substring(0, originatingUrl.LastIndexOf("/") + 1) + link.Trim('/');
            }
            else if (String.IsNullOrEmpty(formattedLink) && link.IndexOf(originatingUrl) == -1 && link.IndexOf("http") != 0)
            {
                formattedLink = originatingUrl.Trim('/') + '/' + link.Trim('/');///////////////////////////////////////
            }

            if (String.IsNullOrEmpty(formattedLink) && !link.StartsWith(originatingUrl) && link.IndexOf("http") != 0)
            {
                formattedLink = originatingUrl + link.TrimStart('/');///////////////////////////////////////
            }

            if (formattedLink.Equals(String.Empty))
                formattedLink = link;// linkType + link;

            return formattedLink.Trim('/');
        }

        private static string ResolveRelativePaths(string relativeUrl, string originatingUrl)
        {
            string resolvedUrl = String.Empty;

            bool tampered = false;
            Uri originatingUri = new Uri(originatingUrl);
            string originalHost = originatingUri.Scheme + "://" + originatingUri.Host;
            if (originatingUrl.StartsWith("http"))
            {
                tampered = true;
                originatingUrl = originatingUrl.Substring(originalHost.Length, originatingUrl.Length - originalHost.Length);
            }

            string[] relativeUrlArray = relativeUrl.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            string[] originatingUrlElements = originatingUrl.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            int indexOfFirstNonRelativePathElement = 0;
            for (int i = 0; i <= relativeUrlArray.Length - 1; i++)
            {
                if (relativeUrlArray[i] != "..")
                {
                    indexOfFirstNonRelativePathElement = i;
                    break;
                }
            }

            int countOfOriginatingUrlElementsToUse = originatingUrlElements.Length - indexOfFirstNonRelativePathElement - 1;
            for (int i = 0; i <= countOfOriginatingUrlElementsToUse - 1; i++)
            {
                if (originatingUrlElements[i] == "http:" || originatingUrlElements[i] == "https:")
                    resolvedUrl += originatingUrlElements[i] + "//";
                else
                    resolvedUrl += originatingUrlElements[i] + "/";
            }

            for (int i = 0; i <= relativeUrlArray.Length - 1; i++)
            {
                if (i >= indexOfFirstNonRelativePathElement)
                {
                    resolvedUrl += relativeUrlArray[i];

                    if (i < relativeUrlArray.Length - 1)
                        resolvedUrl += "/";
                }
            }

            if (tampered)
                resolvedUrl = originalHost.Trim('/') + '/' + resolvedUrl.Trim('/');

            return resolvedUrl;
        }

        private static bool IsExternalUrl(string url)
        {
            try
            {
                if ((url.Length >= 7 && url.Substring(0, 7) == "http://") || (url.Length >= 8 && url.Substring(0, 8) == "https://") || url.Length >= 2 && url.Substring(0, 2) == "//")
                {
                    Uri tempUri = new Uri(url);
                    string tempDomain = DomainUtility.GetDomainFromUrl(tempUri);
                    try
                    {
                        if (CrawlerContext.IgnoreDirectory.Count != 0 && IgnoreDirectory(url, tempDomain))
                            return true;


                        if (CrawlerContext.IncludeSubdomains)
                        {
                            if (tempDomain.Equals(DomainUtility.GetDomainFromUrl(CrawlerContext.URI)))
                                return false;
                            else if (Uri.Compare(tempUri, CrawlerContext.URI, UriComponents.Host, UriFormat.SafeUnescaped, StringComparison.CurrentCulture) != 0)
                                return true;
                            else
                                return false;
                        }
                        else
                        {
                            if (Uri.Compare(tempUri, CrawlerContext.URI, UriComponents.Host, UriFormat.SafeUnescaped, StringComparison.CurrentCulture) != 0)
                                return true;
                            else
                                return false;
                        }
                    }
                    catch
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                //todo
                return true;
            }
        }
    }
}
