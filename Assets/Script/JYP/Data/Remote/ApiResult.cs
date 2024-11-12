using System;

public class ApiResult
{
    public Exception error;
    
    public static ApiResult Success()
    {
        return new ApiResult()
        {
            error = null,
        };
    }
    
    public static ApiResult Fail(Exception error)
    {
        return new ApiResult()
        {
            error = error,
        };
    }
    
    public bool IsSuccess => error == null;
    public bool IsFail => error != null;
}

public class ApiResult<T>
{
    public T value;
    public Exception error;
    
    public static ApiResult<T> Success(T value)
    {
        return new ApiResult<T>()
        {
            value = value,
            error = null,
        };
    }
    
    public static ApiResult<T> Fail(Exception error)
    {
        return new ApiResult<T>()
        {
            value = default,
            error = error,
        };
    }
    
    public ApiResult<TResult> Map<TResult>(Func<T, TResult> mapper)
    {
        if (IsSuccess)
        {
            return ApiResult<TResult>.Success(mapper(value));
        }
        else
        {
            return ApiResult<TResult>.Fail(error);
        }
    }
    
    public bool IsSuccess => error == null;
    public bool IsFail => error != null;
}