using Clark.Common.Models;
using Clark.Common.Utility;
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

        public static string Check(string domain)
        {
            if (_fileNames.Count == 0)
                Initialize();

            foreach (string fileName in _fileNames)
            {
                string testedFile = domain.Trim('/') + "/" + fileName;
                WebPageRequest request = new WebPageRequest(testedFile);
                WebPageLoader.Load(request);
                if (Check_Contents(request.Response.Body))
                    return testedFile;
            }
            return "";
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