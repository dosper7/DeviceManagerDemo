using DeviceManager.Business.Core.Common;
using DeviceManager.Business.Core.Contracts;
using DeviceManager.Business.Models;
using DeviceManager.Business.Ports;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DeviceManager.Business.UseCases.Device.AddDevice
{
    public class AddDeviceCommandHandler : IBaseRequestHandler<AddDeviceCommand, DeviceModel>
    {
        private readonly IDeviceStore _store;
        private readonly ILogger<AddDeviceCommandHandler> _logger;
        public AddDeviceCommandHandler(IDeviceStore store, ILogger<AddDeviceCommandHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _store = store ?? throw new ArgumentNullException(nameof(store));
        }

        public async Task<ApiResult<DeviceModel>> Handle(AddDeviceCommand request, CancellationToken cancellationToken)
        {
            var deviceModel = new DeviceModel()
            {
                Brand = request.Brand,
                Name = request.Name,
                CreationTime = request.CreationTime
            };

            var device = await _store.AddDeviceAsync(deviceModel).ConfigureAwait(false);
            if(device.Id == default)
            {
                _logger.LogError("Add device it's not returning id.");
                return ApiResult.FromError<DeviceModel>("Error Adding device.");
            }

            return ApiResult.FromResult(device);
        }
    }
}
