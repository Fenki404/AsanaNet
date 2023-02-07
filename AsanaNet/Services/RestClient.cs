using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using AsanaNet.Services.Interfaces;
using Newtonsoft.Json;

namespace AsanaNet.Services
{
    public class RestClient : HttpClient, IRestClient
    {
        private readonly string _apiKey;
        private string _clientId;
        private string _clientSecret;
        private readonly string _baseUri;
        private readonly string _tokenUri;
        private readonly bool _authenticationApiKey = true;

        private readonly object _lockObject = new object();
        private DateTime _timeToRefreshToken;

        public RestClient(string apiKey)
        {
            //_clientId = _config.ClientId;
            //_clientSecret = _config.ClientSecret;
            //_baseUri = _config.BaseUri;
            //_tokenUri = _config.TokenUri;

            _apiKey = apiKey;
            if (!string.IsNullOrWhiteSpace(apiKey))
                _timeToRefreshToken = DateTime.MaxValue;

            InitializeTlsProtocol();
        }

        public RestClient(string clientId, string clientSecret, string baseUri, string tokenUri)
        {
            _clientId = string.IsNullOrEmpty(clientId) ? throw new ArgumentNullException(nameof(clientId)) : clientId;
            _clientSecret = string.IsNullOrEmpty(clientSecret) ? throw new ArgumentNullException(nameof(clientSecret)) : clientSecret;
            _baseUri = string.IsNullOrEmpty(baseUri) ? throw new ArgumentNullException(nameof(baseUri)) : baseUri;
            _tokenUri = string.IsNullOrEmpty(tokenUri) ? throw new ArgumentNullException(nameof(tokenUri)) : tokenUri;

            InitializeTlsProtocol();
        }

        private void InitializeTlsProtocol()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
        }


        public void Login(string clientId, string clientSecret)
        {
            _clientId = string.IsNullOrEmpty(clientId) ? throw new ArgumentNullException(nameof(clientId)) : clientId;
            _clientSecret = string.IsNullOrEmpty(clientSecret) ? throw new ArgumentNullException(nameof(clientSecret)) : clientSecret;
        }


        public async Task<T> GetAsync<T>(string url)
        {
            var emptyQuery = new Dictionary<string, string>();
            return await GetAsync<T>(url, emptyQuery);
        }

        public async Task<T> GetAsync<T>(string url, Dictionary<string, string> query)
        {
            var token = GetToken();

            DefaultRequestHeaders.Clear();
            DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };
            DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(_authenticationApiKey ? "Token" : "Bearer", token);
            DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var result = await GetRequestAsync<T>(_baseUri + url, query);
            return result;
        }


        public async Task<IEnumerable<T>> GetListAsync<T>(string url)
        {
            var emptyQuery = new Dictionary<string, string>();
            return await GetListAsync<T>(url, emptyQuery);
        }

        public async Task<IEnumerable<T>> GetListAsync<T>(string url, Dictionary<string, string> query)
        {
            var token = GetToken();

            DefaultRequestHeaders.Clear();
            DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };
            DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(_authenticationApiKey ? "Token" : "Bearer", token);
            DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                var result = await GetListRequestAsync<T>(_baseUri + url, query);
                return result;
            }
            catch (Exception e)
            {
                //_logger?.LogError(LoggingEvents.REST_CLIENT_PUT_CALL, e, "An error occurred in the RestClient");
                throw e;
            }
        }


        private async Task<T> GetRequestAsync<T>(string url, IDictionary<string, string> query = null)
        {
            var emptyQuery = new Dictionary<string, string>();
            using var response = await GetAsync(Utils.AddQueryString(url, query ?? emptyQuery));
            if (response.IsSuccessStatusCode)
            {
                #region RecursivePageination

                var headers = response.Headers;
                var next = GetMocoNextPageLink(response);

                #endregion

                var stringResponse = await response.Content.ReadAsStringAsync();
                var dto = JsonConvert.DeserializeObject<T>(stringResponse);
                return dto;
            }

            throw new HttpRequestException($"{response.StatusCode}: {response.ReasonPhrase}");
        }

        private async Task<IEnumerable<T>> GetListRequestAsync<T>(string url, IDictionary<string, string> query = null)
        {
            var emptyQuery = new Dictionary<string, string>();
            using var response = await GetAsync(Utils.AddQueryString(url, query ?? emptyQuery));
            if (response.IsSuccessStatusCode)
            {
                #region RecursivePageination

                var headers = response.Headers;
                var next = GetMocoNextPageLink(response);

                #endregion

                var stringResponse = await response.Content.ReadAsStringAsync();
                var dto = JsonConvert.DeserializeObject<IEnumerable<T>>(stringResponse);

                var nextUrl = next?.NextUri?.AbsoluteUri;
                if (!string.IsNullOrWhiteSpace(nextUrl))
                {
                    var nextPageResults = await GetListRequestAsync<T>(nextUrl);
                    dto = dto?.Concat(nextPageResults);
                }

                return dto;
            }

            throw new HttpRequestException($"{response.StatusCode}: {response.ReasonPhrase}");
        }


        private MocoPagination GetMocoNextPageLink(HttpResponseMessage message)
        {
            if (message == null) 
                return null;

            var containsPages = message.Headers.Contains("X-Page");
            containsPages = containsPages && message.Headers.Contains("X-Per-Page");
            containsPages = containsPages && message.Headers.Contains("X-Total");
            containsPages = containsPages && message.Headers.Contains("Link");

            if (!containsPages) return null;

            var headXPage = message.Headers.FirstOrDefault(x => x.Key == "X-Page");
            var headXPerPage = message.Headers.FirstOrDefault(x => x.Key == "X-Per-Page");
            var headXTotal = message.Headers.FirstOrDefault(x => x.Key == "X-Total");
            var headLink = message.Headers.FirstOrDefault(x => x.Key == "Link");
            int.TryParse(headXPage.Value.First(), out var page);
            int.TryParse(headXTotal.Value.First(), out var total);
            int.TryParse(headXPerPage.Value.First(), out var countPerPage);

            var linkRaw = headLink.Value.FirstOrDefault() ?? "";
            var linkClean = linkRaw.Replace(" ", "");
            var links = linkClean.Split(',');
            var nextLink = GetLink(links.FirstOrDefault(x => x.Contains("rel=\"next\"")));
            var lastLink = GetLink(links.FirstOrDefault(x => x.Contains("rel=\"last\"")));

            var result = new MocoPagination
            {
                LastLink = lastLink,
                NextLink = nextLink,
                Total = total,
                ItemsPerPage = countPerPage,
                Page = page
            };

            return result;
        }

        private static string GetLink(string source)
        {
            if (source == null) 
                return string.Empty;

            var startIndex = source.IndexOf('<') + 1;
            var endIndex = source.IndexOf('>');

            if (startIndex < 0) return string.Empty;
            if (endIndex < 0) return string.Empty;

            return source.Substring(startIndex, endIndex - startIndex);
        }



        public async Task<T> PostAsync<T>(string url, string data)
        {
            var token = GetToken();

            DefaultRequestHeaders.Clear();
            DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };
            DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(_authenticationApiKey ? "Token" : "Bearer", token);

            var content = new StringContent(data, Encoding.UTF8, "application/json");

            using var response = await PostAsync(_baseUri + url, content);
            if (response.IsSuccessStatusCode)
            {
                var dto = JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
                return dto;
            }

            throw new HttpRequestException($"{response.StatusCode}: {response.ReasonPhrase}");
        }

