using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SampleCachingApp;
using SampleCachingApp.DynamicFilter;
using SampleCachingApp.Middleware;
using SampleCachingApp.Services;
using SampleCachingApp.Validator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var dbPath = Path.Join(builder.Environment.ContentRootPath, "SampleCachingApp.db");

builder.Services.AddDbContext<SampleCachingAppContext>(options => options.UseSqlite($"Data Source={dbPath}"));

builder.Services.AddScoped<EmployeeService>();
builder.Services.AddScoped<ILogicalExpressionBuilder, LogicalExpressionBuilder>();
builder.Services.AddScoped<IConditionToCodeBuilder, ConditionToCodeBuilder>();

builder.Services.AddMemoryCache();

builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();
builder.Services.AddTransient<IValidator<EmployeeQueryParameters>, EmployeeQueryParametersValidator>();
builder.Services.AddTransient<IValidator<Condition>, ConditionValidator>();

var app = builder.Build();

// Seed data
SeedDatabase(app);

// Configure the HTTP request pipeline.
app.UseAuthorization();

app.MapControllers();
app.UseMiddleware<InvalidateCacheMiddleware>();
app.Run();

void SeedDatabase(WebApplication app)
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<SampleCachingAppContext>();
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        // Check if any employees already exist
        if (!context.Employee.Any())
        {
            var employees = new List<Employee>
            {
                new Employee { Name = "Amit Kumar", Designation = "Manager" },
                new Employee { Name = "Neha Sharma", Designation = "Developer" },
                new Employee { Name = "Ravi Singh", Designation = "Developer" },
                new Employee { Name = "Priya Gupta", Designation = "Tester" },
                new Employee { Name = "Rohit Mehta", Designation = "Designer" },
                new Employee { Name = "Anita Reddy", Designation = "Developer" },
                new Employee { Name = "Vikas Jain", Designation = "Manager" },
                new Employee { Name = "Sneha Patil", Designation = "Developer" },
                new Employee { Name = "Kiran Desai", Designation = "Tester" },
                new Employee { Name = "Manoj Nair", Designation = "Designer" },
                new Employee { Name = "Suresh Pillai", Designation = "Manager" },
                new Employee { Name = "Deepa Iyer", Designation = "Developer" },
                new Employee { Name = "Rahul Rao", Designation = "Tester" },
                new Employee { Name = "Sunita Kapoor", Designation = "Designer" },
                new Employee { Name = "Arun Bhat", Designation = "Developer" },
                new Employee { Name = "Meena Verma", Designation = "Manager" },
                new Employee { Name = "Ajay Khanna", Designation = "Developer" },
                new Employee { Name = "Pooja Chawla", Designation = "Tester" },
                new Employee { Name = "Rajesh Aggarwal", Designation = "Designer" },
                new Employee { Name = "Geeta Joshi", Designation = "Developer" }
            };

            context.Employee.AddRange(employees);
            context.SaveChanges();
        }
    }
}
