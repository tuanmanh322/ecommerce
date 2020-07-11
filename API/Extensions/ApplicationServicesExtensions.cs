using System.Linq;
using API.Errors;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace API.Extensions
{
  public static class ApplicationServicesExtensions
  {
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
      services.AddScoped<IProductRepository, ProductRepository>();

      services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

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

      return services;
    }
  }
}