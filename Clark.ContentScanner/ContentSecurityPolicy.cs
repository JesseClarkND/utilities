using Clark.Common.Models;
using Clark.Common.Utility;
using Clark.ContentScanner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clark.ContentScanner
{
    public static class ContentSecurityPolicy
    {
        public static ScannerResult Check(ScannerRequest request)
        {
            ScannerResult result = new ScannerResult();

            WebPageRequest webRequest = new WebPageRequest(request.URL);
            WebPageLoader.Load(webRequest);

            List<string> csp = new List<string>();

            if (webRequest.Response.Headers.AllKeys.Contains("Content-Security-Policy"))
            {
                csp = webRequest.Response.Headers["Content-Security-Policy"].Split(' ').ToList();

                Uri uriResult;
                foreach (string url in csp)
                {
                    string testUrl = url;

                    if (!testUrl.Contains('.'))
                        continue;

                    if (testUrl.StartsWith("wss"))
                        continue;

                    if (testUrl.StartsWith("*."))
                        testUrl = testUrl.Replace("*.", "");

                    if (!testUrl.StartsWith("http"))
                        testUrl = DomainUtility.EnsureHTTPS(testUrl);

                    bool validURL = Uri.TryCreate(testUrl, UriKind.Absolute, out uriResult)
                        && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

                    if (validURL)
                    {
                        WebPageRequest testRequest = new WebPageRequest(uriResult.ToString());
                        WebPageLoader.Load(testRequest);

                        if (!testRequest.Response.Code.Equals("200"))
                        {
                            if (!testRequest.Response.Code.Equals("403"))
                            {
                                result.Success = true;
                                result.Results.Add(uriResult.ToString());
                            }
                        }
                    }
                }
            }

            return result;
        }
    }
}