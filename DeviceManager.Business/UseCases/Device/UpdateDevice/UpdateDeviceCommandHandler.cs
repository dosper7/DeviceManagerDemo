using DeviceManager.Business.Core.Common;
using DeviceManager.Business.Core.Contracts;
using DeviceManager.Business.Models;
using DeviceManager.Business.Ports;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DeviceManager.Business.UseCases.Device.UpdateDevice
{
    public class UpdateDeviceCommandHandler : IBaseRequestHandler<UpdateDeviceCommand, DeviceModel>
    {
        private readonly IDeviceStore _store;

        public UpdateDeviceCommandHandler(IDeviceStore store)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
        }

        public async Task<ApiResult<DeviceModel>> Handle(UpdateDeviceCommand request, CancellationToken cancellationToken)
        {
            if (await _store.GetDeviceByIdAsync(request.Id).ConfigureAwait(false) == null)
                return ApiResult.FromError<DeviceModel>($"Device with id {request.Id} doesn't exist.");

            var device = GetDeviceModel(request);
            var updatedModel = await _store.UpateDeviceAsync(device, request.IsPartialUpdate).ConfigureAwait(false);
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
