using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clark.Attack.ContentScanner.Models
{
    internal class Fingerprint
    {
        public string Name = "";
        public List<string> Fingerprints = new List<string>();
        public string Reference = "";
    }
}