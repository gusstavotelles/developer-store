using System;
using System.Collections.Generic;
using System.Linq;
using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Exceptions;

namespace Ambev.DeveloperEvaluation.Domain.Entities
{
    /// <summary>
    /// Represents a sale with items, customer/branch reference and totals.
    /// Business rules (quantity-based discounts and limits) are applied when items are added or updated.
    /// </summary>
    public class Sale : BaseEntity
    {
        public string SaleNumber { get; set; } = string.Empty;
        public DateTimeOffset Date { get; set; }
        public string CustomerId { get; set; } = string.Empty; // external identity (denormalized in items if needed)
        public string BranchId { get; set; } = string.Empty; // external identity
        public bool IsCancelled { get; private set; }
        public decimal Total { get; private set; }

        private readonly List<SaleItem> _items = new();
        public IReadOnlyCollection<SaleItem> Items => _items.AsReadOnly();

        public Sale()
        {
            Date = DateTimeOffset.UtcNow;
        }

        public Sale(string saleNumber, DateTimeOffset date, string customerId, string branchId)
        {
            SaleNumber = saleNumber;
            Date = date;
            CustomerId = customerId;
            BranchId = branchId;
        }

        /// <summary>
        /// Adds an item to the sale and recalculates totals.
        /// Throws DomainException if business rules are violated (e.g. quantity > 20).
        /// </summary>
        public void AddItem(SaleItem item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            if (item.Quantity > 20) throw new DomainException("Maximum quantity per item is 20.");
            item.ApplyDiscountAndCalculateTotal();
            _items.Add(item);
            RecalculateTotal();
        }

        /// <summary>
        /// Removes an item (by id) from the sale and recalculates totals.
        /// </summary>
        public void RemoveItem(Guid itemId)
        {
            var item = _items.FirstOrDefault(i => i.Id == itemId);
            if (item == null) throw new DomainException("Item não encontrado na venda.");
            _items.Remove(item);
            RecalculateTotal();
        }

        /// <summary>
        /// Marks an item as cancelled and recalculates totals.
        /// </summary>
        public void CancelItem(Guid itemId)
        {
            var item = _items.FirstOrDefault(i => i.Id == itemId);
            if (item == null) throw new DomainException("Item não encontrado na venda.");
            item.Cancel();
            RecalculateTotal();
        }

        /// <summary>
        /// Cancels the entire sale.
        /// </summary>
        public void CancelSale()
        {
            IsCancelled = true;
        }

        /// <summary>
        /// Recalculates totals summing only non-cancelled items.
        /// </summary>
        public void RecalculateTotal()
        {
            Total = _items.Where(i => !i.IsCancelled).Sum(i => i.Total);
        }

        /// <summary>
        /// Replace items collection (used for update scenarios). Validates business rules.
        /// </summary>
        public void ReplaceItems(IEnumerable<SaleItem> items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));

            var itemList = items.ToList();
            if (itemList.Any(i => i.Quantity > 20)) throw new DomainException("Quantidade máxima por item é 20.");

            _items.Clear();
            foreach (var it in itemList)
            {
                it.ApplyDiscountAndCalculateTotal();
                _items.Add(it);
            }

            RecalculateTotal();
        }
    }
}
