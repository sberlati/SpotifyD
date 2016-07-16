
using SpotifyAPI.Web.Models;

namespace SpotifyD.App.Spotify
{
    class PlaylistModel
    {
        /// <summary>
        /// Clase modelo para la entidad "Playlist".
        /// </summary>
        private string id;
        private string name;
        private PublicProfile owner;

        public string Id
        {
            get { return this.id; }
        }

        public string Name
        {
            get { return this.name; }
        }

        public PublicProfile Owner
        {
            get { return this.owner; }
        }

        public PlaylistModel(string id, string name, PublicProfile owner)
        {
            this.id = id;
            this.name = name;
            this.owner = owner;
        }
    }
}
