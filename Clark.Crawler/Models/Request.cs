using Clark.Crawler.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clark.Crawler.Models
{
    public class Request : IEquatable<Request>, IRequest
    {
        private string _url = "";
        private IResponse _response;

        public Request()
        { }

        public Request(string url)
        {
            _url = url;
            _response = new Response();
        }

        public Request(Uri uri)
        {
            _url = uri.ToString();
            _response = new Response();
        }

        public string Url
        {
            get
            {
                return _url;
            }
            set
            {
                _url = value;
            }
        }

        public IResponse Response
        {
            get
            {
                return _response;
            }
            set
            {
                _response = value;
            }
        }

        #region IEquatable
        //public bool Equals(Request other)
        //{
        //    // Would still want to check for null etc. first.
        //    return this.Url == other.Url;
        //}

        public bool Equals(Request other)
        {
            return null != other && this.Url == other.Url;
        }
        public override bool Equals(object obj)
        {
            return Equals(obj as Request);
        }
        public override int GetHashCode()
        {
            return this.Url.GetHashCode();
        }

        #endregion
    }
}
