using DeviceManager.Business.Core.Common;
using DeviceManager.Business.Models;
using DeviceManager.Business.UseCases.Device.SearchDevice;
using DeviceManager.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DeviceManager.UnitTests
{
    public class SearchDevices : BaseDeviceTest<SearchDeviceQueryHandler>
    {
        [Fact]
        public async Task Handler_SearchDevices_should_return_PagedResult_with_success()
        {
            var query = new SearchDeviceQuery();
            var devices = Enumerable.Range(0, 100).Select(p => GetDeviceMock());
            var mock = new PagedResult<DeviceModel>()
            {
                Items = devices,
                TotalCount = devices.Count()
            };

            var handler = new SearchDeviceQueryHandler(Database.Object);
            Database.Setup(x => x.SearchDeviceAsync(It.IsAny<DeviceModel>(), It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(mock);

            var response = await handler.Handle(new SearchDeviceQuery(), default).ConfigureAwait(false);

            response.Data.Should().NotBeNull();
            response.Data.Items.Should().NotBeEmpty();
        }

        [Fact]
        public async Task Validator_SearchDevices_should_fail_if_StartIndex_is_negative()
        {
            var query = new SearchDeviceQuery()
            {
                StartIndex = -1,
                PageSize = 1
            };

            var validator = new SearchDeviceQueryValidator();
            var validations = await validator.ValidateAsync(query);

            validations.IsValid.Should().BeFalse();
        }

        [Fact]
        public async Task Validator_SearchDevices_should_fail_if_PageSize_is_not_higher_than_one()
        {
            var query = new SearchDeviceQuery()
            {
                StartIndex = 0,
                PageSize = 0
            };

            var validator = new SearchDeviceQueryValidator();
            var validations = await validator.ValidateAsync(query);

            validations.IsValid.Should().BeFalse();
        }

        [Fact]
        public async Task Validator_SearchDevices_should_pass_if_PageSize_greater_than_0_and_startIndex_not_negative()
        {
            var query = new SearchDeviceQuery()
            {
                StartIndex = 0,
                PageSize = 1,
                Name = "1"
            };

            var validator = new SearchDeviceQueryValidator();
            var validations = await validator.ValidateAsync(query);

            validations.IsValid.Should().BeTrue();
        }

        [Fact]
        public async Task Validator_SearchDevices_should_pass_if_at_least_one_field_has_value()
        {
            var query = new SearchDeviceQuery()
            {
                Name = "1"
            };

            var validator = new SearchDeviceQueryValidator();
            var validations = await validator.ValidateAsync(query);

            validations.IsValid.Should().BeTrue();
        }

        [Fact]
        public async Task Validator_SearchDevices_should_fail_if_no_field_has_value()
        {
            var validator = new SearchDeviceQueryValidator();
            var validations = await validator.ValidateAsync(new SearchDeviceQuery());

            validations.IsValid.Should().BeFalse();
        }

        [Fact]
        public async Task Controller_SearchDevices_should_return_OkResult_when_results_exists()
        {
            var mock = new ApiResult<PagedResult<DeviceModel>>(new PagedResult<DeviceModel>() { Items = new[] { GetDeviceMock() } });

            Mediator.Setup(x => x.Send(It.IsAny<SearchDeviceQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(mock);
            var controller = new DevicesController(Mediator.Object);

            var action = await controller.Search(new SearchDeviceQuery()).ConfigureAwait(false);

            action.Should().BeAssignableTo<OkObjectResult>();
        }

        [Fact]
        public async Task Controller_SearchDevices_should_return_PagedResult_Object_when_results_exists()
        {
            var mock = new ApiResult<PagedResult<DeviceModel>>(new PagedResult<DeviceModel>() { Items = new[] { GetDeviceMock() } });

            Mediator.Setup(x => x.Send(It.IsAny<SearchDeviceQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(mock);
            var controller = new DevicesController(Mediator.Object);

            var action = await controller.Search(new SearchDeviceQuery()).ConfigureAwait(false);
            action.Should().BeAssignableTo<ObjectResult>();
            (action as ObjectResult).Value.Should().BeAssignableTo<PagedResult<DeviceModel>>();
        }

        [Fact]
        public async Task Controller_SearchDevices_should_return_BadRequest_when_result_has_errors()
        {
            var mock = new ApiResult<PagedResult<DeviceModel>>();
            mock.AddError(string.Empty);

            Mediator.Setup(x => x.Send(It.IsAny<SearchDeviceQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(mock);
            var controller = new DevicesController(Mediator.Object);

            var action = await controller.Search(new SearchDeviceQuery() { StartIndex = -1 }).ConfigureAwait(false);

            action.Should().BeAssignableTo<BadRequestObjectResult>();
        }


    }
}
