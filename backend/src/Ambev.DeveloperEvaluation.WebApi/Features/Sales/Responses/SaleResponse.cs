using System;
using System.Collections.Generic;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.Responses
{
    public class SaleItemResponse
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

    public class SaleResponse
    {
        public Guid Id { get; set; }
        public string SaleNumber { get; set; } = string.Empty;
        public DateTimeOffset Date { get; set; }
        public string CustomerId { get; set; } = string.Empty;
        public string BranchId { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public IList<SaleItemResponse> Items { get; set; } = new List<SaleItemResponse>();
    }
}
