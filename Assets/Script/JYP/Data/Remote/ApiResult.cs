using System;

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
    
    public bool IsSuccess => error == null;
    public bool IsFail => error != null;
}