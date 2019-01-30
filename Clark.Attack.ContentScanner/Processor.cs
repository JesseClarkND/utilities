using Clark.Attack.Common.Interfaces;
using Clark.Attack.Common.Models;
using Clark.Attack.ContentScanner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clark.Attack.ContentScanner
{
    /// <summary>
    /// Compare a body of html against known fingerprints, find default server pages, service pages, etc
    /// </summary>
    public class Processor : IAttack
    {
        public string Name = "Content Scanner";
        #region Private
        private static List<Fingerprint> _serviceFingerPrints = new List<Fingerprint>()
        {
            new Fingerprint(){
                Name = "Jenkins",
                Fingerprints = new List<string>() { "<a href=\"https://jenkins.io/\">Jenkins", "<span class=\"jenkins_ver\">" },
                Reference = ""
            },
            new Fingerprint(){
                Name = "Nexus",
                Fingerprints = new List<string>() { "<title>Nexus Repository Manager</title>" },
                Reference = ""
            },
            new Fingerprint(){
                Name = "Artifactory",
                Fingerprints = new List<string>() { "Jfrog" },
                Reference = ""
            },
            new Fingerprint(){
                Name = "SonarQube",
                Fingerprints = new List<string>() { "<title>SonarQube</title>" },
                Reference = ""
            },
            new Fingerprint(){
                Name = "JIRA",
                Fingerprints = new List<string>() { "Jira</title>" },
                Reference = ""
            },
            new Fingerprint(){
                Name = "Github SSO",
                Fingerprints = new List<string>() { "New to GitHub?", "Create an account" },
                Reference = ""
            }
        };

        private static List<Fingerprint> _subDomainFingerPrints = new List<Fingerprint>()
        {
            new Fingerprint(){
                Name = "AWS/S3",
                Fingerprints = new List<string>(){"<Code>NoSuchBucket</Code>"},
                Reference = ""
            },
            new Fingerprint(){
                Name = "AWS/S3 2.0",
                Fingerprints = new List<string>() { "Code: IncorrectEndpoint", "400 Bad Request" },
                Reference = ""
            },
            new Fingerprint(){
                Name = "Bitbucket",
                Fingerprints = new List<string>() { "Repository not found" },
                Reference = ""
            },
            new Fingerprint(){
                Name = "Campaign Monitor",
                Fingerprints = new List<string>() { "<title>Campaign Monitor</title>" },//todo: not tested
                Reference = "https://help.campaignmonitor.com/custom-domain-names"
            },
            new Fingerprint(){
                Name = "Cargo Collective",
                Fingerprints = new List<string>() { "<div class=\"tesseract\"></div>", "<title>404 &mdash; File not found</title>" },
                Reference = "https://support.2.cargocollective.com/Using-a-Third-Party-Domain"
            },
            new Fingerprint(){
                Name = "Cloudfront",
                Fingerprints = new List<string>() { "Bad Request: ERROR: The request could not be satisfied" },
                Reference = "https://blog.zsec.uk/subdomainhijack/"
            },
            new Fingerprint(){
                Name = "Fastly",
                Fingerprints = new List<string>() { "Fastly error: unknown domain:" },
                Reference = ""
            },
            new Fingerprint(){
                Name = "Feedpress",
                Fingerprints = new List<string>() { "The feed has not been found." },
                Reference = "https://hackerone.com/reports/195350"
            },
            new Fingerprint(){
                Name = "Ghost",
                Fingerprints = new List<string>() { "The thing you were looking for is no longer here, or never was" },
                Reference = ""
            },
            new Fingerprint(){
                Name = "Github",
                Fingerprints = new List<string>() { "There isn't a Github Pages site here." },
                Reference = "https://hackerone.com/reports/263902"
            },
            new Fingerprint(){
                Name = "Help Juice",
                Fingerprints = new List<string>() { "We could not find what you're looking for." },
                Reference = "https://helpjuice.com"
            },
            new Fingerprint(){
                Name = "Help Scout",
                Fingerprints = new List<string>() { "No settings were found for this company:" },
                Reference = "https://docs.helpscout.net/article/42-setup-custom-domain"
            },
            new Fingerprint(){
                Name = "Heroku",
                Fingerprints = new List<string>() { "No such app" },
                Reference = ""
            },
            new Fingerprint(){
                Name = "JetBrains",
                Fingerprints = new List<string>() { "is not a registered InCloud YouTrack" },
                Reference = ""
            },
            new Fingerprint(){
                Name = "Shopify",
                Fingerprints = new List<string>() { "Sorry, this shop is currently unavailable." },
                Reference = ""
            },
            new Fingerprint(){
                Name = "Statuspage.io",
                Fingerprints = new List<string>() { "This page is currently inactive and can only be viewed by team members. To see this page" },
                Reference = ""
            },
            new Fingerprint(){
                Name = "Surge.sh",
                Fingerprints = new List<string>() { "project not found" },
                Reference = "	https://surge.sh/help/adding-a-custom-domain"
            },
            new Fingerprint(){
                Name = "Tumblr",
                Fingerprints = new List<string>() { "Whatever you were looking for doesn't currently exist at this address" },
                Reference = ""
            },
            new Fingerprint(){
                Name = "UserVoice",
                Fingerprints = new List<string>() { "This UserVoice subdomain is currently available!" },
                Reference = ""
            },
            new Fingerprint(){
                Name = "Wordpress",
                Fingerprints = new List<string>() { "Do you want to register", "wordpress.com?" },
                Reference = ""
            },
            //new Fingerprint(){
            //    Name = "Zendesk",
            //    Fingerprints = new List<string>() { "Help Center Closed" },
            //    Reference = ""
            //},
            new Fingerprint(){
                Name = "SurveyGizmo",
                Fingerprints = new List<string>() { "Oh dear! We can’t seem to find that page, but all is not lost." },
                Reference = "https://help.surveygizmo.com/help/set-up-a-branded-subdomain"
            },
        };

        private static List<Fingerprint> _defaultPageFingerPrints = new List<Fingerprint>()
        {
            new Fingerprint(){
                Name = "IIS7",
                Fingerprints = new List<string>() { "<title>IIS7</title>" },
                Reference = ""
            },
            new Fingerprint(){
                Name = "IIS8",
                Fingerprints = new List<string>() { "<title>Microsoft Internet Information Services 8</title>" },
                Reference = ""
            },
            new Fingerprint(){
                Name = "JBoss",
                Fingerprints = new List<string>() { "<title>Welcome to JBoss Application Server" },
                Reference = ""
            },
            new Fingerprint(){
                Name = "Apache Tomcat",
                Fingerprints = new List<string>() { "<title>Apache Tomcat</title>" },
                Reference = ""
            },
            new Fingerprint(){
                Name = "Apache",
                Fingerprints = new List<string>() { "<title>Apache HTTP Server Test Page" },
                Reference = ""
            },
            new Fingerprint(){
                Name = "ngix on EPEL",
                Fingerprints = new List<string>() { "Welcome to <strong>nginx</strong> on EPEL!" },
                Reference = ""
            },
            new Fingerprint(){
                Name = "RedhatL",
                Fingerprints = new List<string>() { "<title>Test Page for the Apache HTTP Server on Red Hat Enterprise Linux</title>" },
                Reference = ""
            },
            new Fingerprint(){
                Name = "Apache",
                Fingerprints = new List<string>() { "<title>Apache HTTP Server Test Page powered by CentOS</title>" },
                Reference = ""
            },
            new Fingerprint(){
                Name = "Oracle iPlanet",
                Fingerprints = new List<string>() { "<title>Oracle iPlanet Web Server" },
                Reference = ""
            },
            new Fingerprint(){
                Name = "Sun Java",
                Fingerprints = new List<string>() { "<title>Sun Java[TM] System Web Serve" },
                Reference = ""
            },
        };

        private static List<Fingerprint> _indexOfFingerPrints = new List<Fingerprint>()
        {
            new Fingerprint(){
                Name = "Directory Listing",
                Fingerprints = new List<string>() { "<title>Index of" },
                Reference = ""
            },
        };
        #endregion

        public AttackResult Check(AttackRequest request)
        {
            var result = new AttackResult();

            CheckFingerprints(result, request.Body, _defaultPageFingerPrints, " - Default Page found");
            CheckFingerprints(result, request.Body, _indexOfFingerPrints, " - Directory Traversal found");
            CheckFingerprints(result, request.Body, _serviceFingerPrints, " - Service Page found");
            CheckFingerprints(result, request.Body, _subDomainFingerPrints, " - Subdomain takeover found");

            return result;
        }

        private void CheckFingerprints(AttackResult result, string body, List<Fingerprint> fingerPrints, string message)
        {
            foreach (var fp in fingerPrints)
            {
                bool anyFingerPrintsConfirmed = false;
                foreach (var fpString in fp.Fingerprints)
                {
                    if (body.Contains(fpString))
                    {
                        anyFingerPrintsConfirmed = true;
                        break;
                    }
                }

                if (anyFingerPrintsConfirmed)
                {
                    result.Success = true;
                    result.Results.Add(fp.Name + " - Default Page found");
                }
            }
        }

        //public void Initilize()
        //{
        //    throw new NotImplementedException();
        //}
    }
}