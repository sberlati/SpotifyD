using SpotifyAPI.Web.Models;
using System.Collections.Generic;

namespace SpotifyD.App.Spotify
{
    class TrackModel
    {
        /// <summary>
        /// Clase modelo para la entidad "Track".
        /// </summary>
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
