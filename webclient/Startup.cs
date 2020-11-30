using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace webclient
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
      services.AddControllersWithViews().AddRazorRuntimeCompilation();

      services.AddAuthentication(options => {
        options.DefaultChallengeScheme = "oidc";
        options.DefaultScheme = "cookieAuth";
      })
        .AddCookie("cookieAuth")
        .AddOpenIdConnect("oidc", options =>
        {
          options.Authority = "https://localhost:5001";

          options.ClientId = "interactive";
          options.ClientSecret = "49C1A7E1-0C79-4A89-A3D6-A37998FB86B0";
          options.ResponseType = "code";

          options.SaveTokens = true;

          options.Scope.Add("profile");
          options.Scope.Add("scope2");
          options.GetClaimsFromUserInfoEndpoint = true;

          options.Events = new OpenIdConnectEvents
          {
            OnTicketReceived = n =>
            {
              // var idSvc = n.HttpContext.RequestServices.GetRequiredService<MovieIdentityService>();

              doStuff(n);
              // n.Principal.Identities.First().AddClaims(appClaims);

              return Task.CompletedTask;
            }
          };
        });
    }

    public void doStuff(TicketReceivedContext data)
    {
      var scopes = data.Principal.FindFirst("scope")?.Value;
      System.Console.WriteLine("");
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
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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
                  pattern: "{controller=Home}/{action=Index}/{id?}");
      });
    }
  }
}
