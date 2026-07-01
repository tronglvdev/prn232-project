using FluentValidation;
using LaptopShop.BLL.DTOs;

namespace LaptopShop.BLL.Validators;

public class ProductDtoValidator : AbstractValidator<ProductDto>
{
    public ProductDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Tên sản phẩm không được để trống");
        RuleFor(x => x.Price).GreaterThan(0).WithMessage("Price phải lớn hơn 0");
        RuleFor(x => x.DetailDesc).NotEmpty().WithMessage("DetailDesc không được để trống");
        RuleFor(x => x.ShortDesc).NotEmpty().WithMessage("ShortDesc không được để trống");
        RuleFor(x => x.Quantity).GreaterThanOrEqualTo(1).WithMessage("Số lượng cần lớn hơn hoặc bằng 1");
    }
}
