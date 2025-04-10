using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using MijnWebApi.WebApi.Controllers;
using MijnWebApi.WebApi.Classes.Interfaces;
using MijnWebApi.WebApi.Classes.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MijnWebApi.WebApi.Test
{
    [TestClass]
    public sealed class AllTests
    {
        // Environment2DController Tests
        [TestMethod]
        public async Task TestCreateEnvironment()
        {
            // Arrange
            var environmentRepository = new Mock<IEnvironment2DRepository>();
            var logger = new Mock<ILogger<Environment2DController>>();
            var authRepository = new Mock<IAuthenticationService>();

            var userId = Guid.NewGuid().ToString();
            authRepository.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns(userId);

            var controller = new Environment2DController(environmentRepository.Object, logger.Object, authRepository.Object);
            var environment = new Environment2D
            {
                Id = Guid.NewGuid(),
                Name = "Test Environment",
                MaxHeight = 10,
                MaxWidth = 10,
                OwnerUserID = Guid.NewGuid()
            };

            // Act
            var result = await controller.Add(environment);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task TestUpdateEnvironment()
        {
            // Arrange
            var environmentRepository = new Mock<IEnvironment2DRepository>();
            var logger = new Mock<ILogger<Environment2DController>>();
            var authRepository = new Mock<IAuthenticationService>();

            var userId = Guid.NewGuid().ToString();
            var environmentId = Guid.NewGuid();
            authRepository.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns(userId);

            var environment = new Environment2D
            {
                Id = environmentId,
                Name = "Test Environment",
                MaxHeight = 10,
                MaxWidth = 10,
                OwnerUserID = Guid.NewGuid()
            };
            environmentRepository.Setup(x => x.GetWorldAsync(environmentId)).ReturnsAsync(environment);

            var controller = new Environment2DController(environmentRepository.Object, logger.Object, authRepository.Object);
            var environmentUpdate = new Environment2D
            {
                Id = environmentId,
                Name = "Updated Environment",
                MaxHeight = 100,
                MaxWidth = 100,
                OwnerUserID = Guid.NewGuid()
            };

            // Act
            var result = await controller.Update(environmentId, environmentUpdate);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task TestDeleteEnvironment()
        {
            // Arrange
            var environmentRepository = new Mock<IEnvironment2DRepository>();
            var logger = new Mock<ILogger<Environment2DController>>();
            var authRepository = new Mock<IAuthenticationService>();

            var userId = Guid.NewGuid().ToString();
            var environmentId = Guid.NewGuid();
            authRepository.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns(userId);
            environmentRepository.Setup(x => x.GetWorldAsync(environmentId)).ReturnsAsync(new Environment2D());

            var controller = new Environment2DController(environmentRepository.Object, logger.Object, authRepository.Object);

            // Act
            var result = await controller.Delete(environmentId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            environmentRepository.Verify(x => x.DeleteWorldAsync(environmentId), Times.Once);
        }

        // Object2DController Tests
        [TestMethod]
        public async Task TestGetAllObjects()
        {
            // Arrange
            var objectRepository = new Mock<IObject2DRepository>();
            var logger = new Mock<ILogger<Object2DController>>();
            var authRepository = new Mock<IAuthenticationService>();

            var expectedObjects = new List<Object2D>
            {
                new Object2D { Id = Guid.NewGuid(), PrefabId = "Prefab1", PositionX = 10, PositionY = 20 },
                new Object2D { Id = Guid.NewGuid(), PrefabId = "Prefab2", PositionX = 30, PositionY = 40 }
            };
            objectRepository.Setup(x => x.GetAllObjectsAsync()).ReturnsAsync(expectedObjects);

            var controller = new Object2DController(objectRepository.Object, authRepository.Object, logger.Object);

            // Act
            var result = await controller.Get();

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(expectedObjects, okResult.Value);
        }

        [TestMethod]
        public async Task TestGetObjectsByEnvironmentId()
        {
            // Arrange
            var objectRepository = new Mock<IObject2DRepository>();
            var logger = new Mock<ILogger<Object2DController>>();
            var authRepository = new Mock<IAuthenticationService>();

            var environmentId = Guid.NewGuid();
            var expectedObjects = new List<Object2D>
            {
                new Object2D { Id = Guid.NewGuid(), Environment2DID = environmentId, PrefabId = "Prefab1" },
                new Object2D { Id = Guid.NewGuid(), Environment2DID = environmentId, PrefabId = "Prefab2" }
            };
            objectRepository.Setup(x => x.GetObjectAsync(environmentId)).ReturnsAsync(expectedObjects);

            var controller = new Object2DController(objectRepository.Object, authRepository.Object, logger.Object);

            // Act
            var result = await controller.Get(environmentId);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(expectedObjects, okResult.Value);
        }

        [TestMethod]
        public async Task TestAddObject()
        {
            // Arrange
            var objectRepository = new Mock<IObject2DRepository>();
            var logger = new Mock<ILogger<Object2DController>>();
            var authRepository = new Mock<IAuthenticationService>();

            var userId = Guid.NewGuid().ToString();
            authRepository.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns(userId);

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
                UserID = Guid.Parse(userId)
            };
            objectRepository.Setup(x => x.PostObjectAsync(It.IsAny<Object2D>())).ReturnsAsync(createdObject);

            var controller = new Object2DController(objectRepository.Object, authRepository.Object, logger.Object);

            // Act
            var result = await controller.Add(newObject);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(createdObject, okResult.Value);
        }

        [TestMethod]
        public async Task TestDeleteObject()
        {
            // Arrange
            var objectRepository = new Mock<IObject2DRepository>();
            var logger = new Mock<ILogger<Object2DController>>();
            var authRepository = new Mock<IAuthenticationService>();

            var objectId = Guid.NewGuid();
            objectRepository.Setup(x => x.GetObjectAsync(objectId)).ReturnsAsync(new List<Object2D> { new Object2D { Id = objectId } });

            var controller = new Object2DController(objectRepository.Object, authRepository.Object, logger.Object);

            // Act
            var result = await controller.Delete(objectId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkResult));
            objectRepository.Verify(x => x.DeleteObjectAsync(objectId), Times.Once);
        }
    }
}
