using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using core;
using core.FileConversion;
using core.FlickrIntegration;

namespace console
{
    class Program
    {
        static void Main(string[] args)
        {
            var apiKey = ConfigurationSettings.AppSettings["apiKey"];
            var secret = ConfigurationSettings.AppSettings["sharedSecret"];
            var authKeyPath = System.IO.Path.Combine("auth.key");            

            FlickrClient client;
            if (!File.Exists(authKeyPath))
            {
                client = new FlickrClient(apiKey, secret);
                var auth = new FlickrAuthentication(client);

                Console.WriteLine("Please authenticate flickrbox and press a return");
                Process.Start(auth.AuthenticationUrl);

                Console.ReadLine();

                client.Authenticate(auth);

                File.WriteAllText(authKeyPath, auth.Token);

                Console.WriteLine("Authenticated!");
            }
            else
            {
                var token = File.ReadAllText(authKeyPath);
                client = new FlickrClient(apiKey, secret, token);
                Console.WriteLine("Already authenticated!");
            }

            Console.ReadLine();
        }
    }
}
