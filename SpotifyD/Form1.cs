using SpotifyD.App.YouTube;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpotifyD
{
    public partial class Form1 : Form
    {
        private functions f;
        private SpotifyLib sl;
        private YouTubeLib ytl;

        public Form1()
        {
            InitializeComponent();
            sl = new SpotifyLib();
            ytl = new YouTubeLib();
            f = new functions(sl, ytl, progressBar, button3, statusLabel);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void FolderSelectButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            DialogResult result = fbd.ShowDialog();

            actualFolder.Text = fbd.SelectedPath;
        }

        private void playlistSelect_Enter(object sender, EventArgs e)
        {
            if(this.userId.Text != null || this.userId.Text != "")
            {
                f.fillComboPlaylists(this.userId.Text, this.playlistSelect);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            /*
               1º Busco todos los temas de la playlist
               2º Por cada tema busco el link de descarga
               3º Lo descargo, lo guardo en la carpeta elegida
               4º Repito hasta quedarme sin canciones
               5º LISTO PERRO
            */
            f.startDownloads(userId.Text, playlistSelect.SelectedItem.ToString(), actualFolder.Text);
        }

        /*
            Minimiza el formulario
        */
        private void button2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
    }
}
