﻿using System;
using System.Drawing;
using System.Windows.Forms;
using SoftCinema.Services;
using SoftCinema.Services.Utilities;

namespace SoftCinema.Client.Forms.ContentHolders
{
    public partial class LoginForm : ContentHolderForm
    {
        private readonly UserService userService;

        public LoginForm()
        {
            this.userService = new UserService();
            if (instance == null)
                InitializeComponent();
        }

        private static LoginForm instance;

        public static LoginForm Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new LoginForm();
                }
                return instance;
            }
        }

        private void loginButton_Click(object sender, EventArgs e)
        {
            var username = this.usernameTextBox.Text;
            var password = this.passwordTextBox.Text;

            try
            {
                if (userService.isUsernamePasswordMatching(username, password) && !userService.IsUserDeleted(username))
                {
                    AuthenticationManager.Login(userService.GetUser(username));
                    MessageBox.Show(Constants.SuccessMessages.SuccessfulLogin);

                    

                    //TopPanelForm.ShowGreetings();
                    //Refresh main form's sidebar
                    var mainForm = (SoftCinemaForm)((Button)sender).Parent.Parent.Parent;
                    mainForm.RenderTopPanelForm();
                    mainForm.RenderSideBar();
                    //Redirect to home page view
                    SoftCinemaForm.SetContentHolderForm(new HomeForm());
                }
                else
                {
                    MessageBox.Show(Constants.ErrorMessages.InvalidLogin);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(Constants.ErrorMessages.InvalidLogin);
            }
            
        }

        private void usernameTextBox_TextChanged(object sender, EventArgs e)
        {
            
            if (!userService.isUsernameExisting(this.usernameTextBox.Text))
            {
                this.usernameInfoLabel.Show();
                this.usernameInfoLabel.Text = Constants.ErrorMessages.NoSuchUserExisting;
            }

            else if (userService.isUsernameExisting(this.usernameTextBox.Text) && userService.IsUserDeleted(this.usernameTextBox.Text))
            {
                this.usernameInfoLabel.Show();
                this.usernameInfoLabel.Text = Constants.ErrorMessages.UserIsInactive;
            }

            else
            {
                this.usernameInfoLabel.Hide();
            }
        }

        private void passwordLabel_Click(object sender, EventArgs e)
        {

        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            
        }
    }
}