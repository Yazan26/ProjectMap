using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MijnWebApi.WebApi.Controllers;
using MijnWebApi.WebApi.Classes.Interfaces;
using MijnWebApi.WebApi.Classes.Models;
using Microsoft.Extensions.Logging;

namespace TestProject.Controllers
{
    [TestClass]
    public class Object2DControllerTests
    {
        private Mock<IObject2DRepository> _mockRepository;
        private Mock<IAuthenticationService> _mockAuthService;
        private Mock<ILogger<Object2DController>> _mockLogger;
        private Object2DController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockRepository = new Mock<IObject2DRepository>();
            _mockAuthService = new Mock<IAuthenticationService>();
            _mockLogger = new Mock<ILogger<Object2DController>>();
            _controller = new Object2DController(_mockRepository.Object, _mockAuthService.Object, _mockLogger.Object);
        }

        [TestMethod]
        public async Task Get_ShouldReturnAllObjects()
        {
            // Arrange
            var expectedObjects = new List<Object2D>
            {
                new Object2D { Id = Guid.NewGuid(), PrefabId = "Prefab1", PositionX = 10, PositionY = 20 },
                new Object2D { Id = Guid.NewGuid(), PrefabId = "Prefab2", PositionX = 30, PositionY = 40 }
            };

            _mockRepository.Setup(repo => repo.GetAllObjectsAsync()).ReturnsAsync(expectedObjects);

            // Act
            var result = await _controller.Get();

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(expectedObjects, okResult.Value);
        }

        [TestMethod]
        public async Task GetByEnvironmentId_ShouldReturnObjects_WhenObjectsExist()
        {
            // Arrange
            var environmentId = Guid.NewGuid();
            var expectedObjects = new List<Object2D>
            {
                new Object2D { Id = Guid.NewGuid(), Environment2DID = environmentId, PrefabId = "Prefab1" },
                new Object2D { Id = Guid.NewGuid(), Environment2DID = environmentId, PrefabId = "Prefab2" }
            };

            _mockRepository.Setup(repo => repo.GetObjectAsync(environmentId)).ReturnsAsync(expectedObjects);

            // Act
            var result = await _controller.Get(environmentId);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(expectedObjects, okResult.Value);
        }

        [TestMethod]
        public async Task GetByEnvironmentId_ShouldReturnNotFound_WhenNoObjectsExist()
        {
            // Arrange
            var environmentId = Guid.NewGuid();
            _mockRepository.Setup(repo => repo.GetObjectAsync(environmentId)).ReturnsAsync((IEnumerable<Object2D>)null);

            // Act
            var result = await _controller.Get(environmentId);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Add_ShouldCreateObject()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var newObject = new Object2D
            {
                PrefabId = "Prefab1",
                PositionX = 10,
                PositionY = 20,
                ScaleX = 1,
                ScaleY = 1,
                RotationZ = 0,
                SortingLayer = 0,
                Environment2DID = Guid.NewGuid()
            };
            var createdObject = new Object2D
            {
                Id = Guid.NewGuid(),
                PrefabId = "Prefab1",
                PositionX = 10,
                PositionY = 20,
                ScaleX = 1,
                ScaleY = 1,
                RotationZ = 0,
                SortingLayer = 0,
                Environment2DID = newObject.Environment2DID,
                UserID = userId
            };

            _mockAuthService.Setup(auth => auth.GetCurrentAuthenticatedUserId()).Returns(userId.ToString());
            _mockRepository.Setup(repo => repo.PostObjectAsync(It.IsAny<Object2D>())).ReturnsAsync(createdObject);

            // Act
            var result = await _controller.Add(newObject);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(createdObject, okResult.Value);
        }

        [TestMethod]
        public async Task Delete_ShouldDeleteObject_WhenObjectExists()
        {
            // Arrange
            var objectId = Guid.NewGuid();
            var existingObject = new List<Object2D>
            {
                new Object2D { Id = objectId, PrefabId = "Prefab1" }
            };

            _mockRepository.Setup(repo => repo.GetObjectAsync(objectId)).ReturnsAsync(existingObject);
            _mockRepository.Setup(repo => repo.DeleteObjectAsync(objectId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(objectId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkResult));
        }

        [TestMethod]
        public async Task Delete_ShouldReturnNotFound_WhenObjectDoesNotExist()
        {
            // Arrange
            var objectId = Guid.NewGuid();
            _mockRepository.Setup(repo => repo.GetObjectAsync(objectId)).ReturnsAsync((IEnumerable<Object2D>)null);

            // Act
            var result = await _controller.Delete(objectId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Update_ShouldUpdateObject_WhenObjectExists()
        {
            // Arrange
            var objectId = Guid.NewGuid();
            var updatedObject = new Object2D
            {
                Id = objectId,
                PrefabId = "UpdatedPrefab",
                PositionX = 15,
                PositionY = 25,
                ScaleX = 1.5f,
                ScaleY = 1.5f,
                RotationZ = 45,
                SortingLayer = 1,
                Environment2DID = Guid.NewGuid()
            };
            var existingObject = new List<Object2D>
            {
                new Object2D { Id = objectId, PrefabId = "Prefab1" }
            };

            _mockRepository.Setup(repo => repo.GetObjectAsync(objectId)).ReturnsAsync(existingObject);
            _mockRepository.Setup(repo => repo.UpdateObjectAsync(updatedObject)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Update(objectId, updatedObject);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(updatedObject, okResult.Value);
        }

        [TestMethod]
        public async Task Update_ShouldReturnNotFound_WhenObjectDoesNotExist()
        {
            // Arrange
            var objectId = Guid.NewGuid();
            var updatedObject = new Object2D
            {
                Id = objectId,
                PrefabId = "UpdatedPrefab",
                PositionX = 15,
                PositionY = 25,
                ScaleX = 1.5f,
                ScaleY = 1.5f,
                RotationZ = 45,
                SortingLayer = 1,
                Environment2DID = Guid.NewGuid()
            };

            _mockRepository.Setup(repo => repo.GetObjectAsync(objectId)).ReturnsAsync((IEnumerable<Object2D>)null);

            // Act
            var result = await _controller.Update(objectId, updatedObject);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }
    }
}
