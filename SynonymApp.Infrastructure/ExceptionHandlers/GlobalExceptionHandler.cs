using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Net;

namespace SynonymApp.Infrastructure.ExceptionHandlers
{
    public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IProblemDetailsService problemDetailsService) : IExceptionHandler
    {
        /// <inheritdoc/>
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            Exception ex = exception;
            logger.LogError(exception,
                @$"Error: 
            requestID: {Activity.Current?.Id ?? httpContext.TraceIdentifier} 
            Path: {httpContext.Request.Path}
            Message: {ex.Message} 
            StackTrace: {ex.StackTrace}");

            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            await problemDetailsService.WriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                Exception = ex,
                ProblemDetails = new Microsoft.AspNetCore.Mvc.ProblemDetails
                {
                    Title = "Internal server error",
                    Detail = ex.Message,
                    Status = (int)HttpStatusCode.InternalServerError
                }
            });

            return true;
        }
    }
}
