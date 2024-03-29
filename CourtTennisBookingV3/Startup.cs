﻿using CourtTennisBookingV3.Helpers;
using CourtTennisBookingV3.Models;
using CourtTennisBookingV3.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using CorePush.Apple;
using CorePush.Google;

namespace CourtTennisBookingV3
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
            services.AddControllers();


            services.AddMvc();
            var ConnectionString = Configuration.GetConnectionString("MbkDbConstr");

            services.AddDbContext<TennisBooking_v1Context>(options => options.UseSqlServer(ConnectionString));


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Court Tennis Booking ",
                    Description = "Court Tennis Booking Core Web API ",
                    TermsOfService = new Uri("https://example.com/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "",
                        Url = new Uri("https://example.com/contact")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "",
                        Url = new Uri("https://example.com/license")
                    }
                });




                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                  {
                    {
                      new OpenApiSecurityScheme
                      {
                        Reference = new OpenApiReference
                          {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                          },
                         Scheme = "oauth",
                          Name = "Bearer",
                          In = ParameterLocation.Header,
                        },
                        new List<string>()
                      }
                    });


                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

                //
                services.AddSingleton(c);

            });
            services.AddTransient<IAccountRespository, AccountRespository>();
            services.AddTransient<IBookingsRespository, BookingsRespository>();
            services.AddTransient<ICourtOwnersRespository, CourtOwnersRespository>();
            services.AddTransient<ICustomerRespository, CustomerRespository>();
            services.AddTransient<IRoleRepository, RoleRepository>();
            services.AddTransient<ITenisCourtsRespository, TenisCourtsRespository>();


            //Enable cors
            services.AddCors();

            // jwt
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            var secretKey = Configuration["AppSettings:Secret"];
            var secretKeybytes = Encoding.UTF8.GetBytes(secretKey);
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
              .AddJwtBearer(options =>
              {
                  options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                  {
                      //Tự cấp token
                      ValidateIssuer = false,
                      ValidateAudience = false,
                      //Kí vào token
                      ValidateIssuerSigningKey = true,
                      IssuerSigningKey = new SymmetricSecurityKey(secretKeybytes),

                      ClockSkew = TimeSpan.Zero

                  };
              });

            //noti
            //services.AddTransient<INotificationService, NotificationService>();
            //services.AddHttpClient<FcmSender>();
            //services.AddHttpClient<ApnSender>();
            //var appSettingsSection = Configuration.GetSection("FcmNotification");
            //services.Configure<FcmNotificationSetting>(appSettingsSection);

            // Register the swagger generator
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(name: "V1", new OpenApiInfo { Title = "My API", Version = "V1" });
            });

            //Mail server- smtp




        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors(options => options.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().AllowCredentials());
            if (env.IsDevelopment() || env.IsProduction())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CourtTennisBookingV3 v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
