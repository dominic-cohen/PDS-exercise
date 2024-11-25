

using FluentValidation;
using UKParliament.CodeTest.Web.ViewModels;

namespace UKParliament.CodeTest.Web.Validators
{

    public class PersonViewModelValidator : AbstractValidator<PersonViewModel>
    {
        public PersonViewModelValidator()
        {

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.");

            RuleFor(x => x.DOB)
                .NotEmpty().WithMessage("Date of birth is required.")
                .Must(BeAValidDate).WithMessage("Date of birth must be a valid date.");

            RuleFor(x => x.DepartmentId)
                    .GreaterThan(0).WithMessage("Department must be selected.");
            }

        private bool BeAValidDate(string date)
        {
            return DateTime.TryParse(date, out _);
        }
    }


    public class ErrorResponse
    {
        public string Message { get; set; }
        public IEnumerable<ErrorDetail> Errors { get; set; }
    }

    public class ErrorDetail
    {
        public string PropertyName { get; set; }
        public string ErrorMessage { get; set; }
    }


}
