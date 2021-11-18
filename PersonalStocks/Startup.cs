using Microsoft.EntityFrameworkCore;
using PersonalStocks.Data;
using Blazorise;
using Blazorise.Bootstrap5;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddRazorPages();
        services.AddServerSideBlazor();
        services.AddTransient<PersonalStocksService>();
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddDbContext<PersonalStocksContext>(item => item.UseSqlServer(Configuration.GetConnectionString("myconn")));
        services.AddBlazorise();
        services.AddServerSideBlazor();
        services.AddSingleton<IClassProvider, BootstrapClassProvider>();
        services.AddSingleton<IStyleProvider, Bootstrap5StyleProvider>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapBlazorHub();
            endpoints.MapFallbackToPage("/_Host");
        });
    }
}