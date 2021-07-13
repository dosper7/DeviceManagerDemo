using FluentValidation;

namespace DeviceManager.Business.UseCases.Device.UpdateDevice
{
    public class UpdateDeviceCommandValidator : AbstractValidator<UpdateDeviceCommand>
    {
        public UpdateDeviceCommandValidator()
        {
            RuleFor(m => m.Id).NotEmpty().WithMessage("Id is missing.");
            When(m => m.UpdateType == UpdateDeviceCommand.UpdateTypeEnum.Full, () =>
            {
                RuleFor(m => m.Name).NotNull().WithMessage("Name is missing.");
                RuleFor(m => m.Brand).NotNull().WithMessage("Brand is missing.");
                RuleFor(m => m.CreationTime).NotEmpty().WithMessage("CreationTimeme is missing.");
            });

            When(m => m.UpdateType == UpdateDeviceCommand.UpdateTypeEnum.Partial, () => RuleFor(m => m).Custom(AtLeastOneFieldShouldHaveValue));
        }

        private void AtLeastOneFieldShouldHaveValue(UpdateDeviceCommand command, ValidationContext<UpdateDeviceCommand> ctx)
        {
            if (string.IsNullOrWhiteSpace(command.Name) &&
                string.IsNullOrWhiteSpace(command.Brand) &&
                command.CreationTime.GetValueOrDefault() == default)
            {
                ctx.AddFailure("At least one field should be define.");
            }
        }


    }
}
