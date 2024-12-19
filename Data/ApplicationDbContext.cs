﻿using Azure;
using BerberSite.Models;
using Microsoft.EntityFrameworkCore;

namespace BerberSite.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Operation> Operations { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<EmployeeOperation> EmployeeOperations { get; set; }
        public DbSet<WorkingHour> WorkingHours { get; set; }
    }
}
