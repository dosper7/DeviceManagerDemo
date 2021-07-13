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

namespace DeviceManager.UnitTests.UseCases
{

    public class UpdateDeviceUnitTests : BaseDeviceTest<UpdateDeviceCommandHandler>
    {
        [Fact]
        public async Task Handler_Update_Device_should_return_updated_device()
        {
            var (mock, handler) = SetupMockAndHandler();

            var response = await handler.Handle(new UpdateDeviceCommand(), default).ConfigureAwait(false);

            response.Data.Should().NotBeNull();
            response.Data.Id.Should().NotBeEmpty();
        }

        [Fact]
        public async Task Handler_Update_Device_should_override_all_fields_when_is_fullUpdate()
        {
            var (mock, handler) = SetupMockAndHandler();
            var dataToUpdate = new UpdateDeviceCommand
            {
                Name = "1",
                Brand = "1",
                CreationTime = DateTime.Now.AddDays(1),
                Id = mock.Id,
                UpdateType = UpdateDeviceCommand.UpdateTypeEnum.Full,
            };
            var response = await handler.Handle(dataToUpdate, default).ConfigureAwait(false);

            response.Should().NotBeNull();
            response.Data.Name.Should().Be(dataToUpdate.Name);
            response.Data.Brand.Should().Be(dataToUpdate.Brand);
            response.Data.CreationTime.Should().Be(dataToUpdate.CreationTime.GetValueOrDefault());
            response.Data.Id.Should().Be(dataToUpdate.Id);
        }

        [Fact]
        public async Task Handler_Update_Device_should_update_brand_when_IsPartialUpdate()
        {
            var (mock, handler) = SetupMockAndHandler();
            var dataToUpdate = new UpdateDeviceCommand
            {
                Brand = "1",
                UpdateType = UpdateDeviceCommand.UpdateTypeEnum.Partial,
            };
            var response = await handler.Handle(dataToUpdate, default).ConfigureAwait(false);

            response.Should().NotBeNull();
            response.Data.Name.Should().Be(mock.Name);
            response.Data.Brand.Should().Be(dataToUpdate.Brand);
            response.Data.CreationTime.Should().Be(mock.CreationTime);
            response.Data.Id.Should().Be(mock.Id);
        }

        [Fact]
        public async Task Handler_Update_Device_should_update_Name_when_IsPartialUpdate()
        {
            var (mock, handler) = SetupMockAndHandler();
            var dataToUpdate = new UpdateDeviceCommand
            {
                Name = "1",
                UpdateType = UpdateDeviceCommand.UpdateTypeEnum.Partial,
            };
            var response = await handler.Handle(dataToUpdate, default).ConfigureAwait(false);

            response.Should().NotBeNull();
            response.Data.Name.Should().Be(dataToUpdate.Name);
            response.Data.Brand.Should().Be(mock.Brand);
            response.Data.CreationTime.Should().Be(mock.CreationTime);
            response.Data.Id.Should().Be(mock.Id);
        }

        [Fact]
        public async Task Handler_Update_Device_should_update_CreationTime_when_IsPartialUpdate()
        {
            var (mock, handler) = SetupMockAndHandler();
            var dataToUpdate = new UpdateDeviceCommand
            {
                CreationTime = DateTime.Now.AddDays(1),
                UpdateType = UpdateDeviceCommand.UpdateTypeEnum.Partial,
            };
            var response = await handler.Handle(dataToUpdate, default).ConfigureAwait(false);

            response.Should().NotBeNull();
            response.Data.Name.Should().Be(mock.Name);
            response.Data.Brand.Should().Be(mock.Brand);
            response.Data.CreationTime.Should().Be(dataToUpdate.CreationTime.GetValueOrDefault());
            response.Data.Id.Should().Be(mock.Id);
        }

        [Fact]
        public async Task Handler_Update_Device_should_return_error_if_deviceId_doesnt_exist()
        {
            var (mock, handler) = SetupMockAndHandler(returnNullOnGetDeviceById: true);

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
                UpdateType = UpdateDeviceCommand.UpdateTypeEnum.Full,
            });

