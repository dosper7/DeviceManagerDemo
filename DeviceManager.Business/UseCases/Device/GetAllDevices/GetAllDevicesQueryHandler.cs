using DeviceManager.Business.Core.Common;
using DeviceManager.Business.Core.Contracts;
using DeviceManager.Business.Models;
using DeviceManager.Business.Ports;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DeviceManager.Business.UseCases.Device.GetAllDevices
{
    public class GetAllDevicesQueryHandler : IBaseRequestHandler<GetAllDevicesQuery, PagedResult<DeviceModel>>
    {
        private readonly IDeviceStore _store;
        public GetAllDevicesQueryHandler(IDeviceStore store)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
        }

        public async Task<ApiResult<PagedResult<DeviceModel>>> Handle(GetAllDevicesQuery request, CancellationToken cancellationToken)
        {
            var devices = await _store.GetAllDevicesAsync(request.StartIndex, request.PageSize).ConfigureAwait(false);
            return ApiResult.FromResult(devices);
        }
    }
}
