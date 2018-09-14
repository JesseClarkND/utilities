using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clark.ContentScanner
{
    public class Services
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
                if (String.IsNullOrEmpty(domain))
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
                    break;
                }
            }

            if (anyFingerPrintsConfirmed)
                return domain.Engine;
            return "";
        }

        private static void Initialize()
        {
            _domainList.Add(new Domain()
            {
                Engine = "Jenkins",
                Fingerprints = new List<string>() { "<a href=\"https://jenkins.io/\">Jenkins", "<span class=\"jenkins_ver\">" },
                Reference = ""
            });

            _domainList.Add(new Domain()
            {
                Engine = "Nexus",
                Fingerprints = new List<string>() { "<title>Nexus Repository Manager</title>" },
                Reference = ""
            });

            _domainList.Add(new Domain()
            {
                Engine = "Artifactory",
                Fingerprints = new List<string>() { "Jfrog" },
                Reference = ""
            });

            _domainList.Add(new Domain()
            {
                Engine = "SonarQube",
                Fingerprints = new List<string>() { "<title>SonarQube</title>" },
                Reference = ""
            });

            _domainList.Add(new Domain()
            {
                Engine = "JIRA",
                Fingerprints = new List<string>() { "Jira</title>" },
                Reference = ""
            });
            _domainList.Add(new Domain()
            {
                Engine = "Github SSO",
                Fingerprints = new List<string>() { "New to GitHub?", "Create an account" },
                Reference = ""
            });
        }
    }
}