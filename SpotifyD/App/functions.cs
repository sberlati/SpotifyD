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
        //private bool readyToGo = false;

        public functions(SpotifyLib spotifylib, YouTubeLib youtubeLib, ProgressBar progressBar, Button startButton, Label statusLabel)
        {
            this.spotifyLib = spotifylib;
            this.youtubeLib = youtubeLib;
            this.progressBar = progressBar;
            this.startButton = startButton;
            this.statusLabel = statusLabel;
        }

        public void fillComboPlaylists(string userId, ComboBox source)
        {
            source.Items.Clear();  
            Paging<SimplePlaylist> playlists = this.spotifyLib.getUserPlaylists(this.spotifyLib.getUser(userId));
            playlists.Items.ForEach(playlist => source.Items.Add(playlist.Name));
        }

        public List<TrackModel> getPlaylistTracks(string userId, string playlistId)
        {
            Paging<PlaylistTrack> tracks = this.spotifyLib.getPlaylistTracks(userId, playlistId);
            List<TrackModel> _tracks = new List<TrackModel>();
            tracks.Items.ForEach(track => _tracks.Add(new TrackModel(track.Track.Name, track.Track.Artists)));
            return _tracks;
        }

        public void downloadSong(TrackModel track, ProgressBar progressBar, string videoId, string destination)
        {
            using(WebClient wc = new WebClient())
            {
                wc.DownloadFileCompleted += new AsyncCompletedEventHandler(downloadCompleted);
                wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(downloadProgress);
                wc.DownloadFileAsync(this.generateDownloadLink(videoId), destination + "\\" + track.Name + ".mp3");
            }
        }

        private void downloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            this.progressBar.Value = 0;
            this.totalLoad--;

            if (this.totalLoad < 1)
                MessageBox.Show("Descargas finalizadas!");
        }

        private void downloadProgress(object sender, DownloadProgressChangedEventArgs e)
        {
            this.progressBar.Value = e.ProgressPercentage;
        }

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
            foreach(TrackModel track in tracks)
            {
                //Lo busco en YouTube y agarro el primero
                string song = track.Name + " - " + this.generateArtistsString(track.Artists);
                statusLabel.Text = "Descargando: " + song;
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

        private string getPlaylistIdByName(string playlistName, string userId)
        {
            string playlistId = string.Empty;
            Paging<SimplePlaylist> playlists = this.spotifyLib.getUserPlaylists(this.spotifyLib.getUser(userId));
            List<PlaylistModel> _playlists = new List<PlaylistModel>();
            playlists.Items.ForEach(playlist => _playlists.Add(new PlaylistModel(playlist.Id, playlist.Name, playlist.Owner)));

            foreach(PlaylistModel playlist in _playlists)
            {
                if (playlist.Name == playlistName && playlist.Owner.Id == userId)
                    return playlist.Id;
            }
            return null;
        }

        private Uri generateDownloadLink(string videoId)
        {
            return new Uri("http://www.youtubeinmp3.com/fetch/?video=https://www.youtube.com/watch?v=" + videoId);
        }

        private string generateArtistsString(List<SimpleArtist> artists)
        {
            string final = string.Empty;
            bool isFirst = true;
            foreach(SimpleArtist artist in artists)
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
