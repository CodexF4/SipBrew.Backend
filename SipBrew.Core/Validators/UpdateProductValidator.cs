using FluentValidation;
using SipBrew.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SipBrew.Core.Validators
{
    public class UpdateProductValidator : AbstractValidator<ProductsModel>
    {
        private readonly AppDbContext _ctx;
        public UpdateProductValidator(AppDbContext ctx)
        {
            _ctx = ctx;

            RuleFor(x => x.ProductName)
                .NotEmpty().WithMessage("Product name is required.")
                .MaximumLength(100).WithMessage("Product name cannot exceed 100 characters.")
                .Must((model, name) => !_ctx.Products.Any(p => p.ProductName == name && p.Id != model.Id))
                .WithMessage("Product name must be unique.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than zero.");
        }
    }
}
