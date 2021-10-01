using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using AsanaNet.Extensions;
using AsanaNet.Objects;
using MiniJSON;
using Newtonsoft.Json;

namespace AsanaNet
{
    /// <summary>
    /// Wraps up the request
    /// </summary>
    internal class AsanaRequest
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        /// <param name="host"></param>
        public AsanaRequest(HttpWebRequest request, Asana host)
        {
            _request = request;
            _host = host;
        }

        #region Static variables

        /// <summary>
        /// Static because all Asana requests will have a common throttle
        /// </summary>
        private static bool _throttling = false;
        private static ManualResetEvent _throttlingWaitHandle = new ManualResetEvent(false);

        #endregion

        #region Variables

        /// <summary>
        /// Contains the actual request object
        /// </summary>
        private readonly HttpWebRequest _request;

        /// <summary>
        /// Link with the instance owner of the request
        /// </summary>
        private readonly Asana _host;

        /// <summary>
        /// The callback once the response is received.
        /// </summary>
        private Action<string, WebHeaderCollection> _callback;

        /// <summary>
        /// The error callback
        /// </summary>
        private Action<string, string, string, object> _error;

        private PageOffset _pageOffset;
        public PageOffset PageOffset => _pageOffset;

        #endregion

        #region Task communication pattern

        /// <summary>
        /// Begins the request
        /// </summary> <Task>
        public Task Go(Action<string, WebHeaderCollection> callback, Action<string, string, string, object> error)
        {
            _callback = callback;
            _error = error;

            if (_throttling)
                _throttlingWaitHandle.WaitOne();

            return Task.Factory.FromAsync<WebResponse>(
                    _request.BeginGetResponse,
                    _request.EndGetResponse,
                    null).ContinueWith((requestTask) =>
                   {
                       HttpWebRequest request = (HttpWebRequest)_request;
                       AsanaRequest state = (AsanaRequest)requestTask.AsyncState;

                       if (requestTask.IsFaulted)
                       {
                           _error(request.Address.AbsoluteUri, requestTask.Exception?.InnerException?.Message ?? "arg2", requestTask.Exception?.InnerException?.InnerException?.Message ?? "arg3", _request);
                           return;
                       }

                       WebResponse result = requestTask.Result;

                       if (result.Headers["Retry-After"] != null)
                       {
                           string retryAfter = result.Headers["Retry-After"];
                           ThrottleFor(Convert.ToInt32(retryAfter));
                           Go(callback, error);
                           return;
                       }
                       string responseContent = GetResponseContent(result);
                       _callback(responseContent, result.Headers);
                   }
            );
        }

        private static string GetResponseContent(WebResponse response)
        {
            Encoding enc = Encoding.GetEncoding(65001);
            var stream = new StreamReader(response.GetResponseStream(), enc);
            string output = stream.ReadToEnd();
            stream.Close();
            return output;
        }

        #endregion

        #region Async/Await communication pattern

        /// <summary>
        /// Begins the request
        /// </summary> <Task>
        public async Task<TAsanaObject> GoAsync<TAsanaObject>(bool respectOriginalStructure = false)
            where TAsanaObject : AsanaObject
        {
            if (_throttling)
                _throttlingWaitHandle.WaitOne();

            using (var response = await GetWebResponseAsync(_request))
            {
                if (response.Headers["Retry-After"] != null)
                {
                    var retryAfter = response.Headers["Retry-After"];
                    ThrottleFor(Convert.ToInt32(retryAfter));
                    return await GoAsync<TAsanaObject>();
                }

                using (var dataStream = response.GetResponseStream())
                using (var reader = new StreamReader(dataStream))
                {
                    var definition = new
                    {
                        data = new object(),
                        errors = new object()
                    };

                    var responseFromServer = await reader.ReadToEndAsync();
                    var responseObject = JsonConvert.DeserializeAnonymousType(responseFromServer, definition);

                    if (responseObject.errors != null)
                    {
                        throw new Exception(responseObject.errors?.ToString());
                    }


                    if (respectOriginalStructure)
                    {
                        var result = PackOriginalContent<TAsanaObject>(responseFromServer);
                        return result;
                    }
                    else
                    {
                        var t = typeof(TAsanaObject);
                        var result = PackAndSendResponse<TAsanaObject>(responseFromServer);
                        return result;
                    }
                }
            }
        }


