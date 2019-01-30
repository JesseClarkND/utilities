using Clark.Attack.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clark.Attack.Common.Interfaces
{
    public interface IAttack
    {
        public string Name;
        public AttackResult Check(AttackRequest request);

       // private void Initilize();
    }
}