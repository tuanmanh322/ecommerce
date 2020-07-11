using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using API.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace API.Middleware
{
  public class ExceptionMiddleware
  {
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;

    // If there's no exception then move on to
    // the next middleware (See Startup.cs).
    public ExceptionMiddleware(RequestDelegate next,
                                ILogger<ExceptionMiddleware> logger,
                                IHostEnvironment env)
    {
      _next = next;
      _logger = logger;
      _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
      try
      {
        // If there's no exception then the request moves 
        // on to the next middleware. If an exception
        // exists then move to the catch statement.
        await _next(context);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, ex.Message);

        // All response will be in JSON format.
        context.Response.ContentType = "application/json";
        // Set the status code to be http 500.
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        // DEV environment will receive one response while
        // non-DEV such as UAT or PROD will receive another response.
        // Give more details in DEV.
        var response = _env.IsDevelopment()
                        ? new ApiException((int)HttpStatusCode.InternalServerError, ex.Message, ex.StackTrace.ToString())
                        : new ApiException((int)HttpStatusCode.InternalServerError);

        string json = JsonSerializer.Serialize(response);

        await context.Response.WriteAsync(json);
      }
    }
  }
}