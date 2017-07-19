﻿using SoftCinema.Models;
using SoftCinema.Services;
using SoftCinema.Services.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace SoftCinema.Client.Forms.AdminForms
{
    public partial class CategoryEditForm : Form
    {
        private readonly Category category;
        private readonly CategoryService categoryService;

        public CategoryEditForm(Category category)
        {
            this.category = category;
            this.categoryService = new CategoryService();
            InitializeComponent();
        }

        private void CategoryEditForm_Load(object sender, EventArgs e)
        {
            this.CategoryExistsLabel.Hide();
            string[] addedMovies = categoryService.GetMoviesNameAndYearInCategory(category.Name);
            string[] notAddedMovies = categoryService.GetMoviesNotInCategory(category.Name);
            this.CategoryAddedMovies.Items.AddRange(addedMovies);
            this.CategoryNotAddedMovies.Items.AddRange(notAddedMovies);
            this.CategoryNameTextBox.Text = category.Name;
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show(Constants.WarningMessages.UnsavedChanges, Constants.GoBackPrompt, MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                CategoriesForm categoriesForm = new CategoriesForm();
                categoriesForm.TopLevel = false;
                categoriesForm.AutoScroll = true;
                this.Hide();
                ((Button)sender).Parent.Parent.Controls.Add(categoriesForm);
                categoriesForm.Show();
            }
        }

        private void AddMoviesButton_Click(object sender, EventArgs e)
        {
            var movies = CategoryNotAddedMovies.SelectedItems.OfType<string>().ToList();

            foreach (string movie in movies)
            {
                CategoryAddedMovies.Items.Add(movie);
                CategoryNotAddedMovies.Items.Remove(movie);
            }
        }

        private void RemoveMoviesButton_Click(object sender, EventArgs e)
        {
            var movies = CategoryAddedMovies.SelectedItems.OfType<string>().ToList();

            foreach (string movie in movies)
            {
                CategoryNotAddedMovies.Items.Add(movie);
                CategoryAddedMovies.Items.Remove(movie);
            }
        }

        private void EditCategoryButton_Click(object sender, EventArgs e)
        {
            string oldCategoryName = category.Name;
            string newCategoryName = CategoryNameTextBox.Text;
            List<Tuple<string, int>> addedMovies = GetAddedMovies();
            List<Tuple<string, int>> notAddedMovies = GetNotAddedMovies();
            try
            {
                categoryService.UpdateCategory(oldCategoryName, newCategoryName, addedMovies, notAddedMovies);
                MessageBox.Show(Constants.SuccessMessages.CategoryUpdatedSuccessfully);
                CategoriesForm categoriesForm = new CategoriesForm();
                categoriesForm.TopLevel = false;
                categoriesForm.AutoScroll = true;
                this.Hide();
                ((Button)sender).Parent.Parent.Controls.Add(categoriesForm);
                categoriesForm.Show();
            }
            catch (Exception)
            {
                MessageBox.Show(Constants.ErrorMessages.CategoriesUpdateErrorMessage);
            }
        }

        private void CategoryAddedMovies_DoubleClick(object sender, MouseEventArgs e)
        {
            if (CategoryAddedMovies.SelectedItems.Count == 0)
            {
                return;
            }
            var movies = CategoryAddedMovies.SelectedItems.OfType<string>().ToList();
            foreach (var movie in movies)
            {
                CategoryNotAddedMovies.Items.Add(movie);
                CategoryAddedMovies.Items.Remove(movie);
            }
        }

        private void CategoryNotAddedMovies_DoubleClick(object sender, MouseEventArgs e)
        {
            if (CategoryNotAddedMovies.SelectedItems.Count == 0)
            {
                return;
            }
            var movies = CategoryNotAddedMovies.SelectedItems.OfType<string>().ToList();
            foreach (var movie in movies)
            {
                CategoryAddedMovies.Items.Add(movie);
                CategoryNotAddedMovies.Items.Remove(movie);
            }
        }

        private List<Tuple<string, int>> GetNotAddedMovies()
        {
            List<Tuple<string, int>> movies = new List<Tuple<string, int>>();
            foreach (var m in CategoryNotAddedMovies.Items)
            {
                int yearIndex = m.ToString().LastIndexOf(",");
                string movieYearString = m.ToString().Substring(yearIndex + 1).Split(new char[] { '\"' }).First();
                int movieYear = int.Parse(movieYearString);
                string movieName = m.ToString().Substring(0, m.ToString().Length - movieYearString.Length - 1);
                Tuple<string, int> movie = new Tuple<string, int>(movieName, movieYear);
                movies.Add(movie);
            }
            return movies;
        }

        private List<Tuple<string, int>> GetAddedMovies()
        {
            List<Tuple<string, int>> movies = new List<Tuple<string, int>>();
            foreach (var m in CategoryAddedMovies.Items)
            {
                int yearIndex = m.ToString().LastIndexOf(",");
                string movieYearString = m.ToString().Substring(yearIndex + 1).Split(new char[] { '\"' }).First();
                int movieYear = int.Parse(movieYearString);
                string movieName = m.ToString().Substring(0, m.ToString().Length - movieYearString.Length - 1);
                Tuple<string, int> movie = new Tuple<string, int>(movieName, movieYear);
                movies.Add(movie);
            }
            return movies;
        }

        private void CategoryTextBox_TextChanged(object sender, EventArgs e)
        {
            if (categoryService.IsCategoryExisting(this.CategoryNameTextBox.Text) && this.CategoryNameTextBox.Text != category.Name)
            {
                this.CategoryExistsLabel.Show();
                this.CategoryExistsLabel.Text = Constants.WarningMessages.CategoryExists;
            }
            else
            {
                this.CategoryExistsLabel.Hide();
            }
        }

        private void DeleteCategoryButton_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show(Constants.DeleteCategoryMessage, Constants.CinemaDeletePrompt, MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    categoryService.RemoveCategory(category.Name);
                    MessageBox.Show(Constants.SuccessMessages.CategoryDeletesSuccessfully);
                    CategoriesForm categoriesForm = new CategoriesForm();
                    categoriesForm.TopLevel = false;
                    categoriesForm.AutoScroll = true;
                    this.Hide();
                    ((Button)sender).Parent.Parent.Controls.Add(categoriesForm);
                    categoriesForm.Show();
                }
                catch (Exception)
                {
                    MessageBox.Show(Constants.ErrorMessages.CategoriesDeleteMessage);
                }
            }
        }
    }
}