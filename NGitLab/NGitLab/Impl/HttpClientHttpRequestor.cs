using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

namespace NGitLab.Impl
{
    internal class HttpClientHttpRequestor : IHttpRequestor
    {
        private readonly API _root;
        private readonly MethodType _method;
        private object _data;
        private static HttpMessageHandler _httpMessageHandler;

        internal HttpClientHttpRequestor(API root, MethodType method, HttpMessageHandler httpMessageHandler)
        {
            _root = root;
            _method = method;
            _httpMessageHandler = httpMessageHandler;
        }

        public IBasicHttpRequestor With(object data)
        {
            _data = data;
            return this;
        }

        public T To<T>(string tailApiUrl)
        {
            var result = default(T);
            Stream(tailApiUrl, (s, headers) => result = SimpleJson.DeserializeObject<T>(new StreamReader(s).ReadToEnd()));
            return result;
        }

        public void Stream(string tailApiUrl, Action<Stream, IDictionary<string, IEnumerable<string>>> parser)
        {
            var client = CreateClient();

            var message = new HttpRequestMessage(new HttpMethod(_method.ToString().ToUpperInvariant()), _root.GetAPIUrl(tailApiUrl));
            if (HasOutput())
            {
                message.Content = new StringContent(SimpleJson.SerializeObject(_data))
                {
                    Headers =
                    {
                        ContentType = new MediaTypeHeaderValue("application/json")
                    }
                };
            }

            var response = client.SendAsync(message).Result;
            
            using (var content = Read(response))
            {
                if (response.IsSuccessStatusCode == false)
                {
                    using (var reader = new StreamReader(content))
                    {
                        var error = reader.ReadToEnd();
                        if (string.IsNullOrEmpty(error))
                        {
                            throw new Exception(string.Format("The remote server returned an error ({0}) with an empty response", response.StatusCode));
                        }
                        throw new Exception(string.Format("The remote server returned an error ({0}): {1}", response.StatusCode, SimpleJson.DeserializeObject<JsonError>(error).Message));
                    }
                }

                parser(content, response.Headers.ToDictionary(pair => pair.Key, pair => pair.Value));
            }
        }

        public IEnumerable<T> GetAll<T>(string tailUrl)
        {
            return new Enumerable<T>(tailUrl, new HttpClientHttpRequestor(_root, MethodType.Get, _httpMessageHandler));
        }

        private bool HasOutput()
        {
            return _method == MethodType.Post || _method == MethodType.Put && _data != null;
        }

        private static Stream Read(HttpResponseMessage response)
        {
            if (response.Content.Headers.ContentEncoding.Any(x => x == "gzip"))
            {
                using (var s = response.Content.ReadAsStreamAsync().Result)
                {
                    using (var decompressed = new GZipStream(s, CompressionMode.Decompress))
                    {
                        return decompressed;
                    }
                }
            }

            return response.Content.ReadAsStreamAsync().Result;
        }

        private static HttpClient CreateClient()
        {
            var client = new HttpClient(_httpMessageHandler);
            client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            return client;
        }
    }
}