using SpotifyAPI.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyD.App.Spotify
{
    class TrackModel
    {
        private string name;
        private List<SimpleArtist> artists;

        public string Name
        {
            get { return this.name; }
        }

        public List<SimpleArtist> Artists
        {
            get { return this.artists; }
        }

        public TrackModel(string name, List<SimpleArtist> artists)
        {
            this.name = name;
            this.artists = artists;
        }
    }
}
