using Clark.Common.Models;
using Clark.Common.Utility;
using Clark.ContentScanner.Models;
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
        private static List<string> _bucketURLRegex = new List<string>();
        private static readonly object _syncObject = new object();

        public static ScannerResult BucketCheck(ScannerRequest request)
        {

            if (_bucketURLRegex.Count == 0)
            {
                lock (_syncObject)
                {
                    if (_bucketURLRegex.Count == 0)
                        Initilize();
                }
            }

            ScannerResult result = new ScannerResult();
            List<string> referencedBuckets = new List<string>();
            List<string> bustedBuckets = new List<string>();

            foreach(string search in _bucketURLRegex){
                MatchCollection collection = Regex.Matches(request.Body, search);
                referencedBuckets.AddRange(collection.Cast<Match>().Select(match => match.Value).ToList());
            }

            referencedBuckets = referencedBuckets.Distinct().ToList();
                 //todo: make better regex so we dont have to do this
            referencedBuckets.RemoveAll(x => x.Trim('/') == @"http://s3.amazonaws.com");
            referencedBuckets.RemoveAll(x => x.Trim('/') == @"https://s3.amazonaws.com");

            foreach (string bucket in referencedBuckets)
            {
                WebPageRequest webRequest = new WebPageRequest(bucket);
                WebPageLoader.Load(webRequest);
                if (webRequest.Response.Body.Contains("<Code>NoSuchBucket</Code>"))
                {
                    result.Success = true;
                    bustedBuckets.Add(bucket);
                }
            }

            result.Results.AddRange(bustedBuckets);
            return result;
        }

        private static void Initilize()
        {
            _bucketURLRegex.Add("http[s]?:\\/\\/?(([^.]+)\\.)?s3.amazonaws\\.com");
            _bucketURLRegex.Add("http[s]?:\\/\\/s3.amazonaws.com\\/[^/\"']+");
            //_bucketStrings.Add("s3.amazonaws.com");
           // _bucketStrings.Add(@"s3\.amazonaws\.com");
        }
    }
}