using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using AsanaNet.Extensions;
using AsanaNet.Interfaces;
using AsanaNet.Objects;
using Newtonsoft.Json.Linq;

namespace AsanaNet
{
    public delegate void AsanaResponseEventHandler(AsanaObject response);
    public delegate void AsanaCollectionResponseEventHandler(IAsanaObjectCollection response);

    public enum AuthenticationType
    {
        Basic,
        OAuth
    }

    [Serializable]
    public partial class Asana : IAsana
    {
        #region Variables

        public AsanaFunction AsanaFunction = new AsanaFunction();


        /// <summary>
        /// The URL we use to prefix all the requests
        /// e.g. https://app.asana.com/api/1.0
        /// </summary>
        public string BaseUrl;

        /// <summary>
        /// An error callback for the outside world
        /// </summary>
        private Action<string, string, string, object> _errorCallback;

        #endregion

        #region Properties
        /// <summary>
        /// The Authentication Type used for API access
        /// </summary>
        public AuthenticationType AuthType { get; private set; }

        /// <summary>
        /// The API Key assigned object
        /// </summary>
        public string APIKey { get; private set; }

        /// <summary>
        /// The API Key, but base-64 encoded
        /// </summary>
        public string EncodedAPIKey { get; private set; }

        /// <summary>
        /// The OAuth Bearer Token assigned object
        /// </summary>
        public string OAuthToken { get; set; }

        public int GoCollectionMaxRecursiveCount { get; set; }

        public Asana GetHost()
        {
            return this;
        }

        public string LastPayload { get; private set; }
        public string LastResponse { get; set; }

        #endregion        

        #region Methods

        public Asana(IAsanaOptions config)
        {
            //_asanaFunction = new AsanaFunction();
            BaseUrl = "https://app.asana.com/api/1.0";
            _errorCallback = config.ErrorCallback;

            AuthType = config.AuthType;
            if (AuthType == AuthenticationType.OAuth)
            {
                OAuthToken = config.ApiKeyOrBearerToken;
            }
            else
            {
                SetApiKey(config.ApiKeyOrBearerToken);
            }

            GoCollectionMaxRecursiveCount = config.GoCollectionMaxRecursiveCount;

            AsanaFunction.InitFunctions();
        }

        public void SetApiKey(string key)
        {
            APIKey = key;
            EncodedAPIKey = Convert.ToBase64String(Encoding.ASCII.GetBytes(key + ":"));
            //_restClient = new RestClient(EncodedAPIKey);
        }

        /// <summary>
        /// Creates a new Asana entry point.
        /// </summary>
        /// <param name="apiKeyOrBearerToken">The API key (for Basic authentication) or Bearer Token (for OAuth authentication) for the account we intend to access</param>
        public Asana(string apiKeyOrBearerToken, AuthenticationType authType, Action<string, string, string, object> errorCallback)
        {
            BaseUrl = "https://app.asana.com/api/1.0";
            _errorCallback = errorCallback;

            AuthType = authType;
            if (AuthType == AuthenticationType.OAuth)
            {
                OAuthToken = apiKeyOrBearerToken;
            }
            else
            {
                SetApiKey(apiKeyOrBearerToken);
            }

            AsanaFunction.InitFunctions();
        }



        #region REQUESTS


        /// <summary>
        /// Creates a base request object with authorization data. 
        /// </summary>
        /// <param name="append">The string we want to append to the request</param>
        /// <returns></returns>
        private AsanaRequest GetBaseRequest(AsanaFunction function, params object[] obj)
        {
            return AsanaRequest.GetBaseRequest(this, function, obj);
        }

        /// <summary>
        /// Creates a base request object with authorization data AND POST data.
        /// </summary>
        /// <param name="args">uri arguments</param>
        /// <returns></returns>
        private AsanaRequest GetBaseRequestWithParams(AsanaFunction function, Dictionary<string, object> args, params object[] obj)
        {
            return AsanaRequest.GetBaseRequestWithParams(this, function, args , obj);
        }


