using LaptopShop.BLL.DTOs;

namespace LaptopShop.BLL.Services;

public interface IOrderService
{
    Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
    Task<OrderDto> GetOrderByIdAsync(long id);
    Task<OrderDto> CreateOrderAsync(OrderDto orderDto);
    Task UpdateOrderStatusAsync(long id, string status);
    Task<IEnumerable<OrderDto>> GetOrdersByUserIdAsync(long userId);

    Task RequestReturnAsync(long orderId);
    Task ApproveReturnAsync(long orderId);
}
