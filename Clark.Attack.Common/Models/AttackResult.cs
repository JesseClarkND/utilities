using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clark.Attack.Common.Models
{
    public class AttackResult
    {
        public bool Success = false;
       // public List<string> Results = new List<string>();
        public ConcurrentQueue<string> Results = new ConcurrentQueue<string>();
    }
}