using DeviceManager.Business.Core.Common;
using DeviceManager.Business.Models;
using DeviceManager.Business.UseCases.Device.AddDevice;
using DeviceManager.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DeviceManager.UnitTests.UseCases
{

    public class AddDeviceUnitTests : BaseDeviceTest<AddDeviceCommandHandler>
    {
        [Fact]
        public async Task Handler_Add_Device_should_return_id()
        {
            var handler = new AddDeviceCommandHandler(Database.Object, Logger.Object);
            Database.Setup(x => x.AddDeviceAsync(It.IsAny<DeviceModel>())).ReturnsAsync(MockDeviceBuilder.Build());

            var response = await handler.Handle(new AddDeviceCommand(), default).ConfigureAwait(false);

            response.Data.Should().NotBeNull();
            response.Data.Id.Should().NotBeEmpty();
        }

        [Fact]
        public async Task Handler_Add_Device_should_return_error_if_id_not_present()
        {
            var mock = MockDeviceBuilder.WithId(Guid.Empty).Build();
            var handler = new AddDeviceCommandHandler(Database.Object, Logger.Object);
            Database.Setup(x => x.AddDeviceAsync(It.IsAny<DeviceModel>())).ReturnsAsync(mock);

            var response = await handler.Handle(new AddDeviceCommand(), default).ConfigureAwait(false);

            response.Success.Should().BeFalse();
        }

        [Fact]
        public async Task Validator_Add_Device_should_fail_if_Name_not_defined()
        {
            var validator = new AddDeviceCommandValidator();

            var validations = await validator.ValidateAsync(new AddDeviceCommand()
            {
                Brand = "1",
                CreationTime = DateTime.Now
            });

            validations.IsValid.Should().BeFalse();
        }

        [Fact]
        public async Task Validator_Add_Device_should_fail_if_Brand_not_defined()
        {
            var validator = new AddDeviceCommandValidator();

            var validations = await validator.ValidateAsync(new AddDeviceCommand()
            {
                Name = "1",
                CreationTime = DateTime.Now
            });

            validations.IsValid.Should().BeFalse();
        }

        [Fact]
        public async Task Validator_Add_Device_should_fail_if_CreationTime_not_defined()
        {
            var validator = new AddDeviceCommandValidator();

            var validations = await validator.ValidateAsync(new AddDeviceCommand()
            {
                Brand = "1",
                Name = "1",
            });

            validations.IsValid.Should().BeFalse();
        }

        [Fact]
        public async Task Validator_Add_Device_should_pass_if_Name_Brand_and_CreationDate_are_defined()
        {
            var validator = new AddDeviceCommandValidator();

            var validations = await validator.ValidateAsync(new AddDeviceCommand()
            {
                Brand = "1",
                Name = "1",
                CreationTime = DateTime.Now
            });

            validations.IsValid.Should().BeTrue();
        }


        [Fact]
        public async Task Controller_Add_Device_should_return_BadRequest_when_result_has_errors()
        {
            var mock = new ApiResult<DeviceModel>();
            mock.AddError(string.Empty);

            Mediator.Setup(x => x.Send(It.IsAny<AddDeviceCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(mock);
            var controller = new DevicesController(Mediator.Object);

            var action = await controller.AddDevice(new AddDeviceCommand()).ConfigureAwait(false);

            action.Should().BeAssignableTo<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Controller_Add_Device_should_return_CreatedAtResult_when_result_Is_Success()
        {
            var mock = new ApiResult<DeviceModel>() { Data = MockDeviceBuilder.Build() };

            Mediator.Setup(x => x.Send(It.IsAny<AddDeviceCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(mock);
            var controller = new DevicesController(Mediator.Object);

            var action = await controller.AddDevice(new AddDeviceCommand()).ConfigureAwait(false);

            action.Should().BeOfType<CreatedAtActionResult>();
        }


    }
}
