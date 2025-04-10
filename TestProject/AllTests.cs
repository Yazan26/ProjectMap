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
    }
}
