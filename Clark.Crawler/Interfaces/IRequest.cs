using Clark.Crawler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clark.Crawler.Interfaces
{
    public interface IRequest 
    {
        string Url
        {
            get;
            set;
        }

        IResponse Response
        {
            get;
            set;
        }
    }
}