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

        public static bool Check(string body)
        {
            if (_indexFingerPrints.Count == 0)
                Initialize();
            foreach (string fingerprint in _indexFingerPrints) {
                if (body.Contains(fingerprint))
                    return true;
            }
            return false;
        }

        private static void Initialize()
        {
            _indexFingerPrints.Add("<title>Index of");
        }
    }
}
