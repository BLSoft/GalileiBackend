using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Owin_Auth.Auth;
using Owin_Auth.Id;
using Owin_Auth.Utils;


namespace Owin_Auth
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = true,
                        ValidIssuer = Configuration["Tokens:Issuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(Configuration["Tokens:SecretKey"])),
                        ValidateLifetime = true,
                        ValidAudience = Configuration["Tokens:Audience"]
                    };
                    
                    
                  
                });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("RoleUser", policy => policy.Requirements.Add(new RequiredRoleRequirement(2)));
                options.AddPolicy("RoleManager", policy => policy.Requirements.Add(new RequiredRoleRequirement(3)));
                options.AddPolicy("RoleAdmin", policy => policy.Requirements.Add(new RequiredRoleRequirement(4)));
                options.AddPolicy("RoleSysadmin", policy => policy.Requirements.Add(new RequiredRoleRequirement(5)));
            });
            services.AddDbContext<DataContext>(
                options => options.UseSqlServer(Configuration["Database:ConnectionString"]));

            services.AddSingleton<IUserManager, UserManager>();
            services.AddSingleton<IValidationManager, ValidationManager>();

            Config.Configuration = Configuration;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            //Auth
            app.UseAuthentication();


            //app.UseHttpsRedirection();
            app.UseMvc();


        }
    }
}
