using System;
using System.Linq;
using System.Collections.Generic;
using MediatR;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.Application.Sales.ListSales
{
    public class ListSalesQuery : IRequest<ListSalesResult>
    {
        public int Page { get; set; } = 1;
        public int Size { get; set; } = 10;
    }

    public class ListSalesItemDto
    {
        public Guid Id { get; set; }
        public string ProductId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Total { get; set; }
    }

    public class ListSalesResultItem
    {
        public Guid Id { get; set; }
        public string SaleNumber { get; set; } = string.Empty;
        public DateTimeOffset Date { get; set; }
        public string CustomerId { get; set; } = string.Empty;
        public string BranchId { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public IList<ListSalesItemDto> Items { get; set; } = new List<ListSalesItemDto>();
    }

    public class ListSalesResult
    {
        public IList<ListSalesResultItem> Data { get; set; } = new List<ListSalesResultItem>();
        public int TotalItems { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }

    public class ListSalesHandler : IRequestHandler<ListSalesQuery, ListSalesResult>
    {
        private readonly ISaleRepository _saleRepository;

        public ListSalesHandler(ISaleRepository saleRepository)
        {
            _saleRepository = saleRepository;
        }

        public async Task<ListSalesResult> Handle(ListSalesQuery request, CancellationToken cancellationToken)
        {
            var all = (await _saleRepository.ListAsync(cancellationToken)).OrderBy(s => s.Date).ToList();
            var totalItems = all.Count;
            var size = Math.Max(1, request.Size);
            var page = Math.Max(1, request.Page);
            var totalPages = (int)Math.Ceiling((double)totalItems / size);

            var pageItems = all.Skip((page - 1) * size).Take(size).ToList();

            var data = pageItems.Select(s => new ListSalesResultItem
            {
                Id = s.Id,
                SaleNumber = s.SaleNumber,
                Date = s.Date,
                CustomerId = s.CustomerId,
                BranchId = s.BranchId,
                Total = s.Total,
                Items = s.Items.Select(i => new ListSalesItemDto
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    UnitPrice = i.UnitPrice,
                    Quantity = i.Quantity,
                    Total = i.Total
                }).ToList()
            }).ToList();

            return new ListSalesResult
            {
                Data = data,
                TotalItems = totalItems,
                CurrentPage = page,
                TotalPages = totalPages
            };
        }
    }
}
