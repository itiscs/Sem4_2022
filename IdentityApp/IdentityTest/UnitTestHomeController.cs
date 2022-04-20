using Xunit;
using IdentityApp.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using IdentityApp.Data;
using System.Collections.Generic;
using IdentityApp.Models;

namespace IdentityTest
{
    public class UnitTestHomeController
    {

        [Fact]
        public async void IndexViewDataMessage()
        {
            // Arrange
            HomeController controller = new HomeController(null, null);

            // Act
            ViewResult result = controller.Privacy() as ViewResult;

            // Assert
            Assert.Equal("Hello, world!", result?.ViewData["Message"]);
           // Assert.Equal("Privacy Policy", result?.ViewData["Title"]);
        }

       

        [Fact]
        public async void IndexHomeResult()
        {
            // Arrange
            HomeController controller = new HomeController(null, null);

            // Act
            ViewResult result = await controller.Index() as ViewResult;

            // Assert
            Assert.NotNull(result);
        }
    }
}
