using System;
using System.Collections.Generic;
using System.Linq;
using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Exceptions;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale
{
    // Command DTOs and Validator result adapter
    public class UpdateSaleCommand : IRequest<UpdateSaleResult>
    {
        public Guid Id { get; set; }
        public DateTimeOffset? Date { get; set; }
        public IList<UpdateSaleItemDto> Items { get; set; } = new List<UpdateSaleItemDto>();

        public ValidationResultDetail Validate()
        {
            var validator = new UpdateSaleCommandValidator();
            var result = validator.Validate(this);
            return new ValidationResultDetail
            {
                IsValid = result.IsValid,
                Errors = result.Errors.Select(o => (ValidationErrorDetail)o)
            };
        }
    }

    public class UpdateSaleItemDto
    {
        public string ProductId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
    }

    public class UpdateSaleResult
    {
        public Guid Id { get; set; }
    }

    // Validator
    public class UpdateSaleCommandValidator : AbstractValidator<UpdateSaleCommand>
    {
        public UpdateSaleCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Items).NotNull();
            RuleForEach(x => x.Items).ChildRules(items =>
            {
                items.RuleFor(i => i.ProductId).NotEmpty();
                items.RuleFor(i => i.ProductName).NotEmpty();
                items.RuleFor(i => i.UnitPrice).GreaterThan(0);
                items.RuleFor(i => i.Quantity).GreaterThan(0).LessThanOrEqualTo(20);
            });
        }
    }

    // Handler
    public class UpdateSaleHandler : IRequestHandler<UpdateSaleCommand, UpdateSaleResult>
    {
        private readonly ISaleRepository _saleRepository;

        public UpdateSaleHandler(ISaleRepository saleRepository)
        {
            _saleRepository = saleRepository;
        }

        public async Task<UpdateSaleResult> Handle(UpdateSaleCommand command, CancellationToken cancellationToken)
        {
            var validator = new UpdateSaleCommandValidator();
            var validationResult = await validator.ValidateAsync(command, cancellationToken);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var sale = await _saleRepository.GetByIdAsync(command.Id, cancellationToken);
            if (sale == null) throw new DomainException("Sale not found.");

            var items = command.Items.Select(i => new SaleItem(i.ProductId, i.ProductName, i.UnitPrice, i.Quantity)).ToList();
            sale.ReplaceItems(items);

            if (command.Date.HasValue) sale.Date = command.Date.Value;

            var updated = await _saleRepository.UpdateAsync(sale, cancellationToken);
            return new UpdateSaleResult { Id = updated.Id };
        }
    }
}
