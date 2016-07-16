using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;
using System.Net.Sockets;

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

        /// <summary>
        /// Inicia el proceso de autorización para que la aplicación pueda usar
        /// la API de Spotify.
        /// </summary>
        /// <exception cref="System.Net.Sockets.SocketException">
        /// Cuando el puerto 80 está en uso por otro proceso
        /// </exception>
        private static void startAuth()
        {
            try
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
            catch (SocketException)
            {
                System.Windows.Forms.MessageBox.Show("La aplicación necesita el puerto 80 para poder funcionar. Cierra aplicaciones o servicios como Apache, Skype y vuelve a intentarlo", "Conflicto de puertos", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Método invocado al terminar la autorización
        /// </summary>
        /// <param name="token">Token de Spotify, SpotifyAPI.Web.Models</param>
        /// <param name="state">Estado del proceso de autorización</param>
        private static void auth_OnResponseReceivedEvent(Token token, string state)
        {
            auth.StopHttpServer();
            _spotify = new SpotifyWebAPI()
            {
                TokenType = token.TokenType,
                AccessToken = token.AccessToken
            };
        }

        /// <summary>
        /// Obtengo el PublicProfile de un usuario buscándolo por su ID
        /// </summary>
        /// <param name="userId">ID de usuario</param>
        /// <returns>PublicProfile. Si no encuentra al usuario, null.</returns>
        public PublicProfile getUser(string userId)
        {
            PublicProfile profile = _spotify.GetPublicProfile(userId);
            if (profile.Id == null)
                return null;
            else
                return profile;
        }

        /// <summary>
        /// Busca las playlists por perfil de usuario
        /// </summary>
        /// <param name="profile">PublicProfile del usuario que quiero obtener playlists</param>
        /// <returns>Lista de Playlists correspondientes al usuario</returns>
        public Paging<SimplePlaylist> getUserPlaylists(PublicProfile profile)
        {
            return _spotify.GetUserPlaylists(profile.Id);
        }

        /// <summary>
        /// Obtengo las canciones de una playlist
        /// </summary>
        /// <param name="userId">ID de usuario</param>
        /// <param name="playlistId">ID de la playlist</param>
        /// <returns></returns>
        public Paging<PlaylistTrack> getPlaylistTracks(string userId, string playlistId)
        {
            return _spotify.GetPlaylistTracks(userId, playlistId);
        }
    }
}
