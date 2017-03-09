using System;
using System.Diagnostics;

namespace NGitLab.Impl
{
    [DebuggerStepThrough]
    public class API
    {
        public readonly string APIToken;
        private readonly IHttpRequestorFactory _httpRequestorFactory;
        private readonly string _hostUrl;
        private const string APINamespace = "/api/v3";

        public API(string hostUrl, string apiToken, IHttpRequestorFactory httpRequestorFactory)
        {
            _hostUrl = hostUrl.EndsWith("/") ? hostUrl.Replace("/$", "") : hostUrl;
            APIToken = apiToken;
            _httpRequestorFactory = httpRequestorFactory;
        }
        
        public IHttpRequestor Get()
        {
            return _httpRequestorFactory.Create(this, MethodType.Get);
        }

        public IHttpRequestor Post()
        {
            return _httpRequestorFactory.Create(this, MethodType.Post);
        }

        public IHttpRequestor Put()
        {
            return _httpRequestorFactory.Create(this, MethodType.Put);
        }
        
        public IHttpRequestor Delete()
        {
            return _httpRequestorFactory.Create(this, MethodType.Delete);
        }

        public Uri GetAPIUrl(string tailAPIUrl)
        {
            if (APIToken != null)
            {
                tailAPIUrl = tailAPIUrl + (tailAPIUrl.IndexOf('?') > 0 ? '&' : '?') + "private_token=" + APIToken;
            }

            if (!tailAPIUrl.StartsWith("/"))
            {
                tailAPIUrl = "/" + tailAPIUrl;
            }
            return new Uri(_hostUrl + APINamespace + tailAPIUrl);
        }

        public Uri GetUrl(string tailAPIUrl)
        {
            if (!tailAPIUrl.StartsWith("/"))
            {
                tailAPIUrl = "/" + tailAPIUrl;
            }

            return new Uri(_hostUrl + tailAPIUrl);
        }
    }
}