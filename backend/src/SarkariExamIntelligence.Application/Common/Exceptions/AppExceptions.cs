namespace SarkariExamIntelligence.Application.Common.Exceptions;

public abstract class AppException : Exception
{
    public int StatusCode { get; }
    protected AppException(string message, int statusCode) : base(message)
    {
        StatusCode = statusCode;
    }
}

public class NotFoundException : AppException
{
    public NotFoundException(string message) : base(message, 404) { }
}

public class BadRequestException : AppException
{
    public IReadOnlyList<string> ValidationErrors { get; }

    public BadRequestException(string message, IReadOnlyList<string>? errors = null) : base(message, 400)
    {
        ValidationErrors = errors ?? Array.Empty<string>();
    }
}

public class UnauthorizedException : AppException
{
    public UnauthorizedException(string message) : base(message, 401) { }
}

public class ForbiddenException : AppException
{
    public ForbiddenException(string message) : base(message, 403) { }
}

public class ConflictException : AppException
{
    public ConflictException(string message) : base(message, 409) { }
}
