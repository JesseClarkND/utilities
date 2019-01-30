using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clark.Attack.VulnerableFiles.Models
{
    internal class VulnerableFile
    {
        public string File = "";
        public List<string> Attacks = new List<string>();
        public List<string> FingerPrint = new List<string>();
    }
}