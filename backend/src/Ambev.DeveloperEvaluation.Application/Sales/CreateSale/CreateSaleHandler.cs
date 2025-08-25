using AutoMapper;
using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

/// <summary>
/// Handler for processing CreateSaleCommand requests
/// </summary>
public class CreateSaleHandler : IRequestHandler<CreateSaleCommand, CreateSaleResult>
{
    private readonly ISaleRepository _saleRepository;

    public CreateSaleHandler(ISaleRepository saleRepository)
    {
        _saleRepository = saleRepository;
    }

    public async Task<CreateSaleResult> Handle(CreateSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new CreateSaleCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var saleNumber = string.IsNullOrWhiteSpace(command.SaleNumber) ? Guid.NewGuid().ToString("N") : command.SaleNumber;
        var sale = new Sale(saleNumber, command.Date, command.CustomerId, command.BranchId);

        foreach (var itemDto in command.Items)
        {
            var item = new SaleItem(itemDto.ProductId, itemDto.ProductName, itemDto.UnitPrice, itemDto.Quantity);
            sale.AddItem(item);
        }

        // Ensure totals are calculated
        sale.RecalculateTotal();

        var created = await _saleRepository.CreateAsync(sale, cancellationToken);

        return new CreateSaleResult { Id = created.Id };
    }
}
