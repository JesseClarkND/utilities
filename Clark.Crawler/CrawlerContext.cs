using Clark.Crawler.Interfaces;
using Clark.Crawler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Clark.Crawler
{
    public static class CrawlerContext
    {
        public static ManualResetEvent PauseEvent = new ManualResetEvent(true);
        public static bool SinglePage = false;
        public static int Depth = 100;
        public static Uri URI;
        public static string Authority;
        public static HTTPVerb Verb = HTTPVerb.GET;
        public static List<string> IgnoreDirectory = new List<string>();
        public static List<string> IgnoreVariable = new List<string>();//todo
        public static CookieContainer CookieContainer = null;
        public static List<IRequest> Pages = new List<IRequest>();
        public static List<string> ExhaustedURL = new List<string>();

        public static bool IncludeSubdomains = false;
        public static bool IncludeDOMUrls = false;//todo
        public static bool ExhaustPageInput = false;
        public static string UserAgent = "";
        public static bool LightMode = false;

        public static void Initialize(string userAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:40.0) Gecko/20100101 Firefox/40.1")
        {
            UserAgent = userAgent;
            Pages = new List<IRequest>();
            ExhaustedURL = new List<string>();
        }

        public static void SetURL(string url)
        {
            UriBuilder uriBuilder = new UriBuilder(url);
            URI = uriBuilder.Uri;
            Authority = URI.Authority;
        }
    }

    public enum HTTPVerb
    {
        ALL,
        GET,
        POST
    }
}
