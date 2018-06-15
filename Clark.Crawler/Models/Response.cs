using Clark.Crawler.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clark.Crawler.Models
{
    public class Response : IResponse
    {
        private bool _error = false;
        private string _errorMessage = "";
        private string _code = "";
        private string _body = "";

        public bool Error
        {
            get
            {
                return _error;
            }
            set
            {
                _error = Error;
            }
        }

        public string ErrorMessage
        {
            get
            {
                return _errorMessage;
            }
            set
            {
                _errorMessage = value;
            }
        }

        public string Code
        {
            get
            {
                return _code;
            }
            set
            {
                _code = value;
            }
        }

        public string Body
        {
            get
            {
                return _body;
            }
            set
            {
                _body = value;
            }
        }
    }
}