        /// <summary>
        /// Begins the request
        /// </summary> <Task>
        public async Task<TAsanaObject> GoAsync<TAsanaObject>(TAsanaObject obj, bool respectOriginalStructure = false)
            where TAsanaObject : AsanaObject
        {
            if (_throttling)
                _throttlingWaitHandle.WaitOne();

            using (var response = await GetWebResponseAsync(_request))
            {
                if (response.Headers["Retry-After"] != null)
                {
                    var retryAfter = response.Headers["Retry-After"];
                    ThrottleFor(Convert.ToInt32(retryAfter));
                    return await GoAsync(obj);
                }

                using (var dataStream = response.GetResponseStream())
                using (var reader = new StreamReader(dataStream))
                {
                    var definition = new
                    {
                        data = new object(),
                        errors = new object()
                    };

                    var typeDefinition = new
                    {
                        data = new { resource_type = "" }
                    };

                    var responseFromServer = await reader.ReadToEndAsync();
                    var responseObject = JsonConvert.DeserializeAnonymousType(responseFromServer, definition);
                    var td = JsonConvert.DeserializeAnonymousType(responseFromServer, typeDefinition);
                    var resourceType = td?.data?.resource_type;

                    if (responseObject.errors != null)
                    {
                        throw new Exception(responseObject.errors?.ToString());
                    }


                    if (respectOriginalStructure)
                    {
                        var result = PackOriginalContent<TAsanaObject>(responseFromServer);
                        return result;
                    }
                    else
                    {
                        var t = typeof(TAsanaObject);
                        if (t.IsInstanceOfType(obj) && resourceType == "task")
                        {
                            var at = PackAndSendResponse(new AsanaTask(), responseFromServer);
                            return at as TAsanaObject;
                        }
                        if (t.IsInstanceOfType(obj) && resourceType == "section")
                        {
                            var at = PackAndSendResponse(new AsanaSection(), responseFromServer);
                            return at as TAsanaObject;
                        }

                        var result = PackAndSendResponse<TAsanaObject>(responseFromServer);
                        return result;
                    }
                }
            }
        }

        private async Task<WebResponse> GetWebResponseAsync(HttpWebRequest request)
        {
            try
            {
                return await request.GetResponseAsync();
            }
            catch (WebException e)
            {
                return e.Response;
            }
        }

        /// <summary>
        /// Begins the request
        /// </summary> <Task>
        public async Task<IAsanaObjectCollection<TAsanaObject>> GoCollectionAsync<TAsanaObject>(int maxRounds = 0)
            where TAsanaObject : AsanaObject
        {
            if (_throttling)
                _throttlingWaitHandle.WaitOne();

            IAsanaObjectCollection<TAsanaObject> resultCollection;

            using (var response = await _request.GetResponseAsync())
            {
                if (response.Headers["Retry-After"] != null)
                {
                    var retryAfter = response.Headers["Retry-After"];
                    ThrottleFor(Convert.ToInt32(retryAfter));
                    return await GoCollectionAsync<TAsanaObject>();
                }

                using (var dataStream = response.GetResponseStream())
                using (var reader = new StreamReader(dataStream))
                {
                    var responseFromServer = await reader.ReadToEndAsync();
                    var result = PackAndSendResponseCollection<TAsanaObject>(responseFromServer);

                    resultCollection = result.Item1;
                    _pageOffset = result.Item2;
                }
            }

            if (_pageOffset != null && maxRounds > 0)
            {
                var followUp = _host.GetFollowUpRequest(_pageOffset);
                var followUpResult = await followUp.GoCollectionAsync<TAsanaObject>(maxRounds - 1);

                if (followUpResult != null)
                {
                    foreach (var asanaObject in followUpResult)
                        resultCollection.Add(asanaObject);
                }
            }

            return resultCollection;
        }

        #region Json deserealize utils

