using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Clark.Attack.HTTPHeader.Models
{
    internal class AttackHeader
    {
        public WebHeaderCollection AttackHeaderCollection = new WebHeaderCollection();
        public WebHeaderCollection FingerPrintHeaders = new WebHeaderCollection();
    }
}