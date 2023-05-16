using ChatApp.Business.Helpers;
using ChatApp.Business.ServiceInterfaces;
using ChatApp.Context;
using ChatApp.Hubs;
using ChatApp.Infrastructure.ServiceImplementation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using ChatApp.Context.EntityClasses;
using Microsoft.AspNetCore.Identity;

namespace ChatApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ChatAppContext>(options => {

                options.UseSqlServer(this.Configuration.GetConnectionString("Default"));
                options.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()));
                options.EnableSensitiveDataLogging();
            });
            // Default Policy
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.WithOrigins("https://localhost:44351", "http://localhost:4200")
                                            .AllowAnyHeader()
                                            .AllowAnyMethod();
                    });
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
             .AddJwtBearer(options =>
             {
                 
                 options.TokenValidationParameters = new TokenValidationParameters
                 {
                     ValidateIssuer = true,
                     ValidateAudience = true,
                     ValidateLifetime = true,
                     ValidateIssuerSigningKey = true,
                     ValidIssuer = Configuration["Jwt:Issuer"],
                     ValidAudience = Configuration["Jwt:Issuer"],
                     IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                 };
             });

            
            services.AddAuthorization( options =>
            {
                
                options.AddPolicy("Employee", policy =>
                {
                    string[] employees = new string[]
                    {
                        "Programmer Analyst",
                        "Solution Analyst",
                        "Lead Solution Analyst", 
                        "Group Lead",
                        "Group Director",
                        "Chief Technical Officer",
                        "Chief Executive Officer"
                    };
                    policy.RequireClaim(ClaimsConstant.DesignationClaim, employees);
                });
                options.AddPolicy("Admin", policy =>
                {
                    policy.RequireClaim(ClaimsConstant.DesignationClaim, "Chief Technical Officer", "Chief Executive Officer");
                });
            });

            services.AddControllersWithViews();

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer",
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    },
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Please insert Subscription Reference with Bearer into field",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                });

                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Chat App API", Version = "v1" });
            });

            // Service Registration
            services.AddScoped<IProfileService, ProfileService>();
            services.AddScoped<IDesignationService, DesignationService>();
            services.AddScoped<IChatService, ChatService>();
            services.AddScoped<IOnlineUserService, OnlineUserService>();
            services.AddScoped<IGroupService, GroupService>();
            services.AddScoped<IGroupMemberService, GroupMemberService>();
            services.AddScoped<IGroupMessageService, GroupMessageService>();
            services.AddScoped<INotificationService, NotificationService>();
            //services.AddScoped<IGroupService, GroupService>();
            services.AddSignalR();

            // In production, the Angular files will be served from this directory

            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "MainApp/dist";
            });



            // Uncomment this to enable template app.
            //services.AddSpaStaticFiles(configuration =>
            //{
            //    configuration.RootPath = "ClientApp/dist";
            //});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }
            app.UseAuthentication();
            app.UseRouting();

            // Shows UseCors with CorsPolicyBuilder.
            app.UseCors(builder =>
            {
                builder
                .WithOrigins("http://localhost:4200")
                .AllowAnyMethod()
                .AllowAnyHeader();
            });

            app.UseAuthorization();

            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<ChatHub>("/hubs/chats");
                endpoints.MapControllers();
            });

            
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            

            //app.UseSpa(spa =>
            //{
            //    spa.Options.SourcePath = "MainApp";

            //    if (env.IsDevelopment())
            //    {
            //        spa.UseAngularCliServer(npmScript: "start");
            //    }
            //});

            // Uncomment this to enable template app.

            //app.UseSpa(spa =>
            //{
            //    // To learn more about options for serving an Angular SPA from ASP.NET Core,
            //    // see https://go.microsoft.com/fwlink/?linkid=864501

            //    spa.Options.SourcePath = "ClientApp";

            //    if (env.IsDevelopment())
            //    {
            //        spa.UseAngularCliServer(npmScript: "start");
            //    }
            //});
        }
    }
}
