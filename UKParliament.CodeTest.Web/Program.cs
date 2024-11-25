using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using System;
using UKParliament.CodeTest.Data;
using UKParliament.CodeTest.Data.Repositories;
using UKParliament.CodeTest.Services;
using UKParliament.CodeTest.Web.Validators;
using UKParliament.CodeTest.Web.ViewModels;


namespace UKParliament.CodeTest.Web;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddAutoMapper(typeof(MappingProfile));

        builder.Services.AddControllersWithViews();

        builder.Services.AddDbContext<PersonManagerContext>(op => op.UseInMemoryDatabase("PersonManager"));

        builder.Services.AddScoped<IPersonRepository, PersonRepository>();
        builder.Services.AddScoped<IPersonService, PersonService>();
        builder.Services.AddTransient<IValidator<PersonViewModel>, PersonViewModelValidator>();

        builder.Services.AddSwaggerGen();

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins("*").AllowAnyMethod().AllowAnyHeader(); // any origin - dev only
            });
        });

        var app = builder.Build();

        // Create database so the data seeds
        using (var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
        {
            using var context = serviceScope.ServiceProvider.GetRequiredService<PersonManagerContext>();
            context.Database.EnsureCreated();
        }

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        else
        {
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();  
        }

        app.UseCors();
        app.UseHttpsRedirection();

        // dom has added these options - to work without manual intervention
        var clientAppBuildPath = Path.Combine(Directory.GetCurrentDirectory(), "clientapp", "build");

        if (!Directory.Exists(clientAppBuildPath))
        {
            Directory.CreateDirectory(clientAppBuildPath);
        }

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(clientAppBuildPath),
            RequestPath = ""
        });

        app.UseRouting();
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller}/{action=Index}/{id?}");

        app.MapFallbackToFile("index.html");

        app.Run();
    }
}