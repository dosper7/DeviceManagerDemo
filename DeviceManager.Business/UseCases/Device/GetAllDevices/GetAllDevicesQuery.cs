using DeviceManager.Business.Core.Common;
using DeviceManager.Business.Models;
using MediatR;

namespace DeviceManager.Business.UseCases.Device.GetAllDevices
{
    public record GetAllDevicesQuery : IRequest<ApiResult<PagedResult<DeviceModel>>>
    {
        /// <summary>
        /// zero based index to indicate the page to return.
        /// </summary>
        public int StartIndex { get; set; } = 0;
        /// <summary>
        /// Number of items to return in a page
        /// </summary>
        public int PageSize { get; set; } = 20;
    }
}
