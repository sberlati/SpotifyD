using SpotifyAPI.Web.Models;
using System.Collections.Generic;
using System.Windows.Forms;
using SpotifyD.App.YouTube;
using SpotifyD.App.Spotify;
using System.Net;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;

namespace SpotifyD
{
    class functions
    {
        private SpotifyLib spotifyLib;
        private YouTubeLib youtubeLib;
        private ProgressBar progressBar;
        private Button startButton;
        private Label statusLabel;
        private bool applyFilters = false;
        private int totalLoad = 0;

        /// <summary>
        /// Constructor de la clase.
        /// </summary>
        /// <param name="spotifylib">Instancia de la clase SpotifyLib.</param>
        /// <param name="youtubeLib">Instancia de la clase YouTubeLib.</param>
        /// <param name="progressBar">Instancia de la barra de progreso del formulario.</param>
        /// <param name="startButton">Instancia del botón de comienzo del formulario.</param>
        /// <param name="statusLabel">Instancia del label de estado del formulario.</param>
        public functions(SpotifyLib spotifylib, YouTubeLib youtubeLib, ProgressBar progressBar, Button startButton, Label statusLabel)
        {
            this.spotifyLib = spotifylib;
            this.youtubeLib = youtubeLib;
            this.progressBar = progressBar;
            this.startButton = startButton;
            this.statusLabel = statusLabel;
        }

        /// <summary>
        /// Relleno el combobox de playlists con las que corresponden.
        /// </summary>
        /// <param name="userId">ID de usuario.</param>
        /// <param name="source">Instancia del combobox a rellenar.</param>
        public void fillComboPlaylists(string userId, ComboBox source)
        {
            source.Items.Clear();
            Paging<SimplePlaylist> playlists = this.spotifyLib.getUserPlaylists(this.spotifyLib.getUser(userId));
            playlists.Items.ForEach(playlist => source.Items.Add(playlist.Name));
        }

        /// <summary>
        /// Devuelve una lista de TrackModel de una playlist.
        /// </summary>
        /// <param name="userId">ID de usuario.</param>
        /// <param name="playlistId">ID de la playlist.</param>
        /// <returns>Lista con instancias de TrackModel.</returns>
        public List<TrackModel> getPlaylistTracks(string userId, string playlistId)
        {
            Paging<PlaylistTrack> tracks = this.spotifyLib.getPlaylistTracks(userId, playlistId);
            List<TrackModel> _tracks = new List<TrackModel>();
            tracks.Items.ForEach(track => _tracks.Add(new TrackModel(track.Track.Name, track.Track.Artists)));
            return _tracks;
        }

        /// <summary>
        /// Descarga una canción.
        /// </summary>
        /// <param name="track">Instancia de TrackModel de la canción a descargar.</param>
        /// <param name="progressBar">Barra de progreso donde se refleja como va la descarga.</param>
        /// <param name="videoId">ID del vídeo de YouTube con la canción.</param>
        /// <param name="destination">URI de destino (carpeta local).</param>
        public void downloadSong(TrackModel track, ProgressBar progressBar, string videoId, string destination)
        {
            using (WebClient wc = new WebClient())
            {
                wc.DownloadFileCompleted += new AsyncCompletedEventHandler(downloadCompleted);
                wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(downloadProgress);
                wc.DownloadFileAsync(this.generateDownloadLink(videoId), destination + "\\" + track.Name + ".mp3");
            }
        }

        /// <summary>
        /// Método disparado por downloadSong() al finalizar la descarga del archivo.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void downloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            this.progressBar.Value = 0;
            this.totalLoad--;

            if (this.totalLoad < 1)
                MessageBox.Show("Descargas finalizadas!");
            this.startButton.Enabled = true;
            this.statusLabel.Text = "Esperando";
        }

        /// <summary>
        /// Método disparado por downloadSong() al cambiar el progreso de la descarga.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void downloadProgress(object sender, DownloadProgressChangedEventArgs e)
        {
            this.progressBar.Value = e.ProgressPercentage;
        }

        /// <summary>
        /// Empieza a descargar todas las canciones de una playlist.
        /// </summary>
        /// <param name="userId">ID de usuario de Spotify.</param>
        /// <param name="playlistName">Nombre de la playlist (ítem elegido en el combobox).</param>
        /// <param name="destination">URI de destino (carpeta local).</param>
        public void startDownloads(string userId, string playlistName, string destination)
        {
            //Deshabilito el boton de comenzar y arranco el status y la barra
            startButton.Enabled = false;
            statusLabel.Text = "Iniciando...";
            progressBar.Value = 5;
            //Busco el ID de la playlist seleccionada
            string selectedPlaylistId = this.getPlaylistIdByName(playlistName, userId);
            statusLabel.Text = "Buscando playlist de Spotify";
            //Obtengo las canciones de la playlist
            List<TrackModel> tracks = this.getPlaylistTracks(userId, selectedPlaylistId);
            statusLabel.Text = tracks.Count + " canciones";
            //Ahora recorro la lista de canciones, busco su equivalente en YouTube y la descargo
            this.totalLoad = tracks.Count;
            foreach (TrackModel track in tracks)
            {
                //Lo busco en YouTube y agarro el primero
                string song = track.Name + " - " + this.generateArtistsString(track.Artists);
                statusLabel.Text = "Descargando... ¡Puede tomar un rato!";
                VideoModel video = youtubeLib.getVideosByTitle(song, this.applyFilters).ToArray()[0];
                //Si hay resultados lo descargo
                if (video.Equals(null))
                    continue;
                else
                {
                    this.downloadSong(track, progressBar, video.Id, destination);
                }
            }
            //Listoooooooooo
        }

        /// <summary>
        /// Busca una playlist por su nombre
        /// </summary>
        /// <param name="playlistName">Cadena con el nombre de la playlist</param>
        /// <param name="userId">ID de usuario de Spotify</param>
        /// <returns>Cadena con el ID de la playlist</returns>
        private string getPlaylistIdByName(string playlistName, string userId)
        {
            string playlistId = string.Empty;
            Paging<SimplePlaylist> playlists = this.spotifyLib.getUserPlaylists(this.spotifyLib.getUser(userId));
            List<PlaylistModel> _playlists = new List<PlaylistModel>();
            playlists.Items.ForEach(playlist => _playlists.Add(new PlaylistModel(playlist.Id, playlist.Name, playlist.Owner)));

            foreach (PlaylistModel playlist in _playlists)
            {
                if (playlist.Name == playlistName && playlist.Owner.Id == userId)
                    return playlist.Id;
            }
            return null;
        }

        /// <summary>
        /// Genera la URL con la descarga directa del sonido en MP3 del vídeo.
        /// </summary>
        /// <param name="videoId">ID del vídeo.</param>
        /// <returns>URI para la descarga del MP3.</returns>
        private Uri generateDownloadLink(string videoId)
        {
            return new Uri("http://www.youtubeinmp3.com/fetch/?video=https://www.youtube.com/watch?v=" + videoId);
        }

        /// <summary>
        /// La lista de artistas que tiene una canción los junta en un string 
        /// para poder realizar la búsqueda en YouTube.
        /// </summary>
        /// <param name="artists">Lista de SimpleArtist con los artistas de una canción.</param>
        /// <returns>String con los artistas de la canción</returns>
        private string generateArtistsString(List<SimpleArtist> artists)
        {
            string final = string.Empty;
            bool isFirst = true;
            foreach (SimpleArtist artist in artists)
            {
                if (isFirst)
                    final += artist.Name;
                else
                    final += " & " + artist.Name;
                isFirst = false;
            }
            return final;
        }
    }
}
