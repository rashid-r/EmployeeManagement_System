using System;
using System.Collections.Generic;
using System.Linq;
using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Models;
using EmployeeManagementSystem.Services;

namespace EmployeeManagementSystem.Services
{
    public class DatabaseService
    {
        private readonly AppDbContext _context;
        private int? _currentUserId;

        public DatabaseService(AppDbContext context)
        {
            _context = context;
            _context.Database.EnsureCreated();
        }

        public string Signup(string username, string password, string email)
        {
            try
            {
                // Check if user already exists
                if (_context.Users.Any(u => u.Username == username))
                {
                    return "ERROR: Username already exists";
                }

                if (_context.Users.Any(u => u.Email == email))
                {
                    return "ERROR: Email already registered";
                }

                // Hash password
                string hashedPassword = BCryptSimulatedHasher.HashPassword(password);

                // Create new user
                var newUser = new User
                {
                    Username = username,
                    PasswordHash = hashedPassword,
                    Email = email
                };

                _context.Users.Add(newUser);
                _context.SaveChanges();

                _currentUserId = newUser.Id;
                return "SUCCESS: User registered successfully";
            }
            catch (Exception ex)
            {
                return $"ERROR: Registration failed - {ex.Message}";
            }
        }

        public string Login(string username, string password)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.Username == username);
                if (user == null)
                {
                    return "ERROR: Invalid username or password";
                }

                if (BCryptSimulatedHasher.VerifyPassword(password, user.PasswordHash))
                {
                    _currentUserId = user.Id;
                    return "SUCCESS: Login successful";
                }
                else
                {
                    return "ERROR: Invalid username or password";
                }
            }
            catch (Exception ex)
            {
                return $"ERROR: Login failed - {ex.Message}";
            }
        }

        public void Logout()
        {
            _currentUserId = null;
        }

        public bool IsUserLoggedIn()
        {
            return _currentUserId.HasValue;
        }

        public string AddEmployee(string name, string email, string department, DateTime hireDate, decimal monthlySalary, int workingDaysPerMonth = 22)
        {
            try
            {
                if (!IsUserLoggedIn())
                {
                    return "ERROR: User not logged in";
                }

                // Check if email already exists
                if (_context.Employees.Any(e => e.Email == email))
                {
                    return "ERROR: Employee with this email already exists";
                }

                var employee = new Employee
                {
                    Name = name,
                    Email = email,
                    Department = department,
                    HireDate = hireDate,
                    AbsentDays = 0,
                    MonthlySalary = monthlySalary,
                    WorkingDaysPerMonth = workingDaysPerMonth
                };

                _context.Employees.Add(employee);
                _context.SaveChanges();

                return "SUCCESS: Employee added successfully";
            }
            catch (Exception ex)
            {
                return $"ERROR: Failed to add employee - {ex.Message}";
            }
        }

        public List<Employee> GetAllEmployees()
        {
            if (!IsUserLoggedIn())
            {
                return new List<Employee>();
            }

            return _context.Employees.ToList();
        }

        public Employee GetEmployeeById(int id)
        {
            try
            {
                if (!IsUserLoggedIn())
                {
                    return null;
                }

                return _context.Employees.Find(id);
            }
            catch (Exception ex)
            {
                // Log error
                return null;
            }
        }

        public string UpdateEmployee(int id, string name, string email, string department, DateTime hireDate, decimal monthlySalary, int workingDaysPerMonth)
        {
            try
            {
                if (!IsUserLoggedIn())
                {
                    return "ERROR: User not logged in";
                }

                var employee = _context.Employees.Find(id);
                if (employee == null)
                {
                    return "ERROR: Employee not found";
                }

                // Check if email already exists for other employees
                if (_context.Employees.Any(e => e.Email == email && e.Id != id))
                {
                    return "ERROR: Another employee with this email already exists";
                }

                employee.Name = name;
                employee.Email = email;
                employee.Department = department;
                employee.HireDate = hireDate;
                employee.MonthlySalary = monthlySalary;
                employee.WorkingDaysPerMonth = workingDaysPerMonth;

                _context.SaveChanges();
                return "SUCCESS: Employee updated successfully";
            }
            catch (Exception ex)
            {
                return $"ERROR: Failed to update employee - {ex.Message}";
            }
        }

        public string DeleteEmployee(int id)
        {
            try
            {
                if (!IsUserLoggedIn())
                {
                    return "ERROR: User not logged in";
                }

                var employee = _context.Employees.Find(id);
                if (employee == null)
                {
                    return "ERROR: Employee not found";
                }

                _context.Employees.Remove(employee);
                _context.SaveChanges();
                return "SUCCESS: Employee deleted successfully";
            }
            catch (Exception ex)
            {
                return $"ERROR: Failed to delete employee - {ex.Message}";
            }
        }

        public string UpdateAbsentDays(int employeeId, int newAbsentDays)
        {
            try
            {
                if (!IsUserLoggedIn())
                {
                    return "ERROR: User not logged in";
                }

                if (newAbsentDays < 0)
                {
                    return "ERROR: Absent days cannot be negative";
                }

                var employee = _context.Employees.Find(employeeId);
                if (employee == null)
                {
                    return "ERROR: Employee not found";
                }

                var totalDaysSinceHire = (DateTime.Now - employee.HireDate).Days;
                if (newAbsentDays > totalDaysSinceHire)
                {
                    return $"ERROR: Absent days ({newAbsentDays}) cannot exceed total days since hire ({totalDaysSinceHire})";
                }

                employee.AbsentDays = newAbsentDays;
                _context.SaveChanges();

                return "SUCCESS: Absent days updated successfully";
            }
            catch (Exception ex)
            {
                return $"ERROR: Failed to update absent days - {ex.Message}";
            }
        }

        public string ExportEmployeesToCsv()
        {
            try
            {
                if (!IsUserLoggedIn())
                {
                    return "ERROR: User not logged in";
                }

                var employees = GetAllEmployees();
                var csvContent = new List<string>
                {
                    "Id,Name,Email,Department,HireDate,AbsentDays,WorkingDays,MonthlySalary,CalculatedSalary"
                };

                foreach (var emp in employees)
                {
                    csvContent.Add(
                        $"{emp.Id},{emp.Name},{emp.Email},{emp.Department},{emp.HireDate:yyyy-MM-dd},{emp.AbsentDays},{emp.CalculatedWorkingDays},{emp.MonthlySalary},{emp.CalculatedSalary}"
                    );
                }

                string csvData = string.Join(Environment.NewLine, csvContent);
                return $"SUCCESS:{csvData}";
            }
            catch (Exception ex)
            {
                return $"ERROR: Export failed - {ex.Message}";
            }
        }

        // Calculate salary for specific employee
        public decimal CalculateSalary(int employeeId, int absentDays)
        {
            var employee = GetEmployeeById(employeeId);
            if (employee == null || employee.WorkingDaysPerMonth == 0)
                return 0;

            var dailyRate = employee.MonthlySalary / employee.WorkingDaysPerMonth;
            var workingDays = employee.WorkingDaysPerMonth - absentDays;
            return dailyRate * Math.Max(0, workingDays);
        }
    }
}