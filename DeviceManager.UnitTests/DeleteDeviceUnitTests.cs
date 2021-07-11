using DeviceManager.Business.Core.Common;
using DeviceManager.Business.Models;
using DeviceManager.Business.UseCases.Device.DeleteDevice;
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
    public class DeleteDeviceUnitTests : BaseDeviceTest<DeleteDeviceCommandHandler>
    {
        [Fact]
        public async Task Handler_DeleteDevice_should_return_Device_when_deleted_with_success()
        {
            var handler = new DeleteDeviceCommandHandler(Database.Object, Logger.Object);
            Database.Setup(x => x.GetDeviceByIdAsync(It.IsAny<Guid>())).ReturnsAsync(GetDeviceMock());
            Database.Setup(x => x.DeleteDeviceAsync(It.IsAny<Guid>())).ReturnsAsync(GetDeviceMock());

            var response = await handler.Handle(new DeleteDeviceCommand() {Id = Guid.NewGuid() }, default).ConfigureAwait(false);

            response.Data.Should().NotBeNull();
            response.Data.Id.Should().NotBeEmpty();
        }

        [Fact]
        public async Task Handler_DeleteDevice_should_return_Error_when_deleted_device_is_not_returned()
        {
            var handler = new DeleteDeviceCommandHandler(Database.Object, Logger.Object);
            Database.Setup(x => x.GetDeviceByIdAsync(It.IsAny<Guid>())).ReturnsAsync(GetDeviceMock());
            Database.Setup(x => x.DeleteDeviceAsync(It.IsAny<Guid>()));

            var response = await handler.Handle(new DeleteDeviceCommand() { Id = Guid.NewGuid() }, default).ConfigureAwait(false);

            response.Data.Should().BeNull();
        }

        [Fact]
        public async Task Handler_DeleteDevice_should_return_Error_when_Id_doesnt_exist()
        {
            var handler = new DeleteDeviceCommandHandler(Database.Object, Logger.Object);
            Database.Setup(x => x.GetDeviceByIdAsync(It.IsAny<Guid>()));

            var response = await handler.Handle(new DeleteDeviceCommand() { Id = Guid.NewGuid() }, default).ConfigureAwait(false);

            response.Data.Should().BeNull();
        }

        [Fact]
        public async Task Validator_DeleteDevice_should_fail_if_Id_not_defined()
        {
            var validator = new DeleteDeviceCommandValidator();

            var validations = await validator.ValidateAsync(new DeleteDeviceCommand());

            validations.IsValid.Should().BeFalse();
        }

        [Fact]
        public async Task Validator_DeleteDevice_should_pass_if_Id_defined()
        {
            var validator = new DeleteDeviceCommandValidator();

            var validations = await validator.ValidateAsync(new DeleteDeviceCommand() { Id = Guid.NewGuid() });

            validations.IsValid.Should().BeTrue();
        }

        [Fact]
        public async Task Controller_DeleteDevice_should_return_BadRequest_if_id_doesnt_exists()
        {
            var mock = new ApiResult<DeviceModel>();

            Mediator.Setup(x => x.Send(It.IsAny<DeleteDeviceCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(mock);
            var controller = new DevicesController(Mediator.Object);

            var action = await controller.Delete(Guid.Empty).ConfigureAwait(false);

            action.Should().BeAssignableTo<NotFoundObjectResult>();
        }

        [Fact]
        public async Task Controller_DeleteDevice_should_return_Ok_if_device_with_given_id_exists()
        {
            var mock = new ApiResult<DeviceModel>(GetDeviceMock());

            Mediator.Setup(x => x.Send(It.IsAny<DeleteDeviceCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(mock);
            var controller = new DevicesController(Mediator.Object);

            var action = await controller.Delete(Guid.NewGuid()).ConfigureAwait(false);

            action.Should().BeAssignableTo<OkObjectResult>();
        }
    }
}
