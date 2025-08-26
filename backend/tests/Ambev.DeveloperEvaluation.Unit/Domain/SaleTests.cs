using System;
using Xunit;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Exceptions;

namespace Ambev.DeveloperEvaluation.Unit.Domain
{
    public class SaleTests
    {
        [Fact]
        public void AddItem_WithQuantityBelow4_NoDiscount()
        {
            var sale = new Sale("S001", DateTimeOffset.UtcNow, "C1", "B1");
            var item = new SaleItem("P1", "Product 1", 10m, 1);

            sale.AddItem(item);

            Assert.Equal(10m, item.Total);
            Assert.Equal(10m, sale.Total);
        }

        [Fact]
        public void AddItem_WithQuantity4_Apply10PercentDiscount()
        {
            var sale = new Sale("S002", DateTimeOffset.UtcNow, "C1", "B1");
            var item = new SaleItem("P1", "Product 1", 10m, 4);

            sale.AddItem(item);

            // 4 * 10 = 40, 10% discount => 36.00
            Assert.Equal(36.00m, item.Total);
            Assert.Equal(36.00m, sale.Total);
        }

        [Fact]
        public void AddItem_WithQuantity10_Apply20PercentDiscount()
        {
            var sale = new Sale("S003", DateTimeOffset.UtcNow, "C1", "B1");
            var item = new SaleItem("P1", "Product 1", 5m, 10);

            sale.AddItem(item);

            // 10 * 5 = 50, 20% discount => 40.00
            Assert.Equal(40.00m, item.Total);
            Assert.Equal(40.00m, sale.Total);
        }

        [Fact]
        public void AddItem_WithQuantityAbove20_ThrowsDomainException()
        {
            // The constructor validates and will throw when quantity > 20.
            Assert.Throws<DomainException>(() => new SaleItem("P1", "Product 1", 5m, 21));
        }

        [Fact]
        public void ReplaceItems_RecalculatesTotalCorrectly()
        {
            var sale = new Sale("S005", DateTimeOffset.UtcNow, "C1", "B1");
            var item1 = new SaleItem("P1", "Product 1", 10m, 2); // total 20
            var item2 = new SaleItem("P2", "Product 2", 10m, 4); // total 36 (10% discount)

            sale.ReplaceItems(new[] { item1, item2 });

            Assert.Equal(56.00m, sale.Total);
        }

        [Fact]
        public void CancelItem_SetsItemCancelledAndUpdatesTotal()
        {
            var sale = new Sale("S006", DateTimeOffset.UtcNow, "C1", "B1");
            var item1 = new SaleItem("P1", "Product 1", 10m, 1);
            var item2 = new SaleItem("P2", "Product 2", 10m, 1);
            sale.AddItem(item1);
            sale.AddItem(item2);

            sale.CancelItem(item1.Id);

            Assert.True(item1.IsCancelled);
            Assert.Equal(item2.Total, sale.Total);
        }
    }
}
