﻿using SoftCinema.Client.Forms.ContentHolders;
using SoftCinema.Models;
using SoftCinema.Services;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace SoftCinema.Client.Utilities.CustomTools
{
    public class MoviePosterLink : GroupBox
    {
        private static Font _normalFont = new Font("Arial", 10F, System.Drawing.FontStyle.Bold,
            System.Drawing.GraphicsUnit.Point, ((byte)(0)));

        private static Color _back = System.Drawing.Color.Gray;
        private static Color _border = System.Drawing.Color.Black;
        private static Color _activeBorder = System.Drawing.Color.Red;
        private static Color _fore = System.Drawing.Color.White;

        private static Padding _margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        private static Padding _padding = new System.Windows.Forms.Padding(3, 3, 3, 3);

        private static Size _minSize = new System.Drawing.Size(200, 270);
        private PictureBox _pictureBox = new PictureBox();
        private Button _showDetailsButton = new Button() { Text = "Details" };

        private Movie _movie { get; set; }

        private bool _active;

        private readonly MovieService movieService;
        private readonly ImageService imageService;

        public MoviePosterLink(string movieName) : base()
        {
            this.movieService = new MovieService();
            this.imageService = new ImageService();
            base.Font = _normalFont;
            base.BackColor = _back;
            base.ForeColor = _fore;
            base.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            base.Margin = _margin;
            base.Padding = _padding;
            base.MinimumSize = _minSize;

            var currentMovie = movieService.GetMovie(movieName);
            this._movie = currentMovie;

            this._pictureBox.Image = imageService.byteArrayToImage(currentMovie.Image.Content);
            this._pictureBox.Image = imageService.ScaleImage(this._pictureBox.Image, 200, 310);

            this._pictureBox.Size = new Size(this._pictureBox.Image.Size.Width, this._pictureBox.Image.Size.Height);
            this._showDetailsButton.Location = new Point(this._pictureBox.Location.X, this._pictureBox.Location.Y + 10);
            setDetailsButtonClickEvent();

            this.Controls.Add(this._showDetailsButton);
            this.Controls.Add(this._pictureBox);
        }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
        }

        protected override void OnMouseEnter(System.EventArgs e)
        {
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(System.EventArgs e)
        {
            base.OnMouseLeave(e);
        }

        public void SetStateActive()
        {
            _active = true;
        }

        public void SetStateNormal()
        {
            _active = false;
        }

        private void setDetailsButtonClickEvent()
        {
            _showDetailsButton.Click += new EventHandler(_showDetailsButton_Click);
        }

        private void _showDetailsButton_Click(object sender, System.EventArgs e)
        {
            MovieForm movieForm = new MovieForm(this._movie);
            movieForm.TopLevel = false;
            movieForm.AutoScroll = true;
            var _thisForm = ((Button)sender).Parent.Parent.Parent;
            var contentHolder = ((Button)sender).Parent.Parent.Parent.Parent;
            contentHolder.Controls.Clear();
            contentHolder.Controls.Add(_thisForm);
            contentHolder.Controls.Add(movieForm);
            movieForm.Show();
            _thisForm.Hide();
        }
    }
}