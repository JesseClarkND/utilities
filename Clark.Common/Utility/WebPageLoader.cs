using Clark.Common.Models;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace Clark.Common.Utility
{
    public class WebPageLoader
    {
        //Eventually get a dynamic version loading
        //https://stackoverflow.com/questions/24288726/scraping-webpage-generated-by-javascript-with-c-sharp
        //https://www.seleniumhq.org/docs/03_webdriver.jsp

        public static void Load(WebPageRequest webRequest)
        {
            if (webRequest.Address.StartsWith("http"))
            {
                try
                {
                    MakeRequest(webRequest);//webRequest.Address, webRequest.Response, webRequest.CookieJar, webRequest.Headers, webRequest.Method);
                }
                catch (Exception ex)
                {
                    if (ex.Message == "Trust Issue")
                    {
                        webRequest.Address = DomainUtility.EnsureHTTPS(webRequest.Address);
                        MakeRequest(webRequest);
                    }
                    else
                        throw;
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
                                    //WebPageResponse scriptResponse = new WebPageResponse();
                                    WebPageRequest scriptRequest = new WebPageRequest(webRequest);//scriptSrc
                                    scriptRequest.Address = scriptSrc;
                                    scriptRequest.RequestBody = "";
                                    MakeRequest(scriptRequest);

                                    webRequest.Response.Scripts.Add(scriptSrc, scriptRequest.Response.Body);
                                }
                            }
                            catch { }
                        }
                    }
                }
            }
            else
            {
                string vanillaAddress = webRequest.Address;
                webRequest.Address = "https://" + vanillaAddress;
                MakeRequest(webRequest);
                if (String.IsNullOrEmpty(webRequest.Response.Body))
                {
                    webRequest.Address = "http://" + vanillaAddress;
                    MakeRequest(webRequest);
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
                                    //WebPageResponse scriptResponse = new WebPageResponse();
                                    WebPageRequest scriptRequest = new WebPageRequest(webRequest);//scriptSrc
                                    scriptRequest.Address= scriptSrc;
                                    scriptRequest.RequestBody="";
                                    MakeRequest(scriptRequest);

                                    webRequest.Response.Scripts.Add(scriptSrc, scriptRequest.Response.Body);
                                }
                            }
                            catch { }
                        }
                    }
                }
            }
        }

        private static void MakeRequest(WebPageRequest webRequest)//string url, WebPageResponse webResponse, CookieCollection cookieCollection, WebHeaderCollection headers, string method)
        {
            try
            {
                //This line will ignore any cert errors
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });

                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                       | SecurityProtocolType.Tls11
                       | SecurityProtocolType.Tls12
                       | SecurityProtocolType.Ssl3;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(webRequest.Address);
                if (webRequest.CookieJar.Count != 0)
                {
                    request.CookieContainer = new CookieContainer();
                    request.CookieContainer.Add(webRequest.CookieJar);
                }

                if (webRequest.Headers.Count != 0)
                {
                    request.Headers = webRequest.Headers;
                }

                if (webRequest.ContentType.Length != 0)
                    request.ContentType = webRequest.ContentType;

                request.Method = webRequest.Method;

                if (webRequest.RequestBody.Length != 0) 
                {
                    using (var stream = request.GetRequestStream())
                    {
                        var data = Encoding.ASCII.GetBytes(webRequest.RequestBody);
                        stream.Write(data, 0, data.Length);
                    }
                }
                
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string data = ReadStream(response);

                    response.Close();

                    webRequest.Response.Body = data;
                }

                webRequest.Response.Code = ((int)response.StatusCode).ToString();
                webRequest.Response.CookieJar = response.Cookies;
                webRequest.Response.Headers = response.Headers;
            }
            catch (WebException e)
            {
                switch (e.Status)
                {
                    case WebExceptionStatus.TrustFailure:
                        throw new Exception("Trust Issue");
                    case WebExceptionStatus.ProtocolError:
                        if (e.Response != null)
                        {
                            webRequest.Response.Code = "404";
                            var resp = (HttpWebResponse)e.Response;
                            if (resp.StatusCode == HttpStatusCode.NotFound)
                            {
                                webRequest.Response.Body = ReadStream(resp);
                            }
                            else
                            {
                                // Do something else
                            }
                        }

                        webRequest.Response.NotFound = true;
                        break;
                    case WebExceptionStatus.Timeout:
                        if (webRequest.Response != null)
                        {
                            // Handle timeout exception
                            webRequest.Response.TimeOut = true;
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

            if (response.CharacterSet == null || String.IsNullOrEmpty(response.CharacterSet))
            {
                readStream = new StreamReader(receiveStream);
            }
            else
            {
                readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet.Replace("\\", String.Empty).Replace("\"", String.Empty)));
            }

            string data = readStream.ReadToEnd();
            readStream.Close();
            return data;
        }

        private static string ResolveRelativePaths(string relativeUrl, string originatingUrl)
        {
            string resolvedUrl = String.Empty;

            if (relativeUrl.StartsWith("http"))
                return relativeUrl;



            bool tampered = false;
            Uri originatingUri = new Uri(originatingUrl);
            string originalHost = originatingUri.Scheme + "://" + originatingUri.Host;
            if (originatingUrl.StartsWith("http"))
            {
                tampered = true;
                originatingUrl = originatingUrl.Substring(originalHost.Length, originatingUrl.Length - originalHost.Length);
            }

            if (relativeUrl.StartsWith("/"))
            {
                resolvedUrl = originalHost.Trim('/') + '/' + relativeUrl.Trim('/');
                return resolvedUrl;
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