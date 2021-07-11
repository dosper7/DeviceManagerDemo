using DeviceManager.Business.Core.Common;
using DeviceManager.Business.Models;
using MediatR;
using System;

namespace DeviceManager.Business.UseCases.Device.AddDevice
{
    public record AddDeviceCommand : IRequest<ApiResult<DeviceModel>>
    {
        public string Name { get; set; }
        public string Brand { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
