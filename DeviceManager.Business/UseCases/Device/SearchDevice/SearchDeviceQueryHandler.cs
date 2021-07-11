using DeviceManager.Business.Core.Common;
using DeviceManager.Business.Core.Contracts;
using DeviceManager.Business.Models;
using DeviceManager.Business.Ports;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DeviceManager.Business.UseCases.Device.SearchDevice
{
    public class SearchDeviceQueryHandler : IBaseRequestHandler<SearchDeviceQuery, PagedResult<DeviceModel>>
    {
        private readonly IDeviceStore _store;

        public SearchDeviceQueryHandler(IDeviceStore store)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
        }

        public async Task<ApiResult<PagedResult<DeviceModel>>> Handle(SearchDeviceQuery request, CancellationToken cancellationToken)
        {
            var searchQuery = GetSearchModel(request);
            var pagedResult = await _store.SearchDeviceAsync(searchQuery, request.StartIndex, request.PageSize).ConfigureAwait(false);
            return ApiResult.FromResult(pagedResult);
        }

        private static DeviceModel GetSearchModel(SearchDeviceQuery query)
        {
            return new DeviceModel()
            {
                Id = query.Id.GetValueOrDefault(),
                Name = query.Name,
                Brand = query.Brand,
                CreationTime = query.CreationTime.GetValueOrDefault()
            };
        }
    }
}
