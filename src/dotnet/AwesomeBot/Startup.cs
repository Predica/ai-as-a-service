// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with EmptyBot .NET Template version v4.14.1.2

using System;
using AwesomeBot.Contracts;
using AwesomeBot.Models;
using AwesomeBot.Services;
using Azure;
using Azure.AI.Language.QuestionAnswering;
using Azure.AI.TextAnalytics;
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
            services.AddOptions<LanguageOption>().Configure<IConfiguration>(
                (options, configuration) => configuration.GetSection("Language").Bind(options));
            services.AddOptions<LuisOption>().Configure<IConfiguration>(
                (options, configuration) => configuration.GetSection("Luis").Bind(options));

            services.AddScoped(serviceProvider =>
            {
                var option = serviceProvider.GetService<IOptions<LanguageOption>>()?.Value ??
                             throw new OptionsValidationException(nameof(LanguageOption), typeof(LanguageOption),
                                 new[] { "option doesn't exist" });
                return new QuestionAnsweringProject(option.Project, option.Deployment);
            });
            
            services.AddScoped(serviceProvider =>
            {
                var option = serviceProvider.GetService<IOptions<LanguageOption>>()?.Value ??
                             throw new OptionsValidationException(nameof(LanguageOption), typeof(LanguageOption), new []{"option doesn't exist"});
                return new QuestionAnsweringClient(new Uri(option.Host), new AzureKeyCredential(option.EndpointKey));
            });
            
            services.AddScoped(serviceProvider =>
            {
                var option = serviceProvider.GetService<IOptions<LanguageOption>>()?.Value ??
                             throw new OptionsValidationException(nameof(LanguageOption), typeof(LanguageOption), new []{"option doesn't exist"});
                return new TextAnalyticsClient(new Uri(option.Host), new AzureKeyCredential(option.EndpointKey));
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

            services.AddOptions<TranslationOption>().Configure<IConfiguration>((option, configuration) =>
                configuration.GetSection("Translation").Bind(option));
            
            services.AddHttpClient<ITranslationClient, TranslationClient>();
            services.AddScoped<ITranslationService, TranslationService>();
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
