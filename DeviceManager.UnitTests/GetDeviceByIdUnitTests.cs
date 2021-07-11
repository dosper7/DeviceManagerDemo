using DeviceManager.Business.Core.Common;
using DeviceManager.Business.Models;
using DeviceManager.Business.UseCases.Device.GetAllDevices;
using DeviceManager.Business.UseCases.Device.GetDeviceById;
using DeviceManager.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DeviceManager.UnitTests
{
    public class GetDeviceByIdUnitTests : BaseDeviceTest<GetDeviceByIdQueryHandler>
    {
        [Fact]
        public async Task Handler_GetDeviceById_should_return_Device()
        {
            var handler = new GetDeviceByIdQueryHandler(Database.Object);
            Database.Setup(x => x.GetDeviceByIdAsync(It.IsAny<Guid>())).ReturnsAsync(GetDeviceMock());

            var response = await handler.Handle(new GetDeviceByIdQuery(), default).ConfigureAwait(false);

            response.Data.Should().NotBeNull();
            response.Data.Id.Should().NotBeEmpty();
        }

        [Fact]
        public async Task Validator_GetDeviceById_should_fail_if_Id_not_defined()
        {
            var validator = new GetDeviceByIdQueryValidator();

            var validations = await validator.ValidateAsync(new GetDeviceByIdQuery());

            validations.IsValid.Should().BeFalse();
        }

        [Fact]
        public async Task Validator_GetDeviceById_should_pass_if_Id_defined()
        {
            var validator = new GetDeviceByIdQueryValidator();

            var validations = await validator.ValidateAsync(new GetDeviceByIdQuery() { Id = Guid.NewGuid() });

            validations.IsValid.Should().BeTrue();
        }

        [Fact]
        public async Task Controller_GetDeviceById_should_return_BadRequest_if_id_doesnt_exists()
        {
            var mock = new ApiResult<DeviceModel>();

            Mediator.Setup(x => x.Send(It.IsAny<GetDeviceByIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(mock);
            var controller = new DevicesController(Mediator.Object);

            var action = await controller.GetById(Guid.Empty).ConfigureAwait(false);

            action.Result.Should().BeAssignableTo<NotFoundObjectResult>();
        }

        [Fact]
        public async Task Controller_GetDeviceById_should_return_Ok_if_device_with_given_id_exists()
        {
            var mock = new ApiResult<DeviceModel>(GetDeviceMock());

            Mediator.Setup(x => x.Send(It.IsAny<GetDeviceByIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(mock);
            var controller = new DevicesController(Mediator.Object);

            var action = await controller.GetById(Guid.NewGuid()).ConfigureAwait(false);

            action.Result.Should().BeAssignableTo<OkObjectResult>();
        }
    }
}
