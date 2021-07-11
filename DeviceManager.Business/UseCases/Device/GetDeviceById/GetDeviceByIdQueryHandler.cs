using DeviceManager.Business.Core.Common;
using DeviceManager.Business.Core.Contracts;
using DeviceManager.Business.Models;
using DeviceManager.Business.Ports;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DeviceManager.Business.UseCases.Device.GetDeviceById
{
    public class GetDeviceByIdQueryHandler : IBaseRequestHandler<GetDeviceByIdQuery, DeviceModel>
    {
        private readonly IDeviceStore _store;
        public GetDeviceByIdQueryHandler(IDeviceStore store)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
        }

        public async Task<ApiResult<DeviceModel>> Handle(GetDeviceByIdQuery request, CancellationToken cancellationToken)
        {
            var device = await _store.GetDeviceByIdAsync(request.Id).ConfigureAwait(false);
            return ApiResult.FromResult(device);
        }
    }
}
