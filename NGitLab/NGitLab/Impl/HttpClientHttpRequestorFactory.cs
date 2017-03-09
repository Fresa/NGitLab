using System.Net;
using System.Net.Http;

namespace NGitLab.Impl
{
    public class HttpClientHttpRequestorFactory : IHttpRequestorFactory
    {
        private readonly HttpMessageHandler _httpMessageHandler;

        public HttpClientHttpRequestorFactory(HttpMessageHandler httpMessageHandler)
        {
            _httpMessageHandler = httpMessageHandler;
        }

        public IHttpRequestor Create(API root, MethodType method)
        {
            return new HttpClientHttpRequestor(root, method, _httpMessageHandler);
        }
    }
}