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
    public class SalesIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public SalesIntegrationTests(CustomWebApplicationFactory factory)
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

            if (createResponse.StatusCode != HttpStatusCode.Created)
            {
                var content = await createResponse.Content.ReadAsStringAsync();
                // Fail the test with the response content to get the server error details.
                Assert.True(false, $"Create failed: {(int)createResponse.StatusCode} {createResponse.StatusCode}. Response body: {content}");
            }

            var createdJson = await createResponse.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
            var id = createdJson.GetProperty("id").GetString();
            Assert.False(string.IsNullOrEmpty(id));

            var getResponse = await _client.GetAsync($"/api/sales/{id}");
            if (getResponse.StatusCode != HttpStatusCode.OK)
            {
                var content = await getResponse.Content.ReadAsStringAsync();
                Assert.True(false, $"Get failed: {(int)getResponse.StatusCode} {getResponse.StatusCode}. Response body: {content}");
            }

            var sale = await getResponse.Content.ReadFromJsonAsync<SaleResponse>();
            Assert.Equal("T001", sale.SaleNumber);
            Assert.Equal(36.00m, sale.Total);
        }
    }
}
