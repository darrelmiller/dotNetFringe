using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Tavis.UriTemplates;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            RawHttpRequest().Wait();
            //ServiceApi().Wait();
            Console.ReadLine();
        }

        private static async Task RawHttpRequest()
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders
                .UserAgent.Add(new ProductInfoHeaderValue("DarrelIsTesting", "1.0"));


            var url = new UriTemplate("{+baseUrl}/search/code{?params*}")
                .AddParameter("baseUrl", "https://api.github.com")
                .AddParameter("params", new Dictionary<string, string> { { "q", "GetAsync user:darrelmiller" } })
                .Resolve();

            var resp = await httpClient.GetAsync(url);

            //Check for 300s, 400s, 500s

            // Interpret the response body
            Console.WriteLine(JObject.Parse(await resp.Content.ReadAsStringAsync()));
        }


        private static async Task ServiceApi()
        {
            var gitapi = new GitApiService();
            var result = await gitapi.SearchCode("GetAsync user:darrelmiller");   // No more HTTP! RPC?

            Console.WriteLine(result);
        }

        // Are client SDKs just bad? Or is there a better way?
        
    }
    
    public class GitApiService  
    {
        private readonly HttpClient _httpClient;
        public GitApiService(HttpClient httpClient = null)
        {
            if (httpClient == null)
            {
                httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders
                    .UserAgent.Add(new ProductInfoHeaderValue("GitApiService", "1.0"));
            }
            _httpClient = httpClient;
        }

        public async Task<JObject> SearchCode(string query, int? page = null, int? per_page = null, string sort_order=null)
        {
            var url = new UriTemplate("{+baseUrl}/search/code{?params*}")
                .AddParameter("baseUrl", "https://api.github.com")
                .AddParameter("params", new Dictionary<string, string> { { "q", query } })
                .Resolve();

            var resp = await _httpClient.GetAsync(url);

            //Check for 300s, 400s, 500s

            return JObject.Parse(await resp.Content.ReadAsStringAsync());
        }
    }
}
