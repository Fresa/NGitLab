using System.Collections.Generic;

namespace NGitLab
{
    public interface IHttpRequestor : IBasicHttpRequestor
    {
        IEnumerable<T> GetAll<T>(string tailUrl);
    }
}