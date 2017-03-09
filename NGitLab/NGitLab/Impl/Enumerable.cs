using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NGitLab.Impl
{
    internal class Enumerable<T> : IEnumerable<T>
    {
        private readonly string _tailApiUrl;
        private readonly IBasicHttpRequestor _httpRequestor;

        public Enumerable(string tailApiUrl, IBasicHttpRequestor httpRequestor)
        {
            _tailApiUrl = tailApiUrl;
            _httpRequestor = httpRequestor;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator(_tailApiUrl, _httpRequestor);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private class Enumerator : IEnumerator<T>
        {
            private string _nextUrlToLoad;
            private readonly IBasicHttpRequestor _httpRequestor;
            private readonly List<T> _buffer = new List<T>();

            public Enumerator(string tailApiUrl, IBasicHttpRequestor httpRequestor)
            {
                _nextUrlToLoad = tailApiUrl;
                _httpRequestor = httpRequestor;
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (_buffer.Count == 0)
                {
                    if (_nextUrlToLoad == null)
                    {
                        return false;
                    }

                    _httpRequestor.Stream(_nextUrlToLoad, (stream, headers) =>
                    {
                        // <http://localhost:1080/api/v3/projects?page=2&per_page=0>; rel="next", <http://localhost:1080/api/v3/projects?page=1&per_page=0>; rel="first", <http://localhost:1080/api/v3/projects?page=2&per_page=0>; rel="last"
                        string link = null;
                        IEnumerable<string> links;
                        if (headers.TryGetValue("Link", out links))
                        {
                            link = links.FirstOrDefault();
                        }

                        string[] nextLink = null;
                        if (string.IsNullOrEmpty(link) == false)
                            nextLink = link.Split(',')
                                .Select(l => l.Split(';'))
                                .FirstOrDefault(pair => pair[1].Contains("next"));

                        if (nextLink != null)
                        {
                            _nextUrlToLoad = new Uri(nextLink[0].Trim('<', '>', ' ')).PathAndQuery;
                        }
                        else
                        {
                            _nextUrlToLoad = null;
                        }

                        _buffer.AddRange(SimpleJson.DeserializeObject<T[]>(new StreamReader(stream).ReadToEnd()));
                    });

                    return _buffer.Count > 0;
                }

                if (_buffer.Count > 0)
                {
                    _buffer.RemoveAt(0);
                    return (_buffer.Count > 0) ? true : MoveNext();
                }

                return false;
            }
            
            public void Reset()
            {
                throw new NotImplementedException();
            }

            public T Current
            {
                get
                {
                    return _buffer[0];
                }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }
        }
    }
}