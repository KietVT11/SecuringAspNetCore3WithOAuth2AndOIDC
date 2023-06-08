using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json.Serialization;

namespace ImageGallery.Client
{
	public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
			services.AddControllersWithViews()
				 .AddJsonOptions(opts =>
				 {
                     opts.JsonSerializerOptions.PropertyNamingPolicy = null;
                     opts.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                     opts.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
                 });

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            // create an HttpClient used for accessing the API
            services.AddHttpClient("APIClient", client =>
            {
                client.BaseAddress = new Uri(Configuration["ImageGalleryAPIRoot"]);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
            });

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
			{
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.Authority = "https://localhost:5001/";
                options.ClientId = "imagegalleryclient";
                options.ClientSecret = "secret";
                options.ResponseType = "code";
                //options.Scope.Add("openid");
                //options.Scope.Add("profile");
                //options.CallbackPath = new PathString("signin-oidc");
                // SignedOutCallbackPath: default = host:port/signout-callback-oidc.
                // Must match with the post logout redirect URI at IDP client config if
                // you want to automatically return to the application after logging out
                // of IdentityServer.
                // To change, set SignedOutCallbackPath
                // eg: options.SignedOutCallbackPath = new PathString("pathaftersignout");
                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;
                options.ClaimActions.Remove("aud");
                options.ClaimActions.DeleteClaim("sid");
                options.ClaimActions.DeleteClaim("idp");
                options.Scope.Add("roles");
                options.Scope.Add("imagegalleryapi.fullaccess");
                options.ClaimActions.MapJsonKey("role", "role");
                options.TokenValidationParameters = new()
                {
                    NameClaimType = "given_name",
                    RoleClaimType = "role",
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseStaticFiles();
 
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Shared/Error");
                // The default HSTS value is 30 days. You may want to change this for
                // production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Gallery}/{action=Index}/{id?}");
            });
        }
    }
}
