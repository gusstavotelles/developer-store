using Microsoft.AspNetCore.Mvc;
using MediatR;
using AutoMapper;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.Requests;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.Responses;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Application.Sales.ListSales;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public SalesController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSaleRequest request, CancellationToken cancellationToken)
        {
            var command = _mapper.Map<CreateSaleCommand>(request);
            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(Get), new { id = result.Id }, new { id = result.Id });
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSaleRequest request, CancellationToken cancellationToken)
        {
            var command = new UpdateSaleCommand
            {
                Id = id,
                Date = request.Date,
                Items = request.Items.Select(i => new UpdateSaleItemDto
                {
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    UnitPrice = i.UnitPrice,
                    Quantity = i.Quantity
                }).ToList()
            };

            var result = await _mediator.Send(command, cancellationToken);
            return Ok(new { id = result.Id });
        }

        [HttpPost("{id:guid}/cancel")]
        public async Task<IActionResult> Cancel(Guid id, CancellationToken cancellationToken)
        {
            var command = new CancelSaleCommand { Id = id };
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(new { id = result.Id });
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
        {
            var query = new GetSaleQuery { Id = id };
            var result = await _mediator.Send(query, cancellationToken);
            var response = _mapper.Map<SaleResponse>(result);
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> List([FromQuery] int page = 1, [FromQuery] int size = 10, CancellationToken cancellationToken = default)
        {
            var query = new ListSalesQuery { Page = page, Size = size };
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }
    }
}
