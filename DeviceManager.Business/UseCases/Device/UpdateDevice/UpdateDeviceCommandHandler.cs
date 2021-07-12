using DeviceManager.Business.Core.Common;
using DeviceManager.Business.Core.Contracts;
using DeviceManager.Business.Models;
using DeviceManager.Business.Ports;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DeviceManager.Business.UseCases.Device.UpdateDevice
{
    public class UpdateDeviceCommandHandler : IBaseRequestHandler<UpdateDeviceCommand, DeviceModel>
    {
        private readonly IDeviceStore _store;
        private readonly ILogger<UpdateDeviceCommandHandler> _logger;

        public UpdateDeviceCommandHandler(IDeviceStore store, ILogger<UpdateDeviceCommandHandler> logger)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ApiResult<DeviceModel>> Handle(UpdateDeviceCommand request, CancellationToken cancellationToken)
        {
            var dbDevice = await _store.GetDeviceByIdAsync(request.Id).ConfigureAwait(false);
            if (dbDevice == null)
                return ApiResult.FromError<DeviceModel>($"Device with id {request.Id} doesn't exist.");

            var device = GetDeviceModel(request);

            dbDevice.Brand = !request.IsPartialUpdate ? device.Brand : (device.Brand == default ? dbDevice.Brand : device.Brand);
            dbDevice.CreationTime = !request.IsPartialUpdate ? device.CreationTime : (device.CreationTime == default ? dbDevice.CreationTime : device.CreationTime);
            dbDevice.Name = !request.IsPartialUpdate ? device.Name : (device.Name == default ? dbDevice.Name : device.Name);

            _logger.LogDebug($"updating device with id {device.Id} in mode partialUpdate = {request.IsPartialUpdate}.");
            var updatedModel = await _store.UpateDeviceAsync(device).ConfigureAwait(false);
            return ApiResult.FromResult(updatedModel);
        }


        private static DeviceModel GetDeviceModel(UpdateDeviceCommand request)
        {
            return new DeviceModel()
            {
                Id = request.Id,
                Name = request.Name,
                Brand = request.Brand,
                CreationTime = request.CreationTime.GetValueOrDefault()
            };
        }
       
    }
}
