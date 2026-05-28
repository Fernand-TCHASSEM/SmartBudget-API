namespace SmartBudget.Application.Common;

public readonly struct Result<TValue, TError>
{
    public TValue? Value { get; }
    public TError? Error { get; }
    public bool IsSuccess { get; }

    private Result(TValue value)  { Value = value; Error = default; IsSuccess = true; }
    private Result(TError error)  { Value = default; Error = error; IsSuccess = false; }

    public static Result<TValue, TError> Ok(TValue value)   => new(value);
    public static Result<TValue, TError> Fail(TError error) => new(error);
}
