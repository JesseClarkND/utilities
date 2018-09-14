using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Clark.Subdomain.Utility
{
    public class WebPageRequest
    {
        public string Address = "";
        public WebPageResponse Response = new WebPageResponse();
    }

    public class WebPageResponse
    {
        public string Body="";
        public Dictionary<string, string> Scripts = new Dictionary<string, string>();
        public bool TimeOut = false;
        public string Message = "";
        public bool NotFound = false;
    }

    public class WebPageLoader
    {
        //Eventually get a dynamic version loading
        //https://stackoverflow.com/questions/24288726/scraping-webpage-generated-by-javascript-with-c-sharp
        //https://www.seleniumhq.org/docs/03_webdriver.jsp

        public static void Load(WebPageRequest webRequest)
        {
            if (webRequest.Address.StartsWith("http"))
            {
                MakeRequest(webRequest.Address, webRequest.Response);

                if (!String.IsNullOrEmpty(webRequest.Response.Body))
                {
                    HtmlDocument document = new HtmlDocument();
                    document.LoadHtml(webRequest.Response.Body);

                    var scriptNodes = document.DocumentNode.SelectNodes("//script[contains(@src,'/js')]");
                    if (scriptNodes != null)
                    {
                        foreach (var scriptNode in scriptNodes)
                        {
                            try
                            {
                                string script = scriptNode.Attributes["src"].Value;

                                string scriptSrc = ResolveRelativePaths(script, webRequest.Address);
                                if (!webRequest.Response.Scripts.ContainsKey(scriptSrc))
                                {
                                    WebPageResponse scriptResponse = new WebPageResponse();
                                    MakeRequest(scriptSrc, scriptResponse);
                                    webRequest.Response.Scripts.Add(scriptSrc, scriptResponse.Body);
                                }
                            }
                            catch { }
                        }
                    }
                }
            }
            else
            {
                MakeRequest("https://" + webRequest.Address, webRequest.Response);
                if (String.IsNullOrEmpty(webRequest.Response.Body))
                {
                    MakeRequest("http://" + webRequest.Address, webRequest.Response);
                    webRequest.Address = "http://" + webRequest.Address;
                }
                else
                {
                    webRequest.Address = "https://" + webRequest.Address;
                }

                if (!String.IsNullOrEmpty(webRequest.Response.Body))
                {
                    HtmlDocument document = new HtmlDocument();
                    document.LoadHtml(webRequest.Response.Body);

                    var scriptNodes = document.DocumentNode.SelectNodes("//script[contains(@src,'/js')]");
                    if (scriptNodes != null)
                    {
                        foreach (var scriptNode in scriptNodes)
                        {
                            try
                            {
                                string script = scriptNode.Attributes["src"].Value;

                                string scriptSrc = ResolveRelativePaths(script, webRequest.Address);
                                if (!webRequest.Response.Scripts.ContainsKey(scriptSrc))
                                {
                                    WebPageResponse scriptResponse = new WebPageResponse();
                                    MakeRequest(scriptSrc, scriptResponse);
                                    webRequest.Response.Scripts.Add(scriptSrc, scriptResponse.Body);
                                }
                            }
                            catch { }
                        }
                    }
                }
            }
        }

        private static void MakeRequest(string url, WebPageResponse webResponse)
        {
            try
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                       | SecurityProtocolType.Tls11
                       | SecurityProtocolType.Tls12
                       | SecurityProtocolType.Ssl3;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string data = ReadStream(response);

                    response.Close();

                    webResponse.Body = data;
                }
            }
            catch (WebException e)
            {
                switch(e.Status){

                    case WebExceptionStatus.ProtocolError:
                        if (e.Response != null)
                        {
                            var resp = (HttpWebResponse)e.Response;
                            if (resp.StatusCode == HttpStatusCode.NotFound)
                            {
                                webResponse.Body = ReadStream(resp);
                            }
                            else
                            {
                                // Do something else
                            }
                        }

                        webResponse.NotFound = true;
                    break;
                    case WebExceptionStatus.Timeout:
                        if (webResponse != null)
                        {
                            // Handle timeout exception
                            webResponse.TimeOut = true;
                        }
                    break;
                }
            }
            catch
            {
                //todo
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

        private static string ResolveRelativePaths(string relativeUrl, string originatingUrl)
        {
            string resolvedUrl = String.Empty;

            bool tampered = false;
            Uri originatingUri = new Uri(originatingUrl);
            string originalHost = originatingUri.Scheme + "://" + originatingUri.Host;
            if (originatingUrl.StartsWith("http"))
            {
                tampered = true;
                originatingUrl = originatingUrl.Substring(originalHost.Length, originatingUrl.Length - originalHost.Length);
            }

            string[] relativeUrlArray = relativeUrl.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            string[] originatingUrlElements = originatingUrl.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            int indexOfFirstNonRelativePathElement = 0;
            for (int i = 0; i <= relativeUrlArray.Length - 1; i++)
            {
                if (relativeUrlArray[i] != "..")
                {
                    indexOfFirstNonRelativePathElement = i;
                    break;
                }
            }

            int countOfOriginatingUrlElementsToUse = originatingUrlElements.Length - indexOfFirstNonRelativePathElement - 1;
            for (int i = 0; i <= countOfOriginatingUrlElementsToUse - 1; i++)
            {
                if (originatingUrlElements[i] == "http:" || originatingUrlElements[i] == "https:")
                    resolvedUrl += originatingUrlElements[i] + "//";
                else
                    resolvedUrl += originatingUrlElements[i] + "/";
            }

            for (int i = 0; i <= relativeUrlArray.Length - 1; i++)
            {
                if (i >= indexOfFirstNonRelativePathElement)
                {
                    resolvedUrl += relativeUrlArray[i];

                    if (i < relativeUrlArray.Length - 1)
                        resolvedUrl += "/";
                }
            }

            if (tampered)
                resolvedUrl = originalHost.Trim('/') + '/' + resolvedUrl.Trim('/');

            return resolvedUrl;
        }
    }
}