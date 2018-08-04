using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace WebMvc
{
    public class CustomHttpClient: IHttpClient
    {
        private HttpClient _client;
        private ILogger<CustomHttpClient> _logger;

        public CustomHttpClient(ILogger<CustomHttpClient> logger)
        {
            _client = new HttpClient();
            _logger = logger;
        }

        public async Task<string> GetStringAsync(string uri)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, uri);
            var response       = await _client.SendAsync(requestMessage);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<HttpResponseMessage> DoPostPutAsync<T>
        (
            HttpMethod method,
            string uri,
            T item
        )
        {
            if ( method != HttpMethod.Post
                    && method != HttpMethod.Put )
            {
                throw new ArgumentException
                (
                    "HTTP Method must be POST or PUT.",
                    nameof(method)
                );
            }

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = new StringContent
                (
                    JsonConvert.SerializeObject(item),
                    System.Text.Encoding.UTF8,
                    "application/json"
                )    
            };

            var response = await _client.SendAsync(requestMessage);
            if ( response.StatusCode == HttpStatusCode.InternalServerError )
            {
                throw new HttpRequestException();
            }

            return response;
        }

        public async Task<HttpResponseMessage> PostAsync<T>
        (
            string uri,
            T item
        )
        {
            return await DoPostPutAsync(HttpMethod.Post, uri, item);
        }

        public async Task<HttpResponseMessage> PutAsync<T>
        (
            string uri,
            T item
        )
        {
            return await DoPostPutAsync(HttpMethod.Post, uri, item);
        }        

        public async Task<HttpResponseMessage> DeleteAsync(string uri)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);
            return await _client.SendAsync(requestMessage);
        }
    }
}