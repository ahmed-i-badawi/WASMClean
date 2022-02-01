using Application;
using Application.Common.Interfaces;
using Infrastructure;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using WebUI.Services;
using WebUI.Filters;
using Syncfusion.Blazor;
using NSwag;
using NSwag.Generation.Processors.Security;


namespace WebUI;

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
        services.AddApplication();
        services.AddInfrastructure(Configuration);

        services.AddScoped<ICurrentUserService, CurrentUserService>();

        services.AddHttpContextAccessor();

        services.AddHealthChecks()
        .AddDbContextCheck<ApplicationDbContext>();

        services.AddControllersWithViews(options =>
            options.Filters.Add(new ApiExceptionFilterAttribute()));

        services.AddMvcCore(options =>
        {
            foreach (var outputFormatter in options.OutputFormatters.OfType<OutputFormatter>().Where(x => x.SupportedMediaTypes.Count == 0))
            {
                outputFormatter.SupportedMediaTypes.Add(new Microsoft.Net.Http.Headers.MediaTypeHeaderValue("application/prs.odatatestxx-odata"));
            }

            foreach (var inputFormatter in options.InputFormatters.OfType<InputFormatter>().Where(x => x.SupportedMediaTypes.Count == 0))
            {
                inputFormatter.SupportedMediaTypes.Add(new Microsoft.Net.Http.Headers.MediaTypeHeaderValue("application/prs.odatatestxx-odata"));
            }
        });

        // config for gzip but it's not working but increase size

        services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            options.Providers.Add<GzipCompressionProvider>();
            options.MimeTypes =
        ResponseCompressionDefaults.MimeTypes.Concat(new[] {
                "text/plain",
                "text/css",
                "application/javascript",
                "text/html",
                "application/xml",
                "text/xml",
                "application/json",
                "text/json",
            });
        });

        services.AddRazorPages();

        // Customise default API behaviour
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
        });

        //// In production, the Angular files will be served from this directory
        //services.AddSpaStaticFiles(configuration =>
        //{
        //    configuration.RootPath = "ClientApp/dist";
        //});

        services.AddOpenApiDocument(configure =>
        {
            configure.Title = "WASMClean API";
            configure.AddSecurity("JWT", Enumerable.Empty<string>(), new OpenApiSecurityScheme
            {
                Type = OpenApiSecuritySchemeType.ApiKey,
                Name = "Authorization",
                In = OpenApiSecurityApiKeyLocation.Header,
                Description = "Type into the textbox: Bearer {your JWT token}."
            });

            configure.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));
        });
        services.AddSyncfusionBlazor(options => { options.IgnoreScriptIsolation = true; });

    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseResponseCompression();

        if (env.IsDevelopment())
        {
            //app.UseDeveloperExceptionPage();
            //app.UseDatabaseErrorPage();
            app.UseWebAssemblyDebugging();
        }
        //else
        //{
        app.UseExceptionHandler("/Error");


        var HostHTTPOptionsSection = Configuration.GetSection("HostHTTPOptions");
        var UseForwardedHeaders = bool.Parse(HostHTTPOptionsSection?.GetSection("UseForwardedHeaders")?.Value ?? "false");
        var UseHsts = bool.Parse(HostHTTPOptionsSection?.GetSection("UseHsts")?.Value ?? "false");
        var UseHttpsRedirection = bool.Parse(HostHTTPOptionsSection?.GetSection("UseHttpsRedirection")?.Value ?? "false");
        var AddXForwardedForAndProto = bool.Parse(HostHTTPOptionsSection?.GetSection("AddXForwardedForAndProto")?.Value ?? "false");

        if (UseForwardedHeaders)
        {
            if (AddXForwardedForAndProto)
            {
                app.UseForwardedHeaders(new ForwardedHeadersOptions { ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto });
            }
            else
            {
                app.UseForwardedHeaders();
            }
        }

        if (UseHsts)
        {
            app.UseHsts();
        }

        app.UseHealthChecks("/health");
        if (UseHttpsRedirection)
        {
            app.UseHttpsRedirection();
        }
        app.UseBlazorFrameworkFiles();
        app.UseStaticFiles();

        app.UseSwaggerUi3(settings =>
        {
            settings.Path = "/api";
            settings.DocumentPath = "/api/specification.json";
        });

        app.UseRouting();

        app.UseAuthentication();
        app.UseIdentityServer();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller}/{action=Index}/{id?}");
            endpoints.MapRazorPages();
            endpoints.MapFallbackToFile("index.html");

        });
    }
}
