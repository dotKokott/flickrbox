using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlickrNet;

namespace core.FlickrIntegration
{
    public class FlickrAuthentication
    {
        public Flickr Flickr { get; private set; }

        public string Frob { get; private set; }
        public string AuthenticationUrl { get; private set; }
        public string Token { get; private set; }

        public FlickrAuthentication(FlickrClient client)
        {
            Flickr = client.Flickr;
            Frob = Flickr.AuthGetFrob();
            AuthenticationUrl = Flickr.AuthCalcUrl(Frob, AuthLevel.Delete);
        }

        public Flickr Authenticate()
        {
            var auth = Flickr.AuthGetToken(Frob);
            Token = auth.Token;

            return new Flickr(Flickr.ApiKey, Flickr.ApiSecret, Token);
        }
    }
}
