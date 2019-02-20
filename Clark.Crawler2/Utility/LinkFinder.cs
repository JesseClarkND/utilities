using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Clark.Crawler2.Utility
{
    /// <summary>
    /// Imported from Samadhi2, probably some bad stuff here FixOrToss(), todo: look over and improve
    /// </summary>
    public class LinkFinder
    {
        public static List<string> Parse(string body, string sourceUrl)
        {
            if (sourceUrl.Contains("#"))
            {
                sourceUrl = sourceUrl.Split('#')[0];
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
            //if (CrawlerContext.ExhaustPageInput)
            //    BuildFullQueryString(htmlDoc, srcList, sourceUrl);

            FixOrTossURLs(sourceUrl, srcList);

            return srcList;
        }

        #region Find Links
        private static void FindJavascriptLinks(string body, List<string> srcList)
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

        private static void FindAnchorLinks(HtmlDocument htmlDoc, List<string> srcList)
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

        private static void FindLinkedJavascriptFiles(HtmlDocument htmlDoc, List<string> srcList)
        {
            List<HtmlNode> jsLinks = htmlDoc.DocumentNode.Descendants("script").ToList();
            foreach (HtmlNode node in jsLinks)
            {
                if (node.Attributes["src"] != null && !srcList.Contains(node.Attributes["src"].Value))
                    srcList.Add(node.Attributes["src"].Value.Trim(' '));
            }
        }

        private static void FindFormLinks(HtmlDocument htmlDoc, List<string> srcList)
        {
            List<HtmlNode> formLinks = htmlDoc.DocumentNode.Descendants("form").ToList();
            foreach (HtmlNode node in formLinks)
            {
                if (node.Attributes["action"] != null && !srcList.Contains(node.Attributes["action"].Value))
                    srcList.Add(node.Attributes["action"].Value.Trim(' '));
            }
        }

        private static void FindFrameLinks(HtmlDocument htmlDoc, List<string> srcList)
        {
            List<HtmlNode> frameLinks = htmlDoc.DocumentNode.Descendants("frame").ToList();
            foreach (HtmlNode node in frameLinks)
            {
                if (node.Attributes["src"] != null && !srcList.Contains(node.Attributes["src"].Value))
                    srcList.Add(node.Attributes["src"].Value.Trim(' '));
            }
        }

        private static void BuildFullQueryString(HtmlDocument htmlDoc, List<string> srcList, string sourceUrl)
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

        private static void FixOrTossURLs(string sourceUrl, List<string> srcList)
        {
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

                string formattedLink = FixPath(sourceUrl, srcList[x]);
                srcList[x] = formattedLink;
            }
        }

        private static string FixPath(string originatingUrl, string link)
        {
            string settingUrl = originatingUrl;
            string settingUrlType = "";
            if (settingUrl.StartsWith("https://"))
            {
                settingUrlType = "https://";
            }
            else if (settingUrl.StartsWith("http://"))
            {
                settingUrlType = "http://";
            }

            string formattedLink = String.Empty;


            if (String.IsNullOrEmpty(formattedLink) && link.IndexOf("../") > -1)
            {
                formattedLink = ResolveRelativePaths(link, originatingUrl);
            }
            else if (String.IsNullOrEmpty(formattedLink) && link.StartsWith("#"))
            {
                formattedLink = originatingUrl += link;
            }
            else if (String.IsNullOrEmpty(formattedLink) && originatingUrl.IndexOf(settingUrl) > -1 && link.IndexOf(settingUrl) == -1 && link.IndexOf("http") != 0)
            {
                if (link.StartsWith("/"))
                    formattedLink = settingUrlType + new Uri(originatingUrl).Authority + '/' + link.Trim('/');
                else
                    formattedLink = originatingUrl.Substring(0, originatingUrl.LastIndexOf("/") + 1) + link.Trim('/');
            }
            else if (String.IsNullOrEmpty(formattedLink) && link.IndexOf(settingUrl) == -1 && link.IndexOf("http") != 0)
            {
                formattedLink = settingUrl.Trim('/') + '/' + link.Trim('/');///////////////////////////////////////
            }

            if (String.IsNullOrEmpty(formattedLink) && !link.StartsWith(settingUrl) && link.IndexOf("http") != 0)
            {
                formattedLink = settingUrl + link.TrimStart('/');///////////////////////////////////////
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

    }
}