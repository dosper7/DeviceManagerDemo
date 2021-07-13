using DeviceManager.Business.Core.Common;
using DeviceManager.Business.Models;
using MediatR;
using System;

namespace DeviceManager.Business.UseCases.Device.UpdateDevice
{
    public record UpdateDeviceCommand : IRequest<ApiResult<DeviceModel>>
    {
        public string Name { get; set; }
        public string Brand { get; set; }
        public DateTime? CreationTime { get; set; }
        public Guid Id { get; set; }
        public UpdateTypeEnum UpdateType { get; set; }

        public enum UpdateTypeEnum
        {
            Partial,
            Full,
        }

    }
}
