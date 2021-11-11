﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UdemyRealWorldUnitTest.Web.Controllers;
using UdemyRealWorldUnitTest.Web.Models;
using Xunit;

namespace UdemyRealWorldUnitTest.Test
{
    public class ProductControllerTestWithSqLite : ProductControllerTest
    {
        public ProductControllerTestWithSqLite()
        {
            var connection = new SqliteConnection("DataSource=:memory:");

            connection.Open();

            SetContextOptions(new DbContextOptionsBuilder<UdemyUnitTestDBContext>().UseSqlite(connection).Options);
        }

        [Fact]
        public async void Create_ModelValidProduct_ReturnRedirectToActionWithSaveProduct()
        {

            var newProduct = new Product { Name = "Kalem30", Price = 30, Stock = 30, Color ="Kırmızı"};
            using (var context = new UdemyUnitTestDBContext(_contextOptions))
            {
                var category = context.Category.First();

                newProduct.CategoryId = category.Id;

                var controller = new ProductsController(context);

                var result = await controller.Create(newProduct);

                var redirect = Assert.IsType<RedirectToActionResult>(result);

                Assert.Equal("Index", redirect.ActionName);
            }

            using (var context = new UdemyUnitTestDBContext(_contextOptions))
            {
                var product = context.Product.FirstOrDefault(x => x.Name == newProduct.Name);

                Assert.Equal(newProduct.Name, product.Name);
            }

        }

        [Theory]
        [InlineData(1)]
        public async Task DeleteCategory_ExistCategoryId_DeleteAllProducts(int categoryId)
        {
            using (var context = new UdemyUnitTestDBContext(_contextOptions))
            {
                var category = await context.Category.FindAsync(categoryId);

                Assert.NotNull(category);

                context.Category.Remove(category);

                context.SaveChanges();
            }

            using (var context = new UdemyUnitTestDBContext(_contextOptions))
            {
                var products = await context.Product.Where(x => x.CategoryId == categoryId).ToListAsync();

                Assert.Empty(products);

            }

        }
    }
}
