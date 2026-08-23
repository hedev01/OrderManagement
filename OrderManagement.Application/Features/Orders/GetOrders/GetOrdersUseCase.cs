using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderManagement.Application.Common;
using OrderManagement.Application.Interfaces.Persistence;
using Microsoft.Extensions.Logging;

namespace OrderManagement.Application.Features.Orders.GetOrders
{

    public sealed class GetOrdersUseCase : IGetOrdersUseCase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<GetOrdersUseCase> _logger;

        public GetOrdersUseCase(
            IOrderRepository orderRepository, ILogger<GetOrdersUseCase> logger)
        {
            _orderRepository = orderRepository;
            _logger = logger;
        }

        public async Task<
    Result<PagedResult<GetOrdersResponse>>>
    ExecuteAsync(
        GetOrdersRequest request,
        CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Getting orders. CustomerId: {CustomerId}, Status: {Status}, " +
                "FromDate: {FromDate}, ToDate: {ToDate}, Page: {Page}, PageSize: {PageSize}",
                request.CustomerId,
                request.Status,
                request.FromDate,
                request.ToDate,
                request.Page,
                request.PageSize);




            if (request.Page <= 0)
            {
                _logger.LogWarning(
                    "Get orders failed. Invalid page number: {Page}",
                    request.Page);

                return Result<
                    PagedResult<GetOrdersResponse>>
                    .Failure(
                        "صفحه باید بزرگتر از صفر باشد.");
            }


            if (request.PageSize <= 0 ||
                request.PageSize > 100)
            {
                _logger.LogWarning(
                    "Get orders failed. Invalid page size: {PageSize}",
                    request.PageSize);

                return Result<
                    PagedResult<GetOrdersResponse>>
                    .Failure(
                        "اندازه صفحه باید بین ۱ تا ۱۰۰ باشد.");
            }


            if (request.FromDate.HasValue &&
                request.ToDate.HasValue &&
                request.FromDate > request.ToDate)
            {
                _logger.LogWarning(
                    "Get orders failed. FromDate {FromDate} is greater than ToDate {ToDate}",
                    request.FromDate,
                    request.ToDate);

                return Result<
                    PagedResult<GetOrdersResponse>>
                    .Failure(
                        "مقدار From Date نمی‌تواند بزرگتر از ToDate باشد.");
            }



            var result =
                await _orderRepository.SearchAsync(
                    request.CustomerId,
                    request.Status,
                    request.FromDate,
                    request.ToDate,
                    request.Page,
                    request.PageSize,
                    cancellationToken);



            var response =
                result.Items
                    .Select(order =>
                        new GetOrdersResponse(
                            order.Id,
                            order.CustomerId,
                            order.Status,
                            order.TotalPrice,
                            order.CreatedAt))
                    .ToList();




            var pagedResult =
                new PagedResult<GetOrdersResponse>(
                    response,
                    request.Page,
                    request.PageSize,
                    result.TotalCount);




            _logger.LogInformation(
                "Orders retrieved successfully. " +
                "Page: {Page}, PageSize: {PageSize}, " +
                "ReturnedCount: {ReturnedCount}, TotalCount: {TotalCount}",
                request.Page,
                request.PageSize,
                response.Count,
                result.TotalCount);


            return Result<
                PagedResult<GetOrdersResponse>>
                .Success(pagedResult);
        }
    }
}
