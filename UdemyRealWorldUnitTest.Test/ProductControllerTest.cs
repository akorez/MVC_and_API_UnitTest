using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using UdemyRealWorldUnitTest.Web.Controllers;
using UdemyRealWorldUnitTest.Web.Models;
using UdemyRealWorldUnitTest.Web.Repository;
using Xunit;

namespace UdemyRealWorldUnitTest.Test
{
    public class ProductControllerTest
    {
        private readonly Mock<IRepository<Product>> _mockRepo;
        private readonly ProductsController _controller;
        private List<Product> products;
        public ProductControllerTest()
        {
            _mockRepo = new Mock<IRepository<Product>>();
            _controller = new ProductsController(_mockRepo.Object);

            products = new List<Product> { new Product { Id = 1, Name = "Kalem", Price = 100, Stock = 50, Color = "Kýrmýzý" },
                new Product { Id = 2, Name = "Defter", Price = 70, Stock = 60, Color = "Mavi" } };
        }

        [Fact]
        public async void Index_ActionExecutes_ReturnView()
        {
            var result = await _controller.Index();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async void Index_ActionExecutes_ReturnProductList()
        {
            _mockRepo.Setup(x => x.GetAll()).ReturnsAsync(products);

            var result = await _controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);

            var productList = Assert.IsAssignableFrom<IEnumerable<Product>>(viewResult.Model);

            Assert.Equal<int>(2, productList.Count());
        }

        [Fact]
        public async void Details_IdIsNull_ReturnRedirectToIndexAction()
        {
            var result = await _controller.Details(null);

            var redirect = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        public async void Details_ProductIsNull_ReturnNotFound()
        {
            Product product = null;
            _mockRepo.Setup(x => x.GetById(0)).ReturnsAsync(product);

            var result = await _controller.Details(0);

            var redirect = Assert.IsType<NotFoundResult>(result);

            Assert.Equal<int>(404, redirect.StatusCode);
        }

        [Theory]
        [InlineData(1)]

        public async void Details_ValidId_ReturnView(int productId)
        {
            Product product = products.FirstOrDefault(x => x.Id == productId);

            _mockRepo.Setup(x => x.GetById(productId)).ReturnsAsync(product);

            var result = await _controller.Details(productId);

            var viewResult = Assert.IsType<ViewResult>(result);

            var resultProduct = Assert.IsAssignableFrom<Product>(viewResult.Model);

            Assert.Equal(product.Id, resultProduct.Id);
            Assert.Equal(product.Name, resultProduct.Name);
        }

        [Fact]
        public void Create_ActionExecutes_ReturnView()
        {
            var result = _controller.Create();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async void CreatePOST_ModelInvalid_ReturnCreateView()
        {
            _controller.ModelState.AddModelError("Name", "Name is required");

            var result = await _controller.Create(products.First());

            var viewResult = Assert.IsType<ViewResult>(result);

            Assert.IsType<Product>(viewResult.Model);
        }

        [Fact]
        public async void CreatePOST_ModelValid_ReturnRedirectToIndex()
        {
            var result = await _controller.Create(products.First());

            var redirect = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        public async void CreatePOST_ModelValid_CreateMethodExecute()
        {
            Product newProduct = null;

            _mockRepo.Setup(x => x.Create(It.IsAny<Product>())).Callback<Product>(x => newProduct = x);

            var result = await _controller.Create(products.First());

            _mockRepo.Verify(x => x.Create(It.IsAny<Product>()), Times.Once);

            Assert.Equal(products.First().Id, newProduct.Id);
        }

        [Fact]
        public async void CreatePOST_ModelInvalid_CreateMethodNotExecute()
        {
            _controller.ModelState.AddModelError("Name", "Name is required");

            var result = await _controller.Create(products.First());

            _mockRepo.Verify(x => x.Create(It.IsAny<Product>()), Times.Never);
        }

        [Fact]
        public async void Edit_IdIsNull_RedirectToIndexAction()
        {
            var result = await _controller.Edit(null);

            var viewResult = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Index", viewResult.ActionName);
        }

        [Fact]
        public async void Edit_ProductIsNull_ReturnNotFound()
        {
            Product product = null;

            _mockRepo.Setup(x => x.GetById(0)).ReturnsAsync(product);

            var result = await _controller.Edit(0);

            var redirect = Assert.IsType<NotFoundResult>(result);

            Assert.Equal(404, redirect.StatusCode);
        }

        [Theory]
        [InlineData(2)]
        public async void Edit_ValidId_ReturnView(int productId)
        {
            Product product = products.FirstOrDefault(x => x.Id == productId);

            _mockRepo.Setup(x => x.GetById(productId)).ReturnsAsync(product);

            var result = await _controller.Edit(productId);

            var viewResult = Assert.IsType<ViewResult>(result);

            var resultProduct = Assert.IsAssignableFrom<Product>(viewResult.Model);

            Assert.Equal<int>(resultProduct.Id, product.Id);
        }

        [Theory]
        [InlineData(1)]
        public void EditPOST_IdIsNotEqualProductId_ReturnNotFound(int productId)
        {
            var result = _controller.Edit(2, products.First(x => x.Id == productId));

            var redirect = Assert.IsType<NotFoundResult>(result);
        }

        [Theory]
        [InlineData(1)]

        public void EditPOST_ModelInvalid_ReturnEditView(int productId)
        {
            _controller.ModelState.AddModelError("Name", "");

            var result = _controller.Edit(productId, products.First(x => x.Id == productId));

            var viewResult = Assert.IsType<ViewResult>(result);

            Assert.IsType<Product>(viewResult.Model);
        }


        [Theory]
        [InlineData(1)]

        public void EditPOST_ModelValid_ReturnRedirectToIndex(int productId)
        {
            var result = _controller.Edit(productId, products.First(x => x.Id == productId));

            var viewResult = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Index", viewResult.ActionName);
        }

        [Theory]
        [InlineData(1)]
        public void EditPOST_ModelValid_UpdateMethodExecute(int productId)
        {
            Product product = products.First(x => x.Id == productId);
            _mockRepo.Setup(x => x.Update(product));

            _controller.Edit(productId, product);

            _mockRepo.Verify(x => x.Update(It.IsAny<Product>()), Times.Once);

        }
    }
}
