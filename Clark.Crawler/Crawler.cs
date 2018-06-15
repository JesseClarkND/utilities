using Clark.Crawler.Interfaces;
using Clark.Crawler.Models;
using Clark.Crawler.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Clark.Crawler
{
    public class Crawler
    {
        private static Action<IRequest> _responseHandler;
        private static Action _pageCounter;

        public static void CrawlSite(object responseHandler, object actCounter)
        {
            _responseHandler = (Action<IRequest>)responseHandler;
            _pageCounter = (Action)actCounter;

            IRequest request = new Request(CrawlerContext.URI);
            int step = 0;

            CrawlPage(request, step + 1);
        }

        private static void CrawlPage(IRequest request, int step)
        {
           // if (request.Url.Trim('/').Split('/').Count() - 2 > CrawlerContext.Depth)
            if (step > CrawlerContext.Depth)
                return;

            if (CrawlerContext.SinglePage && step > 2)
                return;

            Uri tempUri = new Uri(request.Url);
            string tempDomain = DomainUtility.GetDomainFromUrl(tempUri);
            if (CrawlerContext.IgnoreDirectory.Count != 0 && IgnoreDirectory(request.Url, tempDomain))
                return;

            if (!PageHasBeenCrawled(request))
            {
                CrawlerContext.Pages.Add(request);

                RequestUtility.GetWebText(request);

                _pageCounter.Invoke();
                if (request.Response.Error)
                {
                    //log
                    return;
                }

                _responseHandler.Invoke(request);

                LinkParser linkParser = new LinkParser();

                linkParser.ParseLinksAgility(request.Response.Body, request.Url);

                if (CrawlerContext.LightMode)
                    request.Response.Body = "";

                foreach (IRequest link in linkParser.GoodUrls)
                {
                    CrawlerContext.PauseEvent.WaitOne(Timeout.Infinite);
                    try
                    {
                        CrawlPage(link, step + 1);
                    }
                    catch
                    {
                        // _failedUrls.Add(link + " (on page at url " + url + ") - " + exc.Message);
                    }
                }
                CrawlerContext.ExhaustedURL.Add(request.Url);
            }
        }

        private static bool IgnoreDirectory(string url, string domain)
        {
            string directory = url.Split(new string[] { domain }, StringSplitOptions.None).Last().Trim('/');

            if (String.IsNullOrEmpty(directory))
                return false;

            foreach (string ignoreDir in CrawlerContext.IgnoreDirectory)
            {
                if (directory.StartsWith(ignoreDir))
                    return true;
            }

            return false;
        }

        private static bool PageHasBeenCrawled(IRequest request)
        {
            if (!CrawlerContext.IncludeDOMUrls)
            {
                request.Url = request.Url.Split('#').First();
            }

            if (CrawlerContext.Pages.Contains(request))
                return true;

            //List<UrlEntity> urls = _attackUrls.Where(x => x.Url.Split('?')[0] == url.Url.Split('?')[0]).ToList();
            //urls = urls.Where(x => x.Attack.Type == url.Attack.Type).ToList();
            //urls = urls.Where(x => x.AttackedParameter == url.AttackedParameter).ToList();

            //if (urls.Count > 0)
            //    return true;

            return false;
        }
    }
}