        private T PackOriginalContent<T>(string rawData) where T : AsanaObject
        {
            var data = Json.Deserialize(rawData) as Dictionary<string, object>;
            var u = AsanaObject.Create(typeof(T));
            Parsing.Deserialize(data, u, _host);
            return (T)u;
        }

        /// <summary>
        /// Packs the data and into a response object and sends it to the callback
        /// </summary>
        /// <typeparam name="T"></typeparam>        
        private T PackAndSendResponse<T>(string rawData) where T : AsanaObject
        {
            var u = AsanaObject.Create(typeof(T));
            Parsing.Deserialize(GetDataAsDict(rawData), u, _host);
            return (T)u;
        }

        /// <summary>
        /// Packs the data and into a response object and sends it to the callback
        /// </summary>
        /// <typeparam name="T"></typeparam>        
        private T PackAndSendResponse<T>(T obj, string rawData) where T : AsanaObject
        {
            var u = AsanaObject.Create(typeof(T));
            Parsing.Deserialize(GetDataAsDict(rawData), obj, _host);
            return (T)obj;
        }

        /// <summary>
        /// Converts the raw string into dictionary format
        /// </summary>
        /// <param name="dataString"></param>
        /// <returns></returns>
        private static Dictionary<string, object> GetDataAsDict(string dataString)
        {
            var data = Json.Deserialize(dataString) as Dictionary<string, object>;
            var data2 = data["data"] as Dictionary<string, object>;
            return data2;
        }

        /// <summary>
        /// Packs the data and into a collection object and sends it to the callback
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private Tuple<IAsanaObjectCollection<T>, PageOffset> PackAndSendResponseCollection<T>(string rawData) where T : AsanaObject
        {
            var k = GetDataAsDictArray(rawData);
            var offset = GetPageOffset(rawData);
            AsanaObjectCollection<T> collection = new AsanaObjectCollection<T>();
            foreach (var j in k)
            {
                var t = (T)AsanaObject.Create(typeof(T));
                Parsing.Deserialize(j, t, _host);
                collection.Add(t);
            }

            return new Tuple<IAsanaObjectCollection<T>, PageOffset>(collection, offset);
        }

        /// <summary>
        /// Converts the raw string into list of dictionaries format
        /// </summary>
        /// <param name="dataString"></param>
        /// <returns></returns>
        private static Dictionary<string, object>[] GetDataAsDictArray(string dataString)
        {
            var data = Json.Deserialize(dataString) as Dictionary<string, object>;
            var data2 = data["data"] as List<object>;

            var data3 = new Dictionary<string, object>[data2.Count];
            for (int i = 0; i < data2.Count; ++i)
                data3[i] = data2[i] as Dictionary<string, object>;

            return data3;
        }


        /// <summary>
        /// Converts the raw string into PageOffset Object
        /// </summary>
        /// <param name="dataString"></param>
        /// <returns></returns>
        private static PageOffset GetPageOffset(string dataString)
        {
            var data = Json.Deserialize(dataString) as Dictionary<string, object>;

            const string nextPageKey = "next_page";
            if (data == null || !data.ContainsKey(nextPageKey))
                return null;

            var dataOffset = data[nextPageKey] as Dictionary<string, object>;
            if (dataOffset == null)
                return null;

            var offsetOk = dataOffset.TryGetValue("offset", out var offset);
            var pathOk = dataOffset.TryGetValue("path", out var path);
            var uriOk = dataOffset.TryGetValue("uri", out var uri);

            if (offsetOk && pathOk && uriOk)
                return new PageOffset(offset?.ToString(), path?.ToString(), uri?.ToString());

            return null;
        }


        #endregion

        #endregion

        #region Methods

        private static void ThrottleFor(int seconds)
        {
            _throttling = true;
            Timer timer = null;
            timer = new Timer(s =>
                {
                    _throttling = false;
                    _throttlingWaitHandle.Set();
                    _throttlingWaitHandle = new ManualResetEvent(false);
                    timer.Dispose();
                }, null, 1000 * seconds, Timeout.Infinite);
        }

        #endregion
    }
}
