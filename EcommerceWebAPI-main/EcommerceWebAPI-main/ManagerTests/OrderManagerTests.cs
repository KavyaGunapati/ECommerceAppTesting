using AutoMapper;
using Bogus;
using EcommerceWebAPI.Interfaces.IRepository;
using EcommerceWebAPI.Managers;
using EcommerceWebAPI.Models.DTOs;
using ECommerceWebAPI.DataAccess.Entities;
using Moq;

namespace ManagerTests
{

    [TestFixture]
    public class OrderManagerTests
    {
        //Mocks
        private Mock<IGenericRepository<Order>> _orderRepoMock;
        private Mock<IGenericRepository<OrderItem>> _orderItemRepoMock;
        private Mock<IGenericRepository<Product>> _productRepoMock;
        private Mock<IMapper> _mapperMock;
        // System under test
        private OrderManager _orderManager;
        // Simple Faker instance
        private Faker _faker;

        [SetUp]
        public void Setup()
        {
            _orderItemRepoMock = new Mock<IGenericRepository<OrderItem>>();
            _orderRepoMock = new Mock<IGenericRepository<Order>>();
            _mapperMock = new Mock<IMapper>();
            _faker = new Faker();
            _productRepoMock = new Mock<IGenericRepository<Product>>();
            _orderManager = new OrderManager(_orderRepoMock.Object, _orderItemRepoMock.Object, _productRepoMock.Object, _mapperMock.Object);
        }
        private Product FakeProduct(int? id, int? quantity, decimal? price)
        {
            return new Product
            {
                Id = id ?? _faker.Random.Int(1, 1000),
                Name = _faker.Commerce.ProductName(),
                Description = _faker.Commerce.ProductDescription(),
                StockQuantity = quantity ?? _faker.Random.Int(1, 1000),
                Price = price ?? _faker.Random.Decimal(1, 10000)

            };
        }
        private Order FakeOrder(int? id = null, string? userId = null, decimal? total = null)
        {
            return new Order
            {
                Id = id ?? _faker.Random.Int(0, 1000),
                TotalAmount = total ??0m,
                OrderDate = DateTime.Now,
                UserId=userId??_faker.Random.Guid().ToString(),
            };

        }
        private CreateOrder FakeCreateOrder(params (int , int )[] items)
        {
            var dto = new CreateOrder
            {
                Items = items.Select(i => new OrderItemResponse
                {
                    ProductId = i.Item1,
                    Quantity = i.Item2,
                }).ToList()
            };
            return dto;
        }
        private OrderResponse FakeOrderResponse(Order order)
        {
            return new OrderResponse
            {
                Id = order.Id,

                TotalAmount = order.TotalAmount,
                OrderDate = order.OrderDate,

            };
        }
        [Test]
        public async Task PlaceOrderAsync_Should_Create_Order_And_OrderItems_And_Compute_Total()
        {
            //Arrange
            var userId=_faker.Random.Guid().ToString();
            var productA = FakeProduct(id: 2, price: 10m, quantity: 2);
            var productB= FakeProduct(id: 3, price:20m, quantity: 2);
            var dto=FakeCreateOrder((productA.Id,2),(productB.Id,2));
            _orderRepoMock.Setup(r=>r.AddAsync(It.IsAny<Order>())).Callback<Order>(r => r.Id=999).Returns(Task.CompletedTask);

            _productRepoMock.Setup(r => r.GetByIdAsync(productA.Id)).ReturnsAsync(productA);
            _productRepoMock.Setup(r=>r.GetByIdAsync(productB.Id)).ReturnsAsync(productB);

            var capturedItems=new List<OrderItem>();

            _orderItemRepoMock.Setup(r => r.AddAsync(It.IsAny<OrderItem>()))
                .Callback<OrderItem>(oi => capturedItems.Add(oi)).Returns(Task.CompletedTask);

            _mapperMock.Setup(r=>r.Map<OrderResponse>(It.Is<Order>(o=>o.Id==999))).Returns((Order o)=>FakeOrderResponse(o));

            //Act
            var result = await _orderManager.PlaceOrderAsync(userId, dto);
            //Assert
            Assert.That(result.Success,Is.True);
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data.Id!, Is.EqualTo( 999));
            Assert.That(result.Data.TotalAmount, Is.EqualTo(60m));
            //Verify
            Assert.That(capturedItems.Count, Is.EqualTo(2));
            Assert.Multiple(() =>
            {
                Assert.That(capturedItems[0].OrderId, Is.EqualTo(999));
                Assert.That(capturedItems[0].ProductId, Is.EqualTo(productA.Id));
                Assert.That(capturedItems[0].Quantity, Is.EqualTo(2));
                Assert.That(capturedItems[1].ProductId, Is.EqualTo(productB.Id));
                Assert.That(capturedItems[1].Quantity, Is.EqualTo(2));
                _orderRepoMock.Verify(r => r.AddAsync(It.IsAny<Order>()), Times.Once());
                _orderItemRepoMock.Verify(r => r.AddAsync(It.IsAny<OrderItem>()), Times.Exactly(2));

            });
        }
        [Test]
        public async Task PlaceOrderAsync_Should_Skip_OrderItem_When_Product_Not_Found()
        {
            //Arrange
            var userId = _faker.Random.Guid().ToString();
            var productA = FakeProduct(id: 2, price: 10m, quantity: 2);
            var missingId = 20;
            var dto = FakeCreateOrder((productA.Id, 2), (missingId, 2));
           _orderRepoMock.Setup(r=>r.AddAsync(It.IsAny<Order>())).Callback<Order>(o=>o.Id=999).Returns(Task.CompletedTask);

            _productRepoMock.Setup(r => r.GetByIdAsync(productA.Id)).ReturnsAsync(productA);
            _productRepoMock.Setup(r=>r.GetByIdAsync(missingId)).ReturnsAsync((Product?)null);

            var captureItems=new List<OrderItem>();

            _orderItemRepoMock.Setup(r => r.AddAsync(It.IsAny<OrderItem>())).Callback<OrderItem>(captureItems.Add).Returns(Task.CompletedTask);

            _mapperMock.Setup(r=>r.Map<OrderResponse>(It.Is<Order>(o=>o.Id==999))).Returns(FakeOrderResponse);
            //Act
            var result = await _orderManager.PlaceOrderAsync(userId, dto);
            //Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data.Id!, Is.EqualTo(999));
            Assert.That(result.Data.TotalAmount, Is.EqualTo(20));

            Assert.That(captureItems.Count, Is.EqualTo(1));
        }
        [Test]
        public async Task PlaceOrderAsync_EmptyItems_Should_Create_Order_With_Zero_Total_And_No_Items()
        {
            //Arrange
            var userId=_faker.Random.Guid().ToString();
            var dto=new CreateOrder {Items= new List<OrderItemResponse>() };

            _orderRepoMock.Setup(r => r.AddAsync(It.IsAny<Order>())).Callback<Order>(o => o.Id = 999).Returns(Task.CompletedTask);
            _mapperMock.Setup(r => r.Map<OrderResponse>(It.Is<Order>(o => o.Id == 999))).Returns(FakeOrderResponse);

            //Act
            var result = await _orderManager.PlaceOrderAsync(userId, dto);
            //Assert
            Assert.That(result.Success,Is.True);
            Assert.That(result.Data!.TotalAmount, Is.EqualTo(0));
            _orderItemRepoMock.Verify(r => r.AddAsync(It.IsAny<OrderItem>()), Times.Never);

        }
        [Test]
        public async Task PlaceOrderAsync_When_OrderItem_Add_Throws_Should_Return_InternalServerError()
        {
            //Arrange
            var userId = _faker.Random.Guid().ToString();
            var productA = FakeProduct(id: 2, price: 10m, quantity: 2);
            var dto = FakeCreateOrder((productA.Id, 2));

            _orderRepoMock.Setup(r=>r.AddAsync(It.IsAny<Order>())).Callback<Order>(o=>o.Id=999).Returns(Task.CompletedTask);

           _productRepoMock.Setup(r=>r.GetByIdAsync(productA.Id)).ReturnsAsync(productA);

            _orderItemRepoMock.Setup(r => r.AddAsync(It.IsAny<OrderItem>())).ThrowsAsync(new Exception("Invalid Item"));

            //Act
            var result=await _orderManager.PlaceOrderAsync(userId, dto);
            //Assert
            Assert.That(result.Success, Is.False);
        }

        [Test]
        public async Task GetOrderByIdAsync_Found_Should_Return_Mapped_Response()
        {
            //Arrange
            var order = FakeOrder(id: 2, userId: "uuhq", total: 20m);
            _orderRepoMock.Setup(r => r.GetByIdAsync(order.Id)).ReturnsAsync(order);
            _mapperMock.Setup(r=>r.Map<OrderResponse>(order)).Returns(FakeOrderResponse(order));

            //Act
            var result=await _orderManager.GetOrderByIdAsync(order.Id);
            //Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data!.Id, Is.EqualTo(2));
            Assert.That(result.Data!.TotalAmount, Is.EqualTo(20));

        }
        [Test]
        public async Task GetOrderByIdAsync_NotFound_Should_Return_OrderNotFound()
        {
            //Arrange
            var orderId = 2;
            _orderRepoMock.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync((Order?)null);
            //Act
            var result=await _orderManager.GetOrderByIdAsync((int)orderId);
            //Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.Data, Is.Null);
        }
        

    }
}
