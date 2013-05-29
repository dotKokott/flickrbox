using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using core.FlickrIntegration;
using FlickrNet;

namespace core
{
    public class FlickrBoxContext
    {
        public FlickrClient Client { get; private set; }
        public Dictionary<string, Photoset> Photosets { get; private set; }

        //TEMPORARY
        public string Path = @"C:\flickrbox";


        public FlickrBoxContext(FlickrClient client)
        {
            if (!client.Authenticated) throw new ArgumentException("Client has to be authenticated!");
            
            Client = client;

            init();
        }

        private void init()
        {
            Console.WriteLine("getting photosets");

            Photosets = new Dictionary<string, Photoset>();
            foreach (var photoset in Client.Flickr.PhotosetsGetList(Client.User.UserId).Where(ps => ps.Title.StartsWith("flickrbox")))
            {
                Photosets.Add(photoset.Title, photoset);
            }

            Console.WriteLine("checking for flickrbox photoset");

            if (Directory.GetFiles(Path).Count() > 0 && !Photosets.ContainsKey("flickrbox"))            
            {
                Console.WriteLine("creating flickrbox photoset");

                var photoId = Client.UploadPicture(Directory.GetFiles(Path)[0], "firstFile");
                Client.Flickr.PhotosetsCreate("flickrbox", photoId);
            }
        }
    }
}
