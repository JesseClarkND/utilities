using Clark.Crawler.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Clark.Crawler.Utilities
{
    public static class RequestUtility
    {
        public static void GetWebText(IRequest request)
        {
            HttpWebResponse httpResponse = null;
            try
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                // UriBuilder uriBuilder = new UriBuilder(_myUri.Scheme, url);
                // HttpWebRequest request = BuildRequest(uriBuilder.Uri);
                HttpWebRequest httpRequest = BuildRequest(request.Url);
                httpRequest.Proxy = null;
                httpRequest.MaximumAutomaticRedirections = 2;
                httpRequest.AllowAutoRedirect = true;
                httpResponse = (HttpWebResponse)httpRequest.GetResponse();

                foreach (Cookie cookie in httpResponse.Cookies)
                    CrawlerContext.CookieContainer.Add(cookie);

                Stream stream = httpResponse.GetResponseStream();
                StreamReader readStream = null;
                if (httpResponse.CharacterSet == null)
                {
                    readStream = new StreamReader(stream);
                }
                else
                {
                    readStream = new StreamReader(stream, Encoding.GetEncoding(httpResponse.CharacterSet));
                }

                request.Response.Code = ((int)httpResponse.StatusCode).ToString();

                string httpBody = readStream.ReadToEnd();
                stream.Close();
                readStream.Close();

                request.Response.Body = httpBody;
            }
            catch (Exception e)
            {
                request.Response.Error = true;
                request.Response.ErrorMessage = e.Message;
            }
            finally
            {
                if (httpResponse != null)
                {
                    httpResponse.Close();
                }
            }
        }

        private static HttpWebRequest BuildRequest(string myUri)
        {
            HttpWebRequest request = HttpWebRequest.CreateHttp(myUri);
            request.CookieContainer = CrawlerContext.CookieContainer;
            request.Method = CrawlerContext.Verb.ToString();
            //request.Method = "HEAD";
            //request.AllowAutoRedirect = false;
            request.ProtocolVersion = HttpVersion.Version10;
            request.UserAgent = CrawlerContext.UserAgent;
            //  request.KeepAlive = false;
            return request;
        }
    }
}