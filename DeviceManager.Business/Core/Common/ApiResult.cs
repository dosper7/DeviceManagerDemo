using System.Collections.Generic;
using System.Linq;

namespace DeviceManager.Business.Core.Common
{
    public class ApiResult
    {
        public static readonly ApiResult Empty = new ApiResult();
        public List<string> Errors { get; set; }
        public bool Success => !Errors.Any();

        public ApiResult()
        {
            Errors = new List<string>();
        }

        public ApiResult AddError(string message)
        {
            Errors.Add(message);
            return this;
        }

        public static ApiResult<T> FromResult<T>(T data) => new ApiResult<T>() { Data = data };
        public static ApiResult<T> FromError<T>(params string[] errorMessages) => AddErrors(errorMessages, new ApiResult<T>());
        public static ApiResult FromError(params string[] errorMessages) => AddErrors(errorMessages, new ApiResult());

        private static T AddErrors<T>(string[] errorMessages, T result)
            where T : ApiResult
        {
            foreach (var msg in errorMessages)
            {
                result.Errors.Add(msg);
            }

            return result;
        }
    }

    public class ApiResult<T> : ApiResult
    {
        public T Data { get; set; }

        public ApiResult()
        { }

        public ApiResult(T data)
        {
            Data = data;
        }

    }
}
