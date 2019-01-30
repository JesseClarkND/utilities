using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clark.Attack.SocialMedia.Models
{
    internal class DomainData
    {
        public string DomainName;
        public List<string> IgnorePhrases;
        public List<string> UserNames;

        public DomainData(string domainName)
        {
            DomainName = domainName;
            IgnorePhrases = new List<string>();
        }

        public DomainData(string domainName, List<string> ignorePhrases)
        {
            DomainName = domainName;
            IgnorePhrases = ignorePhrases;
        }
    }
}