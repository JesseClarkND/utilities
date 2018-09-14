using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clark.ContentScanner
{
    public static class SubdomainTakeover
    {
        private static List<Domain> _domainList = new List<Domain>();

        public static string Check(string body)
        {
            if (_domainList.Count == 0)
                Initialize();

            if (String.IsNullOrEmpty(body))
                return "";

            string domain = "";
            foreach (Domain domainTest in _domainList)
            {
                domain = Check_Domain(body, domainTest);
                if(String.IsNullOrEmpty(domain))
                    return domain;
            }

            return domain;
        }

        private static string Check_Domain(string body, Domain domain)
        {
            bool anyFingerPrintsConfirmed = false;
            foreach (string fingerPrint in domain.Fingerprints)
            {
                if (body.Contains(fingerPrint))
                {
                    anyFingerPrintsConfirmed = true;
                    break; ;
                }
            }

            if (anyFingerPrintsConfirmed)
                return domain.Engine;
            return "";
        }

        private static void Initialize() {
            _domainList.Add(new Domain()
            {
                Engine = "AWS/S3",
                Fingerprints = new List<string>(){"<Code>NoSuchBucket</Code>"},
                Reference = ""
            });

            _domainList.Add(new Domain()
            {
                Engine = "Bitbucket",
                Fingerprints = new List<string>() { "Repository not found" },
                Reference = ""
            });

            _domainList.Add(new Domain()
            {
                Engine = "Campaign Monitor",
                Fingerprints = new List<string>() { "<title>Campaign Monitor</title>" },//todo: not tested
                Reference = "https://help.campaignmonitor.com/custom-domain-names"
            });


            _domainList.Add(new Domain()
            {
                Engine = "Cargo Collective",
                Fingerprints = new List<string>() { "<div class=\"tesseract\"></div>", "<title>404 &mdash; File not found</title>" },
                Reference = "https://support.2.cargocollective.com/Using-a-Third-Party-Domain"
            });


            _domainList.Add(new Domain()
            {
                Engine = "Cloudfront",
                Fingerprints = new List<string>() { "Bad Request: ERROR: The request could not be satisfied" },
                Reference = "https://blog.zsec.uk/subdomainhijack/"
            });

            _domainList.Add(new Domain()
            {
                Engine = "Fastly",
                Fingerprints = new List<string>() { "Fastly error: unknown domain:" },
                Reference = ""
            });

            _domainList.Add(new Domain()
            {
                Engine = "Feedpress",
                Fingerprints = new List<string>() { "The feed has not been found." },
                Reference = "https://hackerone.com/reports/195350"
            });

            _domainList.Add(new Domain()
            {
                Engine = "Ghost",
                Fingerprints = new List<string>() { "The thing you were looking for is no longer here, or never was" },
                Reference = ""
            });

            _domainList.Add(new Domain()
            {
                Engine = "Github",
                Fingerprints = new List<string>() { "There isn't a Github Pages site here." },
                Reference = "https://hackerone.com/reports/263902"
            });

            _domainList.Add(new Domain()
            {
                Engine = "Help Juice",
                Fingerprints = new List<string>() { "We could not find what you're looking for." },
                Reference = "https://helpjuice.com"
            });

            _domainList.Add(new Domain()
            {
                Engine = "Help Scout",
                Fingerprints = new List<string>() { "No settings were found for this company:" },
                Reference = "https://docs.helpscout.net/article/42-setup-custom-domain"
            });

            _domainList.Add(new Domain()
            {
                Engine = "Heroku",
                Fingerprints = new List<string>() { "No such app" },
                Reference = ""
            });

            _domainList.Add(new Domain()
            {
                Engine = "JetBrains",
                Fingerprints = new List<string>() { "is not a registered InCloud YouTrack" },
                Reference = ""
            });

            //_domainList.Add(new Domain()
            //{
            //    Engine = "Microsoft Azure",
            //    Fingerprints = new List<string>() { "" },
            //    Reference = ""
            //});

            _domainList.Add(new Domain()
            {
                Engine = "Shopify",
                Fingerprints = new List<string>() { "Sorry, this shop is currently unavailable." },
                Reference = ""
            });

            _domainList.Add(new Domain()
            {
                Engine = "Statuspage.io",
                Fingerprints = new List<string>() { "This page is currently inactive and can only be viewed by team members. To see this page" },
                Reference = ""
            });

            _domainList.Add(new Domain()
            {
                Engine = "Surge.sh",
                Fingerprints = new List<string>() { "project not found" },
                Reference = "	https://surge.sh/help/adding-a-custom-domain"
            });

            _domainList.Add(new Domain()
            {
                Engine = "Tumblr",
                Fingerprints = new List<string>() { "Whatever you were looking for doesn't currently exist at this address" },
                Reference = ""
            });

            _domainList.Add(new Domain()
            {
                Engine = "Unbounce",
                Fingerprints = new List<string>() { "The requested URL was not found on this server." },
                Reference = ""
            });

            _domainList.Add(new Domain()
            {
                Engine = "UserVoice",
                Fingerprints = new List<string>() { "This UserVoice subdomain is currently available!" },
                Reference = ""
            });

            _domainList.Add(new Domain()
            {
                Engine = "Wordpress",
                Fingerprints = new List<string>() { "Do you want to register", "wordpress.com?" },
                Reference = ""
            });
            _domainList.Add(new Domain()
            {
                Engine = "Zendesk",
                Fingerprints = new List<string>() { "Help Center Closed" },
                Reference = ""
            });
            _domainList.Add(new Domain()
            {
                Engine = "SurveyGizmo",
                Fingerprints = new List<string>() { "Oh dear! We can’t seem to find that page, but all is not lost." },
                Reference = "https://help.surveygizmo.com/help/set-up-a-branded-subdomain"
            });
        }

    }

    internal class Domain{
        public string Engine="";
        public List<string> Fingerprints=new List<string>();
        public string Reference="";
    }
}