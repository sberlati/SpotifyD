using System.Collections.Generic;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

namespace SpotifyD.App.YouTube
{
    class YouTubeLib
    {
        private static YouTubeService youtube;
        private static string youtube_api_key = "AIzaSyDaGH4Zxit0rLgO4v_Vl_KHY0DtAli8WoI";

        public YouTubeLib()
        {
            youtube = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = youtube_api_key
            });
        }

        public List<VideoModel> getVideosByTitle(string name, bool applyLyricsFilter)
        {
            List<VideoModel> toReturn = new List<VideoModel>();
            SearchResource.ListRequest listRequest = youtube.Search.List("snippet");
            listRequest.Q = name;
            listRequest.Order = SearchResource.ListRequest.OrderEnum.Relevance;
            SearchListResponse searchResponse = listRequest.Execute();

            foreach (SearchResult searchResult in searchResponse.Items)
            {
                switch (searchResult.Id.Kind)
                {
                    case "youtube#video":
                        if (applyLyricsFilter)
                        {
                            if (searchResult.Snippet.Title.ToUpper().Contains("LYRICS"))
                                toReturn.Add(new VideoModel(searchResult.Id.VideoId, searchResult.Snippet.Title));
                        }
                        else
                        {
                            toReturn.Add(new VideoModel(searchResult.Id.VideoId, searchResult.Snippet.Title));
                        }
                        break;
                }
            }
            return toReturn;
        }
    }
}
