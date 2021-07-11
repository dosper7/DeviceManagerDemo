using DeviceManager.Business.Core.Common;
using DeviceManager.Business.Core.Contracts;
using DeviceManager.Business.Models;
using DeviceManager.Business.Ports;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DeviceManager.Business.UseCases.Device.DeleteDevice
{
    public class DeleteDeviceCommandHandler : IBaseRequestHandler<DeleteDeviceCommand, DeviceModel>
    {
        private readonly IDeviceStore _store;
        private readonly ILogger<DeleteDeviceCommandHandler> _logger;
        public DeleteDeviceCommandHandler(IDeviceStore store, ILogger<DeleteDeviceCommandHandler> logger)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger)); ;
        }

        public async Task<ApiResult<DeviceModel>> Handle(DeleteDeviceCommand request, CancellationToken cancellationToken)
        {
            if (await _store.GetDeviceByIdAsync(request.Id).ConfigureAwait(false) == null)
                return ApiResult.FromError<DeviceModel>($"Device with id {request.Id} doesn't exist.");

            var deletedDevice = await _store.DeleteDeviceAsync(request.Id).ConfigureAwait(false);
            if (deletedDevice == null)
            {
                _logger.LogError("Delete device didn't return device model.");
                return ApiResult.FromError<DeviceModel>("Error Adding device.");
            }

            return ApiResult.FromResult(deletedDevice);
        }
    }
}
