using DeviceManager.Business.Core.Common;
using DeviceManager.Business.Core.Contracts;
using DeviceManager.Business.Models;
using DeviceManager.Business.Ports;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using static DeviceManager.Business.UseCases.Device.UpdateDevice.UpdateDeviceCommand;

namespace DeviceManager.Business.UseCases.Device.UpdateDevice
{
    public class UpdateDeviceCommandHandler : IBaseRequestHandler<UpdateDeviceCommand, DeviceModel>
    {
        private readonly IDeviceStore _store;
        private readonly ILogger<UpdateDeviceCommandHandler> _logger;
        private readonly Dictionary<UpdateTypeEnum, IUpdateStrategy> updateStrategyFactory;

        public UpdateDeviceCommandHandler(IDeviceStore store, ILogger<UpdateDeviceCommandHandler> logger)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            updateStrategyFactory = new Dictionary<UpdateTypeEnum, IUpdateStrategy>() 
            {
                { UpdateTypeEnum.Full, new FullUpdateStrategy() },
                { UpdateTypeEnum.Partial, new PartialUpdateStrategy() },
            };
        }

        public async Task<ApiResult<DeviceModel>> Handle(UpdateDeviceCommand request, CancellationToken cancellationToken)
        {
            var dbDevice = await _store.GetDeviceByIdAsync(request.Id).ConfigureAwait(false);
            if (dbDevice == null)
                return ApiResult.FromError<DeviceModel>($"Device with id {request.Id} doesn't exist.");

            var updateStrategy = GetUpdateStrategy(request);
            updateStrategy.UpdateDevice(dbDevice, request);

            _logger.LogDebug($"updating device with id {dbDevice.Id} in mode partialUpdate = {request.UpdateType}.");
            var updatedModel = await _store.UpateDeviceAsync(dbDevice).ConfigureAwait(false);
            return ApiResult.FromResult(updatedModel);
        }

        private IUpdateStrategy GetUpdateStrategy(UpdateDeviceCommand request)
        {
            if (updateStrategyFactory.TryGetValue(request.UpdateType, out IUpdateStrategy strategy))
                return strategy;

            _logger.LogError($"No strategy available for {request.UpdateType}");
            throw new NotImplementedException($"No strategy available for {request.UpdateType}");
        }

        private interface IUpdateStrategy
        {
            void UpdateDevice(DeviceModel original, UpdateDeviceCommand request);
        }

        private class PartialUpdateStrategy : IUpdateStrategy
        {
            public void UpdateDevice(DeviceModel original, UpdateDeviceCommand request)
            {
                if (request.Brand != default)
                    original.Brand = request.Brand;

                if (request.Name != default)
                    original.Name = request.Name;

                if (request.CreationTime.HasValue && request.CreationTime.Value != default)
                    original.CreationTime = request.CreationTime.Value;
            }
        }

        private class FullUpdateStrategy : IUpdateStrategy
        {
            public void UpdateDevice(DeviceModel original, UpdateDeviceCommand request)
            {
                original.Brand = request.Brand;
                original.CreationTime = request.CreationTime.GetValueOrDefault();
                original.Name = request.Name;
            }
        }

    }
}
