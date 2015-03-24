using System;
using System.Web.Mvc;

namespace Swagger.Api
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
	public class ApiDocumentationAttribute : Attribute
	{
        internal string MethodName { get; set; }
		public string Url { get; internal set; }
        public string SummaryDescription { get; internal set; }
        public string Notes { get; internal set; }
        public Type ReturnType { get; internal set; }
        public HttpVerbs RequestType { get; internal set; }
        public Type FormBody { get; internal set; }

        internal ApiDocumentationAttribute() { }

        public ApiDocumentationAttribute(string summaryDescription)
        {
            Url = string.Empty;
            SummaryDescription = summaryDescription;
            Notes = summaryDescription;
            ReturnType = null;
            RequestType = HttpVerbs.Get;
            FormBody = null;
        }	

        public ApiDocumentationAttribute(string summaryDescription, string notes = "")
        {
            Url = string.Empty;
            SummaryDescription = summaryDescription;
            Notes = notes;
            ReturnType = null;
            RequestType = HttpVerbs.Get;
            FormBody = null;
        }

        public ApiDocumentationAttribute(string url, string summaryDescription = "", Type returnType = null, HttpVerbs requestType = HttpVerbs.Get, Type formBody = null)
        {
            Url = url;
            SummaryDescription = summaryDescription;
            Notes = summaryDescription;            
            ReturnType = returnType;
            RequestType = requestType;
            FormBody = formBody;
        }
        
		public ApiDocumentationAttribute(string url, string summaryDescription = "", string notes = "", Type returnType = null, HttpVerbs requestType = HttpVerbs.Get, Type formBody = null)
		{
			Url = url;
			SummaryDescription = summaryDescription;
		    Notes = notes;
			ReturnType = returnType;
			RequestType = requestType;
			FormBody = formBody;
		}		
	}
}