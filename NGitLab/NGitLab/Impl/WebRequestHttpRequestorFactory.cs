namespace NGitLab.Impl
{
    public class WebRequestHttpRequestorFactory : IHttpRequestorFactory
    {
        public IHttpRequestor Create(API root, MethodType method)
        {
            return new WebRequestHttpRequestor(root, method);
        }
    }
}