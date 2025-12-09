using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Shop.APIProducts.Data;
using Shop.APIProducts.Dto.Products;

namespace Shop.APIProducts.Validators
{
    public class UpdateProductRequestValidator : AbstractValidator<UpdateProductRequest>
    {
        private readonly ProductDbContext _context;

        public UpdateProductRequestValidator(ProductDbContext context)
        {
            _context = context;

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre del producto es requerido.")
                .MinimumLength(3).WithMessage("El nombre debe tener al menos 3 caracteres.")
                .MaximumLength(200).WithMessage("El nombre no puede exceder 200 caracteres.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("La descripción del producto es requerida.")
                .MinimumLength(10).WithMessage("La descripción debe tener al menos 10 caracteres.")
                .MaximumLength(1000).WithMessage("La descripción no puede exceder 1000 caracteres.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("El precio debe ser mayor a 0.")
                .LessThanOrEqualTo(999999.99m).WithMessage("El precio no puede exceder 999,999.99.");

            RuleFor(x => x.Stock)
                .GreaterThanOrEqualTo(0).WithMessage("El stock no puede ser negativo.");

            RuleFor(x => x.CategoriesId)
                .MustAsync(CategoryExists).WithMessage("La categoría especificada no existe.")
                .When(x => x.CategoriesId.HasValue);

            RuleFor(x => x.ImageUrl)
                .MaximumLength(500).WithMessage("La URL de la imagen no puede exceder 500 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.ImageUrl));
        }

        private async Task<bool> CategoryExists(int? categoryId, CancellationToken cancellationToken)
        {
            if (!categoryId.HasValue)
                return true;

            return await _context.Categories
                .AnyAsync(c => c.Id == categoryId.Value, cancellationToken);
        }
    }
}
