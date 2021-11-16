// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with EmptyBot .NET Template version v4.14.1.2

using AwesomeBot.Contracts;
using AwesomeBot.Models;
using AwesomeBot.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Builder.AI.QnA;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace AwesomeBot
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
            services.AddHttpClient().AddControllers().AddNewtonsoftJson();

            // Create the Bot Framework Adapter with error handling enabled.
            services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();
            services.AddSingleton<IAdaptiveCardsTemplates, AdaptiveCardsTemplates>();
            services.AddScoped<IContextOrchestrator, ContextOrchestrator>();
            services.AddScoped<IQnAService, QnAService>();
            // Create the bot as a transient. In this case the ASP Controller is expecting an IBot.
            services.AddTransient<IBot, Bot>();
            services.AddOptions<QnAMakerOption>().Configure<IConfiguration>(
                (options, configuration) => configuration.GetSection("QnAMaker").Bind(options));
            services.AddOptions<LuisOption>().Configure<IConfiguration>(
                (options, configuration) => configuration.GetSection("Luis").Bind(options));
            
            services.AddScoped(serviceProvider =>
            {
                var option = serviceProvider.GetService<IOptions<QnAMakerOption>>()?.Value ??
                             throw new OptionsValidationException(nameof(QnAMakerOption), typeof(QnAMakerOption), new []{"option doesn't exist"});
                return new QnAMaker(
                    new QnAMakerEndpoint
                    {
                        KnowledgeBaseId = option.KnowledgeBaseId,
                        Host = option.Host,
                        EndpointKey = option.EndpointKey
                    });
            });

            services.AddScoped(serviceProvider =>
            {
                var option = serviceProvider.GetService<IOptions<LuisOption>>()?.Value ??
                             throw new OptionsValidationException(nameof(LuisOption), typeof(LuisOption), new []{"option doesn't exist"});
                return new LuisRecognizer(
                    new LuisRecognizerOptionsV3(
                        new LuisApplication(
                            option.ApplicationId,
                            option.EndpointKey,
                            option.Host)));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles()
                .UseStaticFiles()
                .UseWebSockets()
                .UseRouting()
                .UseAuthorization()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });

            // app.UseHttpsRedirection();
        }
    }
}