            validations.IsValid.Should().BeFalse();
        }

        [Fact]
        public async Task Validator_Update_Device_should_fail_if_is_fullUpdate_and_name_field_not_defined()
        {
            var validator = new UpdateDeviceCommandValidator();

            var validations = await validator.ValidateAsync(new UpdateDeviceCommand()
            {
                Brand = "1",
                CreationTime = DateTime.Now,
                Id = Guid.NewGuid(),
                UpdateType = UpdateDeviceCommand.UpdateTypeEnum.Full,
            });

            validations.IsValid.Should().BeFalse();
        }

        [Fact]
        public async Task Validator_Update_Device_should_fail_if_is_fullUpdate_and_brand_field_not_defined()
        {
            var validator = new UpdateDeviceCommandValidator();

            var validations = await validator.ValidateAsync(new UpdateDeviceCommand()
            {
                Name = "1",
                CreationTime = DateTime.Now,
                Id = Guid.NewGuid(),
                UpdateType = UpdateDeviceCommand.UpdateTypeEnum.Full,
            });

            validations.IsValid.Should().BeFalse();
        }

        [Fact]
        public async Task Validator_Update_Device_should_fail_if_is_fullUpdate_and_CreationTime_field_not_defined()
        {
            var validator = new UpdateDeviceCommandValidator();

            var validations = await validator.ValidateAsync(new UpdateDeviceCommand()
            {
                Name = "1",
                Brand = "1",
                Id = Guid.NewGuid(),
                UpdateType = UpdateDeviceCommand.UpdateTypeEnum.Full,
            });

            validations.IsValid.Should().BeFalse();
        }

        [Fact]
        public async Task Validator_Update_Device_should_fail_if_is_PartialUpdate_and_no_field_is_defined()
        {
            var validator = new UpdateDeviceCommandValidator();

            var validations = await validator.ValidateAsync(new UpdateDeviceCommand()
            {
                Id = Guid.NewGuid(),
                UpdateType = UpdateDeviceCommand.UpdateTypeEnum.Partial,
            });

            validations.IsValid.Should().BeFalse();
        }

        [Fact]
        public async Task Validator_Update_Device_should_pass_if_is_PartialUpdate_and_and_Name_field_is_defined()
        {
            var validator = new UpdateDeviceCommandValidator();

            var validations = await validator.ValidateAsync(new UpdateDeviceCommand()
            {
                Id = Guid.NewGuid(),
                Name = "1",
                UpdateType = UpdateDeviceCommand.UpdateTypeEnum.Partial,
            });

            validations.IsValid.Should().BeTrue();
        }

        [Fact]
        public async Task Validator_Update_Device_should_pass_if_is_PartialUpdate_and_and_Brand_field_is_defined()
        {
            var validator = new UpdateDeviceCommandValidator();

            var validations = await validator.ValidateAsync(new UpdateDeviceCommand()
            {
                Id = Guid.NewGuid(),
                Brand = "1",
                UpdateType = UpdateDeviceCommand.UpdateTypeEnum.Partial,
            });

            validations.IsValid.Should().BeTrue();
        }

        [Fact]
        public async Task Validator_Update_Device_should_pass_if_is_PartialUpdate_and_and_CreationTime_field_is_defined()
        {
            var validator = new UpdateDeviceCommandValidator();

            var validations = await validator.ValidateAsync(new UpdateDeviceCommand()
            {
                Id = Guid.NewGuid(),
                CreationTime = DateTime.Now,
                UpdateType = UpdateDeviceCommand.UpdateTypeEnum.Partial,
            });

            validations.IsValid.Should().BeTrue();
        }

        [Fact]
        public async Task Validator_Update_Device_should_pass_if_is_fullUpdate_and_all_fields_are_defined()
        {
            var validator = new UpdateDeviceCommandValidator();

            var validations = await validator.ValidateAsync(new UpdateDeviceCommand()
            {
                Id = Guid.NewGuid(),
                Name = "1",
                CreationTime = DateTime.Now,
                Brand = "1",
                UpdateType = UpdateDeviceCommand.UpdateTypeEnum.Full,
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
            var mock = new ApiResult<DeviceModel>() { Data = MockDeviceBuilder.Build() };

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
            var mock = new ApiResult<DeviceModel>() { Data = MockDeviceBuilder.Build() };

            Mediator.Setup(x => x.Send(It.IsAny<UpdateDeviceCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(mock);
            var controller = new DevicesController(Mediator.Object);

            var action = await controller.PartialUpdate(new DeviceModel()).ConfigureAwait(false);

            action.Should().BeOfType<OkObjectResult>();
        }


        private (DeviceModel mock, UpdateDeviceCommandHandler handler) SetupMockAndHandler(bool returnNullOnGetDeviceById = false, bool returnMockOnHandler = false)
        {
            var mock = MockDeviceBuilder.Build();
            var handler = new UpdateDeviceCommandHandler(Database.Object, Logger.Object);
            Database.Setup(x => x.GetDeviceByIdAsync(It.IsAny<Guid>())).ReturnsAsync(returnNullOnGetDeviceById ? null : mock);
            Database.Setup(x => x.UpateDeviceAsync(It.IsAny<DeviceModel>())).ReturnsAsync(returnMockOnHandler ? null : mock);
            return (mock, handler);
        }




    }
}
