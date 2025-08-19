using E_LaptopShop.Application.Common;
using E_LaptopShop.Application.Common.Pagination;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Application.Features.InventoryHistory.Queries.GetInventoryHistory;
using E_LaptopShop.Application.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_LaptopShop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Require authentication
    [Tags("📊 Inventory History")]
    public class InventoryHistoryController : ControllerBase
    {
        private readonly IMediator _mediator;

        public InventoryHistoryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get inventory history with comprehensive filtering, searching, sorting, and pagination
        /// </summary>
        /// <param name="request">Filter, search, sort, and pagination parameters</param>
        /// <returns>Paginated inventory history records</returns>
        [HttpGet]
        [Authorize(Roles = "Admin,Manager,Warehouse")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<InventoryHistoryDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ApiResponse<PagedResult<InventoryHistoryDto>>>> GetInventoryHistory([FromQuery] GetInventoryHistoryQuery request)
        {
            var result = await _mediator.Send(request);
            
            return Ok(ApiResponse<PagedResult<InventoryHistoryDto>>.SuccessResponse(
                result, 
                $"Retrieved {result.Items.Count()} inventory history records out of {result.TotalCount} total"
            ));
        }

        /// <summary>
        /// Get inventory history for a specific product
        /// </summary>
        /// <param name="productId">Product ID</param>
        /// <param name="fromDate">Start date (optional)</param>
        /// <param name="toDate">End date (optional)</param>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10, max: 50)</param>
        /// <returns>Paginated inventory history for the product</returns>
        [HttpGet("product/{productId:int}")]
        [Authorize(Roles = "Admin,Manager,Warehouse")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<InventoryHistoryDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<PagedResult<InventoryHistoryDto>>>> GetProductHistory(
            int productId,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = new GetInventoryHistoryQuery
            {
                ProductId = productId,
                FromDate = fromDate,
                ToDate = toDate,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);
            
            return Ok(ApiResponse<PagedResult<InventoryHistoryDto>>.SuccessResponse(
                result, 
                $"Retrieved history for product {productId}"
            ));
        }

        /// <summary>
        /// Get inventory history by transaction type
        /// </summary>
        /// <param name="transactionType">Transaction type (Purchase, Sale, Return, etc.)</param>
        /// <param name="fromDate">Start date (optional)</param>
        /// <param name="toDate">End date (optional)</param>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10, max: 50)</param>
        /// <returns>Paginated inventory history by transaction type</returns>
        [HttpGet("transaction/{transactionType}")]
        [Authorize(Roles = "Admin,Manager,Warehouse")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<InventoryHistoryDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<PagedResult<InventoryHistoryDto>>>> GetByTransactionType(
            string transactionType,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = new GetInventoryHistoryQuery
            {
                TransactionType = transactionType,
                FromDate = fromDate,
                ToDate = toDate,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);
            
            return Ok(ApiResponse<PagedResult<InventoryHistoryDto>>.SuccessResponse(
                result, 
                $"Retrieved {transactionType} transaction history"
            ));
        }
    }
}
