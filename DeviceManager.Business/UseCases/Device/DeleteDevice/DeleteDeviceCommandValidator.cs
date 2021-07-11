using DeviceManager.Business.UseCases.Device.AddDevice;
using FluentValidation;

namespace DeviceManager.Business.UseCases.Device.DeleteDevice
{
    public class DeleteDeviceCommandValidator : AbstractValidator<DeleteDeviceCommand>
    {
        public DeleteDeviceCommandValidator()
        {
            RuleFor(m => m.Id).NotEmpty().WithMessage("Id is missing.");
        }

    }
}
