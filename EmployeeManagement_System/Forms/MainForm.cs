using EmployeeManagementSystem.Models;
using EmployeeManagementSystem.Services;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;

namespace EmployeeManagementSystem.Forms
{
    public class MainForm : Form
    {
        private DatabaseService _dbService;
        private DataGridView dgvEmployees;
        private Button btnAddEmployee;
        private Button btnExport;
        private Button btnLogout;
        private ContextMenuStrip contextMenu;
        private IContainer components;

        public MainForm()
        {
            BuildForm();
            _dbService = Program.ServiceProvider.GetRequiredService<DatabaseService>();
            LoadEmployees();
            AddContextMenu();
        }

        private void BuildForm()
        {
            this.components = new Container();
            this.Text = "Employee Management System";
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            CreateControls();
        }

        private void CreateControls()
        {
            // Data Grid View
            dgvEmployees = new DataGridView();
            dgvEmployees.Location = new Point(20, 60);
            dgvEmployees.Size = new Size(940, 400);
            dgvEmployees.AutoGenerateColumns = false;
            dgvEmployees.ReadOnly = true;
            dgvEmployees.AllowUserToAddRows = false;
            dgvEmployees.AllowUserToDeleteRows = false;
            dgvEmployees.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvEmployees.MultiSelect = false;
            dgvEmployees.CellDoubleClick += DgvEmployees_CellDoubleClick;

            // Add columns to DataGridView
            dgvEmployees.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Name",
                HeaderText = "Name",
                Width = 150
            });
            dgvEmployees.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Email",
                HeaderText = "Email",
                Width = 200
            });
            dgvEmployees.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Department",
                HeaderText = "Department",
                Width = 120
            });
            dgvEmployees.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "HireDate",
                HeaderText = "Hire Date",
                Width = 100
            });
            dgvEmployees.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "AbsentDays",
                HeaderText = "Absent Days",
                Width = 80
            });
            dgvEmployees.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "CalculatedWorkingDays",
                HeaderText = "Working Days",
                Width = 80
            });
            dgvEmployees.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "MonthlySalary",
                HeaderText = "Monthly Salary",
                Width = 100
            });
            dgvEmployees.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "CalculatedSalary",
                HeaderText = "Calculated Salary",
                Width = 100
            });

            // Add Employee Button
            btnAddEmployee = new Button();
            btnAddEmployee.Text = "Add Employee";
            btnAddEmployee.Location = new Point(20, 20);
            btnAddEmployee.Size = new Size(120, 35);
            btnAddEmployee.BackColor = Color.FromArgb(0, 123, 255);
            btnAddEmployee.ForeColor = Color.White;
            btnAddEmployee.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnAddEmployee.Click += BtnAddEmployee_Click;

            // Export Button
            btnExport = new Button();
            btnExport.Text = "Export to CSV";
            btnExport.Location = new Point(150, 20);
            btnExport.Size = new Size(120, 35);
            btnExport.BackColor = Color.FromArgb(40, 167, 69);
            btnExport.ForeColor = Color.White;
            btnExport.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnExport.Click += BtnExport_Click;

            // Logout Button
            btnLogout = new Button();
            btnLogout.Text = "Logout";
            btnLogout.Location = new Point(280, 20);
            btnLogout.Size = new Size(120, 35);
            btnLogout.BackColor = Color.FromArgb(220, 53, 69);
            btnLogout.ForeColor = Color.White;
            btnLogout.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnLogout.Click += BtnLogout_Click;

            // Add all controls to form
            this.Controls.Add(dgvEmployees);
            this.Controls.Add(btnAddEmployee);
            this.Controls.Add(btnExport);
            this.Controls.Add(btnLogout);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void LoadEmployees()
        {
            var employees = _dbService.GetAllEmployees();
            dgvEmployees.DataSource = employees;
        }

        private void BtnAddEmployee_Click(object sender, EventArgs e)
        {
            var employeeForm = new EmployeeForm();
            employeeForm.FormClosed += (s, args) => LoadEmployees();
            employeeForm.ShowDialog();
        }

        private void DgvEmployees_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var employee = dgvEmployees.Rows[e.RowIndex].DataBoundItem as Employee;
                if (employee != null)
                {
                    var employeeForm = new EmployeeForm(employee);
                    employeeForm.FormClosed += (s, args) => LoadEmployees();
                    employeeForm.ShowDialog();
                }
            }
        }

        private void AddContextMenu()
        {
            contextMenu = new ContextMenuStrip();
            var editItem = new ToolStripMenuItem("Edit Employee");
            var deleteItem = new ToolStripMenuItem("Delete Employee");
            var absentDaysItem = new ToolStripMenuItem("Update Absent Days");

            editItem.Click += (s, e) => {
                if (dgvEmployees.CurrentRow?.DataBoundItem is Employee employee)
                {
                    var employeeForm = new EmployeeForm(employee);
                    employeeForm.FormClosed += (s, args) => LoadEmployees();
                    employeeForm.ShowDialog();
                }
            };

            deleteItem.Click += (s, e) => {
                if (dgvEmployees.CurrentRow?.DataBoundItem is Employee employee)
                {
                    var result = MessageBox.Show($"Are you sure you want to delete {employee.Name}?",
                        "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (result == DialogResult.Yes)
                    {
                        var deleteResult = _dbService.DeleteEmployee(employee.Id);
                        if (deleteResult.StartsWith("SUCCESS"))
                        {
                            LoadEmployees();
                            MessageBox.Show("Employee deleted successfully", "Success",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show(deleteResult.Replace("ERROR: ", ""), "Delete Failed",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            };

            absentDaysItem.Click += (s, e) => {
                if (dgvEmployees.CurrentRow?.DataBoundItem is Employee employee)
                {
                    ShowUpdateAbsentDaysForm(employee);
                }
            };

            contextMenu.Items.Add(editItem);
            contextMenu.Items.Add(deleteItem);
            contextMenu.Items.Add(new ToolStripSeparator());
            contextMenu.Items.Add(absentDaysItem);
            dgvEmployees.ContextMenuStrip = contextMenu;
        }

        private void ShowUpdateAbsentDaysForm(Employee employee)
        {
            var form = new Form();
            form.Text = "Update Absent Days";
            form.Size = new Size(300, 200);
            form.StartPosition = FormStartPosition.CenterScreen;
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.MaximizeBox = false;

            var lblInfo = new Label();
            lblInfo.Text = $"Employee: {employee.Name}";
            lblInfo.Location = new Point(20, 20);
            lblInfo.Size = new Size(250, 20);
            lblInfo.Font = new Font("Segoe UI", 10F, FontStyle.Bold);

            var lblAbsentDays = new Label();
            lblAbsentDays.Text = "Absent Days:";
            lblAbsentDays.Location = new Point(20, 60);
            lblAbsentDays.Size = new Size(100, 20);
            lblAbsentDays.Font = new Font("Segoe UI", 10F, FontStyle.Regular);

            var txtAbsentDays = new NumericUpDown();
            txtAbsentDays.Location = new Point(120, 60);
            txtAbsentDays.Size = new Size(100, 25);
            txtAbsentDays.Value = employee.AbsentDays;
            txtAbsentDays.Minimum = 0;
            txtAbsentDays.Maximum = 365;

            var btnUpdate = new Button();
            btnUpdate.Text = "Update";
            btnUpdate.Location = new Point(60, 100);
            btnUpdate.Size = new Size(80, 30);
            btnUpdate.BackColor = Color.FromArgb(40, 167, 69);
            btnUpdate.ForeColor = Color.White;
            btnUpdate.Click += (s, e) => {
                var result = _dbService.UpdateAbsentDays(employee.Id, (int)txtAbsentDays.Value);
                if (result.StartsWith("SUCCESS"))
                {
                    MessageBox.Show("Absent days updated successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadEmployees();
                    form.Close();
                }
                else
                {
                    MessageBox.Show(result.Replace("ERROR: ", ""), "Update Failed",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            var btnCancel = new Button();
            btnCancel.Text = "Cancel";
            btnCancel.Location = new Point(150, 100);
            btnCancel.Size = new Size(80, 30);
            btnCancel.BackColor = Color.FromArgb(108, 117, 125);
            btnCancel.ForeColor = Color.White;
            btnCancel.Click += (s, e) => form.Close();

            form.Controls.Add(lblInfo);
            form.Controls.Add(lblAbsentDays);
            form.Controls.Add(txtAbsentDays);
            form.Controls.Add(btnUpdate);
            form.Controls.Add(btnCancel);

            form.ShowDialog();
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            var result = _dbService.ExportEmployeesToCsv();
            if (result.StartsWith("SUCCESS"))
            {
                MessageBox.Show("Export successful!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(result.Replace("ERROR: ", ""), "Export Failed",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            _dbService.Logout();
            this.Close();
        }
    }
}