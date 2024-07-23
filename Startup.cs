using OptimizelySAML.Extensions;
using EPiServer.Cms.Shell;
using EPiServer.Cms.UI.AspNetIdentity;
using EPiServer.Scheduler;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;
using Microsoft.AspNetCore.Authentication.Cookies;
using Sustainsys.Saml2.Metadata;
using Sustainsys.Saml2;
using System.Security.Cryptography.X509Certificates;
using EPiServer.DataAbstraction;
using Org.BouncyCastle.Crypto.Tls;
using System.Runtime.Intrinsics.Arm;
using Sustainsys.Saml2.WebSso;
using Microsoft.AspNetCore.StaticFiles.Infrastructure;
using OptimizelySAML.Middleware;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace OptimizelySAML;

public class Startup
{
    private readonly IWebHostEnvironment _webHostingEnvironment;
    public IConfiguration _configuration { get; }

    public Startup(IWebHostEnvironment webHostingEnvironment, IConfiguration configuration)
    {
        _webHostingEnvironment = webHostingEnvironment;
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        if (_webHostingEnvironment.IsDevelopment())
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", Path.Combine(_webHostingEnvironment.ContentRootPath, "App_Data"));

            services.Configure<SchedulerOptions>(options => options.Enabled = false);
        }

        services.AddSingleton<IOptionsMonitor<CookieAuthenticationOptions>, CustomCookieOptionsMonitor>();

        services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //options.DefaultChallengeScheme = "Saml2";
        })
        .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddSaml2("Saml2Instance1", options =>
        {
            options.SPOptions.EntityId = new EntityId("https://localhost:5000/AccountPage/Login");
            options.SPOptions.ReturnUrl = new Uri("https://localhost:5000/Saml2/Acs");

            var idp = new IdentityProvider(
                new EntityId("https://sts.windows.net/02106cf3-4f4e-4616-b4f0-1b3caaffb2a8/"),
                options.SPOptions)
            {

                LoadMetadata = true,
                MetadataLocation = "https://login.microsoftonline.com/02106cf3-4f4e-4616-b4f0-1b3caaffb2a8/federationmetadata/2007-06/federationmetadata.xml?appid=86a41bdb-dca2-4a7b-9d6e-9aa6c30d005c"
            };

            options.IdentityProviders.Add(idp);
        });

        services.AddAuthentication(options =>
        {
            options.DefaultScheme = "SAML_Cookie";
            options.DefaultSignInScheme = "SAML_Cookie";
            //options.DefaultChallengeScheme = "Saml2";
        })
       .AddCookie("SAML_Cookie")
       .AddSaml2("Saml2Instance2", options =>
         {
             options.SPOptions.EntityId = new EntityId("https://localhost:5000/AccountPage/Login1");
             options.SPOptions.ReturnUrl = new Uri("https://localhost:5000/Saml2/Acs");

             var idp2 = new IdentityProvider(
                 new EntityId("https://sts.windows.net/19e7c385-f88b-4426-9277-fb2314904077/"),
                 options.SPOptions)
             {
                 LoadMetadata = true,
                 MetadataLocation = "https://login.microsoftonline.com/19e7c385-f88b-4426-9277-fb2314904077/federationmetadata/2007-06/federationmetadata.xml?appid=d5957637-8b31-4f24-8995-eefb1aeaac71"
             };

             options.IdentityProviders.Add(idp2);
         });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("SamlUserPolicy", policy =>
            {
                policy.RequireAuthenticatedUser();
            });
        });

        //services.AddSingleton<IAuthorizationMiddlewareResultHandler, LoginRedirectAuthorizationMiddlewareHandler>();

        //services.AddScoped<CustomCookieAuthenticationEvents>();
        //services.ConfigureApplicationCookie(option =>
        //{
        //    option.EventsType = typeof(CustomCookieAuthenticationEvents);
        //});

        services.AddControllersWithViews();
        //services.AddRazorPages();
        services.AddCms();
        services.AddAlloy();
        //services.AddCmsAspNetIdentity<ApplicationUser>();

        // Required by Wangkanai.Detection
        services.AddDetection();

        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromSeconds(10);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        // Required by Wangkanai.Detection
        app.UseDetection();
        app.UseSession();

        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
               name: "default",
               pattern: "{controller=Home}/{action=Index}/{id?}");
            endpoints.MapContent();
        });
    }
}
