using System;
using System.Collections.Generic;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.Requests
{
    public class CreateSaleItemRequest
    {
        public string ProductId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
    }

    public class CreateSaleRequest
    {
        public string? SaleNumber { get; set; }
        public DateTimeOffset? Date { get; set; }
        public string CustomerId { get; set; } = string.Empty;
        public string BranchId { get; set; } = string.Empty;
        public IList<CreateSaleItemRequest> Items { get; set; } = new List<CreateSaleItemRequest>();
    }
}
