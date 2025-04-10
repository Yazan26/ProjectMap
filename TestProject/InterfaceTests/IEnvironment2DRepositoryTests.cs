using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MijnWebApi.WebApi.Classes.Interfaces;
using MijnWebApi.WebApi.Classes.Models;

namespace TestProject.Interfaces
{
    [TestClass]
    public class IEnvironment2DRepositoryTests
    {
        private Mock<IEnvironment2DRepository> _mockRepository;

        [TestInitialize]
        public void Setup()
        {
            _mockRepository = new Mock<IEnvironment2DRepository>();
        }

        [TestMethod]
        public async Task PostWorldAsync_ShouldInsertEnvironment()
        {
            // Arrange
            var environment2D = new Environment2D
            {
                Id = Guid.NewGuid(),
                Name = "Test Environment",
                MaxHeight = 100,
                MaxWidth = 200,
                OwnerUserID = Guid.NewGuid()
            };

            _mockRepository
                .Setup(repo => repo.PostWorldAsync(environment2D))
                .ReturnsAsync(environment2D);

            // Act
            var result = await _mockRepository.Object.PostWorldAsync(environment2D);

            // Assert
            Assert.AreEqual(environment2D, result);
            _mockRepository.Verify(repo => repo.PostWorldAsync(environment2D), Times.Once);
        }

        [TestMethod]
        public async Task GetWorldAsync_ShouldReturnEnvironment()
        {
            // Arrange
            var id = Guid.NewGuid();
            var expectedEnvironment = new Environment2D
            {
                Id = id,
                Name = "Test Environment",
                MaxHeight = 100,
                MaxWidth = 200,
                OwnerUserID = Guid.NewGuid()
            };

            _mockRepository
                .Setup(repo => repo.GetWorldAsync(id))
                .ReturnsAsync(expectedEnvironment);

            // Act
            var result = await _mockRepository.Object.GetWorldAsync(id);

            // Assert
            Assert.AreEqual(expectedEnvironment, result);
            _mockRepository.Verify(repo => repo.GetWorldAsync(id), Times.Once);
        }

        [TestMethod]
        public async Task GetWorldsForUserAsync_ShouldReturnEnvironmentsForUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var expectedEnvironments = new List<Environment2D>
            {
                new Environment2D
                {
                    Id = Guid.NewGuid(),
                    Name = "Environment 1",
                    MaxHeight = 100,
                    MaxWidth = 200,
                    OwnerUserID = userId
                },
                new Environment2D
                {
                    Id = Guid.NewGuid(),
                    Name = "Environment 2",
                    MaxHeight = 150,
                    MaxWidth = 250,
                    OwnerUserID = userId
                }
            };

            _mockRepository
                .Setup(repo => repo.GetWorldsForUserAsync(userId))
                .ReturnsAsync(expectedEnvironments);

            // Act
            var result = await _mockRepository.Object.GetWorldsForUserAsync(userId);

            // Assert
            Assert.AreEqual(expectedEnvironments.Count, result.Count());
            _mockRepository.Verify(repo => repo.GetWorldsForUserAsync(userId), Times.Once);
        }

        [TestMethod]
        public async Task GetWorldsAsync_ShouldReturnAllEnvironments()
        {
            // Arrange
            var expectedEnvironments = new List<Environment2D>
            {
                new Environment2D
                {
                    Id = Guid.NewGuid(),
                    Name = "Environment 1",
                    MaxHeight = 100,
                    MaxWidth = 200,
                    OwnerUserID = Guid.NewGuid()
                },
                new Environment2D
                {
                    Id = Guid.NewGuid(),
                    Name = "Environment 2",
                    MaxHeight = 150,
                    MaxWidth = 250,
                    OwnerUserID = Guid.NewGuid()
                }
            };

            _mockRepository
                .Setup(repo => repo.GetworldsAsync())
                .ReturnsAsync(expectedEnvironments);

            // Act
            var result = await _mockRepository.Object.GetworldsAsync();

            // Assert
            Assert.AreEqual(expectedEnvironments.Count, result.Count());
            _mockRepository.Verify(repo => repo.GetworldsAsync(), Times.Once);
        }

        [TestMethod]
        public async Task UpdateWorldAsync_ShouldUpdateEnvironment()
        {
            // Arrange
            var environment2D = new Environment2D
            {
                Id = Guid.NewGuid(),
                Name = "Updated Environment",
                MaxHeight = 120,
                MaxWidth = 220,
                OwnerUserID = Guid.NewGuid()
            };

            _mockRepository
                .Setup(repo => repo.UpdateWorldAsync(environment2D))
                .Returns(Task.CompletedTask);

            // Act
            await _mockRepository.Object.UpdateWorldAsync(environment2D);

            // Assert
            _mockRepository.Verify(repo => repo.UpdateWorldAsync(environment2D), Times.Once);
        }

        [TestMethod]
        public async Task DeleteWorldAsync_ShouldDeleteEnvironment()
        {
            // Arrange
            var id = Guid.NewGuid();

            _mockRepository
                .Setup(repo => repo.DeleteWorldAsync(id))
                .Returns(Task.CompletedTask);

            // Act
            await _mockRepository.Object.DeleteWorldAsync(id);

            // Assert
            _mockRepository.Verify(repo => repo.DeleteWorldAsync(id), Times.Once);
        }
    }
}
