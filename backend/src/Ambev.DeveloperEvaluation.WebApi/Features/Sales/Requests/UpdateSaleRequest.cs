using System;
using System.Collections.Generic;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.Requests
{
    public class UpdateSaleItemRequest
    {
        public string ProductId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
    }

    public class UpdateSaleRequest
    {
        public DateTimeOffset? Date { get; set; }
        public IList<UpdateSaleItemRequest> Items { get; set; } = new List<UpdateSaleItemRequest>();
    }
}
