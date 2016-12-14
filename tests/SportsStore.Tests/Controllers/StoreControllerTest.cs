using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SportsStore.Controllers;
using SportsStore.Models.Domain;
using SportsStore.Tests.Data;
using Xunit;
using System.Linq;

namespace SportsStore.Tests.Controllers
{
    public class StoreControllerTest
    {
        private readonly StoreController _controller;
        private readonly Mock<IProductRepository> _productRepository;
        private readonly DummyApplicationDbContext _context;

        public StoreControllerTest()
        {
            _context = new DummyApplicationDbContext();
            _productRepository = new Mock<IProductRepository>();
            _controller = new StoreController(_productRepository.Object);
        }
      
        [Fact]
        public void IndexShouldPassOrderedListOfProductsInViewResultModel()
        {
            _productRepository.Setup(p => 
                p.GetByAvailability(new List<Availability> { Availability.ShopAndOnline, Availability.OnlineOnly }))
                .Returns(_context.ProductsOnline);
            IEnumerable<Product> products = (_controller.Index() as ViewResult)?.Model  as IEnumerable<Product>;
            Assert.Equal(10, products?.Count());
        }
    }
}
