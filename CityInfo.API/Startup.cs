using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using NLog.Extensions.Logging;

// this class is the entry point of our web app

namespace CityInfo.API
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // ConfigureServices is used to add services to the container
            // and to configure those services.

            //call service then add MVC middleware to the pipeline
            services.AddMvc()
               .AddMvcOptions(o => o.OutputFormatters.Add(
                   new XmlDataContractSerializerOutputFormatter()));

            // if we want to see json string to be the same as in class object 
            // like writing Name instead of name, we use this formattin optiom
            //services.AddMvc().AddJsonOptions(o => {
            //if (o.SerializerSettings.ContractResolver != null) {
            //    var castedResolver = o.SerializerSettings.ContractResolver as DefaultContractResolver;
            //        castedResolver.NamingStrategy = null;
            //    }
            //});
        }

        // ConfigureServices is an optional method. 

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            // dependecy injection
            // built-in logger system
            //loggerFactory.AddConsole(); // log to the console window

            //loggerFactory.AddDebug(); // log to debug window

            // add NLog from Nuget
            //loggerFactory.AddProvider(new NLog.Extensions.Logging.NLogLoggerProvider());
            // or
            loggerFactory.AddNLog(); // it writes to source\repos\CityInfo.API\CityInfo.API\bin\Debug\netcoreapp2.1

            // Configure method uses services that are registered and configured in that method
            // it's used to specify how an asp.net core app will respond to individual HTTP requests.
            // use MVC for handling HTTP request

            if (env.IsDevelopment())
            {
                // this configures request pipeline by adding the developer exception page middleware
                // So, when an exception is thrown this middleware will handle it
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // IHostingEnvironment env gives us info about our hosting environment, app name vs
                // şu an development environmentındayız ancak istersek bunu DEBUG 
                // altından production'a da çevirebilirz
                // eğer development'ta değilsek hata buraya düşecek
                app.UseExceptionHandler();
            }

            // add statuscode middleware to the pipeline
            app.UseStatusCodePages();

            // add mvc middleware to the pipeline
            app.UseMvc();

            //exception handler middleware
            // show only in development envirenment (not in production environment)
            //app.Run((context) =>
            //{
            //    throw new Exception("Sample Exception");
            //});

            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("Hello World!");
            //});
        }
    }
}
