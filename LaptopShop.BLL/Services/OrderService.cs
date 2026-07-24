using LaptopShop.BLL.DTOs;
using LaptopShop.DAL.Entities;
using LaptopShop.DAL.Repositories;

namespace LaptopShop.BLL.Services;

public class OrderService : IOrderService
{
    private readonly IRepository<Order> _orderRepository;
    private readonly IRepository<Product> _productRepository;

    public OrderService(IRepository<Order> orderRepository, IRepository<Product> productRepository)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
    }

    public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
    {
        var orders = await _orderRepository.GetAllAsync(includeProperties: "User,OrderDetails,OrderDetails.Product");
        return orders.Select(o => new OrderDto
        {
            Id = o.Id,
            TotalPrice = o.TotalPrice,
            ReceiverName = o.ReceiverName,
            ReceiverAddress = o.ReceiverAddress,
            ReceiverPhone = o.ReceiverPhone,
            Status = o.Status,
            UserId = o.UserId,
            UserName = o.User?.FullName,
            OrderDetails = o.OrderDetails.Select(od => new OrderDetailDto
            {
                Id = od.Id,
                Quantity = od.Quantity,
                Price = od.Price,
                ProductId = od.ProductId,
                ProductName = od.Product?.Name,
                ProductImage = od.Product?.Image
            }).ToList()
        });
    }

    public async Task<OrderDto> GetOrderByIdAsync(long id)
    {
        var orders = await _orderRepository.GetAllAsync(o => o.Id == id, includeProperties: "User,OrderDetails,OrderDetails.Product");
        var o = orders.FirstOrDefault();
        if (o == null) return null;

        return new OrderDto
        {
            Id = o.Id,
            TotalPrice = o.TotalPrice,
            ReceiverName = o.ReceiverName,
            ReceiverAddress = o.ReceiverAddress,
            ReceiverPhone = o.ReceiverPhone,
            Status = o.Status,
            UserId = o.UserId,
            UserName = o.User?.FullName,
            OrderDetails = o.OrderDetails.Select(od => new OrderDetailDto
            {
                Id = od.Id,
                Quantity = od.Quantity,
                Price = od.Price,
                ProductId = od.ProductId,
                ProductName = od.Product?.Name,
                ProductImage = od.Product?.Image
            }).ToList()
        };
    }

    public async Task<IEnumerable<OrderDto>> GetOrdersByUserIdAsync(long userId)
    {
        var orders = await _orderRepository.GetAllAsync(o => o.UserId == userId, includeProperties: "User,OrderDetails,OrderDetails.Product");
        return orders.Select(o => new OrderDto
        {
            Id = o.Id,
            TotalPrice = o.TotalPrice,
            ReceiverName = o.ReceiverName,
            ReceiverAddress = o.ReceiverAddress,
            ReceiverPhone = o.ReceiverPhone,
            Status = o.Status,
            UserId = o.UserId,
            UserName = o.User?.FullName,
            OrderDetails = o.OrderDetails.Select(od => new OrderDetailDto
            {
                Id = od.Id,
                Quantity = od.Quantity,
                Price = od.Price,
                ProductId = od.ProductId,
                ProductName = od.Product?.Name,
                ProductImage = od.Product?.Image
            }).ToList()
        });
    }

    public async Task<OrderDto> CreateOrderAsync(OrderDto orderDto)
    {
        var order = new Order
        {
            TotalPrice = orderDto.TotalPrice,
            ReceiverName = orderDto.ReceiverName,
            ReceiverAddress = orderDto.ReceiverAddress,
            ReceiverPhone = orderDto.ReceiverPhone,
            Status = "Pending",
            UserId = orderDto.UserId,
            OrderDetails = orderDto.OrderDetails.Select(od => new OrderDetail
            {
                Quantity = od.Quantity,
                Price = od.Price,
                ProductId = od.ProductId
            }).ToList()
        };

        await _orderRepository.InsertAsync(order);

        await _orderRepository.SaveAsync();

        orderDto.Id = order.Id;
        return orderDto;
    }

    public async Task UpdateOrderStatusAsync(long id, string status)
    {
        var orders = await _orderRepository.GetAllAsync(o => o.Id == id, includeProperties: "OrderDetails");
        var order = orders.FirstOrDefault();
        if (order != null)
        {
            if (status == "Shipping" && order.Status == "Pending")
            {
                foreach (var detail in order.OrderDetails)
                {
                    var product = await _productRepository.GetByIdAsync(detail.ProductId);
                    if (product != null)
                    {
                        product.Quantity -= detail.Quantity;
                        if (product.Quantity < 0) product.Quantity = 0;
                        product.Sold += detail.Quantity;
                        _productRepository.Update(product);
                    }
                }
            }

            if (status == "Cancelled" && (order.Status == "Shipping" || order.Status == "Delivered" || order.Status == "ReturnRequested"))
            {
                foreach (var detail in order.OrderDetails)
                {
                    var product = await _productRepository.GetByIdAsync(detail.ProductId);
                    if (product != null)
                    {
                        product.Quantity += detail.Quantity;
                        product.Sold -= detail.Quantity;
                        if (product.Sold < 0) product.Sold = 0;
                        _productRepository.Update(product);
                    }
                }
            }

            order.Status = status;
            _orderRepository.Update(order);
            await _orderRepository.SaveAsync();
        }
    }

    public async Task RequestReturnAsync(long orderId)
    {
        await UpdateOrderStatusAsync(orderId, "ReturnRequested");
    }

    public async Task ApproveReturnAsync(long orderId)
    {
        var orders = await _orderRepository.GetAllAsync(o => o.Id == orderId, includeProperties: "OrderDetails");
        var order = orders.FirstOrDefault();

        if (order != null && order.Status == "ReturnRequested")
        {
            foreach (var detail in order.OrderDetails)
            {
                var product = await _productRepository.GetByIdAsync(detail.ProductId);
                if (product != null)
                {
                    product.Quantity += detail.Quantity;
                    product.Sold -= detail.Quantity;
                    if (product.Sold < 0) product.Sold = 0;
                    _productRepository.Update(product);
                }
            }

            order.Status = "Returned";
            _orderRepository.Update(order);

            await _orderRepository.SaveAsync();
        }
    }
}