        /// <summary>
        /// Creates a base request object with authorization data AND POST data.
        /// </summary>
        /// <param name="pageOffset">Object contains the Followup Path</param>
        /// <returns></returns>
        internal AsanaRequest GetFollowUpRequest(PageOffset pageOffset)
        {
            return AsanaRequest.GetFollowUpRequest(this, pageOffset);
        }


        /// <summary>
        /// Creates a base request object with authorization data AND POST data.
        /// </summary>
        /// <param name="args">uri arguments</param>
        /// <returns></returns>
        private AsanaRequest GetBaseRequestWithParamsJson(AsanaFunction function, Dictionary<string, object> args, params object[] obj)
        {
            return AsanaRequest.GetBaseRequestWithParamsJson(this, function, args, obj);
        }

        #endregion


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


        /// <summary>
        /// Converts the raw string into dictionary format
        /// </summary>
        /// <param name="dataString"></param>
        /// <returns></returns>
        private Dictionary<string, object> GetDataAsDict(string dataString)
        {
            var data = MiniJSON.Json.Deserialize(dataString) as Dictionary<string, object>;
            var data2 = data["data"] as Dictionary<string, object>;
            return data2;
        }

        /// <summary>
        /// Converts the raw string into list of dictionaries format
        /// </summary>
        /// <param name="dataString"></param>
        /// <returns></returns>
        private Dictionary<string, object>[] GetDataAsDictArray(string dataString)
        {
            var data = MiniJSON.Json.Deserialize(dataString) as Dictionary<string, object>;
            var data2 = data["data"] as List<object>;
            var data3 = new Dictionary<string, object>[data2.Count];
            for (int i = 0; i < data2.Count; ++i)
                data3[i] = data2[i] as Dictionary<string, object>;
            return data3;
        }

        /// <summary>
        /// The callback for response errors
        /// </summary>
        /// <param name="error"></param>
        internal void ErrorCallback(string requestString, string error, string responseContent, object response)
        {
            Debug.WriteLine($"error: {error},   request: {requestString}, response: {responseContent}");
            _errorCallback(requestString, error, responseContent, response);
        }

        /// <summary>
        /// Packs the data and into a collection object and sends it to the callback
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="callback"></param>
        internal void PackAndSendResponseCollection<T>(string rawData, AsanaCollectionResponseEventHandler callback) where T : AsanaObject
        {
            var k = GetDataAsDictArray(rawData);
            AsanaObjectCollection collection = new AsanaObjectCollection();
            foreach (var j in k)
            {
                var t = AsanaObject.Create(typeof(T));
                Parsing.Deserialize(j, t, this);
                collection.Add(t);
            }
            callback(collection);
        }

        /// <summary>
        /// Packs the data and into a response object and sends it to the callback
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="callback"></param>
        internal void PackAndSendResponse<T>(string rawData, AsanaResponseEventHandler callback) where T : AsanaObject
        {
            var u = AsanaObject.Create(typeof(T));
            Parsing.Deserialize(GetDataAsDict(rawData), u, this);
            callback(u);
        }

        /// <summary>
        /// Repacks data into an existing object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rawData"></param>
        /// <param name="?"></param>
        internal void RepackAndCallback<T>(string rawData, T obj) where T : AsanaObject
        {
            Parsing.Deserialize(GetDataAsDict(rawData), obj, this);

            obj.SavingCallback(Parsing.Serialize(obj, false, false));
            obj.SavedCallback();
        }

