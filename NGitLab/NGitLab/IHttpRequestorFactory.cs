using NGitLab.Impl;

namespace NGitLab
{
    public interface IHttpRequestorFactory
    {
        IHttpRequestor Create(API root, MethodType method);
    }
}