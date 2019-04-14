using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using HomeControl.Web.Activities;
using HomeControl.Web.Devices;
using HomeControl.Web.Devices.Denon;
using HomeControl.Web.Devices.Meta;
using HomeControl.Web.Devices.Sony;
using HomeControl.Web.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;

namespace HomeControl.Web
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            #region SWAGGER
            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "StreamDeck API",
                    Description = "API to get images for StreamDeck",
                    Contact = new OpenApiContact
                    {
                        Name = "Mark Zuber",
                        Email = string.Empty,
                        Url = new Uri("https://github.com/markzuber")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "MIT License",
                        Url = new Uri("https://example.com/license")
                    }
                });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
            #endregion SWAGGER

            // Register Devices

            // todo: wire this up to json config...
            services.AddSingleton<IDenonHttpReceiverDevice>(new DenonHttpReceiverDevice("192.168.2.198"));
            services.AddSingleton<IDenonNetworkReceiver, DenonNetworkReceiver>();
            services.AddSingleton<ISonyNetworkProjector>(new SonyNetworkProjector("192.168.2.22"));

            services.AddSingleton<ITheater, Theater>();

            // Register Activities
            services.AddSingleton<TheaterOffActivity>();
            services.AddSingleton<TheaterOnActivity>();
            services.AddSingleton<TheaterAppleTvActivity>();
            services.AddSingleton<TheaterFireTvActivity>();
            services.AddSingleton<TheaterPs4Activity>();
            services.AddSingleton<TheaterToggleMuteActivity>();
            services.AddSingleton<TheaterVolumeDownActivity>();
            services.AddSingleton<TheaterVolumeUpActivity>();
            services.AddSingleton<TheaterXboxActivity>();
            services.AddSingleton<TheaterNullActivity>();
            services.AddSingleton<IActivityFactory, ActivityFactory>();

            // Configuration
            var streamDeckActivityServiceConfig = new StreamDeckActivityServiceConfig(
                new ConcurrentDictionary<int, StreamDeckKeyInfo>(new Dictionary<int, StreamDeckKeyInfo>
                {
                    {  0, new StreamDeckKeyInfo(ActivityKey.TheaterXboxOn,  "xbox.png", "xbox.png") },
                    {  1, new StreamDeckKeyInfo(ActivityKey.TheaterPs4On, "ps4.png", "ps4.png") },
                    {  2, new StreamDeckKeyInfo(ActivityKey.TheaterFireTvOn,  "firetv.png", "firetv.png") },
                    {  3, new StreamDeckKeyInfo(ActivityKey.TheaterAppleTvOn,  "appletv.png", "appletv.png") },
                    {  4, new StreamDeckKeyInfo(ActivityKey.TheaterVolumeToggleMute, "volmute.png", "volmute.png") },
                    {  5, new StreamDeckKeyInfo(ActivityKey.None,  "blank.png", "blank.png") },
                    {  6, new StreamDeckKeyInfo(ActivityKey.None,  "blank.png", "blank.png") },
                    {  7, new StreamDeckKeyInfo(ActivityKey.None,  "blank.png", "blank.png") },
                    {  8, new StreamDeckKeyInfo(ActivityKey.None,  "blank.png", "blank.png") },
                    {  9, new StreamDeckKeyInfo(ActivityKey.TheaterVolumeUp,  "volup.png", "volup.png") },
                    { 10, new StreamDeckKeyInfo(ActivityKey.TheaterOff,  "poweroff.png", "poweroff.png") },
                    { 11, new StreamDeckKeyInfo(ActivityKey.TheaterOn,  "dim.png", "dim.png") },
                    { 12, new StreamDeckKeyInfo(ActivityKey.TheaterOn,  "bright.png", "bright.png") },
                    { 13, new StreamDeckKeyInfo(ActivityKey.None,  "blank.png", "blank.png") },
                    { 14, new StreamDeckKeyInfo(ActivityKey.TheaterVolumeDown,  "voldown.png", "voldown.png") },
                }));

            // todo: there are better ways to register config...
            services.AddSingleton<StreamDeckActivityServiceConfig>(streamDeckActivityServiceConfig);

            // Register Services
            services.AddSingleton<IActivityService, ActivityService>();
            services.AddSingleton<IStreamDeckActivityService, StreamDeckActivityService>();

            services
                .AddMvc()
                .AddNewtonsoftJson();
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
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            // When we deploy, we're not running in IIS Express so env.WebRootPath will be null.
            // Need to set it based on the fact that wwwroot is under the publish directory as part of
            // deployment.
            if (string.IsNullOrWhiteSpace(env.WebRootPath))
            {
                env.WebRootPath = Path.Combine(
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                    "publish",
                    "wwwroot");
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "StreamDeck API");
            });

            app.UseRouting(routes =>
            {
                routes.MapControllerRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
                routes.MapRazorPages();
            });

            app.UseCookiePolicy();

            app.UseAuthorization();
        }
    }
}
