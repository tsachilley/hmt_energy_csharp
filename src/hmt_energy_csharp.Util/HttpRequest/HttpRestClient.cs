using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace hmt_energy_csharp.HttpRequest
{
    public class HttpRestClient
    {
        private readonly string _apiUrl;
        private readonly IRestClient _client;

        public HttpRestClient(string apiUrl)
        {
            _apiUrl = apiUrl;
            _client = new RestClient(_apiUrl);
        }

        public async Task<string> ExecuteAsync(RequestParams baseRequest)
        {
            try
            {
                var request = new RestRequest(baseRequest.Route);
                request.Method = baseRequest.Method;
                request.AddHeader("Content-Type", baseRequest.ContentType);
                foreach (var entity in baseRequest.Parameter)
                {
                    request.AddParameter(entity.Key, entity.Value);
                }
                var response = await _client.ExecuteAsync(request);
                if (response.StatusCode == HttpStatusCode.OK)
                    return response.Content;
                else
                    throw new Exception("网络不通。");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}