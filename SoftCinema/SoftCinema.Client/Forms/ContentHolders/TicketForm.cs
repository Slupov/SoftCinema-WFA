﻿using SoftCinema.Models;
using SoftCinema.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace SoftCinema.Client.Forms.ContentHolders
{
    public partial class TicketForm : ContentHolderForm
    {
        private string _townName { get; set; }
        private string _cinemaName { get; set; }
        private string _movieName { get; set; }

        //        private ICollection<Screening> _screenings { get; set; }
        private ICollection<Movie> _movies { get; set; }

        public static Screening Screening;
        private string _date;
        private string _time;
        private readonly CinemaService cinemaService;
        private readonly ScreeningService screeningService;
        private readonly MovieService movieService;
        private readonly TownService townService;

        public TicketForm()
        {
            this.cinemaService = new CinemaService();
            this.screeningService = new ScreeningService();
            this.movieService = new MovieService();
            this.townService = new TownService();
            this._movies = new List<Movie>();

            InitializeComponent();
        }

        private void townComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.cinemaComboBox.Text = "Select cinema";
            this.cinemaComboBox.Items.Clear();
            this.movieComboBox.Text = "";
            this.movieComboBox.Items.Clear();
            this.dateComboBox.Text = "";
            this.dateComboBox.Items.Clear();
            this.timeComboBox.Text = "";
            this.timeComboBox.Items.Clear();

            this._townName = this.townComboBox.SelectedItem.ToString();
            var cinemaNames = cinemaService.GetCinemasNamesBySelectedTown(this._townName);

            //adds options to the Cinema select box
            this.cinemaComboBox.Items.AddRange(cinemaNames);

            //default
            if (this.cinemaComboBox.Items.Count == 0)
            {
                this.cinemaComboBox.Text = "(no cinemas)";
            }
        }

        private void selectTicketTypeButton_Click(object sender, EventArgs e)
        {
            var dateTime = screeningService.GetDateTimeFromDateAndTime(_date, _time);
            TicketForm.Screening = screeningService.GetScreening(this._townName, this._cinemaName, this._movieName,
                dateTime);
            TicketTypeForm ticketTypeForm = new TicketTypeForm();
            ticketTypeForm.TopLevel = false;
            ticketTypeForm.AutoScroll = true;
            this.Hide();
            var contentHolder = ((Button)sender).Parent.Parent;
            var formsCount = contentHolder.Controls.Count;
            if (formsCount - 1 <= contentHolder.Controls.IndexOf(this))
            {
                contentHolder.Controls.Add(ticketTypeForm);
                ticketTypeForm.Show();
            }
            else
            {
                contentHolder.Controls[contentHolder.Controls.IndexOf(this) + 1].Show();
            }
        }

        private void cinemaComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.movieComboBox.Text = "Select movie";
            this.movieComboBox.Items.Clear();

            this._cinemaName = this.cinemaComboBox.SelectedItem.ToString();
            this._movies = movieService.GetMoviesByCinemaAndTown(this._cinemaName, this._townName);

            this.movieComboBox.Items.AddRange(this._movies.Select(m => m.Name).ToArray());

            //default
            if (this.movieComboBox.Items.Count == 0)
            {
                this.movieComboBox.Text = "(no movies)";
            }
        }

        private void movieComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.dateComboBox.Text = "Select date";
            this.dateComboBox.Items.Clear();
            this._movieName = this.movieComboBox.SelectedItem.ToString();

            var dates = screeningService.GetAllDatesForMovieInCinema(this._townName,
                this._cinemaName, this._movieName);
            this.dateComboBox.Items.AddRange(dates);
        }

        private void timeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            this._time = timeComboBox.SelectedItem.ToString();

            this.selectTicketTypeButton.Enabled = true;
        }

        private void dateComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.timeComboBox.Text = "Select time";
            this.timeComboBox.Items.Clear();
            this._date = this.dateComboBox.SelectedItem.ToString();

            var hours = screeningService.GetHoursForMoviesByDate(this._townName,
                this._cinemaName, this._movieName, _date);
            ;
            this.timeComboBox.Items.AddRange(hours);
        }

        private void TicketForm_Load(object sender, EventArgs e)
        {
            this.townComboBox.Items.AddRange(townService.GetTownsNames());
        }
    }
}