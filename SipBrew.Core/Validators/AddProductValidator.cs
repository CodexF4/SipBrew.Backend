using FluentValidation;
using SipBrew.Core.DTO;
using SipBrew.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SipBrew.Core.Validators
{
    public class AddProductValidator : AbstractValidator<AddProductDTO>
    {
        private readonly AppDbContext _ctx;
        public AddProductValidator( AppDbContext ctx)
        {
            _ctx = ctx;

            RuleFor(x => x.ProductName)
                .NotEmpty().WithMessage("Product name is required.")
                .MaximumLength(100).WithMessage("Product name cannot exceed 100 characters.")
                .Must(name => !_ctx.Products.Any(p => p.ProductName == name))
                .WithMessage("Product name must be unique.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than zero.");
        }
    }
}
