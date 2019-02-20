using Clark.Attack.Common.Interfaces;
using Clark.Attack.Common.Models;
using Clark.Attack.InformationLeak.Models;
using Clark.Common.Models;
using Clark.Common.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clark.Attack.InformationLeak
{
    /// <summary>
    /// Check files that are known for leaking info, phpinfo, htaccess, etc
    /// </summary>
    public class Processor : IAttack
    {
        public string Name { get { return "Information Leak"; } set { } }

        #region Private

        private static List<Leak> _leaky = new List<Leak>()
        {
            new Leak()
            {//https://medium.com/@kedrisec/how-i-found-2-9-rce-at-yahoo-bug-bounty-program-20ab50dbfac7
                FileNames = new List<string>(){
                    "s3_adbox_setup",
                    "q/s3_adbox_setup"
                },
                FingerPrints = new List<string>(){
                    "New Message",
                }
            },
            new Leak()
            {//https://medium.com/bugbountywriteup/900-xss-in-yahoo-recon-wins-65ee6d4bfcbd
                FileNames = new List<string>(){
                    "about.php"
                },
                FingerPrints = new List<string>(){
                    "WebPagetest is an open",
                }
            },
            new Leak()
            {
                FileNames = new List<string>(){
                    "Debug"
                },
                FingerPrints = new List<string>(){
                    "Unisphere Management Server Debugging Facility", "CIMOM",
                }
            },
            new Leak()
            {
                FileNames = new List<string>(){
                    "server-status"
                },
                FingerPrints = new List<string>(){
                    "Server Status for",
                }
            },
            new Leak()
            {
                FileNames = new List<string>(){
                    "getphpinfo.php",
                    "phpinfo.php",
                    "info.php",
                    "php.php",
                    "pi.php"
                },
                FingerPrints = new List<string>(){
                    "<title>phpinfo()</title>",
                    "PHP Version"
                }
            },
            new Leak(){
                FileNames = new List<string>(){
                    ".htaccess",
                },
                FingerPrints = new List<string>(){
                    "AuthUserFile",
                    "AuthType",
                    "ErrorDocument",
                    "RewriteRule"
                }
            },
            new Leak(){
                FileNames = new List<string>(){
                    ".git",
                },
                FingerPrints = new List<string>(){
                    "gitignore",
                    "README",
                }
            },
            new Leak(){
                FileNames = new List<string>(){
                    "web.xml",
                    "WEB-INF/web.xml",
                    "./WEB-INF/web.xml",
                    ".//WEB-INF/web.xml",
                    ".;/WEB-INF/web.xml"
                },
                FingerPrints = new List<string>(){
                    "<?xml version=",
                }
            },
        };

        #endregion

        public AttackResult Check(AttackRequest request)
        {
            var result = new AttackResult();

            foreach (var leak in _leaky)
            {
                FindLeak(request.URL, leak, result);
            }

            return result;
        }

        private void FindLeak(string url, Leak leak, AttackResult result)
        {
            foreach (var fileName in leak.FileNames)
            {
                string testedFile = url + "/" + fileName;
                WebPageRequest webRequest = new WebPageRequest(testedFile);
                WebPageLoader.Load(webRequest);
                if (Check_Contents(webRequest.Response.Body, leak.FingerPrints))
                {
                    result.Success = true;
                    result.Results.Enqueue("Tested File: " + testedFile);
                    break;
                }
            }
        }

        private static bool Check_Contents(string body, List<string> fingerPrints)
        {
            bool anyFingerPrintsConfirmed = false;
            foreach (string fingerPrint in fingerPrints)
            {
                if (body.Contains(fingerPrint))
                {
                    anyFingerPrintsConfirmed = true;
                    break;
                }
            }

            return anyFingerPrintsConfirmed;
        }
    }
}