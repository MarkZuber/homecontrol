using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using HomeControl.Web.Activities;
using HomeControl.Web.Devices;
using HomeControl.Web.Devices.Denon;
using HomeControl.Web.Devices.Epson;
using HomeControl.Web.Devices.Meta;
using HomeControl.Web.Services;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

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

            // Logging
            string logFilePrefix = "homecontrolweb";
            string logsDir = "/tmp";
            var logger = LoggerFactory.CreateLogger(logsDir, logFilePrefix);
            services.AddSingleton(logger);

            logger.Information("HomeControl.Web starting up");

            // Register Devices

            services.AddSingleton<INetworkSettings, NetworkSettings>();
            services.AddSingleton<IDenonHttpReceiverDevice, DenonHttpReceiverDevice>();
            services.AddSingleton<IDenonNetworkReceiver, DenonNetworkReceiver>();
            services.AddSingleton<IEpsonNetworkProjector, EpsonNetworkProjector>();

            services.AddSingleton<ITheater, Theater>();

            // Register Activities
            services.AddSingleton<TheaterOffActivity>();
            services.AddSingleton<TheaterOnActivity>();
            services.AddSingleton<TheaterAppleTvActivity>();
            services.AddSingleton<TheaterFireTvActivity>();
            services.AddSingleton<TheaterPlaystationActivity>();
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
                    {  1, new StreamDeckKeyInfo(ActivityKey.TheaterPlaystationOn, "ps4.png", "ps4.png") },
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

            var alexaActivityServiceConfig = new AlexaActivityServiceConfig(
                new ConcurrentDictionary<string, string>(new Dictionary<string, string>
                {
                    { AlexaMessage.TheaterXboxOn, ActivityKey.TheaterXboxOn },
                    { AlexaMessage.TheaterPlaystationOn, ActivityKey.TheaterPlaystationOn },
                    { AlexaMessage.TheaterFireTvOn, ActivityKey.TheaterFireTvOn },
                    { AlexaMessage.TheaterAppleTvOn, ActivityKey.TheaterAppleTvOn },
                    { AlexaMessage.TheaterVolumeMute, ActivityKey.TheaterVolumeToggleMute },
                    { AlexaMessage.TheaterVolumeUp, ActivityKey.TheaterVolumeUp },
                    { AlexaMessage.TheaterVolumeDown, ActivityKey.TheaterVolumeDown },
                    { AlexaMessage.TheaterOn, ActivityKey.TheaterOn },
                    { AlexaMessage.TheaterOff, ActivityKey.TheaterOff },
                }));

            services.AddSingleton<AlexaActivityServiceConfig>(alexaActivityServiceConfig);

            // Register Services
            services.AddSingleton<IActivityService, ActivityService>();
            services.AddSingleton<IStreamDeckActivityService, StreamDeckActivityService>();
            services.AddSingleton<IAlexaActivityService, AlexaActivityService>();

            services.AddMediatR(Assembly.GetExecutingAssembly());

            services.AddMvc();
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

            app.UseRouting()
               .UseEndpoints(
                endpoints =>
                {
                    endpoints.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
                    endpoints.MapRazorPages();
                });

            app.UseCookiePolicy();

            app.UseAuthorization();
        }
    }
}
