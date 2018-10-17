using Clark.ContentScanner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clark.ContentScanner
{
    public static class IndexOf
    {
        private static List<string> _indexFingerPrints = new List<string>();
        private static readonly object _syncObject = new object();

        public static ScannerResult Check(ScannerRequest request)
        {
            if (_indexFingerPrints.Count == 0)
            {
                lock (_syncObject)
                {
                    if (_indexFingerPrints.Count == 0)
                        Initialize();
                }
            }

            ScannerResult result = new ScannerResult();
            foreach (string fingerprint in _indexFingerPrints) {
                if (request.Body.Contains(fingerprint))
                    result.Success=true;
            }
            return result;
        }

        private static void Initialize()
        {
            _indexFingerPrints.Add("<title>Index of");
        }
    }
}
