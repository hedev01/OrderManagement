using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagement.Application.Common
{
    public class Result<T>
    {
        public T? Data { get; init; }

        public bool IsSuccess { get; init; }
        public string? ErrorMessage { get; init; }

        public static Result<T> Success(T value) => new Result<T>()
        {
            IsSuccess = true,
            Data = value,
        };

        public static Result<T> Failure(string errorMessage) => new Result<T>()
        {
            IsSuccess = false,
            ErrorMessage = errorMessage
        };

    }
}
