using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Errors;
using API.Helpers;
using API.Middleware;
using AutoMapper;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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
      services.AddScoped<IProductRepository, ProductRepository>();

      services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

      services.AddAutoMapper(typeof(MappingProfiles));

      services.AddControllers();

      services.AddDbContext<StoreContext>(options =>
                                            options.UseNpgsql(_config.GetConnectionString("DefaultConnection")));

      services.Configure<ApiBehaviorOptions>(options =>
      {
        // Be sure to register this service configuration AFTER
        // services.AddControllers().
        // This entire services configuration is done to override
        // the [ApiController] attribute, which is classified as
        // a ModelState response, with our own response.
        options.InvalidModelStateResponseFactory = actionContext =>
        {
          string[] errors = actionContext.ModelState.Where(e => e.Value.Errors.Count > 0)
                                                    .SelectMany(x => x.Value.Errors)
                                                    .Select(x => x.ErrorMessage).ToArray();

          var errorResponse = new ApiValidationErrorResponse()
          {
            Errors = errors
          };

          return new BadRequestObjectResult(errorResponse);
        };
      });
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

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
      });
    }
  }
}
