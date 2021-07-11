using DeviceManager.Business.Core.Common;
using DeviceManager.Business.Models;
using DeviceManager.Business.UseCases.Device.GetAllDevices;
using DeviceManager.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace DeviceManager.UnitTests
{
    public class GetAllDevices : BaseDeviceTest<GetAllDevicesQueryHandler>
    {
        [Fact]
        public async Task Handler_GetAllDevices_should_return_PagedResult_with_success()
        {
            var query = new GetAllDevicesQuery();
            var devices = Enumerable.Range(0, 100).Select(p => GetDeviceMock());
            var mock = new PagedResult<DeviceModel>()
            {
                Items = devices,
                TotalCount = devices.Count()
            };

            var handler = new GetAllDevicesQueryHandler(Database.Object);
            Database.Setup(x => x.GetAllDevicesAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(mock);

            var response = await handler.Handle(new GetAllDevicesQuery(), default).ConfigureAwait(false);

            response.Data.Should().NotBeNull();
            response.Data.Items.Should().NotBeEmpty();
        }

        [Fact]
        public async Task Validator_GetAllDevices_should_fail_if_StartIndex_is_negative()
        {
            var query = new GetAllDevicesQuery()
            {
                StartIndex = -1,
                PageSize = 1
            };

            var validator = new GetAllDevicesQueryValidator();
            var validations = await validator.ValidateAsync(query);

            validations.IsValid.Should().BeFalse();
        }

        [Fact]
        public async Task Validator_GetAllDevices_should_fail_if_PageSize_is_not_higher_than_one()
        {
            var query = new GetAllDevicesQuery()
            {
                StartIndex = 0,
                PageSize = 0
            };

            var validator = new GetAllDevicesQueryValidator();
            var validations = await validator.ValidateAsync(query);

            validations.IsValid.Should().BeFalse();
        }

        [Fact]
        public async Task Validator_GetAllDevices_should_pass_if_PageSize_greater_than_0_and_startIndex_not_negative()
        {
            var query = new GetAllDevicesQuery()
            {
                StartIndex = 0,
                PageSize = 1
            };

            var validator = new GetAllDevicesQueryValidator();
            var validations = await validator.ValidateAsync(query);

            validations.IsValid.Should().BeTrue();
        }

        [Fact]
        public async Task Controller_GetAllDevices_should_return_OkResult_when_results_exists()
        {
            var mock = new ApiResult<PagedResult<DeviceModel>>(new PagedResult<DeviceModel>() { Items = new[] { GetDeviceMock() } });

            Mediator.Setup(x => x.Send(It.IsAny<GetAllDevicesQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(mock);
            var controller = new DevicesController(Mediator.Object);

            var action = await controller.GetAll(new GetAllDevicesQuery()).ConfigureAwait(false);

            action.Result.Should().BeAssignableTo<OkObjectResult>();
        }

        [Fact]
        public async Task Controller_GetAllDevices_should_return_PagedResult_Object_when_results_exists()
        {
            var mock = new ApiResult<PagedResult<DeviceModel>>(new PagedResult<DeviceModel>() { Items = new[] { GetDeviceMock() } });

            Mediator.Setup(x => x.Send(It.IsAny<GetAllDevicesQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(mock);
            var controller = new DevicesController(Mediator.Object);

            var action = await controller.GetAll(new GetAllDevicesQuery()).ConfigureAwait(false);
            action.Result.Should().BeAssignableTo<OkObjectResult>();
            (action.Result as OkObjectResult).Value.Should().BeAssignableTo<PagedResult<DeviceModel>>();
        }

        [Fact]
        public async Task Controller_GetAllDevices_should_return_NoContent_when_results_dont_exists()
        {
            var mock = new ApiResult<PagedResult<DeviceModel>>();

            Mediator.Setup(x => x.Send(It.IsAny<GetAllDevicesQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(mock);
            var controller = new DevicesController(Mediator.Object);

            var action = await controller.GetAll(new GetAllDevicesQuery()).ConfigureAwait(false);
            
            action.Result.Should().BeAssignableTo<NoContentResult>();
        }
    }
}
