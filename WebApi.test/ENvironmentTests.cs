using Microsoft.AspNetCore.Mvc;
using Moq;
using LeerUitkomst2.WebApi.Controllers;
using LeerUitkomst2.WebApi.Models;
using ProjectMap.WebApi.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LeerUitkomst2.WebApi.Tests
{
    [TestClass]
    public class EnvironmentControllerTests
    {
        private Mock<EnvironmentRepository> _environmentRepositoryMock;
        private Mock<Object2DRepository> _object2DRepositoryMock;
        private Mock<ILogger<EnvironmentController>> _loggerMock;
        private Mock<IAuthenticationService> _authServiceMock;
        private EnvironmentController _controller;

        [TestInitialize]
        public void Setup()
        {
            // Mock the dependencies
            _environmentRepositoryMock = new Mock<EnvironmentRepository>("dummy_connection_string");
            _object2DRepositoryMock = new Mock<Object2DRepository>("dummy_connection_string");
            _loggerMock = new Mock<ILogger<EnvironmentController>>();
            _authServiceMock = new Mock<IAuthenticationService>();

            // Setup the controller with mocked dependencies
            _controller = new EnvironmentController(
                _environmentRepositoryMock.Object,
                _object2DRepositoryMock.Object,
                _loggerMock.Object,
                _authServiceMock.Object
            );
        }

        [TestMethod]
        public async Task Add_ValidEnvironment_ReturnsOk()
        {
            // Arrange
            var environment = new Environment2DTemplate
            {
                Name = "Test Environment",
                MaxHeight = 50,
                MaxLength = 100
            };

            _authServiceMock.Setup(auth => auth.GetCurrentAuthenticatedUserId()).Returns("user123");
            _environmentRepositoryMock.Setup(repo => repo.GetAmountByUser("user123")).ReturnsAsync(2);
            _environmentRepositoryMock.Setup(repo => repo.FindEnvironmentByName("user123", "Test Environment")).ReturnsAsync(new List<Environment2D>());
            _environmentRepositoryMock.Setup(repo => repo.CreateEnvironmentByUser(It.IsAny<Environment2DTemplate>(), "user123")).ReturnsAsync(new Environment2D
            {
                Id = 1,
                Name = "Test Environment",
                MaxHeight = 50,
                MaxLength = 100,
                UserId = "user123"
            });

            // Act
            var result = await _controller.Add(environment);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task Add_EnvironmentAlreadyExists_ReturnsBadRequest()
        {
            // Arrange
            var environment = new Environment2DTemplate
            {
                Name = "Test Environment",
                MaxHeight = 50,
                MaxLength = 100
            };

            _authServiceMock.Setup(auth => auth.GetCurrentAuthenticatedUserId()).Returns("user123");
            _environmentRepositoryMock.Setup(repo => repo.GetAmountByUser("user123")).ReturnsAsync(2);
            _environmentRepositoryMock.Setup(repo => repo.FindEnvironmentByName("user123", "Test Environment")).ReturnsAsync(new List<Environment2D> { new Environment2D() });

            // Act
            var result = await _controller.Add(environment);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual("Only 1 environment allowed with the same name.", badRequestResult.Value);
        }

        [TestMethod]
        public async Task DeleteEnvironmentById_ValidId_ReturnsOk()
        {
            // Arrange
            int environmentId = 1;
            _authServiceMock.Setup(auth => auth.GetCurrentAuthenticatedUserId()).Returns("user123");
            _environmentRepositoryMock.Setup(repo => repo.GetSingleByUser("user123", environmentId)).ReturnsAsync(new Environment2D { Id = environmentId, UserId = "user123" });
            _environmentRepositoryMock.Setup(repo => repo.DeleteAsync(environmentId)).ReturnsAsync(new DataBoolean(true, "Success"));

            // Act
            var result = await _controller.Update(environmentId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task DeleteEnvironmentById_UserNotAuthorized_ReturnsBadRequest()
        {
            // Arrange
            int environmentId = 1;
            _authServiceMock.Setup(auth => auth.GetCurrentAuthenticatedUserId()).Returns("user123");
            _environmentRepositoryMock.Setup(repo => repo.GetSingleByUser("user123", environmentId)).ReturnsAsync((Environment2D)null);

            // Act
            var result = await _controller.Update(environmentId);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual("This environment does not exist in this context.", badRequestResult.Value);
        }
    }
}
