using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clark.Attack.FileUpload.Models
{
    public class FileUploadAttack
    {
        public string Name = "";
        public string HTTPVerb = "POST";
        public string FilePath = "";
        public string Body = "";
        public List<int> SuccessResponseHTTPCode = new List<int>();
        public List<string> SuccessReponseBody = new List<string>();
    }
}