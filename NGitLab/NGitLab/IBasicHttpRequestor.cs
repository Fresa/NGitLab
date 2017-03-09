using System;
using System.Collections.Generic;
using System.IO;

namespace NGitLab
{
    public interface IBasicHttpRequestor
    {
        IBasicHttpRequestor With(object data);
        T To<T>(string tailAPIUrl);
        void Stream(string tailAPIUrl, Action<Stream, IDictionary<string, IEnumerable<string>>> parser);
    }
}