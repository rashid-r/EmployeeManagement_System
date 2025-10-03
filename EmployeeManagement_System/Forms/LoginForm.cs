using EmployeeManagementSystem.Services;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;

namespace EmployeeManagementSystem.Forms
{
    public class LoginForm : Form
    {
        private DatabaseService _dbService;
        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnLogin;
        private Button btnSignup;
        private Label lblUsername;
        private Label lblPassword;
        private IContainer components;

        public LoginForm()
        {
            BuildForm();
            _dbService = Program.ServiceProvider.GetRequiredService<DatabaseService>();
        }

        private void BuildForm()
        {
            this.components = new Container();
            this.Text = "Employee Management System - Login";
            this.Size = new Size(400, 300);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            CreateControls();
        }

        private void CreateControls()
        {
            // Username Label
            lblUsername = new Label();
            lblUsername.Text = "Username:";
            lblUsername.Location = new Point(50, 50);
            lblUsername.Size = new Size(100, 20);
            lblUsername.Font = new Font("Segoe UI", 10F, FontStyle.Regular);

            // Username TextBox
            txtUsername = new TextBox();
            txtUsername.Location = new Point(150, 50);
            txtUsername.Size = new Size(200, 25);
            txtUsername.Font = new Font("Segoe UI", 10F, FontStyle.Regular);

            // Password Label
            lblPassword = new Label();
            lblPassword.Text = "Password:";
            lblPassword.Location = new Point(50, 100);
            lblPassword.Size = new Size(100, 20);
            lblPassword.Font = new Font("Segoe UI", 10F, FontStyle.Regular);

            // Password TextBox
            txtPassword = new TextBox();
            txtPassword.Location = new Point(150, 100);
            txtPassword.Size = new Size(200, 25);
            txtPassword.PasswordChar = '*';
            txtPassword.Font = new Font("Segoe UI", 10F, FontStyle.Regular);

            // Login Button
            btnLogin = new Button();
            btnLogin.Text = "Login";
            btnLogin.Location = new Point(150, 150);
            btnLogin.Size = new Size(100, 35);
            btnLogin.BackColor = Color.FromArgb(0, 123, 255);
            btnLogin.ForeColor = Color.White;
            btnLogin.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnLogin.Click += BtnLogin_Click;

            // Signup Button
            btnSignup = new Button();
            btnSignup.Text = "Sign Up";
            btnSignup.Location = new Point(150, 200);
            btnSignup.Size = new Size(100, 35);
            btnSignup.BackColor = Color.FromArgb(108, 117, 125);
            btnSignup.ForeColor = Color.White;
            btnSignup.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            btnSignup.Click += BtnSignup_Click;

            // Add all controls to form
            this.Controls.Add(lblUsername);
            this.Controls.Add(txtUsername);
            this.Controls.Add(lblPassword);
            this.Controls.Add(txtPassword);
            this.Controls.Add(btnLogin);
            this.Controls.Add(btnSignup);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            Login(txtUsername.Text, txtPassword.Text);
        }

        private void BtnSignup_Click(object sender, EventArgs e)
        {
            ShowSignupForm();
        }

        private void Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both username and password.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // REMOVED AWAIT - call method directly
            var result = _dbService.Login(username, password);
            if (result.StartsWith("SUCCESS"))
            {
                MessageBox.Show("Login successful!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Hide();
                var mainForm = new MainForm();
                mainForm.Show();
            }
            else
            {
                MessageBox.Show(result.Replace("ERROR: ", ""), "Login Failed",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowSignupForm()
        {
            this.Hide();
            var signupForm = new SignupForm();
            signupForm.FormClosed += (s, args) => this.Close();
            signupForm.Show();
        }
    }
}