#pragma warning disable CS1998
        public async Task<T> PostAsync<T>(string url, FileStream fs)
#pragma warning restore CS1998
        {
            throw new NotImplementedException();
        }

        public async Task<T> PutAsync<T>(string url, string data)
        {
            var token = GetToken();

            DefaultRequestHeaders.Clear();
            DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };
            DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(_authenticationApiKey ? "Token" : "Bearer", token);

            var content = new StringContent(data, Encoding.UTF8, "application/json");

            using var response = await PutAsync(_baseUri + url, content);
            if (response.IsSuccessStatusCode)
            {
                var dto = JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
                return dto;
            }

            throw new HttpRequestException($"{response.StatusCode}: {response.ReasonPhrase}");
        }

        public async Task<T> PutAsync<T>(string url, FileStream fs)
        {
            var token = GetToken();

            DefaultRequestHeaders.Clear();
            DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };
            DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(_authenticationApiKey ? "Token" : "Bearer", token);

            using var response = await PutAsync(_baseUri + url, new StreamContent(fs));
            if (response.IsSuccessStatusCode)
            {
                var dto = JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
                return dto;
            }

            throw new HttpRequestException($"{response.StatusCode}: {response.ReasonPhrase}");
        }

        public async Task<bool> DeleteAsync<T>(string url)
        {
            var token = GetToken();

            DefaultRequestHeaders.Clear();
            DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };
            DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(_authenticationApiKey ? "Token" : "Bearer", token);

            using var response = await DeleteAsync(_baseUri + url);
            return response.IsSuccessStatusCode;
        }



        private string GetToken(bool forceFreshToken = false)
        {
            lock (_lockObject)
            {
                if (forceFreshToken || HasValidToken() == false)
                {
                    
                }

                return _apiKey;
            }
        }


        private bool HasValidToken()
        {
            if (_authenticationApiKey)
            {
                return !string.IsNullOrWhiteSpace(_apiKey) && _apiKey.Length > 20;
            }

            return false;
        }
    }

    public class MocoAuthResponse
    {
        [JsonProperty("user_id")]
        public string Id { get; set; }

        [JsonProperty("api_key")]
        public string ApiKey { get; set; }
    }

    public class MocoPagination
    {
        public string NextLink { get; set; }
        public string LastLink { get; set; }
        public int Total { get; set; }
        public int Page { get; set; }
        public int ItemsPerPage { get; set; }

        public bool HasNextPage => IsValidUri(NextLink) != null;
        public Uri NextUri => IsValidUri(NextLink);

        private static Uri IsValidUri(string url)
        {
            var result = Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
                         && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            return result ? uriResult : null;
        }
    }
}