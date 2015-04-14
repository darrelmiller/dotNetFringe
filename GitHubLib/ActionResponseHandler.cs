using System;
using System.Net.Http;
using System.Threading.Tasks;
using Tavis;

namespace GitHubWebPack
{
    public class ActionResponseHandler : DelegatingResponseHandler
    {
        private readonly Func<string,HttpResponseMessage,Task> _action;

        public ActionResponseHandler(Func<string,HttpResponseMessage,Task> action)
        {
            _action = action;
        }

        public async override Task<HttpResponseMessage> HandleResponseAsync(string linkRelation, HttpResponseMessage responseMessage)
        {
            await _action(linkRelation, responseMessage);

            var response = await base.HandleResponseAsync(linkRelation, responseMessage);

            return response;
        }
    }
}