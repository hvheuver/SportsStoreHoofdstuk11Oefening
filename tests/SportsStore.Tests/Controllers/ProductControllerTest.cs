using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SportsStore.Controllers;
using SportsStore.Models.Domain;
using Moq;
using SportsStore.Models.ViewModels.ProductViewModels;
using SportsStore.Tests.Data;
using Xunit;

namespace SportsStore.Tests.Controllers
{
    public class ProductControllerTest
    {
        private ProductController _productController;
        private Mock<IProductRepository> _mockProductRepository;
        private Mock<ICategoryRepository> _mockCategoryRepository;
        private readonly DummyApplicationDbContext _dummyContext = new DummyApplicationDbContext();
        private Product _runningShoes;
        private int _runningShoesId;
        private Product _nieuwProduct;


        public ProductControllerTest()
        {
            _mockProductRepository = new Mock<IProductRepository>();
            _mockCategoryRepository = new Mock<ICategoryRepository>();
            _runningShoes = _dummyContext.RunningShoes;
            _runningShoesId = _runningShoes.ProductId;

            _productController = new ProductController(_mockProductRepository.Object, _mockCategoryRepository.Object);
            _productController.TempData = new Mock<ITempDataDictionary>().Object;
            _nieuwProduct = new Product()
            {
                ProductId = 100,
                Availability = Availability.OnlineOnly,
                Category = _dummyContext.Categories.First(),
                Description = "nieuw product",
                Name = "nieuw product",
                Price = 10
            };
        }

        #region == Index ==

        [Fact]
        public void IndexGivenNoCategorySelectedShouldReturnAllProductsSortedByName()
        {
            _mockProductRepository.Setup(p => p.GetAll()).Returns(_dummyContext.Products);
            ViewResult result = _productController.Index() as ViewResult;
            List<Product> products = (result?.Model as IEnumerable<Product>).ToList();
            Assert.Equal(11, products.Count);
            Assert.Equal("Bling-bling King", products[0].Name);
            Assert.Equal("Unsteady chair", products[10].Name);
        }

        #endregion

        #region == Edit ==

        [Fact]
        public void EditShouldReturnAEditViewModel()
        {
            _mockProductRepository.Setup(p => p.GetById(4)).Returns(_dummyContext.RunningShoes);
            _mockCategoryRepository.Setup(c => c.GetAll()).Returns(_dummyContext.Categories);
            ViewResult result = _productController.Edit(_runningShoesId) as ViewResult;
            EditViewModel productVm = result?.Model as EditViewModel;
            Assert.Equal(_runningShoesId, productVm.ProductId);
        }

        [Fact]
        public void EditShouldReturnSelectListOfCategories()
        {
            _mockProductRepository.Setup(p => p.GetById(4)).Returns(_dummyContext.RunningShoes);
            _mockCategoryRepository.Setup(c => c.GetAll()).Returns(_dummyContext.Categories);
            ViewResult result = _productController.Edit(_runningShoesId) as ViewResult;
            SelectList categories = result?.ViewData["Categories"] as SelectList;
            Assert.Equal(_dummyContext.Categories.Count(), categories.Count());
            Assert.Equal(2, categories?.SelectedValue);
        }

