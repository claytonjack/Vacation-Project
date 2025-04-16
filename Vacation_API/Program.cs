using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VacationBooking.Data;
using Microsoft.AspNetCore.Identity;
using VacationBooking.Models;
using Microsoft.Extensions.FileProviders;
using System.IO;

namespace Vacation_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
                options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
            });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<VacationDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddIdentity<User, IdentityRole>(options => {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 8;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;

                options.User.RequireUniqueEmail = true;
            })
                .AddEntityFrameworkStores<VacationDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins(builder.Configuration["AllowedOrigins"].Split(','))
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("AllowFrontend");

            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();
            
            
            var imagesPath = Path.Combine(app.Environment.ContentRootPath, "images");
            Directory.CreateDirectory(imagesPath);

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(app.Environment.ContentRootPath, "images", "destination")),
                RequestPath = "/images/destination"
            });

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(app.Environment.ContentRootPath, "images", "vacation")),
                RequestPath = "/images/vacation"
            });

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(app.Environment.ContentRootPath, "images", "accommodation")),
                RequestPath = "/images/accommodation"
            });
            
            app.Use(async (context, next) => 
            {
                if (context.Request.Path.Value.Contains("/images/"))
                {
                    var filePath = Path.Combine(imagesPath, context.Request.Path.Value.Replace("/images/", "").Replace("/", Path.DirectorySeparatorChar.ToString()));
                    Console.WriteLine($"Image request: {context.Request.Path}");
                    Console.WriteLine($"Looking for file: {filePath}");
                    Console.WriteLine($"File exists: {File.Exists(filePath)}");
                }
                await next();
            });

            app.Use(async (context, next) => 
            {
                if (context.Request.Path.Value.Contains("bcn-") || context.Request.Path.Value.Contains("barcelona-"))
                {
                    string physicalPath = Path.Combine(
                        app.Environment.ContentRootPath, 
                        "images", 
                        context.Request.Path.Value.TrimStart('/').Replace("images/", ""));
                    
                    Console.WriteLine($"Barcelona image request: {context.Request.Path}");
                    Console.WriteLine($"Physical path: {physicalPath}");
                    Console.WriteLine($"File exists: {File.Exists(physicalPath)}");
                    
                    if (File.Exists(physicalPath))
                    {
                        var fileInfo = new FileInfo(physicalPath);
                        Console.WriteLine($"File size: {fileInfo.Length} bytes");
                        Console.WriteLine($"File last write: {fileInfo.LastWriteTime}");
                    }
                }
                await next();
            });

            app.Use(async (context, next) =>
            {
                var originalPath = context.Request.Path;
                
                await next();
                
                if (context.Response.StatusCode == 404 && originalPath.Value.Contains("barcelona"))
                {
                    Console.WriteLine($"404 for: {originalPath}");
                    Console.WriteLine($"Physical path would be: {Path.Combine(app.Environment.ContentRootPath, originalPath.Value.TrimStart('/'))}");
                }
            });

            app.MapControllers();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<VacationDbContext>();
                    var userManager = services.GetRequiredService<UserManager<User>>();
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                    
                    context.Database.EnsureCreated();
                    
                    string[] roleNames = { "Admin", "User" };
                    foreach (var roleName in roleNames)
                    {
                        if (!roleManager.RoleExistsAsync(roleName).Result)
                        {
                            roleManager.CreateAsync(new IdentityRole(roleName)).Wait();
                        }
                    }
                    
                    DbInitializer.Initialize(context, userManager);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }

            app.Run();
        }
    }
}
