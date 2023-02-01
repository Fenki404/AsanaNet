using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AsanaNet.Services.Interfaces
{
    public interface IRestClient
    {
        void Login(string clientId, string clientSecret);
        Task<T> GetAsync<T>(string url, Dictionary<string, string> query);
        Task<T> GetAsync<T>(string url);

        Task<IEnumerable<T>> GetListAsync<T>(string url, Dictionary<string, string> query);
        Task<IEnumerable<T>> GetListAsync<T>(string url);

        Task<T> PostAsync<T>(string url, string data);
        Task<T> PostAsync<T>(string url, FileStream fs);

        Task<T> PutAsync<T>(string url, string data);
        Task<T> PutAsync<T>(string url, FileStream fs);

        Task<bool> DeleteAsync<T>(string url);
    }
}