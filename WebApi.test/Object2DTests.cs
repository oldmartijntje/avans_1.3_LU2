using Microsoft.AspNetCore.Mvc;
using Moq;
using LeerUitkomst2.WebApi.Controllers;
using LeerUitkomst2.WebApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using LeerUitkomst2.WebApi.Repositories;
using LeerUitkomst2.WebApi.Services;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LeerUitkomst2.WebApi.Tests
{
    [TestClass]
    public class Object2DControllerTests
    {
        private Mock<EnvironmentRepository> _environmentRepositoryMock;
        private Mock<Object2DRepository> _object2DRepositoryMock;
        private Mock<ILogger<Object2DController>> _loggerMock;
        private Mock<IAuthenticationService> _authServiceMock;
        private Object2DController _controller;

        private Environment2D templateEnvironment = new Environment2D()
        {
            Id = 1,
            Name = "Henk",
            MaxHeight = 69,
            MaxLength = 69,
            UserId = "UserId"
        };

        [TestInitialize]
        public void Setup()
        {
            // Mock the dependencies
            _environmentRepositoryMock = new Mock<EnvironmentRepository>("dummy_connection_string");
            _object2DRepositoryMock = new Mock<Object2DRepository>("dummy_connection_string");
            _loggerMock = new Mock<ILogger<Object2DController>>();
            _authServiceMock = new Mock<IAuthenticationService>();

            // Setup the controller with mocked dependencies
            _controller = new Object2DController(
                _environmentRepositoryMock.Object,
                _object2DRepositoryMock.Object,
                _loggerMock.Object,
                _authServiceMock.Object
            );
        }

        [TestMethod]
        public async Task Add_ValidObject_ReturnsOk()
        {
            // Arrange
            var objectTemplate = new Object2DTemplate
            {
                PrefabId = 1,
                PositionX = 10,
                PositionY = 20,
                ScaleX = 1,
                ScaleY = 1,
                RotationZ = 375,
                SortingLayer = 0,
                EnvironmentId = 1
            };

            _authServiceMock.Setup(auth => auth.GetCurrentAuthenticatedUserId()).Returns("UserId");
            _environmentRepositoryMock.Setup(repo => repo.GetSingleEnvironmentByUser("UserId", 1)).ReturnsAsync(this.templateEnvironment);

            // Act
            var result = await _controller.Add(objectTemplate);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        [DataRow(-1, 10, 1f, 1f, 0, 1)] // out of bounds
        [DataRow(10, -10, 1f, 1f, 0, 1)] // out of bounds
        [DataRow(1000, 0, 1f, 1f, 0, 1)] // out of bounds
        [DataRow(0, 1000, 1f, 1f, 0, 1)] // out of bounds
        [DataRow(0, 0, 0.4f, 1f, 0, 1)] // too small
        [DataRow(0, 0, 10f, 1f, 0, 1)] // too big
        [DataRow(0, 0, 1f, 0.4f, 0, 1)] // too small
        [DataRow(0, 0, 1f, 10f, 0, 1)] // too big
        [DataRow(0, 0, 1f, 1f, -1, 1)] // invalid degree angle
        [DataRow(0, 0, 1f, 1f, 0, 0)] // unknown environment
        [DataRow(0, 0, 1f, 1f, 0, 2)] // not my environment
        public async Task Add_InvalidObject_ReturnsBadRequest(float PositionX, float PositionY, float ScaleX, float ScaleY, float RotationZ, int EnvironmentId)
        {
            // Arrange
            var objectTemplate = new Object2DTemplate
            {
                PrefabId = 1,
                PositionX = PositionX,
                PositionY = PositionY,
                ScaleX = ScaleX,
                ScaleY = ScaleY,
                RotationZ = RotationZ,
                SortingLayer = 0,
                EnvironmentId = EnvironmentId
            };

        _authServiceMock.Setup(auth => auth.GetCurrentAuthenticatedUserId()).Returns("UserId");
            _environmentRepositoryMock.Setup(repo => repo.GetSingleEnvironmentByUser("UserId", 1)).ReturnsAsync(this.templateEnvironment);
            _environmentRepositoryMock.Setup(repo => repo.GetSingleEnvironmentByUser("UserId", 2)).ReturnsAsync(new Environment2D()
            {
                Id = 1,
                Name = "Henk",
                MaxHeight = 69,
                MaxLength = 69,
                UserId = "NotUserId"
            });
            _environmentRepositoryMock.Setup(repo => repo.GetSingleEnvironmentByUser("UserId", 0)).ReturnsAsync((Environment2D)null);

            // Act
            var result = await _controller.Add(objectTemplate);

            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task Update_ValidObject_ReturnsOk()
        {
            // Arrange
            var objectTemplate = new Object2D
            {
                PrefabId = 1,
                PositionX = 10,
                PositionY = 20,
                ScaleX = 1,
                ScaleY = 1,
                RotationZ = 375,
                SortingLayer = 0,
                EnvironmentId = 1,
                Id = 0
            };
             
            _authServiceMock.Setup(auth => auth.GetCurrentAuthenticatedUserId()).Returns("UserId");
            _object2DRepositoryMock.Setup(repo => repo.GetSingleEnvironmentByUser("UserId", objectTemplate.Id)).ReturnsAsync(this.templateEnvironment);

            // Act
            var result = await _controller.Update(objectTemplate);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkResult));
        }

        [TestMethod]
        [DataRow(-1, 10, 1f, 1f, 0, 1)] // out of bounds
        [DataRow(10, -10, 1f, 1f, 0, 1)] // out of bounds
        [DataRow(1000, 0, 1f, 1f, 0, 1)] // out of bounds
        [DataRow(0, 1000, 1f, 1f, 0, 1)] // out of bounds
        [DataRow(0, 0, 0.4f, 1f, 0, 1)] // too small
        [DataRow(0, 0, 10f, 1f, 0, 1)] // too big
        [DataRow(0, 0, 1f, 0.4f, 0, 1)] // too small
        [DataRow(0, 0, 1f, 10f, 0, 1)] // too big
        [DataRow(0, 0, 1f, 1f, -1, 1)] // invalid degree angle
        public async Task Update_InvalidObject_ReturnsBadRequest(float PositionX, float PositionY, float ScaleX, float ScaleY, float RotationZ, int EnvironmentId)
        {
            // Arrange
            var objectTemplate = new Object2D
            {
                PrefabId = 1,
                PositionX = PositionX,
                PositionY = PositionY,
                ScaleX = ScaleX,
                ScaleY = ScaleY,
                RotationZ = RotationZ,
                SortingLayer = 0,
                EnvironmentId = EnvironmentId,
                Id = 0
            };

            _authServiceMock.Setup(auth => auth.GetCurrentAuthenticatedUserId()).Returns("UserId");
            _object2DRepositoryMock.Setup(repo => repo.GetSingleEnvironmentByUser("UserId", objectTemplate.Id)).ReturnsAsync(this.templateEnvironment);

            // Act
            var result = await _controller.Update(objectTemplate);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task DeleteObjectById_ValidId_ReturnsOk()
        {
            int objectId = 0;
            _authServiceMock.Setup(auth => auth.GetCurrentAuthenticatedUserId()).Returns("UserId");
            _object2DRepositoryMock.Setup(repo => repo.GetSingleEnvironmentByUser("UserId", objectId)).ReturnsAsync(this.templateEnvironment);
            _object2DRepositoryMock.Setup(repo => repo.ReadAsync(objectId)).ReturnsAsync(new Object2D
            {
                PrefabId = 1,
                PositionX = 10,
                PositionY = 20,
                ScaleX = 1,
                ScaleY = 1,
                RotationZ = 375,
                SortingLayer = 0,
                EnvironmentId = this.templateEnvironment.Id,
                Id = objectId
            });


            var result = await _controller.Remove(objectId);
            Assert.IsInstanceOfType(result, typeof(OkResult));
        }

        [TestMethod]
        public async Task DeleteObjectById_InvalidId_ReturnsNotFoundRequest()
        {
            int objectId = 0;
            _authServiceMock.Setup(auth => auth.GetCurrentAuthenticatedUserId()).Returns("UserId");
            _object2DRepositoryMock.Setup(repo => repo.GetSingleEnvironmentByUser("UserId", objectId)).ReturnsAsync(this.templateEnvironment);
            _object2DRepositoryMock.Setup(repo => repo.ReadAsync(objectId)).ReturnsAsync((Object2D)null);

            var result = await _controller.Remove(objectId);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task DeleteObjectById_InvalidId_ReturnsBadRequest()
        {
            int objectId = 0;
            _authServiceMock.Setup(auth => auth.GetCurrentAuthenticatedUserId()).Returns("UserId");
            _object2DRepositoryMock.Setup(repo => repo.GetSingleEnvironmentByUser("UserId", objectId)).ReturnsAsync(new Environment2D()
            {
                Id = 1,
                Name = "Henk",
                MaxHeight = 69,
                MaxLength = 69,
                UserId = "NotUserId"
            });

            var result = await _controller.Remove(objectId);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }
    }
}