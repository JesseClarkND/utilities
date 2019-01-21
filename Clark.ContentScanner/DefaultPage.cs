using Clark.ContentScanner.Internal;
using Clark.ContentScanner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clark.ContentScanner
{
    public static class DefaultPage
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
                Name = "IIS7",
                Fingerprints = new List<string>() { "<title>IIS7</title>" },
                Reference = ""
            });

            _domainList.Add(new Fingerprint()
            {
                Name = "IIS8",
                Fingerprints = new List<string>() { "<title>Microsoft Internet Information Services 8</title>" },
                Reference = ""
            });

            _domainList.Add(new Fingerprint()
            {
                Name = "JBoss",
                Fingerprints = new List<string>() { "<title>Welcome to JBoss Application Server" },
                Reference = ""
            });

            _domainList.Add(new Fingerprint()
            {
                Name = "Apache Tomcat",
                Fingerprints = new List<string>() { "<title>Apache Tomcat</title>" },
                Reference = ""
            });

            _domainList.Add(new Fingerprint()
            {
                Name = "Apache",
                Fingerprints = new List<string>() { "<title>Apache HTTP Server Test Page" },
                Reference = ""
            });

            _domainList.Add(new Fingerprint()
            {
                Name = "ngix on EPEL",
                Fingerprints = new List<string>() { "Welcome to <strong>nginx</strong> on EPEL!" },
                Reference = ""
            });

            _domainList.Add(new Fingerprint()
            {
                Name = "RedhatL",
                Fingerprints = new List<string>() { "<title>Test Page for the Apache HTTP Server on Red Hat Enterprise Linux</title>" },
                Reference = ""
            });

            _domainList.Add(new Fingerprint()
            {
                Name = "Apache",
                Fingerprints = new List<string>() { "<title>Apache HTTP Server Test Page powered by CentOS</title>" },
                Reference = ""
            });

            _domainList.Add(new Fingerprint()
            {
                Name = "Oracle iPlanet",
                Fingerprints = new List<string>() { "<title>Oracle iPlanet Web Server" },
                Reference = ""
            });

            _domainList.Add(new Fingerprint()
            {
                Name = "Sun Java",
                Fingerprints = new List<string>() { "<title>Sun Java[TM] System Web Serve" },
                Reference = ""
            });
        }
    }
}