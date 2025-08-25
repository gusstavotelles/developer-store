using System;
using System.Linq;
using System.Collections.Generic;
using MediatR;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Exceptions;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale
{
    public class GetSaleQuery : IRequest<GetSaleResult>
    {
        public Guid Id { get; set; }
    }

    public class GetSaleItemDto
    {
        public Guid Id { get; set; }
        public string ProductId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal Total { get; set; }
        public bool IsCancelled { get; set; }
    }

    public class GetSaleResult
    {
        public Guid Id { get; set; }
        public string SaleNumber { get; set; } = string.Empty;
        public DateTimeOffset Date { get; set; }
        public string CustomerId { get; set; } = string.Empty;
        public string BranchId { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public IList<GetSaleItemDto> Items { get; set; } = new List<GetSaleItemDto>();
    }

    public class GetSaleHandler : IRequestHandler<GetSaleQuery, GetSaleResult>
    {
        private readonly ISaleRepository _saleRepository;

        public GetSaleHandler(ISaleRepository saleRepository)
        {
            _saleRepository = saleRepository;
        }

        public async Task<GetSaleResult> Handle(GetSaleQuery request, CancellationToken cancellationToken)
        {
            var sale = await _saleRepository.GetByIdAsync(request.Id, cancellationToken);
            if (sale == null) throw new DomainException("Sale not found.");

            return new GetSaleResult
            {
                Id = sale.Id,
                SaleNumber = sale.SaleNumber,
                Date = sale.Date,
                CustomerId = sale.CustomerId,
                BranchId = sale.BranchId,
                Total = sale.Total,
                Items = sale.Items.Select(i => new GetSaleItemDto
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    UnitPrice = i.UnitPrice,
                    Quantity = i.Quantity,
                    DiscountPercent = i.DiscountPercent,
                    Total = i.Total,
                    IsCancelled = i.IsCancelled
                }).ToList()
            };
        }
    }
}
