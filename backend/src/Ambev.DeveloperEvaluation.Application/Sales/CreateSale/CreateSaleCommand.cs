using System;
using System.Collections.Generic;
using Ambev.DeveloperEvaluation.Common.Validation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale
{
    /// <summary>
    /// Command to create a new Sale.
    /// </summary>
    public class CreateSaleCommand : IRequest<CreateSaleResult>
    {
        public string? SaleNumber { get; set; }
        public DateTimeOffset Date { get; set; } = DateTimeOffset.UtcNow;
        public string CustomerId { get; set; } = string.Empty;
        public string BranchId { get; set; } = string.Empty;

        public IList<CreateSaleItemDto> Items { get; set; } = new List<CreateSaleItemDto>();

        public ValidationResultDetail Validate()
        {
            var validator = new CreateSaleCommandValidator();
            var result = validator.Validate(this);
            return new ValidationResultDetail
            {
                IsValid = result.IsValid,
                Errors = result.Errors.Select(o => (ValidationErrorDetail)o)
            };
        }
    }

    public class CreateSaleItemDto
    {
        public string ProductId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
    }
}
