using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Clark.ContentScanner
{
    public static class S3Bucket
    {
        private static List<string> _bucketStrings = new List<string>();

        public static List<string> BucketCheck(string body) {
            if (_bucketStrings.Count == 0)
                Initilize();

            List<string> references = new List<string>();
            foreach(string search in _bucketStrings){
                MatchCollection collection = Regex.Matches(body, search);
                references.AddRange(collection.Cast<Match>().Select(match => match.Value).ToList());
            }

            return references.Distinct().ToList();
        }

        private static void Initilize()
        {
            _bucketStrings.Add("https:\\/\\/?(([^.]+)\\.)?s3.amazonaws\\.com");
            _bucketStrings.Add("https:\\/\\/s3.amazonaws.com\\/[^/\"']+");
            //_bucketStrings.Add("s3.amazonaws.com");
           // _bucketStrings.Add(@"s3\.amazonaws\.com");
        }
    }
}