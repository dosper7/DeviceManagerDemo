using DeviceManager.Business.Core.Common;
using DeviceManager.Business.Models;
using DeviceManager.Business.UseCases.Device.UpdateDevice;
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

    public class UpdateDeviceUnitTests : BaseDeviceTest<UpdateDeviceCommandHandler>
    {
        [Fact]
        public async Task Handler_Update_Device_should_return_updated_device()
        {

            var mock = GetDeviceMock();
            var handler = new UpdateDeviceCommandHandler(Database.Object);
            Database.Setup(x => x.GetDeviceByIdAsync(It.IsAny<Guid>())).ReturnsAsync(mock);
            Database.Setup(x => x.UpateDeviceAsync(It.IsAny<DeviceModel>(), It.IsAny<bool>())).ReturnsAsync(mock);

            var response = await handler.Handle(new UpdateDeviceCommand(), default).ConfigureAwait(false);

            response.Data.Should().NotBeNull();
            response.Data.Id.Should().NotBeEmpty();
        }

        [Fact]
        public async Task Handler_Update_Device_should_return_error_if_deviceId_doesnt_exist()
        {

            var mock = GetDeviceMock();
            var handler = new UpdateDeviceCommandHandler(Database.Object);
            Database.Setup(x => x.GetDeviceByIdAsync(It.IsAny<Guid>()));
            Database.Setup(x => x.UpateDeviceAsync(It.IsAny<DeviceModel>(), It.IsAny<bool>())).ReturnsAsync(mock);

            var response = await handler.Handle(new UpdateDeviceCommand(), default).ConfigureAwait(false);

            response.Should().NotBeNull();
            response.Success.Should().BeFalse();
        }

        [Fact]
        public async Task Validator_Update_Device_should_fail_if_Id_not_defined()
        {
            var validator = new UpdateDeviceCommandValidator();

            var validations = await validator.ValidateAsync(new UpdateDeviceCommand()
            {
                Brand = "1",
                CreationTime = DateTime.Now,
                Name = "1",
                IsPartialUpdate = false,
            });

            validations.IsValid.Should().BeFalse();
        }

        [Fact]
        public async Task Validator_Update_Device_should_fail_if_PartialUpdate_is_false_and_name_field_not_defined()
        {
            var validator = new UpdateDeviceCommandValidator();

            var validations = await validator.ValidateAsync(new UpdateDeviceCommand()
            {
                Brand = "1",
                CreationTime = DateTime.Now,
                Id = Guid.NewGuid(),
                IsPartialUpdate = false,
            });

            validations.IsValid.Should().BeFalse();
        }

        [Fact]
        public async Task Validator_Update_Device_should_fail_if_PartialUpdate_is_false_and_brand_field_not_defined()
        {
            var validator = new UpdateDeviceCommandValidator();

            var validations = await validator.ValidateAsync(new UpdateDeviceCommand()
            {
                Name = "1",
                CreationTime = DateTime.Now,
                Id = Guid.NewGuid(),
                IsPartialUpdate = false,
            });

            validations.IsValid.Should().BeFalse();
        }

        [Fact]
        public async Task Validator_Update_Device_should_fail_if_PartialUpdate_is_false_and_CreationTime_field_not_defined()
        {
            var validator = new UpdateDeviceCommandValidator();

            var validations = await validator.ValidateAsync(new UpdateDeviceCommand()
            {
                Name = "1",
                Brand = "1",
                Id = Guid.NewGuid(),
                IsPartialUpdate = false,
            });

            validations.IsValid.Should().BeFalse();
        }

        [Fact]
        public async Task Validator_Update_Device_should_fail_if_PartialUpdate_is_true_no_field_is_defined()
        {
            var validator = new UpdateDeviceCommandValidator();

            var validations = await validator.ValidateAsync(new UpdateDeviceCommand()
            {
                Id = Guid.NewGuid(),
                IsPartialUpdate = true,
            });

            validations.IsValid.Should().BeFalse();
        }

        [Fact]
        public async Task Validator_Update_Device_should_pass_if_PartialUpdate_is_true_and_Name_field_is_defined()
        {
            var validator = new UpdateDeviceCommandValidator();

            var validations = await validator.ValidateAsync(new UpdateDeviceCommand()
            {
                Id = Guid.NewGuid(),
                Name="1",
                IsPartialUpdate = true,
            });

            validations.IsValid.Should().BeTrue();
        }

        [Fact]
        public async Task Validator_Update_Device_should_pass_if_PartialUpdate_is_true_and_Brand_field_is_defined()
        {
            var validator = new UpdateDeviceCommandValidator();

            var validations = await validator.ValidateAsync(new UpdateDeviceCommand()
            {
                Id = Guid.NewGuid(),
                Brand = "1",
                IsPartialUpdate = true,
            });

            validations.IsValid.Should().BeTrue();
        }

        [Fact]
        public async Task Validator_Update_Device_should_pass_if_PartialUpdate_is_true_and_CreationTime_field_is_defined()
        {
            var validator = new UpdateDeviceCommandValidator();

            var validations = await validator.ValidateAsync(new UpdateDeviceCommand()
            {
                Id = Guid.NewGuid(),
                CreationTime = DateTime.Now,
                IsPartialUpdate = true,
            });

            validations.IsValid.Should().BeTrue();
        }

        [Fact]
        public async Task Validator_Update_Device_should_pass_if_PartialUpdate_is_false_and_all_fields_are_defined()
        {
            var validator = new UpdateDeviceCommandValidator();

            var validations = await validator.ValidateAsync(new UpdateDeviceCommand()
            {
                Id = Guid.NewGuid(),
                Name = "1",
                CreationTime = DateTime.Now,
                Brand = "1",
                IsPartialUpdate = false,
            });

            validations.IsValid.Should().BeTrue();
        }



        [Fact]
        public async Task Controller_PartialUpdate_Device_should_return_BadRequest_when_result_has_errors()
        {
            var mock = new ApiResult<DeviceModel>();
            mock.AddError(string.Empty);

            Mediator.Setup(x => x.Send(It.IsAny<UpdateDeviceCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(mock);
            var controller = new DevicesController(Mediator.Object);

            var action = await controller.PartialUpdate(new DeviceModel()).ConfigureAwait(false);

            action.Should().BeAssignableTo<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Controller_PartialUpdate_should_return_OKResult_when_result_Is_Success()
        {
            var mock = new ApiResult<DeviceModel>() { Data = GetDeviceMock() };

            Mediator.Setup(x => x.Send(It.IsAny<UpdateDeviceCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(mock);
            var controller = new DevicesController(Mediator.Object);

            var action = await controller.PartialUpdate(new DeviceModel()).ConfigureAwait(false);

            action.Should().BeOfType<OkObjectResult>();
        }


        [Fact]
        public async Task Controller_FullUpdate_Device_should_return_BadRequest_when_result_has_errors()
        {
            var mock = new ApiResult<DeviceModel>();
            mock.AddError(string.Empty);

            Mediator.Setup(x => x.Send(It.IsAny<UpdateDeviceCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(mock);
            var controller = new DevicesController(Mediator.Object);

            var action = await controller.PartialUpdate(new DeviceModel()).ConfigureAwait(false);

            action.Should().BeAssignableTo<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Controller_FullUpdate_should_return_OKResult_when_result_Is_Success()
        {
            var mock = new ApiResult<DeviceModel>() { Data = GetDeviceMock() };

            Mediator.Setup(x => x.Send(It.IsAny<UpdateDeviceCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(mock);
            var controller = new DevicesController(Mediator.Object);

            var action = await controller.PartialUpdate(new DeviceModel()).ConfigureAwait(false);

            action.Should().BeOfType<OkObjectResult>();
        }


    }
}
