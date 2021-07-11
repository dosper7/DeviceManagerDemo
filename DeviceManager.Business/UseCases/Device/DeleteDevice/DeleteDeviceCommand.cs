using DeviceManager.Business.Core.Common;
using DeviceManager.Business.Models;
using MediatR;
using System;

namespace DeviceManager.Business.UseCases.Device.DeleteDevice
{
    public record DeleteDeviceCommand : IRequest<ApiResult<DeviceModel>>
    {
        public Guid Id { get; set; }
    }
}
