using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Clark.ContentScanner.Utility
{
    internal class WebRequest
    {
        public string Url = "";
        public WebResponse Response = new WebResponse();

        public WebRequest(string url)
        {
            Url = url;
        }

    }

    internal class WebResponse
    {
        public bool Error = false;
        public string ErrorMessage = "";
        public string Code = "";
        public string Body = "";
        public bool NotFound = false;
        public bool TimeOut = false;
    }

    public static class WebRequestUtility
    {
        internal static void GetWebText(WebRequest request)
        {
            HttpWebResponse httpResponse = null;
            try
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                HttpWebRequest httpRequest = BuildRequest(request.Url);
                httpRequest.Proxy = null;
                httpRequest.MaximumAutomaticRedirections = 2;
                httpRequest.AllowAutoRedirect = true;
                httpResponse = (HttpWebResponse)httpRequest.GetResponse();

                //foreach (Cookie cookie in httpResponse.Cookies)
                //    CrawlerContext.CookieContainer.Add(cookie);

                Stream stream = httpResponse.GetResponseStream();
                StreamReader readStream = null;
                if (httpResponse.CharacterSet == null)
                {
                    readStream = new StreamReader(stream);
                }
                else
                {
                    readStream = new StreamReader(stream, Encoding.GetEncoding(httpResponse.CharacterSet.Replace("\\", String.Empty).Replace("\"", String.Empty)));
                }

                request.Response.Code = ((int)httpResponse.StatusCode).ToString();

                string httpBody = readStream.ReadToEnd();
                stream.Close();
                readStream.Close();

                request.Response.Body = httpBody;
            }
            catch (WebException e)
            {
                switch (e.Status)
                {
                    case WebExceptionStatus.ProtocolError:
                        if (e.Response != null)
                        {
                            var resp = (HttpWebResponse)e.Response;
                            if (resp.StatusCode == HttpStatusCode.NotFound)
                            {
                                request.Response.Body = ReadStream(resp);
                            }
                            else
                            {
                                // Do something else
                            }
                        }

                        request.Response.NotFound = true;
                        break;
                    case WebExceptionStatus.Timeout:
                        if (request.Response != null)
                        {
                            // Handle timeout exception
                            request.Response.TimeOut = true;
                        }
                        break;
                }
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

        private static string ReadStream(HttpWebResponse response)
        {
            Stream receiveStream = response.GetResponseStream();
            StreamReader readStream = null;

            if (response.CharacterSet == null)
            {
                readStream = new StreamReader(receiveStream);
            }
            else
            {
                readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
            }

            string data = readStream.ReadToEnd();
            readStream.Close();
            return data;
        }

        private static HttpWebRequest BuildRequest(string myUri)
        {
            HttpWebRequest request = HttpWebRequest.CreateHttp(myUri);
           // request.CookieContainer = CrawlerContext.CookieContainer;
            //request.Method = CrawlerContext.Verb.ToString();
            //request.Method = "HEAD";
            //request.AllowAutoRedirect = false;
            request.ProtocolVersion = HttpVersion.Version11;
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:40.0) Gecko/20100101 Firefox/40.1";
            //  request.KeepAlive = false;
            return request;
        }
    }
}