using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UdemyRealWorldUnitTest.Web.Controllers;
using UdemyRealWorldUnitTest.Web.Helpers;
using UdemyRealWorldUnitTest.Web.Models;
using UdemyRealWorldUnitTest.Web.Repository;
using Xunit;

namespace UdemyRealWorldUnitTest.Test
{
    public class ProductApiControllerTest
    {
        private readonly Mock<IRepository<Product>> _mockRepo;
        private readonly ProductsApiController _controller;
        private readonly Helper _helper;
        private List<Product> products;

        public ProductApiControllerTest()
        {
            {
                _mockRepo = new Mock<IRepository<Product>>();
                _controller = new ProductsApiController(_mockRepo.Object);
                _helper = new Helper();
                products = new List<Product> { new Product { Id = 1, Name = "Kalem", Price = 100, Stock = 50, Color = "Kırmızı" },
                new Product { Id = 2, Name = "Defter", Price = 70, Stock = 60, Color = "Mavi" } };
            }
        }

        [Theory]
        [InlineData(4,5,9)]

        public void Add_SampleValue_ReturnTotal (int a,int b, int total)
        {
            var result = _helper.add(a, b);

            Assert.Equal(total, result);

        }

        [Fact]
        public async void GetProduct_ActionExecutes_ReturnOkResultWithProduct()
        {
            _mockRepo.Setup(x => x.GetAll()).ReturnsAsync(products);

            var result = await _controller.GetProduct();

            var okResult = Assert.IsType<OkObjectResult>(result);

            var returnProducts = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);

            Assert.Equal<int>(2, returnProducts.ToList().Count());
        }

        [Theory]
        [InlineData(0)]
        public async void GetProduct_ProductIsNull_ReturnNotFound(int productId)
        {
            Product product = null;

            _mockRepo.Setup(x => x.GetById(productId)).ReturnsAsync(product);

            var result = await _controller.GetProduct(productId);

            Assert.IsType<NotFoundResult>(result);
        }

        [Theory]
        [InlineData(1)]

        public async void GetProduct_ProductIsNotNull_ReturnOkResultWithProduct(int productId)
        {
            Product product = products.First(x => x.Id == productId);
            _mockRepo.Setup(x => x.GetById(productId)).ReturnsAsync(product);

            var result = await _controller.GetProduct(productId);

            var okResult = Assert.IsType<OkObjectResult>(result);

            var returnProduct = Assert.IsAssignableFrom<Product>(okResult.Value);

            Assert.Equal(product.Id, returnProduct.Id);
        }

        [Theory]
        [InlineData(1)]

        public void PutProduct_IdIIsNotEqualProductId_ReturnBadRequest(int productId)
        {
            Product product = products.First(x => x.Id == productId);

            var result = _controller.PutProduct(2, product);

            Assert.IsType<BadRequestResult>(result);
        }

        [Theory]
        [InlineData(1)]

        public void PutProduct_ActionExecutes_ReturnNoContentAndUpdateProduct(int productId)
        {
            Product product = products.First(x => x.Id == productId);

            _mockRepo.Setup(x => x.Update(product));

            var result = _controller.PutProduct(productId, product);

            _mockRepo.Verify(x => x.Update(It.IsAny<Product>()), Times.Once);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]

        public async void PostProduct_ActionExecutes_ReturnCreateAtAction()
        {
            _mockRepo.Setup(x => x.Create(products.First())).Returns(Task.CompletedTask);

            var result = await _controller.PostProduct(products.First());

            _mockRepo.Verify(x => x.Create(It.IsAny<Product>()), Times.Once);

            var createdActionResult = Assert.IsType<CreatedAtActionResult>(result);

            Assert.Equal("GetProduct", createdActionResult.ActionName);
        }

        [Theory]
        [InlineData(0)]
        public async void DeleteProduct_IdInValid_ReturnNotFound(int productId)
        {
            Product product = null;

            _mockRepo.Setup(x => x.GetById(productId)).ReturnsAsync(product);

            var resultNotFound = await _controller.DeleteProduct(productId);

            Assert.IsType<NotFoundResult>(resultNotFound.Result);
        }

        [Theory]
        [InlineData(1)]
        public async void DeleteProduct_ActionExecute_ReturnNoContent(int productId)
        {
            var product = products.First(x => x.Id == productId);
            _mockRepo.Setup(x => x.GetById(productId)).ReturnsAsync(product);
            _mockRepo.Setup(x => x.Delete(product));

            var noContentResult = await _controller.DeleteProduct(productId);

            _mockRepo.Verify(x => x.Delete(product), Times.Once);

            Assert.IsType<NoContentResult>(noContentResult.Result);
        }
    }
}
