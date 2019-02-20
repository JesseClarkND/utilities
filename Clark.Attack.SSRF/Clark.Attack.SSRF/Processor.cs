using Clark.Attack.Common.Interfaces;
using Clark.Attack.Common.Models;
using Clark.Common.Models;
using Clark.Common.Utility;
using Clark.Crawler2.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clark.Attack.SSRF
{
    public class Processor : IAttack
    {
        public string Name { get { return "Possible SSRF"; } set { } }

        public List<string> _fingerprints = new List<string>()
        {
            "url=",
            "source=",
            "/",
            "%2f",
            "http"
        };

        public AttackResult Check(AttackRequest request)
        {
            var result = new AttackResult();

            WebPageRequest webRequest = new WebPageRequest(request.URL);
            WebPageLoader.Load(webRequest);

            List<string> links = new List<string>();
            
            if(!String.IsNullOrEmpty(webRequest.Response.Body))
                links = LinkFinder.Parse(webRequest.Response.Body, request.URL);
            //Do javascript files too, expand library to check load/ajax/etc

            foreach (var link in links)
            {
                var testLink = link.Remove(0, 5);
                foreach (var fp in _fingerprints)
                {
                    if (testLink.Contains(fp))
                    {
                        result.Success = true;
                        result.Results.Enqueue("Possible SSRF: " + link);
                    }
                }
            }

            return result;
        }
    }
}