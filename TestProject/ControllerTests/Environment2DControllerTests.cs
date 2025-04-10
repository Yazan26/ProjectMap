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
    public class Environment2DControllerTests
    {
        private Mock<IEnvironment2DRepository> _mockRepository;
        private Mock<ILogger<Environment2DController>> _mockLogger;
        private Mock<IAuthenticationService> _mockAuthService;
        private Environment2DController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockRepository = new Mock<IEnvironment2DRepository>();
            _mockLogger = new Mock<ILogger<Environment2DController>>();
            _mockAuthService = new Mock<IAuthenticationService>();
            _controller = new Environment2DController(_mockRepository.Object, _mockLogger.Object, _mockAuthService.Object);
        }

        [TestMethod]
        public async Task Get_ShouldReturnWorld_WhenWorldExists()
        {
            // Arrange
            var worldId = Guid.NewGuid();
            var expectedWorld = new Environment2D
            {
                Id = worldId,
                Name = "Test World",
                MaxHeight = 100,
                MaxWidth = 200,
                OwnerUserID = Guid.NewGuid()
            };

            _mockRepository.Setup(repo => repo.GetWorldAsync(worldId)).ReturnsAsync(expectedWorld);

            // Act
            var result = await _controller.Get(worldId);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(expectedWorld, okResult.Value);
        }

        [TestMethod]
        public async Task Get_ShouldReturnNotFound_WhenWorldDoesNotExist()
        {
            // Arrange
            var worldId = Guid.NewGuid();
            _mockRepository.Setup(repo => repo.GetWorldAsync(worldId)).ReturnsAsync((Environment2D)null);

            // Act
            var result = await _controller.Get(worldId);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
            var notFoundResult = result.Result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
            Assert.AreEqual("geen wereld gevonden met dit ID, maak snel nieuwe!", notFoundResult.Value);
        }

        [TestMethod]
        public async Task GetAll_ShouldReturnWorldsForUser_WhenWorldsExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var expectedWorlds = new List<Environment2D>
    {
        new Environment2D { Id = Guid.NewGuid(), Name = "World 1", MaxHeight = 100, MaxWidth = 200, OwnerUserID = userId },
        new Environment2D { Id = Guid.NewGuid(), Name = "World 2", MaxHeight = 150, MaxWidth = 250, OwnerUserID = userId }
    };

            _mockAuthService.Setup(auth => auth.GetCurrentAuthenticatedUserId()).Returns(userId.ToString());
            _mockRepository.Setup(repo => repo.GetWorldsForUserAsync(userId)).ReturnsAsync(expectedWorlds);

            // Act
            var result = await _controller.Get();

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(expectedWorlds, okResult.Value);
        }

        [TestMethod]
        public async Task GetAll_ShouldReturnNotFound_WhenNoWorldsExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _mockAuthService.Setup(auth => auth.GetCurrentAuthenticatedUserId()).Returns(userId.ToString());
            _mockRepository.Setup(repo => repo.GetWorldsForUserAsync(userId)).ReturnsAsync((IEnumerable<Environment2D>)null);

            // Act
            var result = await _controller.Get();

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
            var notFoundResult = result.Result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
            Assert.AreEqual("geen werelden gevonden, maak snel nieuwe!", notFoundResult.Value);
        }

    }
}
