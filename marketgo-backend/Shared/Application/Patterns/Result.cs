namespace marketgo_backend.Shared.Application.Patterns;

/// <summary>Represents either a successful value or a domain/application error.</summary>
public abstract record Result<TValue, TError>
{
    public sealed record Success(TValue Value) : Result<TValue, TError>;
    public sealed record Failure(TError Error) : Result<TValue, TError>;

    public bool IsSuccess => this is Success;
    public bool IsFailure => this is Failure;

    public static Result<TValue, TError> Ok(TValue value) => new Success(value);
    public static Result<TValue, TError> Fail(TError error) => new Failure(error);

    public Result<TNext, TError> Map<TNext>(Func<TValue, TNext> transform) => this switch
    {
        Success success => new Result<TNext, TError>.Success(transform(success.Value)),
        Failure failure => new Result<TNext, TError>.Failure(failure.Error),
        _ => throw new InvalidOperationException("Unknown result state.")
    };

    public TResult Fold<TResult>(Func<TValue, TResult> onSuccess, Func<TError, TResult> onFailure) => this switch
    {
        Success success => onSuccess(success.Value),
        Failure failure => onFailure(failure.Error),
        _ => throw new InvalidOperationException("Unknown result state.")
    };
}