        /// <summary>
        /// Tells the asana object to save the specified object
        /// </summary>
        /// <param name="obj"></param>
        internal Task Save<T>(T obj, AsanaFunction func, Dictionary<string, object> data = null) where T : AsanaObject
        {
            if (!(obj is IAsanaData idata))
                throw new NullReferenceException("All AsanaObjects must implement IAsanaData in order to Save themselves.");

            if (data == null)
                data = Parsing.Serialize(obj, true, !idata.IsObjectLocal, true);
            AsanaRequest request = null;
            AsanaFunctionAssociation afa = AsanaFunction.GetFunctionAssociation(obj.GetType());

            if (func == null)
                func = idata.IsObjectLocal ? afa.Create : afa.Update;

            request = GetBaseRequestWithParamsJson(func, data, obj);
            return request.Go((o, h) => RepackAndCallback(o, obj), ErrorCallback);
        }

        /// <summary>
        /// Tells the asana object to save the specified object
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="func"></param>
        /// <param name="data"></param>
        internal Task<T> SaveAsync<T>(T obj, AsanaFunction func, Dictionary<string, object> data = null) where T : AsanaObject
        {
            var asanaData = (IAsanaData)obj;
            if (asanaData == null)
                throw new NullReferenceException("All AsanaObjects must implement IAsanaData in order to Save themselves.");

            data ??= Parsing.Serialize(obj, true, !asanaData.IsObjectLocal, true);

            var afa = AsanaFunction.GetFunctionAssociation(obj.GetType());

            func ??= asanaData.IsObjectLocal ? afa.Create : afa.Update;

            var request = GetBaseRequestWithParamsJson(func, data, obj);
            LastPayload = request.LastPayload;

            return request.GoAsync(obj);
        }

        /// <summary>
        /// Tells the asana object to save the specified object
        /// </summary>
        /// <param name="obj"></param>
        internal Task<IAsanaObjectCollection<TReturn>> SaveCollectionAsync<T, TReturn>(T obj, AsanaFunction func, Dictionary<string, object> data) where T : AsanaObject where TReturn : AsanaObject
        {
            if (func == null)
                throw new NullReferenceException("SaveCollectionAsync needs parameter func.");
            if (data == null)
                throw new NullReferenceException("SaveCollectionAsync needs parameter data.");

            var request = GetBaseRequestWithParamsJson(func, data, obj);
            return request.GoCollectionAsync<TReturn>();
        }

        /// <summary>
        /// Tells asana to delete the specified object
        /// </summary>
        /// <param name="obj"></param>
        internal Task Delete<T>(T obj) where T : AsanaObject
        {
            AsanaFunction func;

            if (!(obj is IAsanaData asanaData))
                throw new NullReferenceException("All AsanaObjects must implement IAsanaData in order to Delete themselves.");

            AsanaRequest request = null;
            var afa = AsanaFunction.GetFunctionAssociation(obj.GetType());

            if (asanaData.IsObjectLocal == false)
                func = afa.Delete;
            else
                throw new Exception("Object is local, cannot delete.");

            if (ReferenceEquals(func, null)) 
                throw new NotImplementedException("This object cannot delete itself.");

            request = GetBaseRequest(func, obj);
            return request.Go((o, h) => RepackAndCallback(o, obj), ErrorCallback);
        }

        /// <summary>
        /// Tells asana to delete the specified object
        /// </summary>
        /// <param name="obj"></param>
        internal Task<T> DeleteAsync<T>(T obj) where T : AsanaObject
        {
            AsanaFunction func;

            IAsanaData idata = obj as IAsanaData;
            if (idata == null)
                throw new NullReferenceException("All AsanaObjects must implement IAsanaData in order to Delete themselves.");

            AsanaRequest request = null;
            AsanaFunctionAssociation afa = AsanaFunction.GetFunctionAssociation(obj.GetType());

            if (idata.IsObjectLocal == false)
                func = afa.Delete;
            else
                throw new Exception("Object is local, cannot delete.");

            if (ReferenceEquals(func, null)) throw new NotImplementedException("This object cannot delete itself.");

            request = GetBaseRequest(func, obj);
            return request.GoAsync(obj);
        }



        #endregion
    }
}
