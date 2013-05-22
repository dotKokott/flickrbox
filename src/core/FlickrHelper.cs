using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FlickrNet;

namespace core
{
    public class FlickrHelper
    {
        public FlickrHelper()
        {

        }

        private Flickr _flickr = null;
        public Flickr flickr
        {
            get { return _flickr ?? (_flickr = new Flickr("cd1cdc0e5ae6bce96b970a3df1d482e3", "3297296eae9e47b4", getAuthToken())); }
        }

        public PhotoCollection getUserPhotos()
        {
            FoundUser user = flickr.PeopleFindByUserName("gluix");
            FlickrNet.PhotoSearchOptions options = new FlickrNet.PhotoSearchOptions();
            options.UserId = user.UserId;
            options.PerPage = 20;
            options.Page = 1;
            return flickr.PhotosSearch(options);
        }


        public string uploadPhoto(string filePath, string title)
        {
            return flickr.UploadPicture(filePath, title);
        }

        public static string getAuthToken()
        {
            //check if we allready have an authKey
            var authKeyPath = System.IO.Path.Combine(System.Environment.SpecialFolder.LocalApplicationData.ToString(), @"/auth.key");
            if (System.IO.File.Exists(authKeyPath))
            {
                return System.IO.File.ReadAllText(authKeyPath);
            }
            else
            {
                Flickr tmpFlickr = new Flickr("cd1cdc0e5ae6bce96b970a3df1d482e3", "3297296eae9e47b4");
                var Frob = tmpFlickr.AuthGetFrob();
                string url = tmpFlickr.AuthCalcUrl(Frob, AuthLevel.Write);
                System.Diagnostics.Process.Start(url);
                //breakpoint setzen.button auf webpage bestätitgen. weiter laufen lassen
                Auth auth = tmpFlickr.AuthGetToken(Frob);
                System.IO.File.WriteAllText(authKeyPath, auth.Token);
                return auth.Token;
            }
        }

        public void downloadPhoto(string photoId, string filePath)
        {
            DateTime startTime = DateTime.UtcNow;
            WebRequest request = WebRequest.Create(flickr.PhotosGetInfo(photoId).LargeUrl);
            WebResponse response = request.GetResponse();
            using (Stream responseStream = response.GetResponseStream())
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