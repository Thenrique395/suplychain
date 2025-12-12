using FluentValidation;
using System;
using SupplyChain.Application;

namespace SupplyChain.Application.Validators;

public class CreateEmployeeValidator : AbstractValidator<CreateEmployeeRequest>
{
    public CreateEmployeeValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty();
        RuleFor(x => x.LastName).NotEmpty();
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.DocNumber).NotEmpty();
        RuleFor(x => x.Phones).NotNull().Must(p => p.Count >= 2).WithMessage("At least two phone numbers are required.");
        RuleFor(x => x.BirthDate).Must(BeAdult).WithMessage("Employee must be at least 18 years old.");
        RuleFor(x => x.Password).MinimumLength(8).WithMessage("Password must be at least 8 characters.");
    }

    private bool BeAdult(DateTime birthDate) => birthDate <= DateTime.UtcNow.AddYears(-18);
}