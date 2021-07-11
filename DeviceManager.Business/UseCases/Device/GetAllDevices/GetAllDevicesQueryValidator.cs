using FluentValidation;

namespace DeviceManager.Business.UseCases.Device.GetAllDevices
{
    public class GetAllDevicesQueryValidator : AbstractValidator<GetAllDevicesQuery>
    {
        public GetAllDevicesQueryValidator()
        {
            RuleFor(m => m.StartIndex).GreaterThanOrEqualTo(0).WithMessage("Start Index cannot be a negative number.");
            RuleFor(m => m.PageSize).GreaterThan(0).WithMessage("Page Size can be 1 or higer.");
        }

    }
}
