using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyD
{
    class SpotifyLib
    {
        private static SpotifyWebAPI _spotify;
        private static ImplicitGrantAuth auth;
        private static string clientId = "b3ebe5df349d4a718eb9bcf7c1ac8e89";
        private static string redirect = "http://localhost";

        public SpotifyLib()
        {
            startAuth();
        }

        private static void startAuth()
        {
            auth = new ImplicitGrantAuth()
            {
                ClientId = clientId,
                RedirectUri = redirect,
                Scope = Scope.UserReadPrivate,
            };
            auth.StartHttpServer();
            auth.OnResponseReceivedEvent += auth_OnResponseReceivedEvent;
            auth.DoAuth();
        }

        private static void auth_OnResponseReceivedEvent(Token token, string state)
        {
            auth.StopHttpServer();
            _spotify = new SpotifyWebAPI()
            {
                TokenType = token.TokenType,
                AccessToken = token.AccessToken
            };
        }

        public PublicProfile getUser(string userId)
        {
            PublicProfile profile = _spotify.GetPublicProfile(userId);
            if(profile.Id == null)
                return null;
            else
                return profile;
        }

        public Paging<SimplePlaylist> getUserPlaylists(PublicProfile profile)
        {
            return _spotify.GetUserPlaylists(profile.Id);
        }

        public Paging<PlaylistTrack> getPlaylistTracks(string userId, string playlistId)
        {
            return _spotify.GetPlaylistTracks(userId, playlistId);
        }
    }
}
