using IdentityApp.Controllers;
using IdentityApp.Data;
using IdentityApp.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace IdentityTest
{
    public class UnitTestProductsController
    {
        [Fact]
        public async void ProductsIndex()
        {
            var moq = new Mock<IUnitOfWork>();
            moq.Setup(repo => repo.Products.GetAll()).Returns(GetTestProducts());
            // Arrange
            ProductsController controller = new ProductsController(moq.Object);


            // Act
            var result = await controller.Index();

            // Assert

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Product>>(viewResult.Model);
            var prods = model as List<Product>;
            Assert.Equal(prods?.Count, (GetTestProducts().Result as List<Product>)?.Count);

        }

        [Fact]
        public async void AddProdcutReturnsViewResultWithUserModel()
        {
            // Arrange
            var mock = new Mock<IUnitOfWork>();
            var controller = new ProductsController(mock.Object);
            controller.ModelState.AddModelError("Name", "Required");
            Product newProduct = new Product();

            // Act
            var result = await controller.Create(newProduct);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(newProduct, viewResult?.Model);
        }

        [Fact]
        public async void AddProductReturnsARedirectAndAddsProduct()
        {
            // Arrange
            var newProduct = new Product()
            {
                ProductId = 5,
                Name = "New Product",
                Description = "New Product",
                Price = 500,
                ImageUrl = "image 5"
            };
            var moq = new Mock<IUnitOfWork>();
            moq.Setup(repo => repo.Products.Add(newProduct)).Returns(AddTestProduct(newProduct));
            var controller = new ProductsController(moq.Object);
            

            // Act
            var result = await controller.Create(newProduct);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            moq.Verify(r => r.Products.Add(newProduct));
        }

        [Fact]
        public async void GetProductReturnsBadRequestResultWhenIdIsNull()
        {
            // Arrange
            var mock = new Mock<IUnitOfWork>();
            var controller = new ProductsController(mock.Object);

            // Act
            var result = await controller.Details(null);

            // Arrange
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async void GetProductReturnsNotFoundResultWhenProductNotFound()
        {
            // Arrange
            int testProductId = 5;
            var mock = new Mock<IUnitOfWork>();
            mock.Setup(repo => repo.Products.GetById(testProductId))
                .Returns(GetTestProductById(testProductId));
            var controller = new ProductsController(mock.Object);

            // Act
            var result = await controller.Details(testProductId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async void GetProductReturnsViewResultWithProduct()
        {
            // Arrange
            int testProductId = 1;
            var mock = new Mock<IUnitOfWork>();
            mock.Setup(repo => repo.Products.GetById(testProductId))
                .Returns(GetTestProductById(testProductId));
            var controller = new ProductsController(mock.Object);

            // Act
            var result = await controller.Details(testProductId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Product>(viewResult.ViewData.Model);
            Assert.Equal("Product 1", model.Name);
            Assert.Equal(100, model.Price);
            Assert.Equal(testProductId, model.ProductId);
        }


        private async Task<IEnumerable<Product>> GetTestProducts()
        {
            var lst = new List<Product>()
            {
                new Product(){ProductId=1, Name="Product 1", Description="Descr 1",
                ImageUrl="image 1", Price=100},
                new Product(){ProductId=2, Name="Product 2", Description="Descr 2",
                ImageUrl="image 2", Price=200},
                new Product(){ProductId=3, Name="Product 3", Description="Descr 3",
                ImageUrl="image 3", Price=300},
                new Product(){ProductId=4, Name="Product 4", Description="Descr 4",
                ImageUrl="image 4", Price=400},
            };

            return lst;
        }

        private async Task<Product> GetTestProductById(int id)
        {
            var prods = await GetTestProducts() as List<Product>;

            return prods.Find(p => p.ProductId == id);
        }

        private async Task<bool> AddTestProduct(Product prod)
        {
            var prods = await GetTestProducts() as List<Product>;

            prods.Add(prod);

            return true;
        }
    }
}