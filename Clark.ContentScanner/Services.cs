using Clark.ContentScanner.Internal;
using Clark.ContentScanner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clark.ContentScanner
{
    public class Services
    {
        private static List<Fingerprint> _domainList = new List<Fingerprint>();
        private static readonly object _syncObject = new object();

        public static ScannerResult Check(ScannerRequest request)
        {
            if (_domainList.Count == 0)
            {
                lock (_syncObject)
                {
                    if (_domainList.Count == 0)
                        Initialize();
                }
            }

            ScannerResult result = new ScannerResult();
            if (String.IsNullOrEmpty(request.Body))
                return result;

            string domain = "";
            foreach (Fingerprint domainTest in _domainList)
            {
                domain = Check_Domain(request.Body, domainTest);
                if (!String.IsNullOrEmpty(domain))
                {
                    result.Success = true;
                    result.Results.Add(domain);
                    return result;
                }
            }

            return result;
        }

        private static string Check_Domain(string body, Fingerprint domain)
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
                return domain.Name;
            return "";
        }

        private static void Initialize()
        {
            _domainList.Add(new Fingerprint()
            {
                Name = "Jenkins",
                Fingerprints = new List<string>() { "<a href=\"https://jenkins.io/\">Jenkins", "<span class=\"jenkins_ver\">" },
                Reference = ""
            });

            _domainList.Add(new Fingerprint()
            {
                Name = "Nexus",
                Fingerprints = new List<string>() { "<title>Nexus Repository Manager</title>" },
                Reference = ""
            });

            _domainList.Add(new Fingerprint()
            {
                Name = "Artifactory",
                Fingerprints = new List<string>() { "Jfrog" },
                Reference = ""
            });

            _domainList.Add(new Fingerprint()
            {
                Name = "SonarQube",
                Fingerprints = new List<string>() { "<title>SonarQube</title>" },
                Reference = ""
            });

            _domainList.Add(new Fingerprint()
            {
                Name = "JIRA",
                Fingerprints = new List<string>() { "Jira</title>" },
                Reference = ""
            });
            _domainList.Add(new Fingerprint()
            {
                Name = "Github SSO",
                Fingerprints = new List<string>() { "New to GitHub?", "Create an account" },
                Reference = ""
            });
        }
    }
}