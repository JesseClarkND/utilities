using Clark.Common.Models;
using Clark.Common.Utility;
using Clark.Subdomain.Utility;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Clark.Subdomain
{
    public class Hunter
    {
        public static List<string> GatherAll(string domain, List<string> knownSubDomains = null)
        {
            List<string> subdomains = knownSubDomains;
            if (subdomains == null)
                subdomains = new List<string>();

            subdomains = Gather_FindSubdomains(domain);
            subdomains.AddRange(Gather_SecurityTrails(domain));
            subdomains.AddRange(Gather_NetCraft(domain));

            return subdomains.Distinct().ToList();
        }

        public static List<string> Gather_FindSubdomains(string domain)
        {
            List<string> subdomains = new List<string>();

            WebPageRequest request = new WebPageRequest();
            request.Address = "https://findsubdomains.com/subdomains-of/" + domain;

            WebPageLoader.Load(request);

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(request.Response.Body);

            var divNodes = htmlDoc.DocumentNode.SelectNodes("//div[contains(@class,'js-domain-name')]");
            if (divNodes != null)
            {
                foreach (HtmlNode div in divNodes)
                {
                    subdomains.Add(Regex.Replace(div.InnerText, @"\n|\s+", ""));
                }
            }
            return subdomains;
        }

        private static List<string> Gather_DNSDumpster(string domain)
        {
            throw new NotImplementedException();

            //https://dnsdumpster.com/static/xls/yahoo.com-201808290224.xlsx
            List<string> subdomains = new List<string>();

            
            return subdomains;
        }

        public static List<string> Gather_SecurityTrails(string domain)
        {
            //https://api.securitytrails.com/v1/ping?apikey=your_api_key
            //

            //https://docs.securitytrails.com/docs/how-to-use-the-dsl

            List<string> subdomains = new List<string>();

            WebPageRequest request = new WebPageRequest();
            //request.Method = "POST";
            //request.Address = "https://api.securitytrails.com/v1/search/list";
            //request.ContentType = "application/json";
            //request.Headers.Add("APIKEY", "IqrDwrRNXp9mzI7pQyL5WplM2l60soBs");
            //request.RequestBody = "{\"query\": \"apex_domain='" + domain + "'\"}";
            request.Address = "https://api.securitytrails.com/v1/domain/" + domain + "/subdomains";
            request.Headers.Add("APIKEY", "IqrDwrRNXp9mzI7pQyL5WplM2l60soBs");
            WebPageLoader.Load(request);

            dynamic d = JObject.Parse(request.Response.Body);

            if (d.subdomains != null) {
                foreach (string subdomain in d.subdomains)
                {
                    subdomains.Add(subdomain + "." + domain);
                }
            }
            return subdomains;
        }

        private static List<string> Gather_PentestTools(string domain)
        {
            throw new NotImplementedException();
            //https://pentest-tools.com/api_reference
            //https://pentest-tools.com/public/api_client.py.txt
            //{
            //    "op":               "start_scan",
            //    "tool_id":          20,
            //    "tool_params": {
            //        "target":       "bbc.com"
            //    }
            //}
            List<string> subdomains = new List<string>();

            return subdomains;
        }


        public static List<string> Gather_NetCraft(string domain)
        {
            //https://searchdns.netcraft.com/?restriction=site+contains&host=yahoo.com&lookup=wait..&position=limited
            //site ends with doesn't work
            //https://searchdns.netcraft.com/?host=yahoo.com&last=es.yahoo.com&from=21&restriction=site%20contains&position=limited

            WebPageRequest exampleRequest = new WebPageRequest();
            exampleRequest.Address = "https://searchdns.netcraft.com/?restriction=site+contains&host=*.example.com&lookup=wait..&position=limited";
            WebPageLoader.Load(exampleRequest);

            List<string> subdomains = new List<string>();

            string nextLink = "";
            Cookie requestCookie = new Cookie();
            Cookie responseCookie = new Cookie();
            do
            {
                WebPageRequest request = new WebPageRequest();
                if (String.IsNullOrEmpty(nextLink))
                {
                    request.Address = "https://searchdns.netcraft.com/?restriction=site+contains&host=*." + domain + "&lookup=wait..&position=limited";

                    if (exampleRequest.Response.Headers.AllKeys.Contains("Set-Cookie"))
                    {
                        var setCookie = exampleRequest.Response.Headers["Set-Cookie"];
                        List<string> cookieList = setCookie.Substring(0, setCookie.IndexOf(';')).Split('=').ToList();
                        string value = Crypto.SHA1HashStringForUTF8String(HttpUtility.UrlDecode(cookieList[1]));
                        
                        requestCookie = new Cookie(cookieList[0], cookieList[1], "/", "searchdns.netcraft.com");
                        responseCookie = new Cookie("netcraft_js_verification_response", value, "/", "searchdns.netcraft.com");
                    }
                    else
                    {
                        requestCookie = new Cookie("netcraft_js_verification_challenge", HttpUtility.UrlDecode("djF8UUhuTnh1YjZMZzB4ZlZTcjJLOU9JVVVOdnVpZFFLMDZ5TGN5NDluaFJYRy9LK2FVQVFLR0tT%0AZEJvdFE5RnpIcHBsTlljTy9ENjMwTQpzRzZDaFpGVHF3PT0KfDE1Mzc4NTEzNzY%3D%0A%7Cc005d4074568d002e7caa065ff3e501d8fd729fd"), "/", "searchdns.netcraft.com");
                        responseCookie = new Cookie("netcraft_js_verification_response", "cd86bb04f8659807f97e212b358dbe82e56f092d", "/", "searchdns.netcraft.com");
                    }

                    request.CookieJar.Add(requestCookie);
                    request.CookieJar.Add(responseCookie);
                }
                else
                {
                    request.Address = "https://searchdns.netcraft.com" + nextLink;
                    request.CookieJar.Add(requestCookie);
                    request.CookieJar.Add(responseCookie);
                }
                WebPageLoader.Load(request);

                MatchCollection urls = Regex.Matches(request.Response.Body, "<a href=\"http://toolbar.netcraft.com/site_report\\?url=(.*)\">");

                foreach (Match url in urls)
                {
                    Match found = Regex.Match(url.Value, "url=?.+" + domain);
                    if (!String.IsNullOrEmpty(found.Value))
                    {
                        string foundDomain = found.Value.Replace("url=http://", "").Replace("url=https://", "");
                        if (foundDomain.EndsWith(domain))
                            subdomains.Add(foundDomain);
                    }
                }

                Match nextPage = Regex.Match(request.Response.Body, "<a href=\"(.*?)\"><b>Next page</b></a>", RegexOptions.IgnoreCase);

                if (!String.IsNullOrEmpty(nextPage.Value))
                {
                    string val = nextPage.Value;
                    int firstIndex = val.IndexOf('"');
                    int lasIndex = val.LastIndexOf('"');
                    nextLink = val.Substring(firstIndex + 1, (lasIndex-1 - firstIndex));
                }
                else
                    nextLink = "";

            } while (!String.IsNullOrEmpty(nextLink));

            return subdomains.Distinct().ToList();


            //List<string> subdomains = new List<string>();

            //WebPageRequest request = new WebPageRequest();
            //request.Address = "https://findsubdomains.com/subdomains-of/" + domain;


            //var divNodes = htmlDoc.DocumentNode.SelectNodes("//div[contains(@class,'js-domain-name')]");
            //if (divNodes != null)
            //{
            //    foreach (HtmlNode div in divNodes)
            //    {
            //        subdomains.Add(Regex.Replace(div.InnerText, @"\n|\s+", ""));
            //    }
            //}
            //return subdomains;
        }


        private static List<string> Gather_VirusTotal(string domain)
        {
            throw new NotImplementedException();
            //https://developers.virustotal.com/v2.0/reference#domain-report

            List<string> subdomains = new List<string>();

            return subdomains;
        }
    }


}