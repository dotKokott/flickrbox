using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FlickrNet;

namespace core.FlickrIntegration
{
    public class FlickrClient
    {
        public Flickr Flickr { get; private set; }
        public bool Authenticated { get; private set; }
        public FoundUser User { get; private set; }

        public FlickrClient(string apiKey, string sharedSecret, string token)
        {
            Flickr = new Flickr(apiKey, sharedSecret, token);

            User = Flickr.PeopleFindByUserName("me");
            Authenticated = true;
        }

        public FlickrClient(string apiKey, string sharedSecret)
        {
            Flickr = new Flickr(apiKey, sharedSecret);
            Authenticated = false;
        }

        public void Authenticate(FlickrAuthentication authentication)
        {            
            Flickr = authentication.Authenticate();
            User = Flickr.PeopleFindByUserName("me");

            Authenticated = true;            
        }

        public PhotoCollection GetPhotoCollectionByName(string name)
        {
            FoundUser user = Flickr.PeopleFindByUserName(name);
            FlickrNet.PhotoSearchOptions options = new FlickrNet.PhotoSearchOptions();
            options.UserId = user.UserId;
            options.PerPage = 20;
            options.Page = 1;

            return Flickr.PhotosSearch(options);
        }

        public Photoset GetPhotoSetByName(string name)
        {
            return Flickr.PhotosetsGetList(User.UserId).Where(ps => ps.Title == name).FirstOrDefault();
        }

        public string UploadPicture(string filePath, string title)
        {                        
            return Flickr.UploadPicture(filePath, title);
        }

        public void DownloadPicture(string photoId, string filePath)
        {
            DateTime startTime = DateTime.UtcNow;
            WebRequest request = WebRequest.Create(Flickr.PhotosGetInfo(photoId).LargeUrl);
            
            using (Stream responseStream = request.GetResponse().GetResponseStream())
            {
                using (Stream fileStream = File.OpenWrite(filePath))
                {
                    byte[] buffer = new byte[4096];
                    int bytesRead = responseStream.Read(buffer, 0, 4096);
                    while (bytesRead > 0)
                    {
                        fileStream.Write(buffer, 0, bytesRead);
                        DateTime nowTime = DateTime.UtcNow;
                        if ((nowTime - startTime).TotalMinutes > 5)
                        {
                            throw new ApplicationException(
                                "Download timed out");
                        }
                        bytesRead = responseStream.Read(buffer, 0, 4096);
                    }
                }
            }
        }
    }
}