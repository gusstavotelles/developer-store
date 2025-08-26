using System;

namespace Ambev.DeveloperEvaluation.Common.Events
{
    public class SaleCreatedEvent
    {
        public Guid Id { get; set; }
        public string SaleNumber { get; set; } = string.Empty;
        public DateTimeOffset Date { get; set; }
        public decimal Total { get; set; }
    }
}
