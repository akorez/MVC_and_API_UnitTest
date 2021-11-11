using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UdemyRealWorldUnitTest.Web.Models;

namespace UdemyRealWorldUnitTest.Test
{
    public class ProductControllerTest
    {
        protected DbContextOptions<UdemyUnitTestDBContext> _contextOptions { get; private set; }

        public void SetContextOptions (DbContextOptions<UdemyUnitTestDBContext> contextOptions)
        {
            _contextOptions = contextOptions;
            Seed();
        }

        public void Seed()
        {
            using (UdemyUnitTestDBContext context = new UdemyUnitTestDBContext(_contextOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();



                context.Category.Add(new Category { Name = "Kalemler" });
                context.Category.Add(new Category { Name = "Defterler" });

                context.SaveChanges();

                context.Product.Add(new Product { CategoryId = 1, Name = "Kalem10", Price = 10, Stock = 10, Color = "Kırmızı" });
                context.Product.Add(new Product { CategoryId = 1, Name = "Kalem20", Price = 20, Stock = 20, Color = "Mavi" });

                context.SaveChanges();

            }
        }
    }
}
