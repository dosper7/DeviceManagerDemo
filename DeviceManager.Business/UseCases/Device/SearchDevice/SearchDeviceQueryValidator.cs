using FluentValidation;

namespace DeviceManager.Business.UseCases.Device.SearchDevice
{
    public class SearchDeviceQueryValidator : AbstractValidator<SearchDeviceQuery>
    {
        public SearchDeviceQueryValidator()
        {
            RuleFor(m => m.StartIndex).GreaterThanOrEqualTo(0).WithMessage("Start Index cannot be a negative number.");
            RuleFor(m => m.PageSize).GreaterThan(0).WithMessage("Page Size can be 1 or higer.");
            RuleFor(model => model).Custom(AtLeastOneFieldShouldHaveValue);

        }

        private void AtLeastOneFieldShouldHaveValue(SearchDeviceQuery search, ValidationContext<SearchDeviceQuery> ctx)
        {
            if(
                string.IsNullOrWhiteSpace(search.Name) && 
                string.IsNullOrWhiteSpace(search.Brand) &&
                search.Id.GetValueOrDefault() == default &&
                search.CreationTime.GetValueOrDefault() == default)
            {
                ctx.AddFailure("At least one field should be define.");
            }
        }
    }
}
