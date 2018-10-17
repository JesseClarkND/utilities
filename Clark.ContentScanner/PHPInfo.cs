using Clark.Common.Models;
using Clark.Common.Utility;
using Clark.ContentScanner.Models;
using Clark.ContentScanner.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clark.ContentScanner
{
    public static class PHPInfo
    {
        private static List<string> _fileNames = new List<string>();
        private static List<string> _fingerPrints = new List<string>();
        private static readonly object _syncObject = new object();

        public static ScannerResult Check(ScannerRequest request)
        {
            if (_fileNames.Count == 0)
            {
                lock (_syncObject)
                {
                    if (_fileNames.Count == 0)
                        Initialize();
                }
            }

            ScannerResult result = new ScannerResult();
            foreach (string fileName in _fileNames)
            {
                string testedFile = request.URL.Trim('/') + "/" + fileName;
                WebPageRequest webRequest = new WebPageRequest(testedFile);
                WebPageLoader.Load(webRequest);
                if (Check_Contents(webRequest.Response.Body))
                {
                    result.Success = true;
                    result.Results.Add(testedFile);
                    return result;
                }
            }
            return result;
        }

        private static bool Check_Contents(string body)
        {
            bool anyFingerPrintsConfirmed = false;
            foreach (string fingerPrint in _fingerPrints)
            {
                if (body.Contains(fingerPrint))
                {
                    anyFingerPrintsConfirmed = true;
                    break;
                }
            }

            return anyFingerPrintsConfirmed;
        }

        private static void Initialize()
        {
            _fileNames.Add("getphpinfo.php");
            _fileNames.Add("phpinfo.php");
            _fileNames.Add("info.php");
            _fileNames.Add("phpinfo.html");
            _fileNames.Add("php.php");
            _fileNames.Add("pi.php");

            _fingerPrints.Add("<title>phpinfo()</title>");
            _fingerPrints.Add("PHP Version");
        }
    }
}