using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Backend.Api.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);

        var problemDetails = new ProblemDetails
        {
            Status = (int)HttpStatusCode.InternalServerError,
            Title = "Server Error",
            Detail = "משהו השתבש בשרת. הצוות הטכני עודכן.", // או הודעה באנגלית
            Instance = httpContext.Request.Path
        };

        // אם אנחנו ב-Development, אפשר להוסיף את ה-StackTrace כדי שיהיה לך קל לדבג
        // אבל ב-Production לעולם לא נחשוף את זה!
        problemDetails.Extensions.Add("exception", exception.Message);

        httpContext.Response.StatusCode = problemDetails.Status.Value;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true; // מציין שהשגיאה טופלה
    }
}