using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clark.Attack.Dependency
{
    /// <summary>
    /// Find all 3rd party URLs on a page and check that they resolve
    /// It is possible to hijack abandoned buckets and 404'd domains.
    /// </summary>
    public class Processor
    {
        public string Name { get { return "Dependency Vulnerability"; } set { } }
    }
}
