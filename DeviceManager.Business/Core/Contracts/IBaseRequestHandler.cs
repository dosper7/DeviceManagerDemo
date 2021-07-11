using DeviceManager.Business.Core.Common;
using MediatR;

namespace DeviceManager.Business.Core.Contracts
{
    internal interface IBaseRequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, ApiResult<TResponse>>
        where TRequest : IRequest<ApiResult<TResponse>>
    {

    }
}
