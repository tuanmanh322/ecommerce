using API.Extensions;
using API.Helpers;
using API.Middleware;
using AutoMapper;
using Infrastructure.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace API
{
  public class Startup
  {
    private readonly IConfiguration _config;

    public Startup(IConfiguration config)
    {
      _config = config;
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddAutoMapper(typeof(MappingProfiles));

      services.AddControllers();

      services.AddDbContext<StoreContext>(options =>
                                            options.UseNpgsql(_config.GetConnectionString("DefaultConnection")));

      // Other services are registered in
      // ApplicationServicesExtensions.cs.
      services.AddApplicationServices();

      // Swagger services are registered in
      // SwaggerServicesExtensions.cs.
      services.AddSwaggerDocumentation();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      // Replace the default exception handling middleware
      // with our own custom middleware to handle exceptions.
      app.UseMiddleware<ExceptionMiddleware>();

      // Requests that aren't exceptions will be re-directed to 
      // our errors controller with the status code passed in.
      app.UseStatusCodePagesWithReExecute("/errors/{0}");

      app.UseHttpsRedirection();

      app.UseRouting();

      app.UseStaticFiles();

      app.UseAuthorization();

      // Swagger middleware are configured in
      // SwaggerServicesExtensions.cs
      app.UseSwaggerDocumentation();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
      });
    }
  }
}
