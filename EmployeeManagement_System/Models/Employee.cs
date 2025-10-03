using System;
using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementSystem.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [StringLength(50)]
        public string Department { get; set; }

        [Required]
        public DateTime HireDate { get; set; }

        [Required]
        public int AbsentDays { get; set; }

        [Required]
        public decimal MonthlySalary { get; set; }

        [Required]
        public int WorkingDaysPerMonth { get; set; } = 22; // Default working days per month

        // Calculated property for working days
        public int CalculatedWorkingDays
        {
            get
            {
                var totalDaysSinceHire = (DateTime.Now - HireDate).Days;
                return Math.Max(0, totalDaysSinceHire - AbsentDays);
            }
        }

        // Calculated property for current month salary
        public decimal CalculatedSalary
        {
            get
            {
                if (WorkingDaysPerMonth == 0) return 0;

                var workingDaysThisMonth = CalculateWorkingDaysThisMonth();
                var dailyRate = MonthlySalary / WorkingDaysPerMonth;
                return dailyRate * workingDaysThisMonth;
            }
        }

        private int CalculateWorkingDaysThisMonth()
        {
            var today = DateTime.Now;
            var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            // Calculate actual working days (excluding weekends)
            int workingDays = 0;
            for (var date = firstDayOfMonth; date <= lastDayOfMonth; date = date.AddDays(1))
            {
                if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                {
                    workingDays++;
                }
            }

            // Subtract absent days for current month
            // Note: This is a simplified calculation - you might want to track monthly absent days separately
            var absentDaysThisMonth = CalculateAbsentDaysThisMonth();
            return Math.Max(0, workingDays - absentDaysThisMonth);
        }

        private int CalculateAbsentDaysThisMonth()
        {
            // Simplified: return all absent days
            // In a real scenario, you'd track absent days per month
            return AbsentDays;
        }
    }
}