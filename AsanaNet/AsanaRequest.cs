using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security.Authentication;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AsanaNet.Extensions;
using AsanaNet.Objects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
        /// <param name="payload"></param>
        public AsanaRequest(HttpWebRequest request, Asana host, string payload = "")
        {
            _request = request;
            _host = host;
            _payload = payload;
        }


        #region Static variables

        /// <summary>
        /// Static because all Asana requests will have a common throttle
        /// </summary>
        private static bool _throttling;
        private static ManualResetEvent _throttlingWaitHandle = new ManualResetEvent(false);
        private static readonly Encoding Encoding = Encoding.GetEncoding(65001);

        #endregion

        #region Variables

        /// <summary>
        /// Contains the actual request object
        /// </summary>
        private readonly HttpWebRequest _request;

        private readonly string _payload;
        private string _responseFromServer;
        public string LastPayload => _payload;

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
        private int _retries;
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

            return Task.Factory.FromAsync(
                    _request.BeginGetResponse,
                    _request.EndGetResponse,
                    null).ContinueWith(requestTask =>
                   {
                       HttpWebRequest request = _request;
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
                       var responseContent = GetResponseContent(result);
                       _host.LastResponse = _responseFromServer = responseContent;
                       _callback(responseContent, result.Headers);
                   }
            );
        }

        private static string GetResponseContent(WebResponse response)
        {
            var stream = new StreamReader(response.GetResponseStream(), Encoding);
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
                if (response == null)
                    return null;

                if (response.Headers["Retry-After"] != null)
                {
                    var retryAfter = response.Headers["Retry-After"];
                    ThrottleFor(Convert.ToInt32(retryAfter));
                    return await GoAsync<TAsanaObject>();
                }
                using (var dataStream = response.GetResponseStream())
                using (var reader = new StreamReader(dataStream, Encoding))
                {
                    var definition = new
                    {
                        data = new object(),
                        errors = new object()
                    };

                    var responseFromServer = await reader.ReadToEndAsync();
                    _host.LastResponse = _responseFromServer = responseFromServer;

                    var responseObject = JsonConvert.DeserializeAnonymousType(responseFromServer, definition);

                    if (responseObject.errors != null)
                    {
                        throw new Exception("PAYLOAD: " + _payload + " RESPONSE" + responseObject.errors);
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

            if (_retries > 3)
                throw new TimeoutException($"GoAsync() too many retries ${_retries}!  PAYLOAD: {_payload}");

            using var response = await GetWebResponseAsync(_request);
            if (response == null)
            {
                ThrottleFor(3);
                _retries++;
                return await GoAsync(obj, respectOriginalStructure);
            }

            if (response.Headers["Retry-After"] != null)
            {
                var retryAfter = response.Headers["Retry-After"];
                ThrottleFor(Convert.ToInt32(retryAfter));
                _retries++;
                return await GoAsync(obj, respectOriginalStructure);
            }

            await using var dataStream = response.GetResponseStream();
            if (dataStream == null)
                throw new NullReferenceException("Asana API response is null.");

            using var reader = new StreamReader(dataStream, Encoding);
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
            _host.LastResponse = _responseFromServer = responseFromServer;

            try
            {
                var jsonObj = JToken.Parse(responseFromServer);
            }
            catch (JsonReaderException jex)
            {
                throw new AsanaException(_request.RequestUri, _payload, $"GetWebResponseAsync, URL: {_request.RequestUri} PAYLOAD: {_payload} RESPONSE: {responseFromServer}", jex);
            }

            var responseObject = JsonConvert.DeserializeAnonymousType(responseFromServer, definition);
            var td = JsonConvert.DeserializeAnonymousType(responseFromServer, typeDefinition);
            var resourceType = td?.data?.resource_type;

            if (responseObject?.errors != null)
            {
                throw new AsanaException($"GetWebResponseAsync, URL: {_request.RequestUri} PAYLOAD: {_payload} RESPONSE Error: {responseObject.errors}");
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

        private async Task<WebResponse> GetWebResponseAsync(HttpWebRequest request)
        {
            try
            {
                return await request.GetResponseAsync();
            }
            catch (WebException e)
            {
                if (e.Response == null)
                    throw new AsanaException($"no return value from request: {e.Message}", e.InnerException);

                return e.Response;
            }
        }


        public async Task<IAsanaObjectCollection<TAsanaObject>> GoCollectionAsync<TAsanaObject>(int maxRounds = 0)
            where TAsanaObject : AsanaObject
        {
            if (_throttling)
                _throttlingWaitHandle.WaitOne();

            IAsanaObjectCollection<TAsanaObject> resultCollection = new AsanaObjectCollection<TAsanaObject>();

            try
            {
                using var response = await _request.GetResponseAsync();
                if (response.Headers["Retry-After"] != null)
                {
                    var retryAfter = response.Headers["Retry-After"];
                    ThrottleFor(Convert.ToInt32(retryAfter));
                    return await GoCollectionAsync<TAsanaObject>();
                }

                await using var dataStream = response.GetResponseStream();
                if (dataStream != null)
                {
                    using var reader = new StreamReader(dataStream);
                    _responseFromServer = await reader.ReadToEndAsync();
                    _host.LastResponse = _responseFromServer;
                    var result = PackAndSendResponseCollection<TAsanaObject>(_responseFromServer);

                    resultCollection = result.Item1;
                    _pageOffset = result.Item2;
                }
            }
            catch (Exception e)
            {
                throw new AsanaException(_responseFromServer, _request.RequestUri, _payload, "GoCollectionAsync failed", e);
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
            var data = MiniJSON.Json.Deserialize(rawData) as Dictionary<string, object>;
            var asanaObject = AsanaObject.Create(typeof(T));
            Parsing.Deserialize(data, asanaObject, _host);
            asanaObject?.SetPropertiesUnchanged();
            return (T)asanaObject;
        }

        /// <summary>
        /// Packs the data and into a response object and sends it to the callback
        /// </summary>
        /// <typeparam name="T"></typeparam>        
        private T PackAndSendResponse<T>(string rawData) where T : AsanaObject
        {
            var asanaObject = AsanaObject.Create(typeof(T));
            Parsing.Deserialize(GetDataAsDict(rawData), asanaObject, _host);
            asanaObject?.SetPropertiesUnchanged();
            return (T)asanaObject;
        }

        /// <summary>
        /// Packs the data and into a response object and sends it to the callback
        /// </summary>
        /// <typeparam name="T"></typeparam>        
        private T PackAndSendResponse<T>(T obj, string rawData) where T : AsanaObject
        {
            var u = AsanaObject.Create(typeof(T));
            Parsing.Deserialize(GetDataAsDict(rawData), obj, _host);

            return obj;
        }

        /// <summary>
        /// Converts the raw string into dictionary format
        /// </summary>
        /// <param name="dataString"></param>
        /// <returns></returns>
        private static Dictionary<string, object> GetDataAsDict(string dataString)
        {
            var data = MiniJSON.Json.Deserialize(dataString) as Dictionary<string, object>;
            var innerData = data["data"] as Dictionary<string, object>;
            return innerData;
        }

        /// <summary>
        /// Packs the data and into a collection object and sends it to the callback
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private Tuple<IAsanaObjectCollection<T>, PageOffset> PackAndSendResponseCollection<T>(string rawData) where T : AsanaObject
        {
            var dataDictionary = GetDataAsDictArray(rawData);
            var offset = GetPageOffset(rawData);
            var collection = new AsanaObjectCollection<T>();
            if (dataDictionary != null)
            {
                foreach (var item in dataDictionary)
                {
                    var asanaObject = (T)AsanaObject.Create(typeof(T));
                    Parsing.Deserialize(item, asanaObject, _host);

                    asanaObject?.SetPropertiesUnchanged();
                    collection.Add(asanaObject);
                }
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
            var dataObject = MiniJSON.Json.Deserialize(dataString) as Dictionary<string, object>;
            if (!(dataObject?["data"] is List<object> innerData))
                return null;

            var returnData = new Dictionary<string, object>[innerData.Count];
            for (var i = 0; i < innerData.Count; ++i)
                returnData[i] = innerData[i] as Dictionary<string, object>;

            return returnData;
        }


        /// <summary>
        /// Converts the raw string into PageOffset Object
        /// </summary>
        /// <param name="dataString"></param>
        /// <returns></returns>
        private static PageOffset GetPageOffset(string dataString)
        {
            var data = MiniJSON.Json.Deserialize(dataString) as Dictionary<string, object>;

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

        #region Factory Builder

        /// <summary>
        /// Creates a base request object with authorization data. 
        /// </summary>
        /// <param name="append">The string we want to append to the request</param>
        /// <returns></returns>
        public static AsanaRequest GetBaseRequest(Asana host, AsanaFunction function, params object[] obj)
        {
            string url = host.BaseUrl + string.Format(new PropertyFormatProvider(), function.Url, obj);

            var request = GetBaseHttpWebRequest(host, function, url);

            return new AsanaRequest(request, host);
        }

        private static HttpWebRequest GetBaseHttpWebRequest(Asana host, AsanaFunction function, string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            if (host.AuthType == AuthenticationType.Basic)
            {
                if (string.IsNullOrWhiteSpace(host.EncodedAPIKey))
                    throw new AuthenticationException("Asana EncodedAPIKey is empty!");
                request.Headers["Authorization"] = "Basic " + host.EncodedAPIKey;
            }
            else if (host.AuthType == AuthenticationType.OAuth)
            {
                if (string.IsNullOrWhiteSpace(host.OAuthToken))
                    throw new AuthenticationException("Asana OAuthToken is empty!");
                request.Headers["Authorization"] = "Bearer " + host.OAuthToken;
            }
            request.Method = function.Method;
            request.UserAgent = "AsanaNet (github.com/acron0/AsanaNet)";
            return request;
        }

        /// <summary>
        /// Creates a base request object with authorization data AND POST data.
        /// </summary>
        /// <param name="args">uri arguments</param>
        /// <returns></returns>
        public static AsanaRequest GetBaseRequestWithParams(Asana host, AsanaFunction function, Dictionary<string, object> args, params object[] obj)
        {
            var url = host.BaseUrl + string.Format(new PropertyFormatProvider(), function.Url, obj);

            if (args != null && args.Count > 0)
            {
                url += "?";
                foreach (var kv in args)
                    url += kv.Key + "=" + Uri.EscapeUriString(kv.Value.ToString()) + "&";
                url = url.TrimEnd('&');
            }

            var request = GetBaseHttpWebRequest(host, function, url);

            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = 0;

            return new AsanaRequest(request, host);
        }


        /// <summary>
        /// Creates a base request object with authorization data AND POST data.
        /// </summary>
        /// <param name="pageOffset">Object contains the Followup Path</param>
        /// <returns></returns>
        public static AsanaRequest GetFollowUpRequest(Asana host, PageOffset pageOffset)
        {
            string url = pageOffset.Uri;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            if (host.AuthType == AuthenticationType.Basic)
            {
                if (string.IsNullOrWhiteSpace(host.EncodedAPIKey))
                    throw new AuthenticationException("Asana EncodedAPIKey is empty!");
                request.Headers["Authorization"] = "Basic " + host.EncodedAPIKey;
            }
            else if (host.AuthType == AuthenticationType.OAuth)
            {
                if (string.IsNullOrWhiteSpace(host.OAuthToken))
                    throw new AuthenticationException("Asana OAuthToken is empty!");
                request.Headers["Authorization"] = "Bearer " + host.OAuthToken;
            }
            request.Method = "GET";

            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = 0;

            return new AsanaRequest(request, host);
        }


        /// <summary>
        /// Creates a base request object with authorization data AND POST data.
        /// </summary>
        /// <param name="args">uri arguments</param>
        /// <returns></returns>
        public static AsanaRequest GetBaseRequestWithParamsJson(Asana host, AsanaFunction function, Dictionary<string, object> args, params object[] obj)
        {
            var url = host.BaseUrl + string.Format(new PropertyFormatProvider(), function.Url, obj);
            var jsonString = string.Empty;

            var request = GetBaseHttpWebRequest(host, function, url);

            request.SendChunked = false;
            request.AllowWriteStreamBuffering = false;

            if (args != null && args.Count > 0)
            {
                var json = JsonConvert.SerializeObject(args);
                var fixedJsonString = FixedArrayJsonString(json);

                jsonString = "{\"data\": " + fixedJsonString + "}";
                var cleanString = RemoveDiacritics(jsonString);

                request.Accept = "Accept=application/json";
                request.ContentType = "application/json; charset=utf-8";
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    var postBytes = Encoding.UTF8.GetBytes(cleanString);
                    streamWriter.Write(cleanString);
                    request.ContentLength = cleanString.Length;
                }
            }
            else
            {
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = 0;
            }

            return new AsanaRequest(request, host, jsonString);
        }


        private static string FixedArrayJsonString(string json)
        {
            var rootToken = JToken.Parse(json);
            rootToken.FixElementArrays();
            var fixedJsonString = rootToken.ToString();
            return fixedJsonString;
        }

        private static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }


        #endregion

    }
}
