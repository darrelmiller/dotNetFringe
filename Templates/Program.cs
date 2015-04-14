using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tavis.UriTemplates;

namespace Templates
{
    class Program
    {
        static void Main(string[] args)
        {
            ExtremeTemplate();

            Console.ReadLine();
        }

        private static void ExtremeTemplate()
        {
            var url = new UriTemplate("{+baseUrl}{/folder*}/search/code{/language}{?params*}")
                .AddParameter("params", new Dictionary<string, string> {{"query", "GetAsync"}, {"sort_order", "desc"}})
                .AddParameter("baseUrl", "http://api.github.com")
                .AddParameter("folder", new List<string> {"home", "src", "widgets"})
                .Resolve();

            Console.WriteLine("");
            Console.WriteLine("Resolved URL : ");
            Console.WriteLine("");
            Console.WriteLine(url);
        }
    }
}
