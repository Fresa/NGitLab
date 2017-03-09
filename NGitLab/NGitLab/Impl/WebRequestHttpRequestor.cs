using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace NGitLab.Impl
{
    internal class WebRequestHttpRequestor : IHttpRequestor
    {
        private readonly API _root;
        private readonly MethodType _method; // Default to GET requests
        private object _data;

        public WebRequestHttpRequestor(API root, MethodType method)
        {
            _root = root;
            _method = method;
        }

        public IBasicHttpRequestor With(object data)
        {
            _data = data;
            return this;
        }

        public T To<T>(string tailAPIUrl)
        {
            var result = default(T);
            Stream(tailAPIUrl, (s, headers) => result = SimpleJson.DeserializeObject<T>(new StreamReader(s).ReadToEnd()));
            return result;
        }

        public void Stream(string tailAPIUrl, Action<Stream, IDictionary<string, IEnumerable<string>>> parser)
        {
            var req = SetupConnection(_root.GetAPIUrl(tailAPIUrl));

            if (HasOutput())
            {
                SubmitData(req);
            }
            else if (_method == MethodType.Put)
            {
                req.Headers.Add("Content-Length", "0");
            }

            try
            {
                using (var response = req.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        parser(stream, response.Headers.ToDictionary());
                    }
                }
            }
            catch (WebException wex)
            {
                if (wex.Response != null)
                {
                    using (var errorResponse = (HttpWebResponse)wex.Response)
                    {
                        using (var reader = new StreamReader(errorResponse.GetResponseStream()))
                        {
                            string jsonString = reader.ReadToEnd();
                            JsonError jsonError;
                            try
                            {
                                jsonError = SimpleJson.DeserializeObject<JsonError>(jsonString);
                            }
                            catch (Exception ex)
                            {
                                throw new Exception(string.Format("The remote server returned an error ({0}) with an empty response", errorResponse.StatusCode));
                            }
                            throw new Exception(string.Format("The remote server returned an error ({0}): {1}", errorResponse.StatusCode, jsonError.Message));
                        }
                    }
                }
                else
                    throw wex;
            }

        }

        public IEnumerable<T> GetAll<T>(string tailUrl)
        {
            return new Enumerable<T>(tailUrl, new WebRequestHttpRequestor(_root, MethodType.Get));
        }

        private void SubmitData(WebRequest request)
        {
            request.ContentType = "application/json";

            using (var writer = new StreamWriter(request.GetRequestStream()))
            {
                var data = SimpleJson.SerializeObject(_data);
                writer.Write(data);
                writer.Flush();
                writer.Close();
            }
        }

        private bool HasOutput()
        {
            return _method == MethodType.Post || _method == MethodType.Put && _data != null;
        }

        private WebRequest SetupConnection(Uri url)
        {
            return SetupConnection(url, _method);
        }

        private static WebRequest SetupConnection(Uri url, MethodType methodType)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = methodType.ToString().ToUpperInvariant();
            request.Headers.Add("Accept-Encoding", "gzip");
            request.AutomaticDecompression = DecompressionMethods.GZip;
            return request;
        }
    }
}