        [Fact]
        public void EditGivenProductDoesNotExistShouldReturnNotFound()
        {
            _mockProductRepository.Setup(p => p.GetById(-1)).Returns((Product)null);
            IActionResult result = _productController.Edit(-1);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void EditPostShouldUpdateProduct()
        {
            _mockProductRepository.Setup(p => p.GetById(4)).Returns(_dummyContext.RunningShoes);
            _mockCategoryRepository.Setup(c => c.GetById(2)).Returns(_dummyContext.Soccer);
            EditViewModel productVm = new EditViewModel(_runningShoes);
            productVm.Name = "RunningShoesGewijzigd";
            productVm.Price = 1000;
            _productController.Edit(productVm);
            Assert.Equal("RunningShoesGewijzigd", _runningShoes.Name);
            Assert.Equal(1000, _runningShoes.Price);
            Assert.Equal("Protective and fashionable", _runningShoes.Description);
            _mockProductRepository.Verify(m => m.SaveChanges(), Times.Once);
        }

        [Fact]
        public void EditPostShouldRedirectToIndexWhenSuccessfull()
        {
            _mockProductRepository.Setup(p => p.GetById(4)).Returns(_dummyContext.RunningShoes);
            _mockCategoryRepository.Setup(c => c.GetById(2)).Returns(_dummyContext.Soccer);

            EditViewModel productVm = new EditViewModel(_runningShoes);
            RedirectToActionResult result = _productController.Edit(productVm) as RedirectToActionResult;
            Assert.Equal("Index", result?.ActionName);
        }

        [Fact]
        public void EditPostGivenInvalidModelShouldNotUpdateProduct()
        {
            _mockProductRepository.Setup(p => p.GetById(4)).Returns(_dummyContext.RunningShoes);
            _mockCategoryRepository.Setup(c => c.GetById(2)).Returns(_dummyContext.Soccer);

            EditViewModel productVm = new EditViewModel(_runningShoes);
            productVm.Price = -1;
            _productController.ModelState.AddModelError("key", "errorMessage");
            _productController.Edit(productVm);
            Assert.Equal(95, _runningShoes.Price);
            _mockProductRepository.Verify(m => m.SaveChanges(), Times.Never);
        }

        [Fact]
        public void EditPostGivenInvalidModelShouldReturnViewModel()
        {
            _mockProductRepository.Setup(p => p.GetById(4)).Returns(_dummyContext.RunningShoes);
            _mockCategoryRepository.Setup(c => c.GetById(2)).Returns(_dummyContext.Soccer);

            EditViewModel productVm = new EditViewModel(_runningShoes);
            productVm.Price = -1;
            _productController.ModelState.AddModelError("key", "errorMessage");
            ViewResult result = _productController.Edit(productVm) as ViewResult;
            Assert.Equal(productVm, result?.Model as EditViewModel);
        }

        [Fact]
        public void EditPostGivenInvalidModelShouldReturnSelectList()
        {
            _mockProductRepository.Setup(p => p.GetById(4)).Returns(_dummyContext.RunningShoes);
            _mockCategoryRepository.Setup(c => c.GetById(2)).Returns(_dummyContext.Soccer);
            _mockCategoryRepository.Setup(c => c.GetAll()).Returns(_dummyContext.Categories);

            EditViewModel productVm = new EditViewModel(_runningShoes);
            productVm.Price = -1;
            _productController.ModelState.AddModelError("key", "errorMessage");
            ViewResult result = _productController.Edit(productVm) as ViewResult;
            Assert.Equal(_dummyContext.Categories.Count(), (result?.ViewData["Categories"] as SelectList).ToArray().Length);
        }

        #endregion

        #region == Create ==

        [Fact]
        public void CreateGetShouldReturnEditViewModelForANewProduct()
        {
            _mockCategoryRepository.Setup(c => c.GetAll()).Returns(_dummyContext.Categories);

            ViewResult result = _productController.Create() as ViewResult;
            EditViewModel productVm = result?.Model as EditViewModel;
            Assert.Null(productVm?.Name);
            Assert.Equal(0, productVm?.ProductId);
        }

        [Fact]
        public void CreatePostShouldRedirectToIndex()
        {
            EditViewModel productVm = new EditViewModel(_nieuwProduct);
            RedirectToActionResult result = _productController.Create(productVm) as RedirectToActionResult;
            Assert.Equal("Index", result?.ActionName);
        }

        [Fact]
        public void CreatePostShouldAddNewProduct()
        {
            _mockProductRepository.Setup(p => p.Add(It.IsNotNull<Product>()));

            EditViewModel productVm = new EditViewModel(_nieuwProduct);
            _productController.Create(productVm);
            _mockProductRepository.Verify(m => m.Add(It.IsNotNull<Product>()), Times.Once);
            _mockProductRepository.Verify(m => m.SaveChanges(), Times.Once);
        }

        [Fact]
        public void CreatePostGivenInvalidModelShouldNotAddProduct()
        {
            _mockProductRepository.Setup(p => p.Add(It.IsNotNull<Product>()));

            EditViewModel productVm = new EditViewModel(_nieuwProduct);
            productVm.Price = -1;
            _productController.ModelState.AddModelError("key", "errorMessage");
            _productController.Create(productVm);
            _mockProductRepository.Verify(m => m.Add(It.IsNotNull<Product>()), Times.Never);
        }

        [Fact]
        public void CreatePostGivenInvalidModelShouldReturnViewModel()
        {
            EditViewModel productVm = new EditViewModel(_nieuwProduct);
            productVm.Price = -1;
            _productController.ModelState.AddModelError("key", "errorMessage");
            ViewResult result = _productController.Create(productVm) as ViewResult;
            Assert.Equal(productVm, result?.Model as EditViewModel);
        }
        #endregion

        #region == Delete ==

        [Fact]
        public void DeleteShouldReturnViewDataWithProductName()
        {
            _mockProductRepository.Setup(p => p.GetById(4)).Returns(_dummyContext.RunningShoes);

            ViewResult result = _productController.Delete(_runningShoesId) as ViewResult;
            Assert.Equal("Running shoes", result?.ViewData["ProductName"]);
        }

        [Fact]
        public void DeleteGivenInvalidProductIdShouldReturnNotFound()
        {
            _mockProductRepository.Setup(p => p.GetById(-1)).Returns((Product)null);
            IActionResult result = _productController.Delete(-1);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void DeletePostShouldDeleteProduct()
        {
            _mockProductRepository.Setup(p => p.GetById(4)).Returns(_dummyContext.RunningShoes);
            _mockProductRepository.Setup(p => p.Delete(_runningShoes));

            _productController.DeleteConfirmed(_runningShoesId);
            _mockProductRepository.Verify(m => m.Delete(_runningShoes), Times.Once);
            _mockProductRepository.Verify(m => m.SaveChanges(), Times.Once);
        }

        [Fact]
        public void DeletePostShouldRedirectToIndex()
        {
            _mockProductRepository.Setup(p => p.GetById(4)).Returns(_dummyContext.RunningShoes);

            RedirectToActionResult result = _productController.DeleteConfirmed(_runningShoesId) as RedirectToActionResult;
            Assert.Equal("Index", result?.ActionName);
        }

        [Fact]
        public void DeletePostGivenNotSuccessfullShouldRedirectToIndex()
        {
            _mockProductRepository.Setup(p => p.Delete(_dummyContext.Football)).Throws<Exception>();
            _productController.DeleteConfirmed(_dummyContext.Football.ProductId);
        }
        #endregion


    }

}
