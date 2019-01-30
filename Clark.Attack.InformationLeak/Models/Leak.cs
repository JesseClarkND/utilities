using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clark.Attack.InformationLeak.Models
{
    public class Leak
    {
        //There will be multiple filenames that match the same fp
        public List<string> FileNames = new List<string>();
        public List<string> FingerPrints = new List<string>();
    }
}