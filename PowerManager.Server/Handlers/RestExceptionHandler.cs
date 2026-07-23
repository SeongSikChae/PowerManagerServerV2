using Microsoft.AspNetCore.Diagnostics;

namespace PowerManager.Server.Handlers
{
    using Entity;
    using PowerManager.Server.Context.Exceptions;

    public sealed class RestExceptionHandler(ILogger<RestExceptionHandler> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            logger.Error("Unhandled exception", exception);

            var (status, code, message) = exception switch
            {
                DuplicateKeyException dke => (StatusCodes.Status409Conflict, "DUPLICATE_KEY_ERROR", dke.Message),
                KeyNotFoundException knfe => (StatusCodes.Status422UnprocessableEntity, "KEY_NOT_FOUND_ERROR", knfe.Message),
                NotImplementedException => (StatusCodes.Status501NotImplemented, "NOT_IMPLEMENTED_ERROR", "Not implementaion yet."),
                _ => (StatusCodes.Status500InternalServerError, "INTERNAL_SERVER_ERROR", "관리자에게 문의하세요.")
            };

            httpContext.Response.StatusCode = status;
            await httpContext.Response.WriteAsJsonAsync(RestResult.Fail(code, message), cancellationToken);
            return true;
        }
    }
}
