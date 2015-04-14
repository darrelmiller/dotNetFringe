using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CollectionJson;
using CollectionJson.Client;
using Tavis;
using Tavis.UriTemplates;
using Link = Tavis.Link;

namespace MediaTypes
{
    class Program
    {
        private static IResponseHandler _ConsoleWriter = new ConsoleWriter();

        static void Main(string[] args)
        {
            GetHomeDocument().Wait();
            //GetProblemDocument().Wait();
            //GetCollectionJsonDocument().Wait();
        
            Console.ReadLine();
        }

        private static async Task GetHomeDocument()
        {
            var httpClient = new HttpClient();
            var homeLink = new Link() {Target = new Uri("http://conference.hypermediaapi.com/")};
            var resp = await httpClient.FollowLinkAsync(homeLink);

            var homeDoc = Tavis.Home.HomeDocument.Parse(await resp.Content.ReadAsStringAsync());

            foreach (var resource in homeDoc.Resources)
            {
                if (resource.Template != null)
                {
                    Console.WriteLine(resource.Relation + " : " + resource.Template.ToString());
                }
                else
                {
                    Console.WriteLine(resource.Relation + " : " + resource.Target);
                }
            }
        }


        private static async Task GetProblemDocument()
        {
            var httpClient = new HttpClient();

            var speakerLink = new Link() { Template = new UriTemplate("http://conference.hypermediaapi.com/sessions{?dayno}") };
            speakerLink.Template.AddParameter("dayno", "10");
            var resp = await httpClient.FollowLinkAsync(speakerLink, _ConsoleWriter);

            var problemDoc = await resp.Content.ReadAsProblemAsync();

            Console.WriteLine(problemDoc.Title);
            Console.WriteLine(problemDoc.ProblemType.ToString());

        }

        private static async Task GetCollectionJsonDocument()
        {
            var httpClient = new HttpClient();
            var homeLink = new Link() { Target = new Uri("http://conference.hypermediaapi.com/speakers") };
            var resp = await httpClient.FollowLinkAsync(homeLink);

            var collection = await resp.Content.ReadAsCollectionJsonAsync();

            foreach (var item in collection.Items)
            {
                Console.WriteLine(String.Join(" , ", item.Data.Select(d => d.Name  + " = " + d.Value)));
            }
        }


    }

    public class ConsoleWriter : IResponseHandler
    {
        public async Task<HttpResponseMessage> HandleResponseAsync(string linkRelation, HttpResponseMessage responseMessage)
        {
            var body = await responseMessage.Content.ReadAsStringAsync();
            Console.WriteLine(body);

            return responseMessage;
        }
    }

    public static class CollectionJsonExtensions
    {
        public static async Task<Collection> ReadAsCollectionJsonAsync(this HttpContent content)
        {
            var readDocument = await content.ReadAsAsync<ReadDocument>(new[] { new CollectionJsonFormatter() });
            return readDocument.Collection;
        }
    }
}
