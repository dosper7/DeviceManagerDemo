using FluentValidation;

namespace DeviceManager.Business.UseCases.Device.GetDeviceById
{
    public class GetDeviceByIdQueryValidator : AbstractValidator<GetDeviceByIdQuery>
    {
        public GetDeviceByIdQueryValidator()
        {
            RuleFor(m => m.Id).NotEmpty().WithMessage("Id is missing.");
        }

    }
}
