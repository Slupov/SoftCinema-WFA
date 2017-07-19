﻿using SoftCinema.Services;
using SoftCinema.Services.Utilities;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace SoftCinema.Client.Forms.ContentHolders
{
    public partial class RegisterForm : ContentHolderForm
    {
        private static RegisterForm instance;
        private byte[] movieImageBytes { get; set; }
        private readonly ImageService imageService;
        private readonly UserService userService;

        public static RegisterForm Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new RegisterForm();
                }
                return instance;
            }
        }

        public RegisterForm()
        {
            this.imageService = new ImageService();
            this.userService = new UserService();
            if (instance == null)
            {
                InitializeComponent();
            }
        }

        private void registerButton_Click(object sender, EventArgs e)
        {
            var username = this.usernameTextBox.Text;
            var password = this.passwordTextBox.Text;
            var repeatPassword = this.repeatPasswordTextBox.Text;
            var email = this.emailTextBox.Text;
            var phone = this.phoneNumberTextBox.Text;
            var image = Image.FromFile(@"../../Utilities/Images/default.jpg");
            var scaledImage = imageService.ScaleImage(image, 55, 52);
            this.profilePictureBox.Width = scaledImage.Width;
            this.profilePictureBox.Height = scaledImage.Height;

            var profilePic = imageService.imageToByteArray(this.profilePictureBox.Image ?? scaledImage);

            bool isDataValid = userService.isUserValid(username, password, repeatPassword, email, phone);

            if (isDataValid)
            {
                userService.AddUser(username, password, email, phone, profilePic);
                MessageBox.Show(Constants.SuccessMessages.SuccessfulRegister);
                var mainForm = (SoftCinemaForm)((Button)sender).Parent.Parent.Parent;
                mainForm.RenderSideBar();

                //Redirect to home page view
                SoftCinemaForm.SetContentHolderForm(new LoginForm());
            }
        }

        private void usernameTextBox_TextChanged(object sender, EventArgs e)
        {
            if (this.usernameTextBox.Text.Length > 25 || this.usernameTextBox.Text.Length < 3)
            {
                this.usernameInfoLabel.Show();
                this.usernameInfoLabel.Text = "Username must be in range 3-25 characters";
            }
            else
            {
                if (userService.isUsernameExisting(this.usernameTextBox.Text))
                {
                    this.usernameInfoLabel.Show();
                    this.usernameInfoLabel.Text = "Username already taken";
                }
                else
                {
                    this.usernameInfoLabel.Hide();
                }
            }
        }

        private void passwordTextBox_TextChanged(object sender, EventArgs e)
        {
            if (!userService.isPasswordValid(this.passwordTextBox.Text))
            {
                this.passwordInfoLabel.Show();
                this.passwordInfoLabel.Text = "Password length must be in range 3-25 characters!";
            }
            else
            {
                this.passwordInfoLabel.Hide();
            }
        }

        private void repeatPasswordTextBox_TextChanged(object sender, EventArgs e)
        {
            if (this.repeatPasswordTextBox.Text != this.passwordTextBox.Text)
            {
                this.rptpasswordInfoLabel.Show();
                this.rptpasswordInfoLabel.Text = "Passwords do no match";
            }
            else
            {
                this.rptpasswordInfoLabel.Hide();
            }
        }

        private void emailTextBox_TextChanged(object sender, EventArgs e)
        {
            if (!userService.isEmailValid(this.emailTextBox.Text))
            {
                this.emailInfoLabel.Show();
                this.emailInfoLabel.Text = "Email is invalid";
            }
            else
            {
                this.emailInfoLabel.Hide();
            }
        }

        private void phoneNumberTextBox_TextChanged(object sender, EventArgs e)
        {
            if (!userService.isPhoneValid(this.phoneNumberTextBox.Text))
            {
                this.phoneInfoLabel.Show();
                this.phoneInfoLabel.Text = "Phone must be in 08[789]... format!";
            }
            else
            {
                this.phoneInfoLabel.Hide();
            }
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Please select a photo";
            ofd.Filter = "PNG|*.png|JPG|*.jpg|GIF|*gif";
            ofd.Multiselect = false;

            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var image = System.Drawing.Image.FromFile(ofd.FileName);
                var scaledImage = imageService.ScaleImage(image, 138, 161);
                this.profilePictureBox.Size = new Size(scaledImage.Size.Width, scaledImage.Size.Height);

                this.profilePictureBox.Image = scaledImage;

                this.movieImageBytes = imageService.imageToByteArray(scaledImage);
            }
        }
    }
}