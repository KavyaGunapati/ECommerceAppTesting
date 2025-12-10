using AutoMapper;
using Bogus;
using EcommerceWebAPI.Interfaces.IRepository;
using EcommerceWebAPI.Managers;
using ECommerceWebAPI.DataAccess.Entities;
using Moq;

namespace ManagerTests
{
  
    [TestFixture]
    public class OrderManagerTests
    {
        //Mocks
        private  Mock<IGenericRepository<Order>> _orderRepoMock;
        private Mock<IGenericRepository<OrderItem>> _orderItemRepoMock;
        private Mock<IGenericRepository<Product>> _productRepoMock;
        private  Mock<IMapper> _mapperMock;
        // System under test
        private OrderManager _orderManager;
        // Simple Faker instance
        private Faker _faker;

        [SetUp]
        public void Setup()
        {
            _orderItemRepoMock= new Mock<IGenericRepository<OrderItem>>();
            _orderRepoMock= new Mock<IGenericRepository<Order>>();
            _mapperMock= new Mock<IMapper>();
            _productRepoMock=new Mock<IGenericRepository<Product>>();
            _orderManager = new OrderManager(_orderRepoMock.Object,_orderItemRepoMock.Object,_productRepoMock.Object,_mapperMock.Object);
        }
    }
}
