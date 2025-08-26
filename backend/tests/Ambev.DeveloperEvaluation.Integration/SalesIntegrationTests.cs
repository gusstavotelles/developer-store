using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using Ambev.DeveloperEvaluation.WebApi;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.Requests;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.Responses;

namespace Ambev.DeveloperEvaluation.Integration
{
    public class SalesIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public SalesIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task CreateAndGetSale_EndToEnd()
        {
            var createRequest = new CreateSaleRequest
            {
                SaleNumber = "T001",
                CustomerId = "C1",
                BranchId = "B1",
                Items = new []
                {
                    new CreateSaleItemRequest { ProductId = "P1", ProductName = "Product 1", UnitPrice = 10m, Quantity = 4 }
                }
            };

            var createResponse = await _client.PostAsJsonAsync("/api/sales", createRequest);
            Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

            var created = await createResponse.Content.ReadFromJsonAsync<dynamic>();
            Assert.NotNull(created.id.ToString());

            string id = created.id.ToString();

            var getResponse = await _client.GetAsync($"/api/sales/{id}");
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            var sale = await getResponse.Content.ReadFromJsonAsync<SaleResponse>();
            Assert.Equal("T001", sale.SaleNumber);
            Assert.Equal(36.00m, sale.Total);
        }
    }
}
