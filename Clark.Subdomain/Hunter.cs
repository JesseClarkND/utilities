using Clark.Common.Models;
using Clark.Common.Utility;
using Clark.Subdomain.Utility;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Clark.Subdomain
{
    public class Hunter
    {
        public static List<string> Gather(string domain, List<string> knownSubDomains = null)
        {
            List<string> subdomains = knownSubDomains;
            if (subdomains == null)
                subdomains = new List<string>();

            subdomains = Gather_FindSubdomains(domain);

            return subdomains.Distinct().ToList();
        }

        private static List<string> Gather_FindSubdomains(string domain)
        {
            List<string> subdomains = new List<string>();

            WebPageRequest request = new WebPageRequest();
            request.Address = "https://findsubdomains.com/subdomains-of/" + domain;

            WebPageLoader.Load(request);

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(request.Response.Body);

            foreach (HtmlNode div in htmlDoc.DocumentNode.SelectNodes("//div[contains(@class,'js-domain-name')]"))
            {
                subdomains.Add(Regex.Replace(div.InnerText, @"\n|\s+", ""));
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

        private static List<string> Gather_SecurityTrails(string domain)
        {
            throw new NotImplementedException();

            //https://api.securitytrails.com/v1/ping?apikey=your_api_key
            //IqrDwrRNXp9mzI7pQyL5WplM2l60soBs

            //https://docs.securitytrails.com/docs/how-to-use-the-dsl

            List<string> subdomains = new List<string>();

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


        private static List<string> Gather_NetCraft(string domain)
        {
            throw new NotImplementedException();
            //http://searchdns.netcraft.com/?host=yahoo.com&last=www.yahoo.com.ar&from=61&restriction=site%20contains&position=unlimited

            List<string> subdomains = new List<string>();

            return subdomains;
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