namespace Framework.Diagnostics.ExecutionEvents;

using HotChocolate.Execution;
using HotChocolate.Execution.Instrumentation;
using HotChocolate.Resolvers;
using Microsoft.Extensions.Logging;

public class CustomExecutionEventListener : ExecutionDiagnosticEventListener
{
    private readonly ILogger<CustomExecutionEventListener> _logger;

    public CustomExecutionEventListener(ILogger<CustomExecutionEventListener> logger)
        => _logger = logger;

    public override bool EnableResolveFieldValue => true;

    public override IDisposable ExecuteRequest(IRequestContext context)
    {
        if (string.IsNullOrWhiteSpace(context.Request.OperationName)) return EmptyScope;

        var start = DateTime.UtcNow;

        _logger.LogInformation("{Start} - Request started. Operation: {Operation}.",
            start.ToString("HH:mm:ss.fff"), context.Request.OperationName);

        return new RequestScope(context, start, _logger);
    }

    public override IDisposable ResolveFieldValue(IMiddlewareContext context)
    {
        if (context.Operation.Name == null) return EmptyScope;

        var start = DateTime.UtcNow;

        _logger.LogInformation("{Start} - Resolver started. Field: {Field}.",
            start.ToString("HH:mm:ss.fff"), context.Selection.Field.Name);

        return new ResolveFieldValueScope(context, start, _logger);
    }
}

public class RequestScope : IDisposable
{
    private readonly ILogger _logger;
    private readonly DateTime _start;
    private readonly IRequestContext _context;

    public RequestScope(IRequestContext context, DateTime start, ILogger logger)
    {
        _context = context;
        _start = start;
        _logger = logger;
    }

    public void Dispose()
    {
        var end = DateTime.UtcNow;
        var elapsed = end - _start;

        _logger.LogInformation("{End} - Request finished. Operation: {Operation}. Diff: {Diff}",
            end.ToString("HH:mm:ss.fff"), _context.Request.OperationName, elapsed.TotalMilliseconds);
    }
}

public class ResolveFieldValueScope : IDisposable
{
    private readonly ILogger _logger;
    private readonly DateTime _start;
    private readonly IMiddlewareContext _context;

    public ResolveFieldValueScope(IMiddlewareContext context, DateTime start, ILogger logger)
    {
        _context = context;
        _start = start;
        _logger = logger;
    }

    public void Dispose()
    {
        var end = DateTime.UtcNow;
        var elapsed = end - _start;

        _logger.LogInformation("{End} - Resolver finished. Field: {Field}. Diff: {Diff}",
            end.ToString("HH:mm:ss.fff"),
            _context.Selection.Field.Name,
            elapsed.TotalMilliseconds);
    }
}
