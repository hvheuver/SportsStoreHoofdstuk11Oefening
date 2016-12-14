using System.Collections.Generic;
using Moq;
using SportsStore.Controllers;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SportsStore.Models.Domain;
using SportsStore.Tests.Data;
using Xunit;

namespace SportsStore.Tests.Controllers
{
    public class CartControllerTest
    {
        private readonly CartController _controller;
        private readonly Cart _cart;

        public  CartControllerTest()
        {
            DummyApplicationDbContext context = new DummyApplicationDbContext();

            Mock<IProductRepository> productRepository = new Mock<IProductRepository>();
            productRepository.Setup(p => p.GetAll()).Returns(context.Products);
            productRepository.Setup(p => p.GetById(1)).Returns(context.Football);
            productRepository.Setup(p => p.GetById(4)).Returns(context.RunningShoes);

            _controller = new CartController(productRepository.Object);
            _controller.TempData = new Mock<ITempDataDictionary>().Object;

            _cart = new Cart();
            _cart.AddLine(context.Football, 2);
        }

        #region Index
        [Fact]
        public void IndexGivenCartIsEmptyShouldShowEmptyCartView()
        {
            Cart emptycart = new Cart();
            ViewResult result = _controller.Index(emptycart) as ViewResult;
            Assert.Equal("EmptyCart", result?.ViewName);
        }

     
        [Fact]
        public void IndexShouldPassCartLinesInModel()
        {
            ViewResult result = _controller.Index(_cart) as ViewResult;
            IEnumerable<CartLine> cartresult = result?.Model as IEnumerable<CartLine>;
            Assert.Equal(1, cartresult?.Count());
        }

        [Fact]
        public void IndexShouldStoreTotalInViewData()
        {
            ViewResult result = _controller.Index(_cart) as ViewResult;
            Assert.Equal(50, result?.ViewData["Total"]);
        }
        #endregion

        #region Add
        [Fact]
        public void AddShouldRedirectToActionIndexOfStoreWhenSuccessfull()
        {
            RedirectToActionResult result = _controller.Add(4, 2, _cart) as RedirectToActionResult;
            Assert.Equal("Index", result?.ActionName);
            Assert.Equal("Store", result?.ControllerName);
        }

        [Fact]
        public void AddShouldAddProductToCartWhenSuccessfull()
        {
            _controller.Add(4, 2, _cart);
            Assert.Equal(2, _cart.NumberOfItems);
        }

        #endregion

        #region Remove
        [Fact]
        public void RemoveShouldRedirectToIndexWhenSuccessfull()
        {
            RedirectToActionResult result = _controller.Remove(1, _cart) as RedirectToActionResult;
            Assert.Equal("Index", result?.ActionName);
        }

        [Fact]
        public void RemoveShouldRemoveProductFromCartWhenSuccessfull()
        {
            _controller.Remove(1, _cart);
            Assert.Equal(0, _cart.NumberOfItems);
        }
        #endregion

        #region Plus
        [Fact]
        public void PlusShouldRedirectToIndexWhenSuccessfull()
        {
            RedirectToActionResult result = _controller.Plus(1, _cart) as RedirectToActionResult;
            Assert.Equal("Index", result?.ActionName);
        }

        [Fact]
        public void PlusShouldIncreaseQuantity()
        {
            _controller.Plus(1, _cart);
            CartLine line = _cart.CartLines.ToList()[0];
            Assert.Equal(3, line.Quantity);
        }
        #endregion

        #region Min
        [Fact]
        public void MinShouldRedirectToIndexWhenSuccessfull()
        {
            RedirectToActionResult result = _controller.Min(1, _cart) as RedirectToActionResult;
            Assert.Equal("Index", result?.ActionName);
        }

        [Fact]
        public void MinShouldDecreaseQuantity()
        {
            _controller.Min(1, _cart);
            CartLine line = _cart.CartLines.ToList()[0];
            Assert.Equal(1, line.Quantity);
        }
        #endregion

    }
}
