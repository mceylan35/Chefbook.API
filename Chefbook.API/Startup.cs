using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Chefbook.API.Helpers;
using Chefbook.API.Repository;
using Chefbook.API.Services.Interface;
using Chefbook.API.Services.RepositoryInterfaces;
using Chefbook.API.Services.RepositoryServices;
using Chefbook.API.Services.Service;
using Chefbook.API.SignalR.Abstract;
using Chefbook.API.SignalR.Concrete;
using Chefbook.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
//using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;

using Swashbuckle.AspNetCore.Swagger;

namespace Chefbook.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }



        public IConfiguration Configuration { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var key = Encoding.ASCII.GetBytes(Configuration.GetSection("Appsettings:Token").Value);
            //services.AddScoped(typeof(IGenericRepository<>),typeof(GenericRepository<>));

            services.Configure<CloudinarySettings>(Configuration.GetSection("CloudinarySettings"));

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<ICommentService,CommentService>();
            services.AddScoped<IImageService,ImageService>();
            services.AddScoped<ILikeService, LikeService>();
            services.AddScoped<IFollowService, FollowService>();
            services.AddScoped<IConnectionService, ConnectionService>();
          
          //  services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IStickerService, StickerService>();
            services.AddScoped<IStepService,StepService>();
            services.AddScoped<IStarService, StarService>();
            services.AddSignalR();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>
           {
               opt.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateIssuerSigningKey = true,
                   IssuerSigningKey = new SymmetricSecurityKey(key),
                   ValidateIssuer = false,
                   ValidateAudience = false,
               };
           });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Chefbook", Version = "v1" });
            });
            //services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
            //{
            //    builder.AllowAnyOrigin();
            //    builder.AllowAnyMethod();
            //    builder.AllowAnyHeader();
            //    builder.AllowCredentials();
            //}));

            services.AddSignalR();
            //services.AddSingleton<IHubNotification, HubNotification>();
            services.AddAutoMapper(typeof(Startup));
            services.AddMvc();
            services.AddCors(config =>
            {
                config.AddPolicy("Ozgul", bldr =>
                {
                    bldr.AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowAnyOrigin(); // .WithOrigins("adres")
                });
            });
            services.Configure<FormOptions>(o =>
            {
                o.ValueLengthLimit = int.MaxValue;
                o.MultipartBodyLengthLimit = int.MaxValue;
                o.MemoryBufferThreshold = int.MaxValue;
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

           // app.UseSignalR(routes => { routes.MapHub<NotificationHub>("/notificationhub"); });

            app.UseSignalR(route => {
                route.MapHub<NotificationHub>("/notificationhub");
            });

            app.UseAuthentication();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Chefbook");
            });

            app.UseStaticFiles();
            //app.UseStaticFiles(new StaticFileOptions()
            //{
            //    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Resources")),
            //    RequestPath = new PathString("/Resources")
            //});
            app.UseMvc();

            app.UseCors("Ozgul");
         


        

        }
    }
}