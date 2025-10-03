using EmployeeManagementSystem.Services;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;

namespace EmployeeManagementSystem.Forms
{
    public class SignupForm : Form
    {
        private DatabaseService _dbService;
        private TextBox txtUsername, txtEmail, txtPassword;
        private Button btnSignup, btnBack;
        private Label lblUsername, lblEmail, lblPassword;
        private IContainer components;

        public SignupForm()
        {
            BuildForm();
            _dbService = Program.ServiceProvider.GetRequiredService<DatabaseService>();
        }

        private void BuildForm()
        {
            this.components = new Container();
            this.Text = "Employee Management System - Sign Up";
            this.Size = new Size(400, 350);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            CreateControls();
        }

        private void CreateControls()
        {
            int y = 50;

            // Username Label
            lblUsername = new Label();
            lblUsername.Text = "Username:";
            lblUsername.Location = new Point(50, y);
            lblUsername.Size = new Size(100, 20);
            lblUsername.Font = new Font("Segoe UI", 10F, FontStyle.Regular);

            // Username TextBox
            txtUsername = new TextBox();
            txtUsername.Location = new Point(150, y);
            txtUsername.Size = new Size(200, 25);
            txtUsername.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            y += 40;

            // Email Label
            lblEmail = new Label();
            lblEmail.Text = "Email:";
            lblEmail.Location = new Point(50, y);
            lblEmail.Size = new Size(100, 20);
            lblEmail.Font = new Font("Segoe UI", 10F, FontStyle.Regular);

            // Email TextBox
            txtEmail = new TextBox();
            txtEmail.Location = new Point(150, y);
            txtEmail.Size = new Size(200, 25);
            txtEmail.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            y += 40;

            // Password Label
            lblPassword = new Label();
            lblPassword.Text = "Password:";
            lblPassword.Location = new Point(50, y);
            lblPassword.Size = new Size(100, 20);
            lblPassword.Font = new Font("Segoe UI", 10F, FontStyle.Regular);

            // Password TextBox
            txtPassword = new TextBox();
            txtPassword.Location = new Point(150, y);
            txtPassword.Size = new Size(200, 25);
            txtPassword.PasswordChar = '*';
            txtPassword.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            y += 50;

            // Sign Up Button
            btnSignup = new Button();
            btnSignup.Text = "Sign Up";
            btnSignup.Location = new Point(150, y);
            btnSignup.Size = new Size(100, 35);
            btnSignup.BackColor = Color.FromArgb(40, 167, 69);
            btnSignup.ForeColor = Color.White;
            btnSignup.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnSignup.Click += BtnSignup_Click;
            y += 50;

            // Back Button
            btnBack = new Button();
            btnBack.Text = "Back to Login";
            btnBack.Location = new Point(150, y);
            btnBack.Size = new Size(100, 35);
            btnBack.BackColor = Color.FromArgb(108, 117, 125);
            btnBack.ForeColor = Color.White;
            btnBack.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            btnBack.Click += BtnBack_Click;

            // Add all controls to form
            this.Controls.Add(lblUsername);
            this.Controls.Add(txtUsername);
            this.Controls.Add(lblEmail);
            this.Controls.Add(txtEmail);
            this.Controls.Add(lblPassword);
            this.Controls.Add(txtPassword);
            this.Controls.Add(btnSignup);
            this.Controls.Add(btnBack);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void BtnSignup_Click(object sender, EventArgs e)
        {
            Signup();
        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Signup()
        {
            if (string.IsNullOrEmpty(txtUsername.Text) || string.IsNullOrEmpty(txtPassword.Text) || string.IsNullOrEmpty(txtEmail.Text))
            {
                MessageBox.Show("Please fill in all fields.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // REMOVED AWAIT - call method directly
            var result = _dbService.Signup(txtUsername.Text, txtPassword.Text, txtEmail.Text);
            if (result.StartsWith("SUCCESS"))
            {
                MessageBox.Show("Registration successful! Please login.", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show(result.Replace("ERROR: ", ""), "Registration Failed",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}