using EmployeeManagementSystem.Models;
using EmployeeManagementSystem.Services;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;

namespace EmployeeManagementSystem.Forms
{
    public class EmployeeForm : Form
    {
        private DatabaseService _dbService;
        private Employee _employee;
        private bool _isEditMode;

        private TextBox txtName, txtEmail, txtDepartment, txtMonthlySalary, txtWorkingDays;
        private DateTimePicker dtpHireDate;
        private Button btnSave, btnCancel;
        private Label lblTitle;
        private IContainer components;

        public EmployeeForm(Employee employee = null)
        {
            _employee = employee;
            _isEditMode = employee != null;
            BuildForm();
            _dbService = Program.ServiceProvider.GetRequiredService<DatabaseService>();
        }

        private void BuildForm()
        {
            this.components = new Container();
            this.Text = _isEditMode ? "Edit Employee" : "Add Employee";
            this.Size = new Size(500, 450);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            CreateControls();
            if (_isEditMode)
            {
                LoadEmployeeData();
            }
        }

        private void CreateControls()
        {
            int y = 20;

            // Title Label
            lblTitle = new Label();
            lblTitle.Text = _isEditMode ? "Edit Employee" : "Add New Employee";
            lblTitle.Location = new Point(20, y);
            lblTitle.Size = new Size(400, 30);
            lblTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblTitle.TextAlign = ContentAlignment.MiddleLeft;
            y += 40;

            // Name
            AddLabel("Name:", 20, y);
            txtName = AddTextBox(150, y, 300);
            y += 40;

            // Email
            AddLabel("Email:", 20, y);
            txtEmail = AddTextBox(150, y, 300);
            y += 40;

            // Department
            AddLabel("Department:", 20, y);
            txtDepartment = AddTextBox(150, y, 300);
            y += 40;

            // Monthly Salary
            AddLabel("Monthly Salary:", 20, y);
            txtMonthlySalary = AddTextBox(150, y, 150);
            txtMonthlySalary.Text = "0";
            y += 40;

            // Working Days Per Month
            AddLabel("Working Days/Month:", 20, y);
            txtWorkingDays = AddTextBox(150, y, 150);
            txtWorkingDays.Text = "22";
            y += 40;

            // Hire Date
            AddLabel("Hire Date:", 20, y);
            dtpHireDate = new DateTimePicker();
            dtpHireDate.Location = new Point(150, y);
            dtpHireDate.Size = new Size(150, 25);
            dtpHireDate.Value = DateTime.Now;
            this.Controls.Add(dtpHireDate);
            y += 50;

            // Save Button
            btnSave = new Button();
            btnSave.Text = _isEditMode ? "Update" : "Save";
            btnSave.Location = new Point(150, y);
            btnSave.Size = new Size(100, 35);
            btnSave.BackColor = Color.FromArgb(40, 167, 69);
            btnSave.ForeColor = Color.White;
            btnSave.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnSave.Click += BtnSave_Click;

            // Cancel Button
            btnCancel = new Button();
            btnCancel.Text = "Cancel";
            btnCancel.Location = new Point(260, y);
            btnCancel.Size = new Size(100, 35);
            btnCancel.BackColor = Color.FromArgb(108, 117, 125);
            btnCancel.ForeColor = Color.White;
            btnCancel.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            btnCancel.Click += (s, e) => this.Close();

            this.Controls.Add(lblTitle);
            this.Controls.Add(btnSave);
            this.Controls.Add(btnCancel);
        }

        private void AddLabel(string text, int x, int y)
        {
            var label = new Label();
            label.Text = text;
            label.Location = new Point(x, y);
            label.Size = new Size(120, 25);
            label.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            label.TextAlign = ContentAlignment.MiddleLeft;
            this.Controls.Add(label);
        }

        private TextBox AddTextBox(int x, int y, int width)
        {
            var textBox = new TextBox();
            textBox.Location = new Point(x, y);
            textBox.Size = new Size(width, 25);
            textBox.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            this.Controls.Add(textBox);
            return textBox;
        }

        private void LoadEmployeeData()
        {
            if (_employee != null)
            {
                txtName.Text = _employee.Name;
                txtEmail.Text = _employee.Email;
                txtDepartment.Text = _employee.Department;
                txtMonthlySalary.Text = _employee.MonthlySalary.ToString();
                txtWorkingDays.Text = _employee.WorkingDaysPerMonth.ToString();
                dtpHireDate.Value = _employee.HireDate;
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs())
                return;

            string result;
            if (_isEditMode)
            {
                result = _dbService.UpdateEmployee(
                    _employee.Id,
                    txtName.Text.Trim(),
                    txtEmail.Text.Trim(),
                    txtDepartment.Text.Trim(),
                    dtpHireDate.Value,
                    decimal.Parse(txtMonthlySalary.Text),
                    int.Parse(txtWorkingDays.Text)
                );
            }
            else
            {
                result = _dbService.AddEmployee(
                    txtName.Text.Trim(),
                    txtEmail.Text.Trim(),
                    txtDepartment.Text.Trim(),
                    dtpHireDate.Value,
                    decimal.Parse(txtMonthlySalary.Text),
                    int.Parse(txtWorkingDays.Text)
                );
            }

            if (result.StartsWith("SUCCESS"))
            {
                MessageBox.Show(result.Replace("SUCCESS: ", ""), "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show(result.Replace("ERROR: ", ""), "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Please enter employee name", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Please enter employee email", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtDepartment.Text))
            {
                MessageBox.Show("Please enter department", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!decimal.TryParse(txtMonthlySalary.Text, out decimal salary) || salary < 0)
            {
                MessageBox.Show("Please enter a valid monthly salary", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!int.TryParse(txtWorkingDays.Text, out int workingDays) || workingDays <= 0 || workingDays > 31)
            {
                MessageBox.Show("Please enter valid working days per month (1-31)", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}