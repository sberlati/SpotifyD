
namespace SpotifyD.App.YouTube
{
    class VideoModel
    {
        private string id;
        private string name;
        private string uri;

        public string Id
        {
            get { return id; }
        }

        public string Name
        {
            get { return name; }
        }

        public string Uri
        {
            get { return "https://www.youtube.com/watch?v=" + this.id; }
        }

        public VideoModel(string id, string name)
        {
            this.id = id;
            this.name = name;
        }
    }
}
