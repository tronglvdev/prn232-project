using FluentValidation;
using LaptopShop.BLL.DTOs;

namespace LaptopShop.BLL.Validators;

public class UserCreateDtoValidator : AbstractValidator<UserCreateDto>
{
    public UserCreateDtoValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Email is not valid");
        RuleFor(x => x.Password).NotEmpty().MinimumLength(2).WithMessage("Password phải có tối thiểu 2 ký tự");
        RuleFor(x => x.FullName).NotEmpty().MinimumLength(3).WithMessage("FullName phải có tối thiểu 3 ký tự");
    }
}
