using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevverRH.Application.DTOs.Common;

public class ResultDTO<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public List<string>? Errors { get; set; }

    public static ResultDTO<T> SuccessResult(T data, string? message = null)
    {
        return new ResultDTO<T>
        {
            Success = true,
            Message = message,
            Data = data
        };
    }

    public static ResultDTO<T> FailureResult(string message, List<string>? errors = null)
    {
        return new ResultDTO<T>
        {
            Success = false,
            Message = message,
            Errors = errors
        };
    }

    public static ResultDTO<T> FailureResult(List<string> errors)
    {
        return new ResultDTO<T>
        {
            Success = false,
            Errors = errors
        };
    }
}
