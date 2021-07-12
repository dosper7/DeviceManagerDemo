using FluentValidation;

namespace DeviceManager.Business.UseCases.Device.AddDevice
{
    public class AddDeviceCommandValidator : AbstractValidator<AddDeviceCommand>
    {
        public AddDeviceCommandValidator()
        {
            RuleFor(m => m.Name).NotEmpty().WithMessage("Name is missing.");
            RuleFor(m => m.Brand).NotEmpty().WithMessage("Brand is missing.");
            RuleFor(m => m.CreationTime).NotEmpty().WithMessage("CreationTimeme is missing.");
        }

    }
}
