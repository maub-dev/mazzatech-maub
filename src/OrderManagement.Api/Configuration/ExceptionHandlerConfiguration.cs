using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using OrderManagement.Domain;

namespace OrderManagement.Api.Configuration;

public static class ExceptionHandlerConfiguration
{
    public static IApplicationBuilder UseExceptionHandlerConfiguration(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(exceptionApp => exceptionApp.Run(async context =>
        {
            var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
            if (exception is ValidationException validationException)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(new
                {
                    title = "One or more validation errors occurred.",
                    errors = validationException.Errors
                        .GroupBy(error => error.PropertyName)
                        .ToDictionary(group => group.Key, group => group.Select(error => error.ErrorMessage).ToArray())
                });
                return;
            }

            if (exception is OrderConflictException conflictException)
            {
                context.Response.StatusCode = StatusCodes.Status409Conflict;
                await context.Response.WriteAsJsonAsync(new { title = conflictException.Message });
                return;
            }

            var logger = context.RequestServices
                .GetRequiredService<ILoggerFactory>()
                .CreateLogger("OrderManagement.Api.ExceptionHandler");
            logger.LogError(exception, "Unhandled exception while processing {Path}.", context.Request.Path);

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new { title = "An unexpected error occurred." });
        }));

        return app;
    }
}
