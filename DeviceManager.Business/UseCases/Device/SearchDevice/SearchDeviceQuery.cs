using DeviceManager.Business.Core.Common;
using DeviceManager.Business.Models;
using MediatR;
using System;

namespace DeviceManager.Business.UseCases.Device.SearchDevice
{
    public record SearchDeviceQuery : IRequest<ApiResult<PagedResult<DeviceModel>>>
    {
        public string Name { get; set; }
        public string Brand { get; set; }
        public DateTime? CreationTime { get; set; }
        public Guid? Id { get; set; }
        public int StartIndex { get; set; } = 0;
        public int PageSize { get; set; } = 20;

    }
}
