using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using core;
using core.FlickrIntegration;

namespace console
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("flickrbox test");

            Console.WriteLine("Generating authentication url -- hit a key, when you authenticated the application");
            var client = new FlickrClient("cd1cdc0e5ae6bce96b970a3df1d482e3", "3297296eae9e47b4");
            var auth = new FlickrAuthentication(client);
            Process.Start(auth.AuthenticationUrl);

            Console.WriteLine("please press allow");

            Console.ReadLine();

            client.Authenticate(auth);

            Console.ReadLine();
        }
    }
}
