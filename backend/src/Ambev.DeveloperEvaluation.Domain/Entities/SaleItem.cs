using System;
using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Exceptions;

namespace Ambev.DeveloperEvaluation.Domain.Entities
{
    /// <summary>
    /// Represents an item inside a Sale.
    /// Encapsulates discount calculation rules and per-item total calculation.
    /// </summary>
    public class SaleItem : BaseEntity
    {
        public Guid SaleId { get; set; } // FK-ish reference (set by repository/mapping)
        public string ProductId { get; set; } = string.Empty; // external identity
        public string ProductName { get; set; } = string.Empty; // denormalized product name
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal DiscountPercent { get; private set; }
        public decimal Total { get; private set; }
        public bool IsCancelled { get; private set; }

        public SaleItem()
        {
        }

        public SaleItem(string productId, string productName, decimal unitPrice, int quantity)
        {
            ProductId = productId;
            ProductName = productName;
            UnitPrice = unitPrice;
            Quantity = quantity;
            ApplyDiscountAndCalculateTotal();
        }

        /// <summary>
        /// Applies business rules for quantity-based discounts and calculates the item total.
        /// Business rules:
        ///  - quantity < 4 => 0% discount
        ///  - 4 <= quantity < 10 => 10% discount
        ///  - 10 <= quantity <= 20 => 20% discount
        ///  - quantity > 20 => throw DomainException
        /// </summary>
        public void ApplyDiscountAndCalculateTotal()
        {
            if (Quantity <= 0) throw new DomainException("Quantidade deve ser maior que zero.");
            if (Quantity > 20) throw new DomainException("Quantidade máxima por item é 20.");

            if (Quantity >= 10)
            {
                DiscountPercent = 0.20m;
            }
            else if (Quantity >= 4)
            {
                DiscountPercent = 0.10m;
            }
            else
            {
                DiscountPercent = 0m;
            }

            var gross = UnitPrice * Quantity;
            Total = Math.Round(gross * (1 - DiscountPercent), 2, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// Marks the item as cancelled and sets its total to zero.
        /// </summary>
        public void Cancel()
        {
            IsCancelled = true;
            Total = 0m;
        }
    }
}
