using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Shop.APIProducts.Data;
using Shop.APIProducts.Dto.Categories;

namespace Shop.APIProducts.Validators
{
    public class CreateCategoryRequestValidator : AbstractValidator<CreateCategoryRequest>
    {
        private readonly ProductDbContext _context;

        public CreateCategoryRequestValidator(ProductDbContext context)
        {
            _context = context;

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre de la categoría es requerido.")
                .MinimumLength(3).WithMessage("El nombre debe tener al menos 3 caracteres.")
                .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe una categoría con este nombre.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("La descripción no puede exceder 500 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.Description));
        }

        private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
        {
            return !await _context.Categories
                .AnyAsync(c => c.Name.ToLower() == name.ToLower(), cancellationToken);
        }
    }
}